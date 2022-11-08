using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalR.Models;
using System.Text.Json;

namespace SignalR.Pages.Admin.Order
{
    public class CancelModel : PageModel
    {
        private readonly PRN221DBContext dbContext;

        private readonly IHubContext<HubServer> hubContext;
        public CancelModel(PRN221DBContext context, IHubContext<HubServer> hubContext)
        {
            this.dbContext = context;
            this.hubContext = hubContext;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {

            var orderToUpdate = await dbContext.Orders.FirstAsync(e => e.OrderId == id);

            if (orderToUpdate == null)
            {
                return NotFound();
            }

            orderToUpdate.RequiredDate = null;

            await dbContext.SaveChangesAsync();
            await hubContext.Clients.All.SendAsync("ReloadOrder");
            return Redirect("/Admin/Order/Index");
        }
    }
}
