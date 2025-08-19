using Microsoft.EntityFrameworkCore;
using DersKayit.Data;
using DersKayit.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSession();

// EF Core - SQLite bağlantısı
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=basvurular.db"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!db.Courses.Any())
    {
        db.Courses.AddRange(
            new Course { Title = "ASP.NET Core", Description = "Web geliştirme temelleri", Category = "Backend" },
            new Course { Title = "C# Giriş", Description = "C# diline giriş", Category = "Backend" },
            new Course { Title = "Veritabanı", Description = "SQL ve Entity Framework", Category = "Database" },
            new Course { Title = "Django", Description = "Python ile web geliştirme", Category = "Backend" },
            new Course { Title = "Web Güvenliği", Description = "Siber güvenlik temelleri", Category = "Security" }
        );
        db.SaveChanges();
    }

    if (!db.Users.Any())
    {
        db.Users.AddRange(
            new User { Username = "admin", Password = "admin123", Role = "Admin" },
            new User { Username = "user",  Password = "user123",  Role = "User"  }
        );
        db.SaveChanges();
    }
}

// Pipeline 
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();          
app.UseAuthorization();

app.MapStaticAssets();


app.MapControllers();      

/* Konvansiyonel route */
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
