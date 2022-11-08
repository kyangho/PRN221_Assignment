using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.Json;
using SignalR.Models;

namespace SignalR.Pages.Cart
{
    public class AddModel : PageModel
    {
        private readonly PRN221DBContext dbContext;

        public AddModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [BindProperty]
        public SignalR.Models.Account Auth { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (HttpContext.Session.GetString("CustSession") == null)
            {
                return Redirect("/Account/Login");
            }

            Auth = JsonSerializer.Deserialize<SignalR.Models.Account>(HttpContext.Session.GetString("CustSession"));

            if (Auth == null)
            {
                return Redirect("/Account/Login");
            }

            SignalR.Models.Product product = (await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == id));

            if (product == null || product.UnitsInStock == 0)
            {
                TempData["fail"] = "Quantity = 0";
            }
            else

                try
                {
                    Dictionary<int, int> list;
                    var session = HttpContext.Session.GetString("cart");

                    if (session == null)
                    {
                        list = new Dictionary<int, int>();
                    }
                    else
                    {
                        list = JsonSerializer.Deserialize<Dictionary<int, int>>(session);
                    }

                    int value = 0;

                    if (list.ContainsKey((int) id))
                    {
                        value = list.GetValueOrDefault((int)id, 0);
                        list.Remove((int)id);
                    }
                    list.Add((int)id, ++value);
                        

                    HttpContext.Session.SetString("cart", JsonSerializer.Serialize(list));
                    TempData["success"] = "Add to cart successfull";
                }
                catch (Exception e)
                {
                    TempData["fail"] = e.Message;
                }

            return Redirect("/Product/Detail/" + id);
        }
    }
}
