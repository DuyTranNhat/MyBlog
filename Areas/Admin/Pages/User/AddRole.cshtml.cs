 // Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Migration_EF.Areas.Admin.Pages.User;
using Migration_EF.Models;

namespace Migration_EF.Areas.Users
{
    public class AddRoleModel : UserPageModel
    {
        private readonly ILogger _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AddRoleModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            BlogContext blogContext,
            ILogger<SetPasswordModel> logger,
            RoleManager<IdentityRole> roleManager) : base(blogContext, userManager, signInManager)

        {
            _logger = logger;
            _roleManager = roleManager;
            _logger.LogInformation("loger again");
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
    

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
       

        public AppUser user { get; set; }

        [DisplayName("Asign roles for user")]
        [BindProperty]

        public string[] RoleNames { get; set; }

        public SelectList allRoles { get; set; }

        public List<IdentityRoleClaim<string>> claimsInRole { get; set; }
        public List<IdentityUserClaim<string>> claimsInUserClaims { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if(string.IsNullOrEmpty(id)) return NotFound($"Cannot find user having '{id}'");

            user = await _userManager.FindByIdAsync(id);

            //_logger.LogInformation("Onget");

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{id}'.");
            }

            RoleNames = (await _userManager.GetRolesAsync(user)).ToArray();

            //var hasPassword = await _userManager.HasPasswordAsync(user);

            //if (hasPassword)
            //{
            //    return Content("Has password");
            //}

            List<string> roleNames = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            allRoles = new SelectList(roleNames);

            var listRoles = from r in _context.Roles
                            join ur in _context.UserRoles on r.Id equals ur.RoleId
                            where ur.UserId == id
                            select r;

            var _claimInRole = from rc in _context.RoleClaims
                               join r in listRoles on rc.RoleId equals r.Id
                               select rc;

            claimsInRole = await _claimInRole.ToListAsync();

            claimsInUserClaims = await (from uc in _context.UserClaims where uc.UserId == id select uc).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound($"Cannot find user having '{id}'");

            user = await _userManager.FindByIdAsync(id);

            //_logger.LogInformation("Onget");

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{id}'.");
            }

            var oldRoles = (await _userManager.GetRolesAsync(user)).ToArray();

            //_logger.LogInformation("_____________OLD ROLES_______________");
            //foreach(var role in oldRoles)
            //{
            //    _logger.LogInformation(role);
            //}

            //_logger.LogInformation("_____________Choosen ROLES_______________");
            //foreach (var role in RoleNames)
            //{
            //    _logger.LogInformation(role);
            //}

            var deleteRoles = oldRoles.Where(r => !RoleNames.Contains(r));
            var addRoles = RoleNames.Where(r => !oldRoles.Contains(r));

            var resultDelete = await _userManager.RemoveFromRolesAsync(user, deleteRoles);
            if(!resultDelete.Succeeded)
            {
                resultDelete.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }

            var resultAdd = await _userManager.AddToRolesAsync(user, addRoles);

            if(!resultAdd.Succeeded)
            {
                resultAdd.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }

            StatusMessage = $"You've already update roles for user '{user.UserName}'";
            return RedirectToPage("./index");

            //var deleteRoles = RoleNames.ToList().ForEach(r => r.Contains())

            //_logger.LogInformation("OnPost");
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            //user = await _userManager.FindByIdAsync(id);

            //if (user == null)
            //{
            //    return NotFound($"Unable to load user with ID '{id}'.");
            //}

            //var addPasswordResult = await _userManager.AddPasswordAsync(user, Input.NewPassword);
            //if (!addPasswordResult.Succeeded)
            //{
            //    foreach (var error in addPasswordResult.Errors)
            //    {
            //        ModelState.AddModelError(string.Empty, error.Description);
            //    }
            //    return Page();
            //}

            //StatusMessage = $"You've just update password for user {user.UserName}";

            //return RedirectToPage("./Index");
        }
    }
}
