using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SignalR.Models;
using SignalR.Helper;

namespace SignalR.Pages.Account
{
    public class SignUpModel : PageModel
    {
        private readonly PRN221DBContext dbContext;

        public SignUpModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [BindProperty]
        public Customer Customer { get; set; }

        [BindProperty]
        public SignalR.Models.Account Account { get; set; }

        [BindProperty, Required(ErrorMessage = "Re-password is required")]
        public string RePassword { get; set; }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (RePassword != Account.Password)
            {
                ViewData["msg-repassword"] = "Re-password not match";
                return Page();
            }

            var acc = await dbContext.Accounts.SingleOrDefaultAsync(a => a.Email.Equals(Account.Email));

            if ( acc != null)
            {
                ViewData["msg"] = "This email is exist";
                return Page();
            }

            var cust = new Customer()
            {
                CustomerId = RandomCustID(5),
                CompanyName = Customer.CompanyName,
                ContactName = Customer.ContactName,
                ContactTitle = Customer.ContactTitle,
                Address = Customer.Address,
            };

            var newAcc = new Models.Account()
            {
                Email = Account.Email,
                Password = HashPasswordHepler.HashPassword(Account.Password),
                CustomerId = cust.CustomerId,
                CreatedDate = DateTime.Now,
                IsActive = false,
                Role = 2,
            };

            await dbContext.Customers.AddAsync(cust);
            await dbContext.Accounts.AddAsync(newAcc);
            await dbContext.SaveChangesAsync();

            var body = $@"<p>Click below link to active: </p><br/><a href='http://localhost:5236/Account/Active?email={Account.Email}'>http:/localhost:5236/Account/ResetPassword?email={Account.Email}</a>";
            MailHelper.SendMail(Account.Email, body);

            return RedirectToPage("/index");
        }

        private string RandomCustID(int length)
        {
            // creating a StringBuilder object()
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();
            char letter;
            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }
            return str_build.ToString();
        }
    }
}
