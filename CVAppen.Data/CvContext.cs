using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace CvAppen.Data
{
    public class CvContext : IdentityDbContext<User>
    {
        //public CvContext() { }
        public CvContext(DbContextOptions<CvContext> options) : base(options) { }
        //public DbSet<User> Users { get; set; } //- får varning om denna för den ärvs tydl när man ärver från identity
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
                .HasOne(m => m.FromUser)
                .WithMany(m => m.SentMessages)
                .HasForeignKey(m => m.FromUserId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
              .HasOne(m => m.ToUser)
              .WithMany(m => m.RecievedMessages)
              .HasForeignKey(u => u.ToUserId).OnDelete(DeleteBehavior.Restrict);

            //behöver kolla på hur vi ska radera User också, fått för mig att kontot inte ska kunna raderas utan endast avaktiveras?

        }

        private void SeedData(ModelBuilder modelBuilder)
        {

            var eduStart1 = new DateTime(2020, 8, 1);
            var eduEnd1 = new DateTime(2022, 6, 1);

            var eduStart2 = new DateTime(2017, 8, 1);
            var eduEnd2 = new DateTime(2020, 6, 1);
            
            var otherDate = new DateTime(2025, 1, 1);

            var user1Id = "5f8dedbb-0023-4225-90a3-fb982dde34e4";
            var user2Id = "70d91398-1e9f-4c6c-9464-31629296e124";

            var user1 = new User
            {
                Id = user1Id,
                UserName = "anna@example.com",
                Email = "anna@example.com",
                Name = "Anna Andersson",
                Address = "Stockholm",
                IsPrivate = false,
                Image = "anna.jpg"
            };

            var user2 = new User
            {
                Id = user2Id,
                UserName = "erik@example.com",
                Email = "erik@example.com",
                Name = "Erik Svensson",
                Address = "Göteborg",
                IsPrivate = true,
                Image = "erik.jpg"
            };

            modelBuilder.Entity<User>().HasData(
                   user1, user2
              );


            modelBuilder.Entity<CV>().HasData(
            new CV
            {
                Id = 1,
                UserId = user1.Id,
                Presentation = "Systemutvecklare med stort intresse för webb och backend.",
                PhoneNumber = "0701234567",
                ImagePath = "cv-images/anna.jpg",
                //User = user1
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
                From = eduStart1,
                To = eduEnd1,
                CVId = 1
            },
            new Education
            {
                Id = 9,
                Name = "Datavetenskap",
                School = "Stockholms Universitet",
                From = eduStart1,
                To = eduEnd1,
                CVId = 1
            });

            modelBuilder.Entity<EarlierExperience>().HasData(
            new EarlierExperience
            {
                Id = 1,
                Title = "Junior .NET-utvecklare",
                Company = "Tech AB",
                Description = "Arbetade med backend i ASP.NET Core",
                From = eduStart1,
                To = eduEnd1,
                CVId = 1
            },
            new EarlierExperience
            {
                Id = 2,
                Title = "IT-support",
                Company = "Servicebolaget",
                Description = "Användarsupport och felsökning",
                From = eduStart1,
                To = eduEnd1,
                CVId = 1
            });

            modelBuilder.Entity<Project>().HasData(
            new Project
            {
                Id = 1,
                Title = "Webbportal",
                Description = "Intern webbportal för företag",
                Date = eduStart1,
            },
            new Project
            {
                Id = 2,
                Title = "Mobilapp",
                Description = "App för bokningssystem",
                Date = eduStart1
            });

            modelBuilder.Entity<UserProject>().HasData(
            new UserProject
            {
                UserId = user1Id,
                ProjectId = 1
            },
            new UserProject
            {
                UserId = user2Id,
                ProjectId = 1
            },
            new UserProject
            {
                UserId = user2Id,
                ProjectId = 2
            });

            modelBuilder.Entity<Message>().HasData(
             new Message
             {
                 Id = 1,
                 Text = "Hej! Är du intresserad av projektet?",
                 FromUserId = user1Id,
                 ToUserId = user2Id,
                 SenderName = "Katten",
                 IsRead = false,
                 SentAt = otherDate
             });
        }
    }
}


