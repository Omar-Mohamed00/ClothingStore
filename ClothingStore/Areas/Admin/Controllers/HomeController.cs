//using DataAccess.Data;
//using Microsoft.AspNetCore.Mvc;
//using Models;
//using SQLitePCL;
//using System.Diagnostics;

//namespace ClothingStore.Areas.Admin.Controllers
//{
//    [Area("Admin")]
//    public class HomeController : Controller
//    {
//        private readonly ILogger<HomeController> _logger;
//        private readonly ApplicationDbContext _context;

//        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
//        {
//            _logger = logger;
//            _context = context;
//        }

//        public IActionResult Index()
//        {
//            // 1. ÅÌãÇáí ÇáÃÑÈÇÍ áßá Úãíá (Total Profit per Customer)
//            var profitPerCustomer = _context.CustomerInvoiceLines
//                .Join(_context.Products,
//                    line => line.ProductId,
//                    product => product.Id,
//                    (line, product) => new
//                    {
//                        CustomerInvoiceId = line.customerInvoiceId,
//                        Profit = (product.SellingPrice - product.PurchaserPrice) * line.Quantity
//                    })
//                .Join(_context.CustomerInvoices,
//                    profit => profit.CustomerInvoiceId,
//                    invoice => invoice.Id,
//                    (profit, invoice) => new
//                    {
//                        CustomerId = invoice.CustomerId,
//                        Profit = profit.Profit
//                    })
//                .GroupBy(x => x.CustomerId)
//                .Select(g => new
//                {
//                    CustomerId = g.Key,
//                    TotalProfit = g.Sum(x => x.Profit)
//                })
//                .Join(_context.Customers,
//                    profit => profit.CustomerId,
//                    customer => customer.Id,
//                    (profit, customer) => new
//                    {
//                        CustomerName = customer.Name,
//                        TotalProfit = profit.TotalProfit
//                    })
//                .ToList();

//            // 2. ÅÌãÇáí ÇáÃÑÈÇÍ áßá Úãíá ÔåÑíðÇ (Total Profit per Customer Monthly)
//            var profitPerCustomerMonthly = _context.CustomerInvoicelines
//                .Join(_context.Products,
//                    line => line.ProductId,
//                    product => product.Id,
//                    (line, product) => new
//                    {
//                        CustomerInvoiceId = line.CustomerInvoiceId,
//                        Profit = (product.SellingPrice - product.PurchaserPrice) * line.Quantity,
//                        Date = _context.CustomerInvoices
//                            .Where(invoice => invoice.Id == line.CustomerInvoiceId)
//                            .Select(invoice => invoice.Date)
//                            .FirstOrDefault()
//                    })
//                .Join(_context.CustomerInvoices,
//                    profit => profit.CustomerInvoiceId,
//                    invoice => invoice.Id,
//                    (profit, invoice) => new
//                    {
//                        CustomerId = invoice.CustomerId,
//                        Profit = profit.Profit,
//                        Year = profit.Date.Year,
//                        Month = profit.Date.Month
//                    })
//                .GroupBy(x => new { x.CustomerId, x.Year, x.Month })
//                .Select(g => new
//                {
//                    CustomerId = g.Key.CustomerId,
//                    Year = g.Key.Year,
//                    Month = g.Key.Month,
//                    TotalProfit = g.Sum(x => x.Profit)
//                })
//                .Join(_context.Customers,
//                    profit => profit.CustomerId,
//                    customer => customer.Id,
//                    (profit, customer) => new
//                    {
//                        CustomerName = customer.Name,
//                        Year = profit.Year,
//                        Month = profit.Month,
//                        TotalProfit = profit.TotalProfit
//                    })
//                .ToList();

//            // 3. ÅÌãÇáí ÇáãÈíÚÇÊ áßá Úãíá (Total Sales per Customer)
//            var salesPerCustomer = _context.CustomerInvoices
//                .GroupBy(i => i.CustomerId)
//                .Select(g => new
//                {
//                    CustomerId = g.Key,
//                    TotalSales = g.Sum(i => i.Total)
//                })
//                .Join(_context.Customers,
//                    sales => sales.CustomerId,
//                    customer => customer.Id,
//                    (sales, customer) => new
//                    {
//                        CustomerName = customer.Name,
//                        TotalSales = sales.TotalSales
//                    })
//                .ToList();

//            // 4. ÅÌãÇáí ÇáãÈíÚÇÊ áßá Úãíá ÔåÑíðÇ (Total Sales per Customer Monthly)
//            var salesPerCustomerMonthly = _context.CustomerInvoices
//                .GroupBy(i => new { i.CustomerId, i.Date.Year, i.Date.Month })
//                .Select(g => new
//                {
//                    CustomerId = g.Key.CustomerId,
//                    Year = g.Key.Year,
//                    Month = g.Key.Month,
//                    TotalSales = g.Sum(i => i.Total)
//                })
//                .Join(_context.Customers,
//                    sales => sales.CustomerId,
//                    customer => customer.Id,
//                    (sales, customer) => new
//                    {
//                        CustomerName = customer.Name,
//                        Year = sales.Year,
//                        Month = sales.Month,
//                        TotalSales = sales.TotalSales
//                    })
//                .ToList();

//            // 5. ßãíÉ ÇáãäÊÌÇÊ ÇáãÊÈÞíÉ (Remaining Product Quantity)
//            var remainingProducts = _context.Products
//                .Select(p => new
//                {
//                    ProductName = p.Name,
//                    RemainingQuantity = p.Quantity
//                })
//                .ToList();

//            // Pass data to the view
//            ViewBag.ProfitPerCustomer = profitPerCustomer;
//            ViewBag.ProfitPerCustomerMonthly = profitPerCustomerMonthly;
//            ViewBag.SalesPerCustomer = salesPerCustomer;
//            ViewBag.SalesPerCustomerMonthly = salesPerCustomerMonthly;
//            ViewBag.RemainingProducts = remainingProducts;

//            return View();
//        }

//        public IActionResult Privacy()
//        {
//            return View();
//        }

//        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//        public IActionResult Error()
//        {
//            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
//        }
//    }
//}
