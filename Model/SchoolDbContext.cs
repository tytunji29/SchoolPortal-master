using Microsoft.EntityFrameworkCore;

namespace SchoolPortal.Model;

public partial class SchoolDbContext : DbContext
{
    public SchoolDbContext()
    {
    }

    public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Login> Logins { get; set; }
    public virtual DbSet<School> Schools { get; set; }
    public virtual DbSet<Parent> Parents { get; set; }
    public virtual DbSet<Subjects> Subjects { get; set; }
    public virtual DbSet<Map_Subjects_Teacher_Class> Map_Subjects_Teacher_Class { get; set; }
    public virtual DbSet<TeacherTimetable> TeacherTimetable { get; set; }
    public virtual DbSet<Staff> Staffs { get; set; }
    public virtual DbSet<Student> Students { get; set; }
    public virtual DbSet<Classes> Classes { get; set; }
    public virtual DbSet<ClassesLevel> ClassesLevel { get; set; }
    public virtual DbSet<DocumentHolder> DocumentHolders { get; set; }
    public virtual DbSet<ClassTeacherHistory> ClassTeacherHistory { get; set; }
}
