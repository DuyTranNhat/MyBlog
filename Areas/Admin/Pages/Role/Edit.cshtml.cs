using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Migration_EF.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static Migration_EF.Areas.Admin.Pages.Role.EditModel;

namespace Migration_EF.Areas.Admin.Pages.Role
{
    [Authorize(Policy = "AllowEditRole")]
    public class EditModel : RolePageModel
    {
        private readonly ILogger _logger;
        public EditModel(RoleManager<IdentityRole> roleManager, BlogContext context, ILogger<CreateModel> logger) : base(roleManager, context)
        {
            _logger = logger;
        }

        public class InputModelEdit
        {
            [DisplayName("Role's Name")]
            [Required(ErrorMessage = "{0} is requierd")]
            [StringLength(256, MinimumLength = 2, ErrorMessage = "Lenght of {0} need be in range from {2} to {1} character")]
            public string Name { get; set; }
        }

        public IdentityRole role { get; set; }

        [BindProperty(SupportsGet = true)]
        public InputModelEdit inputModelEdit { get; set; }

        public List<IdentityRoleClaim<string>> Claims { get; set; }

        public async Task<bool> LoadAsync(string id)
        {
            role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                inputModelEdit = new InputModelEdit()
                {
                    Name = role.Name
                };
                Claims = await _context.RoleClaims.Where(rc => rc.RoleId == role.Id).ToListAsync();
                return true;
            }
            return false;
        }


        public async Task<IActionResult> OnGet(string id)
        {
            if (id == null)
                return NotFound($"Cannot find role {id}");


            var role = await _roleManager.FindByIdAsync(id);

            if (role != null)
            {
                await LoadAsync(id);
                return Page();
            }

            return NotFound($"Cannot find role {id}");

        }


        public async Task<IActionResult> OnPostAsync(string id)
        {
            //    var role = await _roleManager.FindByIdAsync(id);

            //    if (role == null)
            //    {
            //        var message = $"Cannot find role having id{id}";
            //        return Page();
            //    }

            //    if (!ModelState.IsValid)
            //    {
            //        return Page();
            //    }

            //    var newRole = new IdentityRole(inputModel.Name);
            //    var result = await _roleManager.CreateAsync(newRole);

            //    if (result.Succeeded)
            //    {
            //        _logger.LogInformation(result.ToString());
            //        StatusMessage = $"You've already create the new role '{inputModel.Name}'";
            //        return RedirectToPage("./Index");
            //    } else
            //    {
            //        result.Errors.ToList().ForEach(error =>
            //        {
            //            ModelState.AddModelError(string.Empty, error.Description);
            //            _logger.LogInformation($"My Error: {error.Description}");
            //        });
            //    }

            //    return Page();


            if (id == null)
                return NotFound($"Cannot find role {id}");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            role = await _roleManager.FindByIdAsync(id);
            role.Name = inputModelEdit.Name;
            Claims = await _context.RoleClaims.Where(rc => rc.RoleId == role.Id).ToListAsync();



            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                StatusMessage = "You've already update role's name";

                return RedirectToPage("./index");
            }
            else
            {
                result.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
            }
            return Page();

        }
    }
}

