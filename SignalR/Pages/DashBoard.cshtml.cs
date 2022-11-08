using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SignalR.Models;

namespace SignalR.Pages.Admin
{
    public class DashBoardModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        public DashBoardModel(PRN221DBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public IList<Models.Order> Orders { get; set; }

        public List<Models.OrderDetail> OrderDetails { get; set; }
        public IList<Models.Order> TotalOrder { get; set; }

        public async Task OnGetAsync()
        {
            DateTime now = DateTime.Now;
            DateTime sub = DateTime.Now.AddDays(-7);
            Orders = await dBContext.Orders.Where(x => x.OrderDate >= sub
                            && x.OrderDate <= now).ToListAsync();
            OrderDetails = new List<Models.OrderDetail>();
            foreach(var order in Orders)
            {
                var orderDetail = await dBContext.OrderDetails.Where(od => order.OrderId == od.OrderId).ToListAsync();
                OrderDetails.AddRange(orderDetail);
            }
            TotalOrder = await dBContext.Orders.OrderByDescending(x => x.OrderId).ToListAsync();

            int totalCustomers = await dBContext.Accounts.Where(a => a.CustomerId != null).CountAsync();
            ViewData["totalCustomers"] = totalCustomers;
            int allCustomer = await dBContext.Customers.CountAsync();
            int totalGuest = allCustomer - totalCustomers;
            ViewData["totalGuest"] = totalGuest;
            int newCustomers = await dBContext.Customers.Where(p => EF.Functions.DateDiffDay(p.CreatedDate, DateTime.Now).Value <= 30)
               .CountAsync();
            ViewData["newCustomers"] = newCustomers;
            int totalAllCustomer = await dBContext.Customers.CountAsync();
            ViewData["totalAllCustomer"] = totalAllCustomer;
            int totalOrderInJanuary = await dBContext.Orders.Where(o => o.OrderDate.Value.Month == 1
                               && o.OrderDate.Value.Year == DateTime.Now.Year).CountAsync();
            ViewData["totalOrderInJanuary"] = totalOrderInJanuary;
            int totalOrderInFebruary = await dBContext.Orders.Where(o => o.OrderDate.Value.Month == 2
                                && o.OrderDate.Value.Year == DateTime.Now.Year).CountAsync();
            ViewData["totalOrderInFebruary"] = totalOrderInFebruary;
            int totalOrderInMarch = await dBContext.Orders.Where(o => o.OrderDate.Value.Month == 3
                                && o.OrderDate.Value.Year == DateTime.Now.Year).CountAsync();
            ViewData["totalOrderInMarch"] = totalOrderInMarch;
            int totalOrderInApril = await dBContext.Orders.Where(o => o.OrderDate.Value.Month == 4
                                && o.OrderDate.Value.Year == DateTime.Now.Year).CountAsync();
            ViewData["totalOrderInApril"] = totalOrderInApril;
            int totalOrderInMay = await dBContext.Orders.Where(o => o.OrderDate.Value.Month == 5
                                && o.OrderDate.Value.Year == DateTime.Now.Year).CountAsync();
            ViewData["totalOrderInMay"] = totalOrderInMay;
            int totalOrderInJune = await dBContext.Orders.Where(o => o.OrderDate.Value.Month == 6
                                && o.OrderDate.Value.Year == DateTime.Now.Year).CountAsync();
            ViewData["totalOrderInJune"] = totalOrderInJune;
            int totalOrderInJuly = await dBContext.Orders.Where(o => o.OrderDate.Value.Month == 7
                                && o.OrderDate.Value.Year == DateTime.Now.Year).CountAsync();
            ViewData["totalOrderInJuly"] = totalOrderInJuly;
            int totalOrderInAugust = await dBContext.Orders.Where(o => o.OrderDate.Value.Month == 8
                                && o.OrderDate.Value.Year == DateTime.Now.Year).CountAsync();
            ViewData["totalOrderInAugust"] = totalOrderInAugust;
            int totalOrderInSeptember = await dBContext.Orders.Where(o => o.OrderDate.Value.Month == 9
                                && o.OrderDate.Value.Year == DateTime.Now.Year).CountAsync();
            ViewData["totalOrderInSeptember"] = totalOrderInSeptember;
            int totalOrderInOctober = await dBContext.Orders.Where(o => o.OrderDate.Value.Month == 10
                                && o.OrderDate.Value.Year == DateTime.Now.Year).CountAsync();
            ViewData["totalOrderInOctober"] = totalOrderInOctober;

            int totalOrderInNovember = await dBContext.Orders.Where(o => o.OrderDate.Value.Month == 11
                                && o.OrderDate.Value.Year == DateTime.Now.Year).CountAsync();
            ViewData["totalOrderInNovember"] = totalOrderInNovember;
            int totalOrderInDecember = await dBContext.Orders.Where(o => o.OrderDate.Value.Month == 12
                                && o.OrderDate.Value.Year == DateTime.Now.Year).CountAsync();
            ViewData["totalOrderInDecember"] = totalOrderInDecember;

        }
    }
}
