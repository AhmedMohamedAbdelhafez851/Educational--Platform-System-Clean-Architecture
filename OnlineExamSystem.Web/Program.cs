using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OnlineExamSystem.Application.DependencyInjection;
using OnlineExamSystem.Domains.Entities;
using OnlineExamSystem.Domains.Entities.OnlineExamSystem.Domains.Entities;
using OnlineExamSystem.Infrastructure.DependencyInjection;
using OnlineExamSystem.Infrastructure.Persistence;
using OnlineExamSystem.Web.Middleware;
using Serilog;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// ✅ Serilog
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration).Enrich.FromLogContext();
});

// ✅ Add Localization Services
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-US"),
        new CultureInfo("ar-EG")
    };

    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
});

// ✅ Add MVC with View Localization
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

// ✅ Application Services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// ✅ Apply DB and Seed Users
await ApplyDatabaseAsync(app);

// ✅ Use Request Localization
var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);

// ✅ Middleware
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<LoggingEnrichmentMiddleware>();
app.UseSerilogRequestLogging();

// ✅ Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "exam",
    pattern: "{controller=Exam}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "userExam",
    pattern: "UserExam/{action=Index}/{id?}",
    defaults: new { controller = "UserExam" });

app.Run();

// ✅ Database Initialization and Seeding
async Task ApplyDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // ✅ Check if database exists using a simple SQL query
    var databaseExists = await CheckDatabaseExistsAsync(db);

    if (!databaseExists)
    {
        Console.WriteLine("Creating database...");
        await db.Database.MigrateAsync();
        Console.WriteLine("✅ Database created successfully.");
    }
    else
    {
        Console.WriteLine("Database already exists. Checking for pending migrations...");

        try
        {
            var pendingMigrations = await db.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                Console.WriteLine($"Applying {pendingMigrations.Count()} pending migrations...");
                await db.Database.MigrateAsync();
                Console.WriteLine("✅ Migrations applied successfully.");
            }
            else
            {
                Console.WriteLine("✅ Database is up to date.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Migration warning: {ex.Message}");
        }
    }

    // ✅ Seed users
    await SeedProductionUsersAsync(scope);

    // ✅ Seed test data (exams, questions, submissions)
    await SeedTestDataAsync(scope);
}

