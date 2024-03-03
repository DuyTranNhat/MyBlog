

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Migration_EF.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Migration_EF.Areas.Admin.Pages.Role
{
    
    public class DeleteModel : RolePageModel
    {
        private readonly ILogger _logger;

        public IdentityRole role { get; set; }

        public DeleteModel(RoleManager<IdentityRole> roleManager, BlogContext context, ILogger<CreateModel> logger) : base(roleManager, context)
        {
            _logger = logger;
        }


        public async Task<IActionResult> OnGet(string id)
        {
            if (id == null)
                return NotFound($"Cannot find role {id}");

            role = await _roleManager.FindByIdAsync(id);

            if  (role == null) return NotFound($"Cannot find role {id}");

            return Page();

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


            //if (id == null)
            //    return NotFound($"Cannot find role {id}");

            //if (!ModelState.IsValid)
            //{
            //    await LoadAsync(id);
            //    return Page();
            //}

            //role = await _roleManager.FindByIdAsync(id);
            //role.Name = inputModelEdit.Name;

            //var result = await _roleManager.UpdateAsync(role);
            //if (result.Succeeded) {
            //    StatusMessage = "You've already update role's name";

            //    return RedirectToPage("./index");
            //} 
            //else
            //{
            //    result.Errors.ToList().ForEach(error =>
            //    {
            //        ModelState.AddModelError(string.Empty, error.Description);
            //    });
            //}
            //return Page();

            if (id == null)
            {
                return NotFound($"Cannot delete, role with {id} isn't exist!");
            }

            role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound($"Cannot delete, role with {id} isn't exist!");
            }

            var resultl = await _roleManager.DeleteAsync(role);
            if (resultl.Succeeded)
            {
                StatusMessage = $"Succesfully! You've already delete role having ID '{id}'.";
                return RedirectToPage("./index");
            } else
            {
                resultl.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return RedirectToPage("./index");
            }
        }
    }
}
