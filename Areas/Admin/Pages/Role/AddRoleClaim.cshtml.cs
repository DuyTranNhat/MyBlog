using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Migration_EF.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Migration_EF.Areas.Admin.Pages.Role
{
    public class AddRoleClaimModel : RolePageModel
    {
        private readonly ILogger _logger;
        public AddRoleClaimModel(RoleManager<IdentityRole> roleManager, BlogContext context, ILogger<CreateModel> logger) : base(roleManager, context)
        {
            _logger = logger;
        }

        public class InputModel
        {
            [DisplayName("Role's Type")]
            [Required(ErrorMessage = "{0} is requierd")]
            [StringLength(256, MinimumLength = 2, ErrorMessage = "Lenght of {0} need be in range from {2} to {1} character")]
            public string ClaimType { get; set; }
            [DisplayName("Role's Value")]
            [Required(ErrorMessage = "{0} is requierd")]
            [StringLength(256, MinimumLength = 2, ErrorMessage = "Lenght of {0} need be in range from {2} to {1} character")]
            public string ClaimValue { get; set; }
        }


        [BindProperty]
        public InputModel inputModel { get; set; }

        public IdentityRole role { get; set; }

        public async Task<IActionResult> OnGet(string roleId)
        {
            role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return NotFound($"Cannot find role by id '{roleId}'");
            return Page();
        }


        public async Task<IActionResult> OnPostAsync(string roleId)
        {
            if (roleId == null) return NotFound($"Cannot find role by id '{roleId}'");

            role = await _roleManager.FindByIdAsync(roleId);

            if (role == null) return NotFound($"Cannot find role by id '{roleId}'");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            bool isExist = (await _roleManager.GetClaimsAsync(role)).Any(c => c.Type == inputModel.ClaimType && c.ValueType == inputModel.ClaimValue);


            if (isExist)
            {
                ModelState.AddModelError(string.Empty, "Claim have not been available.");
                return Page();
            }

            var newClaim = new Claim(inputModel.ClaimType, inputModel.ClaimValue);
            var result = await _roleManager.AddClaimAsync(role, newClaim);

            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(error => {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }

            StatusMessage = $"Successfully,Complete add claim for {role.Name}";


           
            return RedirectToPage("./Edit", new { id = role.Id });
        }
    }
}
