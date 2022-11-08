using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SignalR.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using SignalR.Helper;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SignalR.Services;
namespace SignalR.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly PRN221DBContext dbContext;
        private readonly IConfiguration _config;
        private string generatedToken = null;
        public LoginModel(PRN221DBContext dbContext, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            _config = configuration;
        }

        [BindProperty]
        public SignalR.Models.Account Account { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Console.WriteLine(Account.Password);
            Console.WriteLine(HashPasswordHepler.HashPassword(Account.Password));
            var acc = await dbContext.Accounts
                .SingleOrDefaultAsync(a => a.Email.Equals(Account.Email) && a.Password.Equals(HashPasswordHepler.HashPassword(Account.Password)));

            if (acc == null)
            {
                ViewData["msg"] = "Email/ Password is wrong";
                return Page();
            } else
            {
                //var scheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                //identity.AddClaim(new Claim(ClaimTypes.Role, acc.Role == 1 ? "Admin" : "Customer"));
                //var principal = new ClaimsPrincipal(identity);
                //var claims = new List<Claim>()
                //{
                //    new Claim(ClaimTypes.Role, acc.Role == 1 ? "Admin" : "Customer")
                //};
                //var claimsIdentity = new ClaimsIdentity(claims, "ducky");
                //await HttpContext.SignInAsync(
                //        scheme,
                //        new ClaimsPrincipal(claimsIdentity

                generatedToken = TokenService.BuildToken(_config["Jwt:Secret"].ToString(), _config["Jwt:ValidIssuer"].ToString(), acc);
                if (generatedToken != null)
                {
                    HttpContext.Session.SetString("Token", generatedToken);
                }
            }

            var customer = await dbContext.Customers.SingleOrDefaultAsync(c => c.CustomerId == acc.CustomerId);

            if (customer != null && !customer.IsActive.Value)
            {
                ViewData["msg"] = "Your Account Cannot Login Into System";
                return Page();
            }

            HttpContext.Session.SetString("CustSession", JsonSerializer.Serialize(acc, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles }));
            if (acc.Role == 1)
            {
                HttpContext.Session.SetString("IsAdmin", "Admin");
                return Redirect("/DashBoard");
            }

            return RedirectToPage("/index");
        }

        private string GenerateJSONWebToken(Models.Account userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
                new Claim("Email", userInfo.Email),
                new Claim("Password", userInfo.Password),
                new Claim("Roles", userInfo.Role == 1 ? "Admin" : "Customer"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            Console.WriteLine(_config["JWT:Secret"]);
            var token = new JwtSecurityToken(issuer: _config["JWT:ValidIssuer"],
              audience: _config["JWT:ValidAudience"],
              claims: claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public IActionResult OnGetLogout()
        {
            HttpContext.Session.Remove("CustSession");

            return RedirectToPage("/index");
        }
    }
}