async Task SeedTestDataAsync(IServiceScope scope)
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Check if data already exists
    if (await db.Exams.AnyAsync())
    {
        Console.WriteLine("📚 Test data already exists, skipping seeding...");
        return;
    }

    Console.WriteLine("🌱 Seeding test data...");

    // =====================================================
    // CREATE EXAMS
    // =====================================================

    var exams = new List<Exam>
    {
        new Exam {
            Title = "الرياضيات الأساسية",
            DurationInMinutes = 60,
            TotalDegree = 100,
            CreatedDate = DateTime.UtcNow.AddDays(-30),
            CreatedBy = "system"
        },
        new Exam {
            Title = "علوم الحاسب",
            DurationInMinutes = 45,
            TotalDegree = 100,
            CreatedDate = DateTime.UtcNow.AddDays(-25),
            CreatedBy = "system"
        },
        new Exam {
            Title = "الفيزياء العامة",
            DurationInMinutes = 90,
            TotalDegree = 100,
            CreatedDate = DateTime.UtcNow.AddDays(-20),
            CreatedBy = "system"
        },
        new Exam {
            Title = "اللغة العربية",
            DurationInMinutes = 30,
            TotalDegree = 50,
            CreatedDate = DateTime.UtcNow.AddDays(-15),
            CreatedBy = "system"
        }
    };

    await db.Exams.AddRangeAsync(exams);
    await db.SaveChangesAsync();

    Console.WriteLine($"✅ Created {exams.Count} exams");

    // =====================================================
    // CREATE QUESTIONS AND CHOICES FOR MATH EXAM
    // =====================================================

    var mathExam = exams[0];
    var mathQuestions = new List<Question>();

    // Question 1
    var q1 = new Question { Title = "ما قيمة س في المعادلة 2س + 5 = 15؟", ExamId = mathExam.ExamId };
    mathQuestions.Add(q1);

    // Question 2
    var q2 = new Question { Title = "ما مساحة دائرة نصف قطرها 7 سم؟", ExamId = mathExam.ExamId };
    mathQuestions.Add(q2);

    // Question 3
    var q3 = new Question { Title = "ما ناتج جمع 1/2 + 1/3؟", ExamId = mathExam.ExamId };
    mathQuestions.Add(q3);

    // Question 4
    var q4 = new Question { Title = "ما قيمة 20% من 200؟", ExamId = mathExam.ExamId };
    mathQuestions.Add(q4);

    // Question 5
    var q5 = new Question { Title = "ما قيمة الجذر التربيعي للعدد 144؟", ExamId = mathExam.ExamId };
    mathQuestions.Add(q5);

    await db.Questions.AddRangeAsync(mathQuestions);
    await db.SaveChangesAsync();

    // Add choices for each question
    var allChoices = new List<Choice>();

    // Q1 Choices
    allChoices.Add(new Choice { Text = "س = 3", IsCorrect = false, QuestionId = q1.QuestionId });
    allChoices.Add(new Choice { Text = "س = 5", IsCorrect = true, QuestionId = q1.QuestionId });
    allChoices.Add(new Choice { Text = "س = 7", IsCorrect = false, QuestionId = q1.QuestionId });
    allChoices.Add(new Choice { Text = "س = 10", IsCorrect = false, QuestionId = q1.QuestionId });

    // Q2 Choices
    allChoices.Add(new Choice { Text = "154 سم²", IsCorrect = true, QuestionId = q2.QuestionId });
    allChoices.Add(new Choice { Text = "44 سم²", IsCorrect = false, QuestionId = q2.QuestionId });
    allChoices.Add(new Choice { Text = "88 سم²", IsCorrect = false, QuestionId = q2.QuestionId });
    allChoices.Add(new Choice { Text = "308 سم²", IsCorrect = false, QuestionId = q2.QuestionId });

    // Q3 Choices
    allChoices.Add(new Choice { Text = "2/5", IsCorrect = false, QuestionId = q3.QuestionId });
    allChoices.Add(new Choice { Text = "2/6", IsCorrect = false, QuestionId = q3.QuestionId });
    allChoices.Add(new Choice { Text = "5/6", IsCorrect = true, QuestionId = q3.QuestionId });
    allChoices.Add(new Choice { Text = "3/5", IsCorrect = false, QuestionId = q3.QuestionId });

    // Q4 Choices
    allChoices.Add(new Choice { Text = "20", IsCorrect = false, QuestionId = q4.QuestionId });
    allChoices.Add(new Choice { Text = "40", IsCorrect = true, QuestionId = q4.QuestionId });
    allChoices.Add(new Choice { Text = "60", IsCorrect = false, QuestionId = q4.QuestionId });
    allChoices.Add(new Choice { Text = "80", IsCorrect = false, QuestionId = q4.QuestionId });

    // Q5 Choices
    allChoices.Add(new Choice { Text = "10", IsCorrect = false, QuestionId = q5.QuestionId });
    allChoices.Add(new Choice { Text = "11", IsCorrect = false, QuestionId = q5.QuestionId });
    allChoices.Add(new Choice { Text = "12", IsCorrect = true, QuestionId = q5.QuestionId });
    allChoices.Add(new Choice { Text = "13", IsCorrect = false, QuestionId = q5.QuestionId });

    await db.Choices.AddRangeAsync(allChoices);
    await db.SaveChangesAsync();

    // Update correct choice IDs
    q1.CorrectChoiceId = allChoices.First(c => c.QuestionId == q1.QuestionId && c.IsCorrect).ChoiceId;
    q2.CorrectChoiceId = allChoices.First(c => c.QuestionId == q2.QuestionId && c.IsCorrect).ChoiceId;
    q3.CorrectChoiceId = allChoices.First(c => c.QuestionId == q3.QuestionId && c.IsCorrect).ChoiceId;
    q4.CorrectChoiceId = allChoices.First(c => c.QuestionId == q4.QuestionId && c.IsCorrect).ChoiceId;
    q5.CorrectChoiceId = allChoices.First(c => c.QuestionId == q5.QuestionId && c.IsCorrect).ChoiceId;

    await db.SaveChangesAsync();

    Console.WriteLine($"✅ Created {mathQuestions.Count} questions for Math exam");

    // =====================================================
    // CREATE INVITATIONS
    // =====================================================

    var invitations = new List<ExamInvitation>();
    foreach (var exam in exams)
    {
        invitations.Add(new ExamInvitation
        {
            ExamId = exam.ExamId,
            Token = Guid.NewGuid().ToString("N"),
            InvitationCode = GenerateInvitationCode(),
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            MaxAttempts = 3,
            CreatedByUserId = "system"
        });
    }
    await db.ExamInvitations.AddRangeAsync(invitations);
    await db.SaveChangesAsync();

    Console.WriteLine($"✅ Created {invitations.Count} invitations");

    // =====================================================
    // CREATE TEST STUDENTS AND SUBMISSIONS
    // =====================================================

    var testStudents = new List<(string Name, string Email, string Id)>
    {
        ("أحمد محمد", "ahmed@test.com", "STU001"),
        ("سارة أحمد", "sara@test.com", "STU002"),
        ("محمد علي", "mohamed@test.com", "STU003"),
        ("فاطمة حسن", "fatma@test.com", "STU004"),
        ("عمر خالد", "omar@test.com", "STU005")
    };

    var invitationAttempts = new List<ExamInvitationAttempt>();
    var submissions = new List<ExamSubmission>();
    var random = new Random();

    foreach (var (name, email, studentId) in testStudents)
    {
        var invitation = invitations.First();

        var attempt = new ExamInvitationAttempt
        {
            InvitationId = invitation.Id,
            StudentName = name,
            StudentEmail = email,
            StudentId = studentId,
            StartedAt = DateTime.UtcNow.AddDays(-random.Next(1, 10)),
            IsCompleted = true
        };
        invitationAttempts.Add(attempt);
    }

    await db.ExamInvitationAttempts.AddRangeAsync(invitationAttempts);
    await db.SaveChangesAsync();

    // Create submissions with different scores
    var mathQuestionsList = await db.Questions.Where(q => q.ExamId == mathExam.ExamId).Include(q => q.Choices).ToListAsync();
    var scores = new int[] { 5, 4, 3, 5, 2 }; // Different scores for each student

    for (int i = 0; i < testStudents.Count; i++)
    {
        var student = testStudents[i];
        var attempt = invitationAttempts[i];
        var correctCount = scores[i];
        var totalQuestions = mathQuestionsList.Count;
        var userAnswers = new List<UserAnswer>();

        // Create answers (first 'correctCount' answers correct, rest wrong)
        for (int q = 0; q < mathQuestionsList.Count; q++)
        {
            var question = mathQuestionsList[q];
            bool isCorrect = q < correctCount;

            var selectedChoice = isCorrect
                ? question.Choices.First(c => c.IsCorrect)
                : question.Choices.First(c => !c.IsCorrect);

            userAnswers.Add(new UserAnswer
            {
                QuestionId = question.QuestionId,
                SelectedChoiceId = selectedChoice.ChoiceId
            });
        }

        double score = (double)correctCount / totalQuestions * 100;
        bool isPassed = score >= 50;

        var submission = new ExamSubmission
        {
            UserId = null,
            ExamId = mathExam.ExamId,
            SubmissionDate = attempt.StartedAt.AddMinutes(random.Next(10, 55)),
            TotalQuestions = totalQuestions,
            CorrectAnswers = correctCount,
            Score = score,
            IsPassed = isPassed,
            StudentName = student.Name,
            StudentEmail = student.Email,
            StudentId = student.Id,
            Answers = userAnswers
        };

        submissions.Add(submission);
    }

    await db.ExamSubmissions.AddRangeAsync(submissions);
    await db.SaveChangesAsync();

    // Update submission IDs in answers
    foreach (var submission in submissions)
    {
        foreach (var answer in submission.Answers)
        {
            answer.SubmissionId = submission.SubmissionId;
        }
    }
    await db.SaveChangesAsync();

    // Update attempt submission IDs
    for (int i = 0; i < submissions.Count; i++)
    {
        invitationAttempts[i].SubmissionId = submissions[i].SubmissionId;
        invitationAttempts[i].CompletedAt = submissions[i].SubmissionDate;
    }
    await db.SaveChangesAsync();

    Console.WriteLine($"✅ Created {submissions.Count} exam submissions");
    Console.WriteLine("✅ Test data seeding completed successfully!");
}

