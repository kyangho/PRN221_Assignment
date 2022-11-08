using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
namespace SignalR.Controllers.Cart
{
    public class CartController : Controller
    {
        [Route("cart/add/{id?}")]
        public async Task<IActionResult> Add(int id)
        {
            if (HttpContext.Session.GetString("CustSession") == null)
            {
                return RedirectToPage("/Account/Login");
            }
            else
            {
                try
                {
                    Dictionary<int, int> listOrders = new Dictionary<int, int>();
                    int? count = 0;
                    if (HttpContext.Session.GetString("ListOrders") == null)
                    {
                        HttpContext.Session.SetString("ListOrders", JsonSerializer.Serialize<Dictionary<int, int>>(listOrders));
                        HttpContext.Session.SetInt32("OrderCount", 0);
                    }
                    else
                    {
                        count = HttpContext.Session.GetInt32("OrderCount") ?? 0;
                        listOrders = JsonSerializer.Deserialize<Dictionary<int, int>>(HttpContext.Session.GetString("ListOrders"));
                    }

                    if (listOrders.Keys.Any(e => e == id))
                    {
                        int value = listOrders[id];
                        listOrders[id] = value + 1;
                    }
                    else
                    {
                        listOrders.Add(id, 1);
                    }
                    HttpContext.Session.SetString("ListOrders", JsonSerializer.Serialize<Dictionary<int, int>>(listOrders));
                    count++;
                    HttpContext.Session.SetInt32("OrderCount", count ?? 0);
                }
                catch { }
                return RedirectToPage("/index");
            }
        }
        [Route("cart/increase/{id?}")]
        public async Task<IActionResult> Increase(int id)
        {
            if (HttpContext.Session.GetString("CustSession") == null)
            {
                return RedirectToPage("/Account/Login");
            }
            else
            {
                try
                {
                    Dictionary<int, int> listOrders = new Dictionary<int, int>();
                    int? count = 0;
                    if (HttpContext.Session.GetString("ListOrders") == null)
                    {
                        HttpContext.Session.SetString("ListOrders", JsonSerializer.Serialize<Dictionary<int, int>>(listOrders));
                        HttpContext.Session.SetInt32("OrderCount", 0);
                    }
                    else
                    {
                        count = HttpContext.Session.GetInt32("OrderCount") ?? 0;
                        listOrders = JsonSerializer.Deserialize<Dictionary<int, int>>(HttpContext.Session.GetString("ListOrders"));
                    }

                    if (listOrders.Keys.Any(e => e == id))
                    {
                        int value = listOrders[id];
                        listOrders[id] = value + 1;
                    }
                    else
                    {
                        listOrders.Add(id, 1);
                    }
                    HttpContext.Session.SetString("ListOrders", JsonSerializer.Serialize<Dictionary<int, int>>(listOrders));
                    count++;
                    HttpContext.Session.SetInt32("OrderCount", count ?? 0);
                }
                catch { }
            }
            return RedirectToPage("/account/cart");
        }
        [Route("cart/remove/{id?}")]
        public async Task<IActionResult> Remove(int id)
        {
            if (HttpContext.Session.GetString("CustSession") == null)
            {
                RedirectToPage("/Account/Login");
            }
            else
            {
                try
                {
                    Dictionary<int, int> listOrders = new Dictionary<int, int>();
                    int? count = 0;
                    if (HttpContext.Session.GetString("ListOrders") == null)
                    {
                        HttpContext.Session.SetString("ListOrders", JsonSerializer.Serialize<Dictionary<int, int>>(listOrders));
                        HttpContext.Session.SetInt32("OrderCount", 0);
                    }
                    else
                    {
                        count = HttpContext.Session.GetInt32("OrderCount") ?? 0;
                        listOrders = JsonSerializer.Deserialize<Dictionary<int, int>>(HttpContext.Session.GetString("ListOrders"));
                    }

                    if (listOrders.Keys.Any(e => e == id))
                    {
                        int value = listOrders[id];
                        listOrders.Remove(id);
                        count = count - value;
                    }
                    HttpContext.Session.SetString("ListOrders", JsonSerializer.Serialize<Dictionary<int, int>>(listOrders));
                    HttpContext.Session.SetInt32("OrderCount", count ?? 0);
                }
                catch { }
            }
            return RedirectToPage("/account/cart");
        }
        [Route("cart/decrease/{id?}")]
        public async Task<IActionResult> Decrease(int id)
        {
            if (HttpContext.Session.GetString("CustSession") == null)
            {
                return RedirectToPage("/Account/Login");
            }
            else
            {
                try
                {
                    Dictionary<int, int> listOrders = new Dictionary<int, int>();
                    int? count = 0;
                    if (HttpContext.Session.GetString("ListOrders") == null)
                    {
                        HttpContext.Session.SetString("ListOrders", JsonSerializer.Serialize<Dictionary<int, int>>(listOrders));
                        HttpContext.Session.SetInt32("OrderCount", 0);
                    }
                    else
                    {
                        count = HttpContext.Session.GetInt32("OrderCount") ?? 0;
                        listOrders = JsonSerializer.Deserialize<Dictionary<int, int>>(HttpContext.Session.GetString("ListOrders"));
                    }

                    if (listOrders.Keys.Any(e => e == id))
                    {
                        int value = listOrders[id];
                        if (listOrders[id] > 0)
                        {
                            listOrders[id] = value - 1;
                            count--;
                        }
                    }
                    HttpContext.Session.SetString("ListOrders", JsonSerializer.Serialize<Dictionary<int, int>>(listOrders));
                    HttpContext.Session.SetInt32("OrderCount", count ?? 0);
                }
                catch { }
            }
            return RedirectToPage("/account/cart");
        }
    }
}
