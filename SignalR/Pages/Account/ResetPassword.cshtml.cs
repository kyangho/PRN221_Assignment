using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SignalR.Models;
using SignalR.Helper;
namespace PRNAssignment.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly PRN221DBContext dbContext;

        public ResetPasswordModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [BindProperty]
        public string NewPassword { get; set; }
        [BindProperty]
        public string RePassword { get; set; }
        [BindProperty]
        public string Email { get; set;}
        public void OnGet(string email)
        {
            Email = email;
        }
        public async Task<IActionResult> OnPost()
        {
            if (RePassword != NewPassword)
            {
                ViewData["msg-repassword"] = "Re-password not match";
                return Page();
            }
            var acc = await dbContext.Accounts.SingleOrDefaultAsync(a => a.Email.Equals(Email));
            Console.WriteLine(acc);

            try
            {
                acc.Password = HashPasswordHepler.HashPassword(NewPassword);
                dbContext.Accounts.Update(acc);
                await dbContext.SaveChangesAsync();
                return RedirectToPage("/Account/Login");
            }
            catch (Exception e)
            {
                ViewData["errMsg"] = "Can not change password";
                Console.WriteLine(e.StackTrace);
                return Page();
            }
        }
    }
}
