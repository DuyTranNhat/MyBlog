using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Migration_EF.Models;

namespace Migration_EF.Pages_Blog
{
    public class EditModel : PageModel
    {
        private readonly Migration_EF.Models.BlogContext _context;

        private readonly IAuthorizationService _authorizationService;
        private ILogger<EditModel> _loggger;

        public EditModel(Migration_EF.Models.BlogContext context, IAuthorizationService authorizationService, ILogger<EditModel> loggger)
        {
            _context = context;
            _authorizationService = authorizationService;
            _loggger = loggger;
        }   

        [BindProperty]
        public Article Article { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Article =  await _context.articals.FirstOrDefaultAsync(m => m.ID == id);

            if (Article == null)
            {
                return NotFound();
            }

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Article).State = EntityState.Modified;

            try
            {
                var canUpdate = await _authorizationService.AuthorizeAsync(this.User, Article, "CanUpdateArticle");
                _loggger.LogError("--------------------");

                if (canUpdate.Succeeded)
                {
                    await _context.SaveChangesAsync();
                } else
                {
                    return Content("You cannot update article over 3 days!");
                }

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(Article.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ArticleExists(int id)
        {
            return _context.articals.Any(e => e.ID == id);
        }
    }
}
