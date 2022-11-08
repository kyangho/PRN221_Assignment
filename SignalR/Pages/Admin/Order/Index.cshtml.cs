using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SignalR.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;

namespace SignalR.Pages.Admin.Order
{
    public class IndexModel : PageModel
    {

        private readonly PRN221DBContext dbContext;

        public IndexModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public List<Models.Order> orderList { get; set; }

        [BindProperty(SupportsGet = true, Name = "currentPage")]
        public int currentPage { get; set; }

        [BindProperty(SupportsGet = true), DataType(DataType.Date)]
        public DateTime? dateFrom { get; set; }
        [BindProperty(SupportsGet = true), DataType(DataType.Date)]
        public DateTime? dateTo { get; set; }

        public int totalPages { get; set; }

        public const int pageSize = 5;

        private static List<Models.Order> listExcelOrders = new List<Models.Order>();

        [BindProperty(SupportsGet = true)]
        public String IsFilter { get; set; }

        public void OnGet()
        {
            if (currentPage < 1 || (IsFilter != null && IsFilter.Equals("true") && currentPage > 1))
            {
                currentPage = 1;
            }
            int totalOrders = getTotalOrders();

            totalPages = (int)Math.Ceiling((double)totalOrders / pageSize);

            orderList = getAllOrders();
        }

        private int getTotalOrders()
        {
            var list = from order in dbContext.Orders orderby order.OrderDate ascending select order;
            if (dateFrom == null && dateTo == null)
            {
                return list.Count();
            }
            else if (dateFrom != null && dateTo == null)
            {
                return list.Where(o => o.OrderDate >= dateFrom).ToList().Count();
            }
            else if (dateFrom == null && dateTo != null)
            {
                return list.Where(o => o.OrderDate <= dateTo).ToList().Count();
            }
            return list.Where(o => o.OrderDate >= dateFrom && o.OrderDate <= dateTo).ToList().Count();

        }

        private List<Models.Order> getAllOrders()
        {
            var list = from order in dbContext.Orders orderby order.OrderDate ascending select order;
            List<Models.Order> orders;
            if (dateFrom == null && dateTo == null)
            {
                orders = list.ToList();
            }
            else if (dateFrom != null && dateTo == null)
            {
                orders = list.Where(o => o.OrderDate >= dateFrom).ToList();
            }
            else if (dateFrom == null && dateTo != null)
            {
                orders = list.Where(o => o.OrderDate <= dateTo)
                    .ToList();
            }
            else
            {
                if (dateFrom != null && dateTo != null)
                {
                    if (DateTime.Compare(dateFrom.Value, dateTo.Value) > 0)
                    {
                        ViewData["msg"] = "Date from must before date to!";
                        return null;
                    }
                }
                orders = list.Where(o => o.OrderDate >= dateFrom && o.OrderDate <= dateTo)
                    .ToList();
            }
            
            foreach (var order in orders)
            {
                var em = dbContext.Employees.FirstOrDefault(e => e.EmployeeId == order.EmployeeId);
                order.Employee = em;
                order.Customer = dbContext.Customers.FirstOrDefault(c => c.CustomerId.Equals(order.CustomerId));
            }
            listExcelOrders = orders;
            orders = orders.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            return orders;
        }

        public async Task<IActionResult> OnGetExport()
        {
            using (ExcelPackage excel = new ExcelPackage(new MemoryStream()))
            {
                var workSheet = excel.Workbook.Worksheets.Add("Order List");
                workSheet.TabColor = System.Drawing.Color.Black;
                workSheet.DefaultRowHeight = 15;
                //Header of table  
                //  
                workSheet.Row(1).Height = 20;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Cells[1, 1].Value = "No";
                workSheet.Cells[1, 2].Value = "OrderID";
                workSheet.Cells[1, 3].Value = "OrderDate";
                workSheet.Cells[1, 4].Value = "RequiredDate";
                workSheet.Cells[1, 5].Value = "ShippedDate";
                workSheet.Cells[1, 6].Value = "Employee";
                workSheet.Cells[1, 7].Value = "Customer";
                workSheet.Cells[1, 8].Value = "Freight($)";
                workSheet.Cells[1, 9].Value = "Status";

                int recordIndex = 2;
                foreach (var order in listExcelOrders)
                {
                    workSheet.Row(recordIndex).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[recordIndex, 1].Value = (recordIndex - 1).ToString();
                    workSheet.Cells[recordIndex, 2].Value = order.OrderId;
                    workSheet.Cells[recordIndex, 3].Value = order.OrderDate.Value.Date.ToString("D");
                    workSheet.Cells[recordIndex, 4].Value = (order.RequiredDate.HasValue) ? order.RequiredDate.Value.Date.ToString("D") : "";
                    workSheet.Cells[recordIndex, 5].Value = (order.ShippedDate.HasValue) ? order.ShippedDate.Value.Date.ToString("D") : "";
                    workSheet.Cells[recordIndex, 6].Value = (order.EmployeeId == null) ? "" : order.Employee.FirstName + " " + order.Employee.LastName;
                    workSheet.Cells[recordIndex, 7].Value = order.Customer.ContactName;
                    workSheet.Cells[recordIndex, 8].Value = order.Freight;
                    if (order.ShippedDate != null)
                    {
                        workSheet.Cells[recordIndex, 9].Value = "Completed";
                    }
                    else if (order.RequiredDate != null && order.ShippedDate == null)
                    {
                        workSheet.Cells[recordIndex, 9].Value = "Pending";
                    }
                    else if (order.RequiredDate == null)
                    {
                        workSheet.Cells[recordIndex, 9].Value = "Canceled";
                    }

                    recordIndex++;
                }

                workSheet.Column(1).AutoFit();
                workSheet.Column(2).AutoFit();
                workSheet.Column(3).AutoFit();
                workSheet.Column(4).AutoFit();
                workSheet.Column(5).AutoFit();
                workSheet.Column(6).AutoFit();
                workSheet.Column(7).AutoFit();
                workSheet.Column(8).AutoFit();
                workSheet.Column(9).AutoFit();
                excel.Save();
                excel.Stream.Position = 0;
                string excelName = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy") + "_OrderList";
                return File(excel.Stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName + ".xlsx");
            }

        }

    }
}
