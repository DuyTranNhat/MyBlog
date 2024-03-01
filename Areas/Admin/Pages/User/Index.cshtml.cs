using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Migration_EF.Models;

namespace Migration_EF.Areas.Admin.Pages.User
{
    public class IndexModel : UserPageModel
    {

        public IndexModel(BlogContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) : base(context, userManager, signInManager)
        {
        }

        public const int ITEMS_PER_PAGE = 10;
        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { get; set; }
        public int countPages { get; set; }
        public int totalUsers { get; set; }


        public List<UserAndRoles> users { get; set; }

        public class UserAndRoles : AppUser
        {
            public string userRoles { get; set; }
        }

        public async Task<IActionResult> OnGet()
        {
            //users = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();
            var qr = _userManager.Users.OrderBy(u => u.UserName);
            totalUsers = await qr.CountAsync();
            countPages = (int)Math.Ceiling((double)totalUsers / ITEMS_PER_PAGE);

            if(currentPage < 1) currentPage = 1;
            if(currentPage >  countPages) currentPage = countPages;

            

            var qr1 = qr.Skip((currentPage - 1) * ITEMS_PER_PAGE)
                        .Take(ITEMS_PER_PAGE)
                        .Select(u => new UserAndRoles()
                        {
                            Id = u.Id,
                            UserName = u.UserName,
                        });
                        
            

            users = await qr1.ToListAsync();

            foreach (var user in users)
            {
                var rolesOfUser = await _userManager.GetRolesAsync(user);
                user.userRoles = string.Join(", ", rolesOfUser);
            }   

            return Page();
        }
    }
}
