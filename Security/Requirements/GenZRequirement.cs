using Microsoft.AspNetCore.Authorization;

namespace Migration_EF.Security.Requirements
{
    public class GenZRequirement : IAuthorizationRequirement
    {
        public int FromYear;
        public int ToYear;

        public GenZRequirement(int fromYear = 1997, int toYear = 2012)
        {
            FromYear = fromYear;
            ToYear = toYear;
        }
    }
}
