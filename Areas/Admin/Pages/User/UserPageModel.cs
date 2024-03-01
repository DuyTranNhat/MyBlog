using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Migration_EF.Models;

namespace Migration_EF.Areas.Admin.Pages.User
{
    public class UserPageModel : PageModel
    {
        protected readonly BlogContext _context;
        protected readonly UserManager<AppUser> _userManager;
        protected SignInManager<AppUser> _signInManager;



        [TempData]
        public string StatusMessage { get; set; }

        public UserPageModel(BlogContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
    }
}