// Helper method to generate invitation code
string GenerateInvitationCode()
{
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    var random = new Random();
    return new string(Enumerable.Repeat(chars, 6)
        .Select(s => s[random.Next(s.Length)]).ToArray());
}
// ✅ Helper method to check if database exists without trying to create it
async Task<bool> CheckDatabaseExistsAsync(ApplicationDbContext db)
{
    try
    {
        // Try to execute a simple query
        await db.Database.ExecuteSqlRawAsync("SELECT 1");
        return true;
    }
    catch (SqlException ex)
    {
        // If database doesn't exist, error number 4060 is returned
        if (ex.Number == 4060) // Database not found
        {
            return false;
        }
        throw;
    }
}

async Task SeedProductionUsersAsync(IServiceScope scope)
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    try
    {
        // =====================================================
        // CREATE ROLES (Only if they don't exist)
        // =====================================================

        string[] roles = { "SuperAdmin", "Admin", "Teacher", "Student" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                Console.WriteLine($"✅ Role '{role}' created.");
            }
            else
            {
                Console.WriteLine($"Role '{role}' already exists.");
            }
        }

        // =====================================================
        // CREATE SUPER ADMIN
        // =====================================================

        var superAdminEmail = "superadmin@examify.com";
        var superAdmin = await userManager.FindByEmailAsync(superAdminEmail);
        if (superAdmin == null)
        {
            superAdmin = new ApplicationUser
            {
                UserName = superAdminEmail,
                Email = superAdminEmail,
                EmailConfirmed = true,
                FullName = "Super Administrator"
            };
            var result = await userManager.CreateAsync(superAdmin, "SuperAdmin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
                await userManager.AddToRoleAsync(superAdmin, "Admin");
                await userManager.AddToRoleAsync(superAdmin, "Teacher");
                Console.WriteLine("✅ Super Admin created: superadmin@examify.com");
            }
            else
            {
                Console.WriteLine("❌ Failed to create Super Admin");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"   Error: {error.Description}");
                }
            }
        }
        else
        {
            Console.WriteLine("Super Admin already exists.");
        }

        // =====================================================
        // CREATE DEMO ADMIN
        // =====================================================

        var demoAdminEmail = "demo@examify.com";
        var demoAdmin = await userManager.FindByEmailAsync(demoAdminEmail);
        if (demoAdmin == null)
        {
            demoAdmin = new ApplicationUser
            {
                UserName = demoAdminEmail,
                Email = demoAdminEmail,
                EmailConfirmed = true,
                FullName = "Demo Administrator"
            };
            var result = await userManager.CreateAsync(demoAdmin, "Demo@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(demoAdmin, "Admin");
                await userManager.AddToRoleAsync(demoAdmin, "Teacher");
                Console.WriteLine("✅ Demo Admin created: demo@examify.com");
            }
        }
        else
        {
            Console.WriteLine("Demo Admin already exists.");
        }

        // =====================================================
        // CREATE DEMO TEACHER
        // =====================================================

        var demoTeacherEmail = "teacher@examify.com";
        var demoTeacher = await userManager.FindByEmailAsync(demoTeacherEmail);
        if (demoTeacher == null)
        {
            demoTeacher = new ApplicationUser
            {
                UserName = demoTeacherEmail,
                Email = demoTeacherEmail,
                EmailConfirmed = true,
                FullName = "Demo Teacher"
            };
            var result = await userManager.CreateAsync(demoTeacher, "Teacher@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(demoTeacher, "Teacher");
                Console.WriteLine("✅ Demo Teacher created: teacher@examify.com");
            }
        }
        else
        {
            Console.WriteLine("Demo Teacher already exists.");
        }

        // =====================================================
        // CREATE DEMO STUDENT
        // =====================================================

        var demoStudentEmail = "student@examify.com";
        var demoStudent = await userManager.FindByEmailAsync(demoStudentEmail);
        if (demoStudent == null)
        {
            demoStudent = new ApplicationUser
            {
                UserName = demoStudentEmail,
                Email = demoStudentEmail,
                EmailConfirmed = true,
                FullName = "Demo Student"
            };
            var result = await userManager.CreateAsync(demoStudent, "Student@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(demoStudent, "Student");
                Console.WriteLine("✅ Demo Student created: student@examify.com");
            }
        }
        else
        {
            Console.WriteLine("Demo Student already exists.");
        }

        // =====================================================
        // CREATE ORIGINAL ADMIN
        // =====================================================

        var originalAdminEmail = "admin@site.com";
        var originalAdmin = await userManager.FindByEmailAsync(originalAdminEmail);
        if (originalAdmin == null)
        {
            originalAdmin = new ApplicationUser
            {
                UserName = originalAdminEmail,
                Email = originalAdminEmail,
                EmailConfirmed = true,
                FullName = "System Administrator"
            };
            var result = await userManager.CreateAsync(originalAdmin, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(originalAdmin, "SuperAdmin");
                await userManager.AddToRoleAsync(originalAdmin, "Admin");
                await userManager.AddToRoleAsync(originalAdmin, "Teacher");
                Console.WriteLine("✅ Original Admin created: admin@site.com");
            }
        }
        else
        {
            Console.WriteLine("Original Admin already exists.");
        }

        Console.WriteLine("✅ User seeding completed.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Seeding warning: {ex.Message}");
    }
}