using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SignalR.Models;

namespace SignalR.Pages.Product
{
    public class IndexModel : PageModel
    {
        private readonly PRN221DBContext dbContext;

        public IndexModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public List<Category> Categories { get; set; }
        public List<Models.Product> products { get; set; }

        [BindProperty(SupportsGet = true, Name = "currentPage")]
        public int currentPage { get; set; }

        [BindProperty(SupportsGet = true)]
        public int categoryChoose { get; set; }

        [BindProperty(SupportsGet = true)]
        public string search { get; set; }
        public int totalPages { get; set; }

        public const int pageSize = 5;

        public static List<Models.Product> listExcelProduct = new List<Models.Product>();

        [BindProperty(SupportsGet = true)]
        public String IsFilter { get; set; }

        public void OnGet()
        {
            if (currentPage < 1 || (IsFilter != null && IsFilter.Equals("true") && currentPage > 1))
            {
                currentPage = 1;
            }

            int totalProducts = getTotalProducts();

            totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            products = getAllProducts();

            //listExcelProductToFilter
            //listExcelProduct = products;

            Categories = dbContext.Categories.ToList();
        }

        private List<Models.Product> getAllProducts()
        {
            var list = (from p in dbContext.Products select p).ToList();

            List<Models.Product> products = new List<Models.Product>();
            products = list;
            if (categoryChoose > 0)
            {
                products = products.Where(p => p.CategoryId == categoryChoose)
                    .ToList();
            }
            if (!String.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.ProductName.ToLower().Contains(search.ToLower()))
                    //.Skip((currentPage - 1) * pageSize).Take(pageSize)
                    .ToList();
            }

            products = products.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            foreach (var product in products)
            {
                product.Category = dbContext.Categories.FirstOrDefault(c => c.CategoryId == product.CategoryId);
            }
            return products;
        }

        private int getTotalProducts()
        {
            var list = (from p in dbContext.Products select p).ToList();
            int total = 0;
            if (categoryChoose > 0)
            {
                total += list.Where(p => p.CategoryId == categoryChoose).ToList().Count;
            }
            else if (!String.IsNullOrEmpty(search))
            {
                total += list.Where(p => p.ProductName.ToLower().Contains(search.ToLower())).ToList().Count;
            }
            if (categoryChoose > 0 && !String.IsNullOrEmpty(search))
            {
                total = list.Where(p => p.CategoryId == categoryChoose && p.ProductName.ToLower().Contains(search.ToLower())).ToList().Count;
            }
            if (categoryChoose <= 0 && String.IsNullOrEmpty(search))
            {
                total = list.Count;
            }
            return total;
        }

        public async Task<IActionResult> OnGetExport()
        {
            listExcelProduct = dbContext.Products.ToList();
            using (ExcelPackage excel = new ExcelPackage(new MemoryStream()))
            {
                var workSheet = excel.Workbook.Worksheets.Add("Product List");
                workSheet.TabColor = System.Drawing.Color.Black;
                workSheet.DefaultRowHeight = 15;
                //Header of table  
                //  
                workSheet.Row(1).Height = 20;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Cells[1, 1].Value = "No";
                workSheet.Cells[1, 2].Value = "ProductID";
                workSheet.Cells[1, 3].Value = "ProductName";
                workSheet.Cells[1, 4].Value = "UnitPrice";
                workSheet.Cells[1, 5].Value = "Unit";
                workSheet.Cells[1, 6].Value = "UnitsInStock";
                workSheet.Cells[1, 7].Value = "Category";
                workSheet.Cells[1, 8].Value = "Discontinued";

                int recordIndex = 2;
                foreach (var product in listExcelProduct)
                {
                    product.Category = dbContext.Categories.FirstOrDefault(c => c.CategoryId == product.CategoryId);
                    workSheet.Row(recordIndex).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[recordIndex, 1].Value = (recordIndex - 1).ToString();
                    workSheet.Cells[recordIndex, 2].Value = product.ProductId;
                    workSheet.Cells[recordIndex, 3].Value = product.ProductName;
                    workSheet.Cells[recordIndex, 4].Value = product.UnitPrice;
                    workSheet.Cells[recordIndex, 5].Value = product.QuantityPerUnit;
                    workSheet.Cells[recordIndex, 6].Value = product.UnitsInStock;
                    workSheet.Cells[recordIndex, 7].Value = product.Category.CategoryName;
                    workSheet.Cells[recordIndex, 8].Value = (product.Discontinued  == true) ? "True" : "False";
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
                excel.Save();
                excel.Stream.Position = 0;
                string excelName = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy") + "_ProductList";
                return File(excel.Stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName + ".xlsx");
            }
            
        }
    }
}
