namespace DersKayit.Models
{
    public class CourseApplication
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? EducationLevel { get; set; }

        public string? Motivation { get; set; }

        public int CourseId { get; set; }
    }
}
