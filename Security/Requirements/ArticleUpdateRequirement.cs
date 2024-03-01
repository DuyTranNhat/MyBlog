using Microsoft.AspNetCore.Authorization;

namespace Migration_EF.Security.Requirements
{
    public class ArticleUpdateRequirement : IAuthorizationRequirement
    {
        public DateTime currentDate { get; set; }   

        public ArticleUpdateRequirement()
        {
            currentDate = DateTime.Now;
        }
    }
}
