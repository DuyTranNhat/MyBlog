using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Migration_EF.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Migration_EF.Areas.Admin.Pages.Role
{
    public class EditRoleClaimModel : RolePageModel
    {
        private readonly ILogger _logger;
        public EditRoleClaimModel(RoleManager<IdentityRole> roleManager, BlogContext context, ILogger<CreateModel> logger) : base(roleManager, context)
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

        IdentityRoleClaim<string> claim { get; set; }

        public async Task<IActionResult> OnGet(int? claimID)
        {
            if (claimID == null) 
                return NotFound($"Cannot Find Claim with ID = '{claimID}'");

            claim = _context.RoleClaims.Where(c => c.Id == claimID).FirstOrDefault();

            inputModel = new InputModel()
            {
                ClaimType = claim.ClaimType,
                ClaimValue = claim.ClaimValue,
            };

            return Page();

        }


        public async Task<IActionResult> OnPost(int? claimID)
        {
            if(claimID == null)
                return NotFound($"Cannot Find Claim with ID = '{claimID}'");

            claim = _context.RoleClaims.Where(c => c.Id == claimID).FirstOrDefault();

            if (claim == null)
                return NotFound($"Cannot Find Claim with ID = '{claimID}'");

            var role = await _roleManager.FindByIdAsync(claim.RoleId);
                                                                                   
            if (!ModelState.IsValid)
            {
                return Page();
            }

            claim.ClaimType = inputModel.ClaimType;
            claim.ClaimValue = inputModel.ClaimValue;

            await _context.SaveChangesAsync();

            StatusMessage = $"You've already update claim with ID = {claimID}";

            //if (result.Succeeded)
            //{
            //    _logger.LogInformation(result.ToString());
            //    StatusMessage = $"You've already create the new role '{inputModel.Name}'";
            //    return RedirectToPage("./Index");
            //}
            //else
            //{
            //    result.Errors.ToList().ForEach(error =>
            //    {
            //        ModelState.AddModelError(string.Empty, error.Description);
            //        _logger.LogInformation($"My Error: {error.Description}");
            //    });
            //}

            return RedirectToPage("./Edit", new { id = role.Id });
        }
    }
}
