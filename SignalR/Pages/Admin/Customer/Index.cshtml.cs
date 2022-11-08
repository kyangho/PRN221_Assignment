using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using SignalR.Models;

namespace SignalR.Pages.Admin.Customer
{
    public class IndexModel : PageModel
    {
        private readonly PRN221DBContext dbContext;
        private readonly IHubContext<HubServer> context;

        public IndexModel(PRN221DBContext dBContext, IHubContext<HubServer> context)
        {
            this.dbContext = dBContext;
            this.context = context;
        }

        public List<Models.Customer> customers;

        [BindProperty(SupportsGet = true, Name = "currentPage")]
        public int currentPage { get; set; }

        [BindProperty(SupportsGet = true)]
        public string search { get; set; }
        public int totalPages { get; set; }

        [BindProperty(SupportsGet = true)]
        public String IsSearch { get; set; }

        public const int pageSize = 5;

        public async Task<IActionResult> OnGet()
        {
            if (currentPage < 1 ||(IsSearch != null && IsSearch.Equals("true") && currentPage > 1))
            {
                currentPage = 1;
            }

            int totalOrders = getTotalCustomers();

            totalPages = (int)Math.Ceiling((double)totalOrders / pageSize);

            customers = getAllCustomers();
            return Page();

        }

        private List<Models.Customer> getAllCustomers()
        {
            var list = (from c in dbContext.Customers select c).ToList();

            List<Models.Customer> customers = new List<Models.Customer>();
            customers = list;
            if (!String.IsNullOrEmpty(search))
            {
                customers = customers.Where(c => c.ContactName.ToLower().Contains(search.ToLower()))
                    //.Skip((currentPage - 1) * pageSize).Take(pageSize)
                    .ToList();
            }

            customers = customers.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            return customers;
        }

        private int getTotalCustomers()
        {
            var list = (from c in dbContext.Customers select c).ToList();
            int total = 0;
            if (!String.IsNullOrEmpty(search))
            {
                total += list.Where(c => c.ContactName.ToLower().Contains(search.ToLower())).ToList().Count;
            }
            else
            {
                total = list.Count();
            }
            return total;
        }

        public async Task<IActionResult> OnGetActive(string cid)
        {
            var customer = await dbContext.Customers.FindAsync(cid);
            if (customer == null)
            {
                return NotFound();
            }
            customer.IsActive = true;
            await dbContext.SaveChangesAsync();
            await context.Clients.All.SendAsync("ReloadCustomer");
            return Redirect("~/Admin/Customer/Index");
        }

        public async Task<IActionResult> OnGetDeactive(string cid)
        {
            var customer = await dbContext.Customers.FindAsync(cid);
            if (customer == null)
            {
                return NotFound();
            }
            customer.IsActive = false;
            await dbContext.SaveChangesAsync();
            await context.Clients.All.SendAsync("ReloadCustomer");
            return Redirect("~/Admin/Customer/Index");
        }
    }
}

