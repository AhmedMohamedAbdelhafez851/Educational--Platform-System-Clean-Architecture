using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Domains.Entities;
using OnlineExamSystem.Domains.Entities.OnlineExamSystem.Domains.Entities;

namespace OnlineExamSystem.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<ExamSubmission> ExamSubmissions { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<ExamInvitation> ExamInvitations { get; set; }
        public DbSet<ExamInvitationAttempt> ExamInvitationAttempts { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Exam>()
                .HasMany(e => e.Questions)
                .WithOne(q => q.Exam)
                .HasForeignKey(q => q.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Question>()
                .HasMany(q => q.Choices)
                .WithOne(c => c.Question)
                .HasForeignKey(c => c.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.CorrectChoice)
                .WithOne()
                .HasForeignKey<Question>(q => q.CorrectChoiceId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExamSubmission>(entity =>
            {
                entity.HasKey(es => es.SubmissionId);

                entity.Property(es => es.UserId).IsRequired(false);
                entity.Property(es => es.StudentName).IsRequired(false);
                entity.Property(es => es.StudentEmail).IsRequired(false);
                entity.Property(es => es.StudentId).IsRequired(false);
                entity.Property(es => es.Score).HasPrecision(18, 2);

                entity.HasOne(es => es.User)
                    .WithMany()
                    .HasForeignKey(es => es.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                entity.HasOne(es => es.Exam)
                    .WithMany(e => e.Submissions)
                    .HasForeignKey(es => es.ExamId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.Submission)
                .WithMany(es => es.Answers)
                .HasForeignKey(ua => ua.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.Question)
                .WithMany()
                .HasForeignKey(ua => ua.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserAnswer>()
                .HasOne(ua => ua.SelectedChoice)
                .WithMany()
                .HasForeignKey(ua => ua.SelectedChoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExamSubmission>()
                .HasIndex(es => new { es.UserId, es.ExamId });

            modelBuilder.Entity<ExamInvitation>()
                .HasIndex(e => e.Token)
                .IsUnique();

            modelBuilder.Entity<ExamInvitation>()
                .HasIndex(e => e.InvitationCode)
                .IsUnique();

            modelBuilder.Entity<ExamInvitationAttempt>()
                .HasOne(e => e.Submission)
                .WithOne()
                .HasForeignKey<ExamInvitationAttempt>(e => e.SubmissionId)
                .OnDelete(DeleteBehavior.SetNull);

            // Seed Roles
            var adminRoleId = "8a3b5d7c-fb0b-42c9-a5c2-bd055b43a6c4";
            var teacherRoleId = "e4c1fa52-9a2e-47b6-9cb1-34a6d612c8e7";
            var studentRoleId = "f5d2fb63-0b3f-58c7-0dc2-45b7e723d9f8";

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = teacherRoleId, Name = "Teacher", NormalizedName = "TEACHER" },
                new IdentityRole { Id = studentRoleId, Name = "Student", NormalizedName = "STUDENT" }
            );
        }
    }
}