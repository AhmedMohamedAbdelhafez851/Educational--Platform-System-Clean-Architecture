//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.ViewFeatures;
//using Moq;
//using OnlineExamSystem.Application.Abstraction;
//using OnlineExamSystem.Domains.Entities;
//using OnlineExamSystem.Web.Controllers;
//using OnlineExamSystem.Web.ViewModels;
//using Xunit;

//namespace OnlineExamSystem.Tests.ControllersTests
//{
//    public class QuestionControllerTests : IDisposable
//    {
//        private readonly Mock<IQuestionService> _mockQuestionService = new();
//        private readonly Mock<IExamService> _mockExamService = new();
//        private readonly QuestionController _controller;

//        public QuestionControllerTests()
//        {
//            // Mock TempData
//            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

//            _controller = new QuestionController(_mockQuestionService.Object, _mockExamService.Object)
//            {
//                TempData = tempData
//            };
//        }

//        [Fact]
//        public async Task Index_InvalidExamId_RedirectsToExamIndex()
//        {
//            // Act
//            var result = await _controller.Index(null);

//            // Assert
//            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
//            Assert.Equal("Index", redirectResult.ActionName);
//            Assert.Equal("Exam", redirectResult.ControllerName);
//        }

//        [Fact]
//        public async Task Index_ValidExamId_ReturnsViewWithQuestions()
//        {
//            var exam = new Exam { ExamId = 1, Title = "Test Exam" };
//            var questions = new List<Question>
//    {
//        new Question { ExamId = 1, Title = "Q1" },
//        new Question { ExamId = 1, Title = "Q2" }
//    };

//            _mockExamService.Setup(s => s.GetExamByIdAsync(1))
//                .ReturnsAsync(exam);
//            _mockQuestionService.Setup(s => s.GetQuestionsByExamAsync(1))
//                .ReturnsAsync(questions);

//            // Act
//            var result = await _controller.Index(1);

//            // Assert
//            var viewResult = Assert.IsType<ViewResult>(result);
//            var model = Assert.IsAssignableFrom<List<Question>>(viewResult.Model);
//            Assert.Equal(2, model.Count);
//            Assert.Equal("Test Exam", viewResult.ViewData["ExamTitle"]);
//        }
//        [Fact]
//        public void Add_GET_ReturnsViewWithEmptyQuestion()
//        {
//            // Arrange
//            var exam = new Exam { ExamId = 1, Title = "Test Exam" };
//            _mockExamService.Setup(s => s.GetExamByIdAsync(1))
//                .ReturnsAsync(exam);

//            // Act
//            var result = _controller.Add(1);

//            // Assert
//            var viewResult = Assert.IsType<ViewResult>(result);
//            var model = Assert.IsType<QuestionViewModel>(viewResult.Model);
//            Assert.Equal(4, model.Choices.Count);
//            Assert.Equal("Test Exam", model.ExamTitle);
//        }

//        [Fact]
//        public async Task Add_POST_InvalidModel_ReturnsViewWithErrors()
//        {
//            // Arrange
//            var model = new QuestionViewModel
//            {
//                ExamId = 1,
//                Choices = { new ChoiceViewModel(), new ChoiceViewModel() } // Only 2 choices
//            };

//            _controller.ModelState.AddModelError("Title", "Required");

//            // Act
//            var result = await _controller.Add(model);

//            // Assert
//            var viewResult = Assert.IsType<ViewResult>(result);
//            Assert.False(_controller.ModelState.IsValid);
//            Assert.Equal(2, model.Choices.Count);
//        }

//        [Fact]
//        public async Task Edit_GET_InvalidId_ReturnsNotFound()
//        {
//            // Arrange
//            _mockQuestionService.Setup(s => s.GetQuestionWithChoicesByIdAsync(999))
//                .ReturnsAsync((Question?)null);

//            // Act
//            var result = await _controller.Edit(999, 1);

//            // Assert
//            Assert.IsType<NotFoundResult>(result);
//        }
//        [Fact]
//        public async Task Edit_POST_ValidModel_RedirectsToIndex()
//        {
//            var exam = new Exam { ExamId = 1, Title = "Test Exam" };
//            var question = new Question
//            {
//                QuestionId = 1,
//                ExamId = 1,
//                Title = "Original",
//                Choices = new List<Choice>
//        {
//            new Choice { ChoiceId = 1, Text = "A" },
//            new Choice { ChoiceId = 2, Text = "B" },
//            new Choice { ChoiceId = 3, Text = "C" },
//            new Choice { ChoiceId = 4, Text = "D" }
//        }
//            };

//            _mockQuestionService.Setup(s => s.GetQuestionWithChoicesByIdAsync(1))
//                .ReturnsAsync(question);
//            _mockExamService.Setup(s => s.GetExamByIdAsync(1))
//                .ReturnsAsync(exam);

//            // Mock service with proper tracking handling
//            _mockQuestionService.Setup(s => s.EditQuestionAsync(It.IsAny<Question>(), It.IsAny<List<Choice>>()))
//                .ReturnsAsync((Question q, List<Choice> c) => (true, q));

//            // Create model with valid data
//            var model = new QuestionViewModel
//            {
//                QuestionId = 1,
//                ExamId = 1,
//                Title = "Updated Question",
//                CorrectChoiceIndex = 0,
//                Choices = question.Choices.Select((c, i) => new ChoiceViewModel
//                {
//                    ChoiceId = c.ChoiceId,
//                    Text = c.Text,
//                    IsCorrect = i == 0
//                }).ToList()
//            };

//            // Act
//            var result = await _controller.Edit(model);

//            // Assert
//            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
//            Assert.Equal("Index", redirectResult.ActionName);
//            Assert.Equal(1, redirectResult.RouteValues!["examId"]);
//        }
//        [Fact]
//        public async Task Delete_ValidId_ReturnsSuccessJson()
//        {
//            // Arrange
//            _mockQuestionService.Setup(s => s.DeleteQuestionAsync(1))
//                .ReturnsAsync(true);

//            // Act
//            var result = await _controller.Delete(1, 1);

//            // Assert
//            var jsonResult = Assert.IsType<JsonResult>(result);
//            var success = jsonResult.Value!.GetType().GetProperty("success")?.GetValue(jsonResult.Value);
//            var message = jsonResult.Value.GetType().GetProperty("message")?.GetValue(jsonResult.Value);

//            Assert.True((bool)success!);
//            Assert.Equal("Question deleted successfully", message);
//        }

//        public void Dispose()
//        {
//            GC.SuppressFinalize(this);
//        }
//    }
//}
