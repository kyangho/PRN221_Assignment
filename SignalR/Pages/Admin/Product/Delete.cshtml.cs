using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalR.Models;

namespace SignalR.Pages.Admin.Product
{
    public class DeleteModel : PageModel
    {
        private readonly PRN221DBContext _context;

        private readonly IHubContext<HubServer> hubContext;
        public DeleteModel(PRN221DBContext context, IHubContext<HubServer> hubContext)
        {
            _context = context;
            this.hubContext = hubContext;
        }

        [BindProperty]
        public Models.Product Product { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                Product = await _context.Products.FindAsync(id);

                if (Product != null)
                {
                    _context.Products.Remove(Product);
                    await _context.SaveChangesAsync();
                    await hubContext.Clients.All.SendAsync("ReloadProduct");
                }
            }
            catch (Exception ex)
            {
                HttpContext.Session.SetString("DeleteMessage", "Cannot Delete Product!!!");
                return Redirect("./Index");
            }

            return RedirectToPage("./Index");
        }
    }
}
