//using FluentValidation;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;

//namespace OnlineExamSystem.Web.Filters
//{
//    public class ValidationExceptionFilter : IExceptionFilter
//    {
//        public void OnException(ExceptionContext context)
//        {
//            if (context.Exception is ValidationException ex)
//            {
//                var controller = context.Controller as Controller;

//                if (controller != null)
//                {
//                    foreach (var error in ex.Errors)
//                    {
//                        controller.ModelState.AddModelError(
//                            error.PropertyName,
//                            error.ErrorMessage);
//                    }

//                    // 🔥 Return the same view with current model
//                    context.Result = new ViewResult
//                    {
//                        ViewName = context.RouteData.Values["action"]?.ToString(),
//                        ViewData = controller.ViewData,
//                        TempData = controller.TempData
//                    };

//                    context.ExceptionHandled = true;
//                }
//            }
//        }
//    }
//}