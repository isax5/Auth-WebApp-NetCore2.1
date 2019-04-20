using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AuthWebAppNetCore.Web.Helpers
{
    /// <summary>
    /// Personalized NotFound pages
    /// </summary>
    public class NotFoundViewResult : ViewResult
    {
        public NotFoundViewResult(string viewName)
        {
            ViewName = viewName;
            StatusCode = (int)HttpStatusCode.NotFound;
        }
    }
}
