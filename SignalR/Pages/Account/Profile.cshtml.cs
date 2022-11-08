using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SignalR.Models;
using System.Text.Json;

namespace SignalR.Pages.Account
{
    [Authorize(Roles = "Customer")]
    public class ProfileModel : PageModel
    {
        private readonly PRN221DBContext dbContext;

        [BindProperty]
        public SignalR.Models.Account Auth { get; set; }

        public ProfileModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        

        public async Task<IActionResult> OnGetAsync()
        {
            var currentUser = HttpContext.User;
            if (HttpContext.Session.GetString("CustSession") != null)
            {
                Auth = JsonSerializer.Deserialize<Models.Account>(HttpContext.Session.GetString("CustSession"));

                if (Auth == null)
                {
                    return NotFound();
                }
                else
                {
                    Auth.Customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == Auth.CustomerId);
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                Models.Account auth = JsonSerializer.Deserialize<Models.Account>(HttpContext.Session.GetString("CustSession"));

                var acc = await dbContext.Accounts.FirstOrDefaultAsync(a => a.AccountId == auth.AccountId && a.Email != Auth.Email);
                if(acc == null)
                {
                    ViewData["fail"] = "Email is existed";
                    return Page();
                }

                if (acc.CustomerId != null)
                {
                    acc.Customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == acc.CustomerId);
                }

                acc.Email = Auth.Email;
                acc.Customer.CompanyName = Auth.Customer.CompanyName;
                acc.Customer.ContactName = Auth.Customer.ContactName;
                acc.Customer.ContactTitle = Auth.Customer.ContactTitle;
                acc.Customer.Address = Auth.Customer.Address;

                auth.Email = Auth.Email;

                dbContext.Accounts.Update(acc);
                await dbContext.SaveChangesAsync();

                HttpContext.Session.Remove("CustSession");
                HttpContext.Session.SetString("CustSession", JsonSerializer.Serialize(auth));

                ViewData["success"] = "Update Successfull";
                return Page();

            } catch (Exception e)
            {
                ViewData["fail"] = e.Message;
                return Page();
            }

        }

    }
}
