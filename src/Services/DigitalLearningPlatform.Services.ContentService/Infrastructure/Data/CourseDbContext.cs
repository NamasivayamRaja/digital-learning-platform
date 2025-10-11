using DigitalLearningPlatform.Services.ContentService.Domain;
using Microsoft.EntityFrameworkCore;

namespace DigitalLearningPlatform.Services.ContentService.Infrastructure.Data
{
    public class CourseDbContext : DbContext
    {
        public CourseDbContext(DbContextOptions<CourseDbContext> options ) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<SectionFile> SectionFiles { get; set; }
        public DbSet<CourseFileUpload> CourseFileUploads { get; set; } // Mapping requires?
        public DbSet<AuthorLookUp> AuthorLookUps { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //key config
            modelBuilder.Entity<AuthorLookUp>().HasKey(c => c.AuthorId);

            // Indexes
            modelBuilder.Entity<Course>().HasIndex(c=> c.Category);
            modelBuilder.Entity<Course>().HasIndex(c => c.Level);

            /*
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Author)
                .WithMany()
                .HasForeignKey(c => c.AuthorId);
            */

            // Relationship
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Sections)
                .WithOne(s => s.Course)
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Section>()
                .HasMany(s=> s.Files) 
                .WithOne(sf=> sf.Section)
                .HasForeignKey(sf => sf.SectionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SectionFile>()
                .ToTable("SectionFiles");

            base.OnModelCreating(modelBuilder);
        }
    }
}
