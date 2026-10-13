"""Published demo startup and HTTP contracts, using an isolated database."""
from pathlib import Path
import os
import shutil
import socket
import subprocess
import sys
import tempfile
import time
import urllib.error
import urllib.request


class NoRedirect(urllib.request.HTTPRedirectHandler):
    def redirect_request(self, req, fp, code, msg, headers, newurl):
        return None


def request(opener, url, data=None):
    try:
        response = opener.open(url, data=data, timeout=3)
    except urllib.error.HTTPError as error:
        response = error
    with response:
        return response.code, response.headers, response.read().decode()


published = Path(sys.argv[1]).resolve()
with tempfile.TemporaryDirectory(prefix="derskayit-test-") as directory:
    runtime = Path(directory) / "app"
    shutil.copytree(published, runtime)
    for database in runtime.glob("*.db*"):
        database.unlink()
    with socket.socket() as sock:
        sock.bind(("127.0.0.1", 0))
        port = sock.getsockname()[1]
    base = f"http://127.0.0.1:{port}"
    env = {**os.environ, "ASPNETCORE_ENVIRONMENT": "Development", "ASPNETCORE_URLS": base}
    opener = urllib.request.build_opener(NoRedirect())
    with tempfile.TemporaryFile() as log:
        process = subprocess.Popen(["dotnet", "DersKayit.dll"], cwd=runtime, env=env,
                                   stdout=log, stderr=log)
        try:
            for _ in range(100):
                if process.poll() is not None:
                    raise RuntimeError("Application exited before becoming ready")
                try:
                    status, _, body = request(opener, base + "/Course/List")
                    break
                except (urllib.error.URLError, TimeoutError):
                    time.sleep(0.1)
            else:
                raise RuntimeError("Application did not become ready")
            assert status == 200 and "ASP.NET Core" in body, "Fresh database must contain courses"
            assert (runtime / "basvurular.db").exists(), "Database must be created locally"
            for route in ["/Course/Apply/1", "/Course/Details/1"]:
                status, headers, _ = request(opener, base + route)
                assert status == 302 and "/Account/Login" in headers["Location"], route
            status, _, _ = request(opener, base + "/Course/Apply/1", data=b"CourseId=1")
            assert status == 400, "Application POST must reject missing CSRF token"
            print("PASS: fresh database, seeded courses, two login boundaries, CSRF rejection")
        finally:
            process.terminate()
            try:
                process.wait(timeout=5)
            except subprocess.TimeoutExpired:
                process.kill()
                process.wait()
