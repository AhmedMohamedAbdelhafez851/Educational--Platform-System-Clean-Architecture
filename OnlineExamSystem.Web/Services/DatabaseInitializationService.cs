using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Domains.Entities;
using OnlineExamSystem.Domains.Entities.OnlineExamSystem.Domains.Entities;
using OnlineExamSystem.Infrastructure.Persistence;

namespace OnlineExamSystem.Web.Services
{
    public class DatabaseInitializationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseInitializationService> _logger;

        public DatabaseInitializationService(
            IServiceProvider serviceProvider,
            ILogger<DatabaseInitializationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await EnsureDatabaseAsync(db);
            await SeedUsersAsync(scope);
            await SeedTestDataAsync(scope);
        }

        private async Task EnsureDatabaseAsync(ApplicationDbContext db)
        {
            var databaseExists = await CheckDatabaseExistsAsync(db);

            if (!databaseExists)
            {
                _logger.LogInformation("Creating database...");
                await db.Database.MigrateAsync();
                _logger.LogInformation("✅ Database created successfully.");
            }
            else
            {
                _logger.LogInformation("Database already exists. Checking for pending migrations...");

                try
                {
                    var pendingMigrations = await db.Database.GetPendingMigrationsAsync();
                    if (pendingMigrations.Any())
                    {
                        _logger.LogInformation("Applying {Count} pending migrations...", pendingMigrations.Count());
                        await db.Database.MigrateAsync();
                        _logger.LogInformation("✅ Migrations applied successfully.");
                    }
                    else
                    {
                        _logger.LogInformation("✅ Database is up to date.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Migration warning");
                }
            }
        }

        private async Task<bool> CheckDatabaseExistsAsync(ApplicationDbContext db)
        {
            try
            {
                await db.Database.ExecuteSqlRawAsync("SELECT 1");
                return true;
            }
            catch (SqlException ex)
            {
                return ex.Number == 4060;
            }
        }

        private async Task SeedUsersAsync(IServiceScope scope)
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await CreateRolesAsync(roleManager);
            await CreateSuperAdminAsync(userManager);
            await CreateDemoUsersAsync(userManager);
        }

