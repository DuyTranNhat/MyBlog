using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Migration_EF.Models;
using System.Security.Claims;

namespace Migration_EF.Security.Requirements
{
    public class AppAuthorizationHandler : IAuthorizationHandler
    {
        private readonly ILogger<AppAuthorizationHandler> _logger;
        private readonly UserManager<AppUser> _userManager;

        public AppAuthorizationHandler(ILogger<AppAuthorizationHandler> logger, UserManager<AppUser> userManager) { 
             _logger = logger;
            _userManager = userManager;
        }
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var requirements = context.PendingRequirements.ToList();
            foreach (var requirement in requirements)
            {
                if (requirement is GenZRequirement)
                {
                    if(IsGenZ(context.User, requirement))
                    {
                        context.Succeed(requirement);
                    }
                } 
                if (requirement is ArticleUpdateRequirement) {
                    bool canUpdate =  CanUpdateArticle(context.User, context.Resource, (ArticleUpdateRequirement)requirement);
                    if (canUpdate)
                    {
                        context.Succeed(requirement);
                    }
                }
            }
            return Task.CompletedTask;
        }

        private bool CanUpdateArticle(ClaimsPrincipal user, object? resource, ArticleUpdateRequirement requirement)
        {
            if (user.IsInRole("Admin"))
                return true;
            var article = resource as Article;
            var publishDate = article.PublishDate;
            var nowDate = new DateTime(requirement.currentDate.Year, 
                requirement.currentDate.Month, requirement.currentDate.Day);
            var diffDays = (nowDate - publishDate).TotalDays;
            if (diffDays > 3) {
                return false;
            }
            return true;
        }

        private bool IsGenZ(ClaimsPrincipal user, IAuthorizationRequirement requirement)
        {
            var appUserTask = _userManager.GetUserAsync(user);
            Task.WaitAll(appUserTask);
            var appUser = appUserTask.Result;

            if (appUser == null)
                return false;

            int year = appUser.BirthDate.Value.Year;

            GenZRequirement requirementGenz = (GenZRequirement)requirement;

            var isSuccess = (year >= requirementGenz.FromYear && year <= requirementGenz.ToYear);

            return isSuccess;

        }
    }
}
