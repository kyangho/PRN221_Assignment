using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalR.Models;

namespace PRNAssignment.Pages.Account
{
    public class ActiveModel : PageModel
    {
        private readonly PRN221DBContext dbContext;

        public ActiveModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IActionResult> OnGet(string email)
        {
            var acc = dbContext.Accounts.SingleOrDefault(a => a.Email.Equals(email));
            if (acc != null)
            {
                var cus = dbContext.Customers.SingleOrDefault(c => c.CustomerId.Equals(acc.CustomerId));
                cus.IsActive = true;

                dbContext.Update(cus);
                await dbContext.SaveChangesAsync();
                return Page();
            } else
            {
                return RedirectToPage("/index");
            }
        }
    }
}
