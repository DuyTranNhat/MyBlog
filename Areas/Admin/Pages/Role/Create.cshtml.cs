using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Migration_EF.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Migration_EF.Areas.Admin.Pages.Role
{
    public class CreateModel : RolePageModel
    {
        private readonly ILogger _logger;
        public CreateModel(RoleManager<IdentityRole> roleManager, BlogContext context, ILogger<CreateModel> logger) : base(roleManager, context)
        {
            _logger = logger;
        }

        public class InputModel
        {
            [DisplayName("Role's Name")]
            [Required(ErrorMessage = "{0} is requierd")]
            [StringLength(256, MinimumLength = 2, ErrorMessage = "Lenght of {0} need be in range from {2} to {1} character")]
            public string Name { get; set; }
        }


        [BindProperty]
        public InputModel inputModel { get; set; }

        public void OnGet()
        {

        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) { 
                return Page();
            }
            var newRole = new IdentityRole(inputModel.Name);
            var result = await _roleManager.CreateAsync(newRole);

            if (result.Succeeded)
            {
                _logger.LogInformation(result.ToString());
                StatusMessage = $"You've already create the new role '{inputModel.Name}'";
                return RedirectToPage("./Index");
            } else
            {
                result.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    _logger.LogInformation($"My Error: {error.Description}");
                });
            }

            return Page();
        }
    }
}
