using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SignalR.Models;
namespace SignalR.Pages.Account
{
    public class CartModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        public CartModel(PRN221DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        [BindProperty]
        public Customer Customer { get; set; }
        //[BindProperty(SupportsGet = false)]
        public Dictionary<SignalR.Models.Product, int> Products { get; set; } = new Dictionary<SignalR.Models.Product, int>();
        public async Task<IActionResult> OnGet()
        {
            Dictionary<int, int> listOrders = new Dictionary<int, int>();
            if (HttpContext.Session.GetString("ListOrders") != null)
            {
                listOrders = JsonSerializer.Deserialize<Dictionary<int, int>>(HttpContext.Session.GetString("ListOrders"));
                foreach (KeyValuePair<int, int> kv in listOrders)
                {
                    var product = await dBContext.Products.FindAsync(kv.Key);
                    Products.Add(product, kv.Value);
                }
            }
            string jsonAccount = HttpContext.Session.GetString("CustSession");
            if (jsonAccount != null)
            {
                Models.Account acc = JsonSerializer.Deserialize<Models.Account>(jsonAccount);
                if (acc != null)
                {
                    Customer = acc.Customer;
                } else
                {
                    return RedirectToPage("/Account/Login");
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                SignalR.Models.Order order = await dBContext.Orders.FirstAsync();
                order.OrderId = 0;
                order.CustomerId = Customer.CustomerId;
                order.OrderDate = DateTime.Now;
                order.ShippedDate = DateTime.Now.AddDays(10);
                order.RequiredDate = DateTime.Now.AddDays(new Random().Next(7));
                await dBContext.Orders.AddAsync(order);
                await dBContext.SaveChangesAsync();
                Dictionary<int, int> listOrders = new Dictionary<int, int>();
                if (HttpContext.Session.GetString("ListOrders") != null)
                {
                    listOrders = JsonSerializer.Deserialize<Dictionary<int, int>>(HttpContext.Session.GetString("ListOrders"));
                    foreach (KeyValuePair<int, int> kv in listOrders)
                    {
                        var product = await dBContext.Products.FindAsync(kv.Key);
                        Products.Add(product, kv.Value);
                    }
                }
                List<OrderDetail> orderDetails = new List<OrderDetail>();
                foreach(var p in Products)
                {
                    orderDetails.Add(new OrderDetail {
                        OrderId = order.OrderId,
                        Discount = 0,
                        Product = p.Key,
                        ProductId = p.Key.ProductId,
                        Order = order,
                        Quantity = (short)p.Value,
                        UnitPrice = p.Key.UnitPrice ?? 0,
                    });
                }
                await dBContext.OrderDetails.AddRangeAsync(orderDetails);
                await dBContext.SaveChangesAsync();
                ViewData["msg"] = "Order successfully";
                listOrders = new Dictionary<int, int>();
                HttpContext.Session.SetInt32("OrderCount", 0);
                HttpContext.Session.SetString("ListOrders", JsonSerializer.Serialize<Dictionary<int, int>>(listOrders));
                return RedirectToPage("/account/listorders");
            }
            else
            {
                return Page();
            }
        }
    }
}
