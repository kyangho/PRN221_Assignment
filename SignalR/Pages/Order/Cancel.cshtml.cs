using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SignalR.Models;
using System.Text.Json;

namespace SignalR.Pages.Order
{
    public class CancelModel : PageModel
    {
        private readonly PRN221DBContext dbContext;

        public CancelModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (HttpContext.Session.GetString("CustSession") == null)
            {
                return Redirect("/Account/Login");
            }

            var Auth = JsonSerializer.Deserialize<Models.Account>(HttpContext.Session.GetString("CustSession"));

            if (Auth == null)
            {
                return Redirect("/Account/Login");
            }
            if (id == null || id == 0)
            {
                string customerId = Auth.CustomerId;

                var orders = dbContext.Orders.Where(o => o.CustomerId == customerId).ToList();
                if (orders == null)
                {
                    return Redirect("/Order/Index");
                }
                dbContext.Orders.RemoveRange(orders);
                foreach (var order in orders)
                {
                    var details = dbContext.OrderDetails.Where(od => od.OrderId == order.OrderId).ToList();
                    dbContext.OrderDetails.RemoveRange(details);
                }
                await dbContext.SaveChangesAsync();
            }
            else
            {
                var orderToUpdate = await dbContext.Orders.FirstAsync(e => e.OrderId == id);

                if (orderToUpdate == null)
                {
                    return NotFound();
                }

                orderToUpdate.RequiredDate = null;

                await dbContext.SaveChangesAsync();
            }
            return Redirect("/Order/Index");
        }
    }
}
