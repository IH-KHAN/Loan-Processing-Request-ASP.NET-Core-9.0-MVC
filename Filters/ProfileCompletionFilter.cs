using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using Loan_Processing_Inzamam.Models;
using System.Threading.Tasks;

namespace Loan_Processing_Inzamam.Filters
{
    public class ProfileCompletionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;

            // 1. Only check authenticated users
            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                var controllerName = context.RouteData.Values["controller"]?.ToString();
                var actionName = context.RouteData.Values["action"]?.ToString();
                var areaName = context.RouteData.Values["area"]?.ToString();

                // 2. Do not intercept the Identity pages (Login/Logout) or the CreateProfile page itself
                if (areaName == "Identity" || (controllerName == "Client" && actionName == "CreateProfile"))
                {
                    await next();
                    return;
                }

                // 3. Get the UserManager to check the database flag
                var userManager = context.HttpContext.RequestServices.GetService(typeof(UserManager<ApplicationUser>)) as UserManager<ApplicationUser>;

                if (userManager != null)
                {
                    var appUser = await userManager.GetUserAsync(user);

                    // 4. If the user exists and hasn't completed their profile, redirect them
                    if (appUser != null && !appUser.IsProfileComplete)
                    {
                        context.Result = new RedirectToActionResult("CreateProfile", "Client", null);
                        return;
                    }
                }
            }

            // Let the request continue normally if all checks pass
            await next();
        }
    }
}