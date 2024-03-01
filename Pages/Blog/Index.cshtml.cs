using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Migration_EF.Models;

namespace Migration_EF.Pages_Blog
{
    public class IndexModel : PageModel
    {
        private readonly Migration_EF.Models.BlogContext _context;

        public IndexModel(Migration_EF.Models.BlogContext context)
        {
            _context = context;
        }

        public IList<Article> Article { get;set; } = default!;

        public const int ITEMS_PER_PAGE = 20 ;
        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { get; set; }
        public int countPages { get; set; }

        public async Task OnGetAsync(string searchInput)
        {

            int totalArticle = await _context.articals.CountAsync();
            countPages = (int)Math.Ceiling((double)totalArticle / ITEMS_PER_PAGE);

            if (currentPage < 1) 
                currentPage = 1;
            if (currentPage > countPages) 
                currentPage = countPages;

            var rs = (from c in _context.articals
                      orderby c.PublishDate descending
                      select c)
                      .Skip(ITEMS_PER_PAGE * (currentPage - 1))
                      .Take(ITEMS_PER_PAGE);

            if (string.IsNullOrEmpty(searchInput))
            {
                Article = await rs.ToListAsync();
            }
            else
            {
                Article = await rs.Where(a => a.Title.Contains(searchInput)).ToListAsync();
            }
        }
    }
}
