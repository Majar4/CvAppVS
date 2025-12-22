using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace CvAppenVS.Models
{
    public class CvContext : IdentityDbContext<User>
    {
        public CvContext() { }
        public CvContext(DbContextOptions<CvContext> options) : base(options) { }
        //public DbSet<User> Users { get; set; } - får varning om denna för den ärvs tydl när man ärver från identity
        public DbSet<Project> Projects { get; set; }
        public DbSet<CV> CVs { get; set; }
        public DbSet<Competence> Competences { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<EarlierExperience> EarlierExperiences { get; set; }
        public DbSet<UserProject> UserProjects { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserProject>()
                .HasKey(up => new { up.UserId, up.ProjectId });

            SeedData(modelBuilder);

            modelBuilder.Entity<Message>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(u => u.FromUserId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
              .HasOne<User>()
              .WithMany()
              .HasForeignKey(u => u.ToUserId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>().Property(u => u.Id).ValueGeneratedNever();
            modelBuilder.Entity<CV>().Property(c => c.Id).ValueGeneratedNever();
            modelBuilder.Entity<Competence>().Property(c => c.Id).ValueGeneratedNever();
            modelBuilder.Entity<Education>().Property(e => e.Id).ValueGeneratedNever();
            modelBuilder.Entity<EarlierExperience>().Property(ee => ee.Id).ValueGeneratedNever();
            modelBuilder.Entity<Project>().Property(p => p.Id).ValueGeneratedNever();
            modelBuilder.Entity<Message>().Property(m => m.Id).ValueGeneratedNever();

            //behöver kolla på hur vi ska radera User också, fått för mig att kontot inte ska kunna raderas utan endast avaktiveras?

        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CV>().HasData(
            new CV
            {
                Id = 1,
                UserId = "user-1",
                Presentation = "Systemutvecklare med stort intresse för webb och backend.",
                PhoneNumber = "0701234567",
                ImagePath = "cv-images/anna.jpg"
            });

            modelBuilder.Entity<Competence>().HasData(
            new Competence
            {
                Id = 1,
                Title = "C# / .NET",
                CVId = 1
            },
            new Competence
            {
                Id = 2,
                Title = "ASP.NET Core MVC",
                CVId = 1
            },
            new Competence
            {
                Id = 3,
                Title = "SQL Server",
                CVId = 1
            });

            modelBuilder.Entity<Education>().HasData(
            new Education
            {
                Id = 8,
                Name = "Systemutveckling .NET",
                School = "Yrkeshögskolan Stockholm",
                From = new DateTime(2020, 8, 1),
                To = new DateTime(2022, 6, 1),
                CVId = 1
            },
            new Education
            {
                Id = 9,
                Name = "Datavetenskap",
                School = "Stockholms Universitet",
                From = new DateTime(2017, 8, 1),
                To = new DateTime(2020, 6, 1),
                CVId = 1
            });

            modelBuilder.Entity<EarlierExperience>().HasData(
            new EarlierExperience
            {
                Id = 1,
                Title = "Junior .NET-utvecklare",
                Company = "Tech AB",
                Description = "Arbetade med backend i ASP.NET Core",
                From = new DateTime(2022, 9, 1),
                To = new DateTime(2023, 12, 31),
                CVId = 1
            },
            new EarlierExperience
            {
                Id = 2,
                Title = "IT-support",
                Company = "Servicebolaget",
                Description = "Användarsupport och felsökning",
                From = new DateTime(2019, 6, 1),
                To = new DateTime(2020, 8, 31),
                CVId = 1
            });

            modelBuilder.Entity<Project>().HasData(
            new Project
            {
                Id = 1,
                Title = "Webbportal",
                Description = "Intern webbportal för företag",
                Date = new DateTime(2025, 1, 1)
            },
            new Project
            {
                Id = 2,
                Title = "Mobilapp",
                Description = "App för bokningssystem",
                Date = new DateTime(2021, 03, 04)
            });

            modelBuilder.Entity<UserProject>().HasData(
            new UserProject
            {
                UserId = "user-1",
                ProjectId = 1
            },
            new UserProject
            {
                UserId = "user-2",
                ProjectId = 1
            },
            new UserProject
            {
                UserId = "user-2",
                ProjectId = 2
            });

            modelBuilder.Entity<Message>().HasData(
             new Message
             {
                 Id = 1,
                 Text = "Hej! Är du intresserad av projektet?",
                 FromUserId = "user-1",
                 ToUserId = "user-2",
                 IsRead = false,
                 SentAt = new DateTime(2025, 1, 1)
             });
        }
    }
}


