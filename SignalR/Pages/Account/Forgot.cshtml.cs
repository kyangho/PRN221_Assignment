using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalR.Models;
using System.Net.Mail;
using System.ComponentModel.DataAnnotations;
using SignalR.Helper;
namespace SignalR.Pages.Account
{
    public class ForgotModel : PageModel
    {
        private readonly PRN221DBContext dbContext;

        public ForgotModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [Required(ErrorMessage = "Email is required")]
        [BindProperty,DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var account = dbContext.Accounts.FirstOrDefault(a => a.Email == Email);
                if(account == null)
                {
                    ViewData["errMsg"] = "Email is not existed";
                    return Page();
                }
                var body = $@"<p>Click below link to change password: </p><br/><a href='http://localhost:5236/Account/ResetPassword?email={Email}'>http:/localhost:5236/Account/ResetPassword?email={Email}</a>";
                MailHelper.SendMail(Email, body);
            }
            return Redirect("/Account/Login");
        }
    }
}