        private async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "SuperAdmin", "Admin", "Teacher", "Student" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    _logger.LogInformation("✅ Role '{Role}' created.", role);
                }
            }
        }

        private async Task CreateSuperAdminAsync(UserManager<ApplicationUser> userManager)
        {
            var email = "superadmin@examify.com";
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FullName = "Super Administrator"
                };
                var result = await userManager.CreateAsync(user, "SuperAdmin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "SuperAdmin");
                    await userManager.AddToRoleAsync(user, "Admin");
                    await userManager.AddToRoleAsync(user, "Teacher");
                    _logger.LogInformation("✅ Super Admin created: {Email}", email);
                }
            }
        }

        private async Task CreateDemoUsersAsync(UserManager<ApplicationUser> userManager)
        {
            var demoUsers = new[]
            {
                new { Email = "demo@examify.com", Password = "Demo@123", Name = "Demo Administrator", Roles = new[] { "Admin", "Teacher" } },
                new { Email = "teacher@examify.com", Password = "Teacher@123", Name = "Demo Teacher", Roles = new[] { "Teacher" } },
                new { Email = "student@examify.com", Password = "Student@123", Name = "Demo Student", Roles = new[] { "Student" } },
                new { Email = "admin@site.com", Password = "Admin@123", Name = "System Administrator", Roles = new[] { "SuperAdmin", "Admin", "Teacher" } }
            };

            foreach (var demoUser in demoUsers)
            {
                var user = await userManager.FindByEmailAsync(demoUser.Email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = demoUser.Email,
                        Email = demoUser.Email,
                        EmailConfirmed = true,
                        FullName = demoUser.Name
                    };
                    var result = await userManager.CreateAsync(user, demoUser.Password);
                    if (result.Succeeded)
                    {
                        foreach (var role in demoUser.Roles)
                        {
                            await userManager.AddToRoleAsync(user, role);
                        }
                        _logger.LogInformation("✅ {Name} created: {Email}", demoUser.Name, demoUser.Email);
                    }
                }
            }
        }

        private async Task SeedTestDataAsync(IServiceScope scope)
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (await db.Exams.AnyAsync())
            {
                _logger.LogInformation("📚 Test data already exists, skipping seeding...");
                return;
            }

            _logger.LogInformation("🌱 Seeding test data...");

            var exams = CreateExams();
            await db.Exams.AddRangeAsync(exams);
            await db.SaveChangesAsync();

            var questions = CreateQuestionsForMathExam(exams[0]);
            await db.Questions.AddRangeAsync(questions);
            await db.SaveChangesAsync();

            var choices = CreateChoicesForQuestions(questions);
            await db.Choices.AddRangeAsync(choices);
            await db.SaveChangesAsync();

            await UpdateCorrectChoiceIds(db, questions, choices);
            await CreateInvitationsAsync(db, exams);
            await CreateTestSubmissionsAsync(db, exams[0]);

            _logger.LogInformation("✅ Test data seeding completed successfully!");
        }

        private List<Exam> CreateExams()
        {
            return new List<Exam>
            {
                new Exam { Title = "الرياضيات الأساسية", DurationInMinutes = 60, TotalDegree = 100, CreatedDate = DateTime.UtcNow.AddDays(-30), CreatedBy = "system" },
                new Exam { Title = "علوم الحاسب", DurationInMinutes = 45, TotalDegree = 100, CreatedDate = DateTime.UtcNow.AddDays(-25), CreatedBy = "system" },
                new Exam { Title = "الفيزياء العامة", DurationInMinutes = 90, TotalDegree = 100, CreatedDate = DateTime.UtcNow.AddDays(-20), CreatedBy = "system" },
                new Exam { Title = "اللغة العربية", DurationInMinutes = 30, TotalDegree = 50, CreatedDate = DateTime.UtcNow.AddDays(-15), CreatedBy = "system" }
            };
        }

        private List<Question> CreateQuestionsForMathExam(Exam mathExam)
        {
            var questions = new[]
            {
                new Question { Title = "ما قيمة س في المعادلة 2س + 5 = 15؟", ExamId = mathExam.ExamId },
                new Question { Title = "ما مساحة دائرة نصف قطرها 7 سم؟", ExamId = mathExam.ExamId },
                new Question { Title = "ما ناتج جمع 1/2 + 1/3؟", ExamId = mathExam.ExamId },
                new Question { Title = "ما قيمة 20% من 200؟", ExamId = mathExam.ExamId },
                new Question { Title = "ما قيمة الجذر التربيعي للعدد 144؟", ExamId = mathExam.ExamId }
            };
            return questions.ToList();
        }

        private List<Choice> CreateChoicesForQuestions(List<Question> questions)
        {
            var choicesConfig = new[]
            {
                new { Question = questions[0], Choices = new[] { ("س = 3", false), ("س = 5", true), ("س = 7", false), ("س = 10", false) } },
                new { Question = questions[1], Choices = new[] { ("154 سم²", true), ("44 سم²", false), ("88 سم²", false), ("308 سم²", false) } },
                new { Question = questions[2], Choices = new[] { ("2/5", false), ("2/6", false), ("5/6", true), ("3/5", false) } },
                new { Question = questions[3], Choices = new[] { ("20", false), ("40", true), ("60", false), ("80", false) } },
                new { Question = questions[4], Choices = new[] { ("10", false), ("11", false), ("12", true), ("13", false) } }
            };

            var choices = new List<Choice>();
            foreach (var config in choicesConfig)
            {
                foreach (var (text, isCorrect) in config.Choices)
                {
                    choices.Add(new Choice { Text = text, IsCorrect = isCorrect, QuestionId = config.Question.QuestionId });
                }
            }
            return choices;
        }

        private async Task UpdateCorrectChoiceIds(ApplicationDbContext db, List<Question> questions, List<Choice> choices)
        {
            foreach (var question in questions)
            {
                var correctChoice = choices.First(c => c.QuestionId == question.QuestionId && c.IsCorrect);
                question.CorrectChoiceId = correctChoice.ChoiceId;
            }
            await db.SaveChangesAsync();
        }

        private async Task CreateInvitationsAsync(ApplicationDbContext db, List<Exam> exams)
        {
            var invitations = exams.Select(exam => new ExamInvitation
            {
                ExamId = exam.ExamId,
                Token = Guid.NewGuid().ToString("N"),
                InvitationCode = GenerateInvitationCode(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                MaxAttempts = 3,
                CreatedByUserId = "system"
            }).ToList();

            await db.ExamInvitations.AddRangeAsync(invitations);
            await db.SaveChangesAsync();
        }

        private async Task CreateTestSubmissionsAsync(ApplicationDbContext db, Exam mathExam)
        {
            var questions = await db.Questions.Where(q => q.ExamId == mathExam.ExamId).Include(q => q.Choices).ToListAsync();
            var testStudents = new[]
            {
                ("أحمد محمد", "ahmed@test.com", "STU001", 5),
                ("سارة أحمد", "sara@test.com", "STU002", 4),
                ("محمد علي", "mohamed@test.com", "STU003", 3),
                ("فاطمة حسن", "fatma@test.com", "STU004", 5),
                ("عمر خالد", "omar@test.com", "STU005", 2)
            };

            var random = new Random();
            var submissions = new List<ExamSubmission>();

            foreach (var (name, email, studentId, correctCount) in testStudents)
            {
                var userAnswers = new List<UserAnswer>();
                for (int i = 0; i < questions.Count; i++)
                {
                    var question = questions[i];
                    var isCorrect = i < correctCount;
                    var selectedChoice = isCorrect
                        ? question.Choices.First(c => c.IsCorrect)
                        : question.Choices.First(c => !c.IsCorrect);

                    userAnswers.Add(new UserAnswer
                    {
                        QuestionId = question.QuestionId,
                        SelectedChoiceId = selectedChoice.ChoiceId
                    });
                }

                var score = (double)correctCount / questions.Count * 100;
                submissions.Add(new ExamSubmission
                {
                    ExamId = mathExam.ExamId,
                    SubmissionDate = DateTime.UtcNow.AddDays(-random.Next(1, 10)),
                    TotalQuestions = questions.Count,
                    CorrectAnswers = correctCount,
                    Score = score,
                    IsPassed = score >= 50,
                    StudentName = name,
                    StudentEmail = email,
                    StudentId = studentId,
                    Answers = userAnswers
                });
            }

            await db.ExamSubmissions.AddRangeAsync(submissions);
            await db.SaveChangesAsync();

            foreach (var submission in submissions)
            {
                foreach (var answer in submission.Answers)
                {
                    answer.SubmissionId = submission.SubmissionId;
                }
            }
            await db.SaveChangesAsync();
        }

        private string GenerateInvitationCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}