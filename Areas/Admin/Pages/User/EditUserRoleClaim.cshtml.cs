using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Migration_EF.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace Migration_EF.Areas.Admin.Pages.User
{
    public class EditUserRoleClaimModel : UserPageModel
    {
        public EditUserRoleClaimModel(BlogContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<EditUserRoleClaimModel> logger) : base(context, userManager, signInManager)
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
        public AppUser user { get; set; }
        public IdentityUserClaim<string> userClaim { get; set; }
        ILogger<EditUserRoleClaimModel> _logger;
        public bool StateDelete = false;


        public async Task<IActionResult> OnGetAddClaim(string userID)
        {
            user = await _userManager.FindByIdAsync(userID);
            if (user == null)
                return NotFound($"Unable to load user with ID '{userID}'.");

            return Page();
        }

        public async Task<IActionResult> OnPostAddClaim(string userID)
        {
            user = await _userManager.FindByIdAsync(userID);
            if (user == null)
                return NotFound($"Unable to load user with ID '{userID}'.");

            if (!ModelState.IsValid) return Page();

            var claims = _context.UserClaims.Where(u => u.UserId == userID);



            if (claims.Any(c => c.ClaimType == inputModel.ClaimType && c.ClaimValue == inputModel.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, "Error, This claim've been existed!");
                return Page();
            }

            await _userManager.AddClaimAsync(user, new Claim(inputModel.ClaimType, inputModel.ClaimValue));
            StatusMessage = $"You've already add new claim for user '{user.UserName}'";


            return RedirectToPage("./AddRole", new { id = user.Id });
        }



        public async Task<IActionResult> OnGetEditClaim(int? claimid)
        {
            if(claimid == null)
                return NotFound($"Unable to load claim with ID '{claimid}'.");

            userClaim = _context.UserClaims.Where(uc => uc.Id == claimid).FirstOrDefault();
            if (userClaim == null)
                return NotFound($"Unable to load claim with ID '{claimid}'.");

            user = await _userManager.FindByIdAsync(userClaim.UserId);

            inputModel = new InputModel()
            {
                ClaimType = userClaim.ClaimType,
                ClaimValue = userClaim.ClaimValue,
            };

            return Page();
        }

        public async Task<IActionResult> OnPostEditClaim(int? claimid)
        {
            if (claimid == null)
                return NotFound($"Unable to load claim with ID '{claimid}'.");

            userClaim = _context.UserClaims.Where(uc => uc.Id == claimid).FirstOrDefault();

            if (userClaim == null)
                return NotFound($"Unable to load claim with ID '{claimid}'.");

            user = await _userManager.FindByIdAsync(userClaim.UserId);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            bool isExisted = _context.UserClaims.Where(uc => uc.ClaimType == inputModel.ClaimType && uc.ClaimValue == inputModel.ClaimValue).Any();

            if (isExisted)
            {

                ModelState.AddModelError(string.Empty, "Error! Claim you just type that have been existed, Please type another values.");
                return Page();
            }

            userClaim.ClaimValue = inputModel.ClaimValue;
            userClaim.ClaimType = inputModel.ClaimType;

            StatusMessage = $"You've already update claim for user '{user.UserName}'";
            _context.SaveChanges();
            return RedirectToPage("./AddRole", new { id = user.Id });
        }

        public async Task<IActionResult> OnGetDeleteClaim(int? claimid)
        {
            if (claimid == null)
                return NotFound($"Unable to load claim with ID '{claimid}'.");

            userClaim = _context.UserClaims.Where(uc => uc.Id == claimid).FirstOrDefault();
            if (userClaim == null)
                return NotFound($"Unable to load claim with ID '{claimid}'.");

            user = await _userManager.FindByIdAsync(userClaim.UserId);

            inputModel = new InputModel()
            {
                ClaimType = userClaim.ClaimType,
                ClaimValue = userClaim.ClaimValue,
            };

            StateDelete = true;

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteClaim(int? claimid)
        {
            if (claimid == null)
                return NotFound($"Unable to load claim with ID '{claimid}'.");

            userClaim = _context.UserClaims.Where(uc => uc.Id == claimid).FirstOrDefault();

            if (userClaim == null)
                return NotFound($"Unable to load claim with ID '{claimid}'.");

            user = await _userManager.FindByIdAsync(userClaim.UserId);

            var resultDelete = await _userManager.RemoveClaimAsync(user, new Claim(userClaim.ClaimType, userClaim.ClaimValue));

            if (!resultDelete.Succeeded)
            {
                resultDelete.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }

            StatusMessage = $"Successfully! You've just delete Claim of user '{user.UserName}'";
            return RedirectToPage("./AddRole", new { id = user.Id });
        }
    }
}
