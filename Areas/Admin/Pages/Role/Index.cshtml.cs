using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Migration_EF.Models;

namespace Migration_EF.Areas.Admin.Pages.Role
{
    public class IndexModel : RolePageModel
    {
        public IndexModel(RoleManager<IdentityRole> roleManager, BlogContext context) : base(roleManager, context)
        {
        }
        
        public class RoleModel : IdentityRole
        {
            public string[] claims { get; set; }
        }

        public List<RoleModel> roles { get; set; }
                
        public async Task<IActionResult> OnGet()
        {
            //roles = await _roleManager.Roles.ToListAsync();
            //return Page();
            var r = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
            roles = new List<RoleModel>();

            foreach (var _r in r)
            {
                var claims = await _roleManager.GetClaimsAsync(_r);   
                var claimsStrs = claims.Select(c => c.Type + "=" + c.Value).ToArray();

                var rm = new RoleModel
                {
                    Name = _r.Name,
                    Id = _r.Id,
                    claims = claimsStrs
                };
                    roles.Add(rm);
            }

            return Page();
            
        }


        public void OnPost() => RedirectToPage();
    }
}
