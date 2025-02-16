using DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Models;
using SQLitePCL;
using System.Diagnostics;

namespace ClothingStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
            // Chart 1: ÅÌãÇáí ÇáÃÑÈÇÍ (Total Profit)
            var totalProfit = _context.CustomerInvoiceLines
                .Join(_context.Products,
                    line => line.ProductId,
                    product => product.Id,
                    (line, product) => (product.SellingPrice - product.PurchasePrice) * line.Quantity)
                .Sum();

            // Chart 2: ÅÌãÇáí ÇáÃÑÈÇÍ áßá ÔåÑ (Total Profit Monthly)
            var profitPerMonthly = _context.CustomerInvoiceLines
                .Join(_context.Products,
                    line => line.ProductId,
                    product => product.Id,
                    (line, product) => new
                    {
                        line.customerInvoiceId,
                        Profit = (product.SellingPrice - product.PurchasePrice) * line.Quantity
                    })
                .Join(_context.CustomerInvoices,
                    profit => profit.customerInvoiceId,
                    invoice => invoice.Id,
                    (profit, invoice) => new { invoice.Date, profit.Profit })
                .GroupBy(x => new { x.Date.Year, x.Date.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalProfit = g.Sum(x => x.Profit)
                })
                .ToList();

            // Chart 3: ÅÌãÇáí ÇáãÈíÚÇÊ (Total Sales)
            var totalSales = _context.CustomerInvoices.Sum(i => i.Total);

            // Chart 4: ÅÌãÇáí ÇáãÈíÚÇÊ áßá ÔåÑ (Total Sales Monthly)
            var salesPerMonth = _context.CustomerInvoices
                .GroupBy(i => new { i.Date.Year, i.Date.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalSales = g.Sum(i => i.Total)
                })
                .ToList();

            // Chart 5: ßãíÉ ÇáãäÊÌÇÊ ÇáãÊÈÞíÉ (Remaining Product Quantity)
            var remainingProducts = _context.Products
                .Select(p => new
                {
                    ProductName = p.Name,
                    RemainingQuantity = p.Quantity
                })
                .ToList();

            // Pass data to the view
            ViewBag.TotalProfit = totalProfit;
            ViewBag.ProfitPerMonthly = profitPerMonthly;
            ViewBag.TotalSales = totalSales;
            ViewBag.SalesPerMonth = salesPerMonth;
            ViewBag.RemainingProducts = remainingProducts;
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}