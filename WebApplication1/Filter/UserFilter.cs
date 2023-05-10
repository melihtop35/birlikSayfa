using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApplication1.Filter
{
    public class UserFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            int? userId = context.HttpContext.Session.GetInt32("Id");
            if (!userId.HasValue)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    {"action","Index" },
                    {"controller","Account" }
                });
            }
            base.OnActionExecuting(context);
        }
    }
}
