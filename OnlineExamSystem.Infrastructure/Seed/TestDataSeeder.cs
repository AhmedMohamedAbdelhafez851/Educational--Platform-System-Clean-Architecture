using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineExamSystem.Domains.Entities;
using OnlineExamSystem.Domains.Entities.OnlineExamSystem.Domains.Entities;
using OnlineExamSystem.Infrastructure.Persistence;

namespace OnlineExamSystem.Infrastructure.Seed
{
    public static class TestDataSeeder
    {
        public static async Task SeedTestDataAsync(IServiceScope scope)
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

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
            // CREATE TEST STUDENTS
            // =====================================================

            var testStudents = new List<(string Name, string Email, string Id)>
            {
                ("أحمد محمد", "ahmed@test.com", "STU001"),
                ("سارة أحمد", "sara@test.com", "STU002"),
                ("محمد علي", "mohamed@test.com", "STU003"),
                ("فاطمة حسن", "fatma@test.com", "STU004"),
                ("عمر خالد", "omar@test.com", "STU005")
            };

        //    var studentRole = await roleManager.FindByNameAsync("Student");
            var invitationAttempts = new List<ExamInvitationAttempt>();
            var submissions = new List<ExamSubmission>();
            var random = new Random();

            foreach (var (name, email, studentId) in testStudents)
            {
                var invitation = invitations.First();

                // Create invitation attempt
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

            // =====================================================
            // CREATE SUBMISSIONS AND ANSWERS
            // =====================================================

            var mathQuestionsList = await db.Questions.Where(q => q.ExamId == mathExam.ExamId).Include(q => q.Choices).ToListAsync();

            for (int i = 0; i < testStudents.Count; i++)
            {
                var student = testStudents[i];
                var attempt = invitationAttempts[i];

                // Calculate score (different scores for each student)
                int correctCount = 0;
                int totalQuestions = mathQuestionsList.Count;
                var userAnswers = new List<UserAnswer>();

                foreach (var question in mathQuestionsList)
                {
                    bool isCorrect = random.Next(0, 2) == 1; // Random correct/wrong
                    if (isCorrect) correctCount++;

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
                    UserId = null, // Anonymous student
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

            // =====================================================
            // CREATE SUBMISSIONS FOR OTHER EXAMS
            // =====================================================

            var csExam = exams[1];
            var csQuestions = new List<Question>
            {
                new Question { Title = "ما هو نوع البيانات الصحيح لتخزين الأرقام العشرية في C#؟", ExamId = csExam.ExamId },
                new Question { Title = "أي من التالي يستخدم لتكرار الأوامر في C#؟", ExamId = csExam.ExamId },
                new Question { Title = "ما هو الفهرس الأول في المصفوفة؟", ExamId = csExam.ExamId }
            };
            await db.Questions.AddRangeAsync(csQuestions);
            await db.SaveChangesAsync();

            var csChoices = new List<Choice>();
            // Q1 choices
            csChoices.Add(new Choice { Text = "int", IsCorrect = false, QuestionId = csQuestions[0].QuestionId });
            csChoices.Add(new Choice { Text = "string", IsCorrect = false, QuestionId = csQuestions[0].QuestionId });
            csChoices.Add(new Choice { Text = "double", IsCorrect = true, QuestionId = csQuestions[0].QuestionId });
            csChoices.Add(new Choice { Text = "bool", IsCorrect = false, QuestionId = csQuestions[0].QuestionId });

            // Q2 choices
            csChoices.Add(new Choice { Text = "if", IsCorrect = false, QuestionId = csQuestions[1].QuestionId });
            csChoices.Add(new Choice { Text = "for", IsCorrect = true, QuestionId = csQuestions[1].QuestionId });
            csChoices.Add(new Choice { Text = "switch", IsCorrect = false, QuestionId = csQuestions[1].QuestionId });
            csChoices.Add(new Choice { Text = "break", IsCorrect = false, QuestionId = csQuestions[1].QuestionId });

            // Q3 choices
            csChoices.Add(new Choice { Text = "0", IsCorrect = true, QuestionId = csQuestions[2].QuestionId });
            csChoices.Add(new Choice { Text = "1", IsCorrect = false, QuestionId = csQuestions[2].QuestionId });
            csChoices.Add(new Choice { Text = "-1", IsCorrect = false, QuestionId = csQuestions[2].QuestionId });
            csChoices.Add(new Choice { Text = "null", IsCorrect = false, QuestionId = csQuestions[2].QuestionId });

            await db.Choices.AddRangeAsync(csChoices);
            await db.SaveChangesAsync();

            // Update correct choice IDs
            for (int i = 0; i < csQuestions.Count; i++)
            {
                csQuestions[i].CorrectChoiceId = csChoices.First(c => c.QuestionId == csQuestions[i].QuestionId && c.IsCorrect).ChoiceId;
            }
            await db.SaveChangesAsync();

            Console.WriteLine("✅ Test data seeding completed successfully!");
        }

        private static string GenerateInvitationCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}