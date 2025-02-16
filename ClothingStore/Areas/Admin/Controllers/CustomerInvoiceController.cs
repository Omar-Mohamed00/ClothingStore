using DataAccess.Data;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Models;
using Models.ViewModels;

namespace ClothingStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomerInvoiceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerInvoiceController(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        // GET: CustomerInvoice
        public async Task<IActionResult> Index()
        {
            var invoices = _context.CustomerInvoices;
            return View(await invoices.ToListAsync());
        }

        [HttpPost]
        public IActionResult SaveInvoice(CustomerInvoiceVM customerInvoiceVM)
        {
            if (ModelState.IsValid)
            {
                if (customerInvoiceVM.CustomerInvoice.Id == 0)
                {
                    // Create logic
                    _unitOfWork.Receipt.Add(customerInvoiceVM.CustomerInvoice);
                }

                _unitOfWork.save();
                return RedirectToAction("Upsert", new { id = customerInvoiceVM.CustomerInvoice.Id });
            }

            return View(customerInvoiceVM);
        }

        public IActionResult Upsert(int? id) // Update & Insert & Details
        {
            if (id == 0) // Changed `id == null` to check against 0 since `id` is an int
            {
                return NotFound();
            }

            var customerInvoice =  _context.CustomerInvoices
                .Include(c => c.customerInvoiceLine)
                .ThenInclude(line => line.product) // Assuming Item is related to CustomerInvoiceLine
                .SingleOrDefault(m => m.Id == id);



            // Fetch existing invoice
            customerInvoice = _unitOfWork.Receipt.Get(u => u.Id == id, includeProperties: "customerInvoiceLine") ?? new CustomerInvoice();

            if (customerInvoice.customerInvoiceLine != null)
            {
                customerInvoice.Total = customerInvoice.customerInvoiceLine.Sum(line => line.SubAmount);
            }

            if (customerInvoice == null)
            {
                return NotFound();
            }
            var viewModel = new CustomerInvoiceVM
            {
                CustomerInvoice = customerInvoice,
                CustomerInvoiceList = customerInvoice.customerInvoiceLine, // Assuming `CustomerInvoiceLine` is a collection
                CustomerList = _context.Customers.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }),
                MedicineList = _context.Products.Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = i.Name
                })
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Upsert(CustomerInvoiceVM customerInvoiceVM)
        {
            if (ModelState.IsValid)
            {
                if (customerInvoiceVM.CustomerInvoice.Id == 0)
                {
                    // Create logic
                    _unitOfWork.Receipt.Add(customerInvoiceVM.CustomerInvoice);
                }

                _unitOfWork.save();
                return RedirectToAction("Upsert", new { id = customerInvoiceVM.CustomerInvoice.Id });
            }

            return View(customerInvoiceVM);
        }

        // GET: CustomerInvoice/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0) // Changed `id == null` to check against 0 since `id` is an int
            {
                return NotFound();
            }

            var customerInvoice = await _context.CustomerInvoices
                .Include(c => c.customerInvoiceLine)
                .ThenInclude(line => line.product) // Assuming Item is related to CustomerInvoiceLine
                .SingleOrDefaultAsync(m => m.Id == id);

            //customerInvoice.Total = customerInvoice.customerInvoiceLine.Sum(line => line.SubAmount);

            if (customerInvoice == null)
            {
                return NotFound();
            }

            var viewModel = new CustomerInvoiceVM
            {
                CustomerInvoice = customerInvoice,
                CustomerInvoiceList = customerInvoice.customerInvoiceLine, // Assuming `CustomerInvoiceLine` is a collection
                CustomerList = _context.Customers.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }),
                MedicineList = _context.Products.Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = i.Name
                })
            };

            return View(viewModel);
        }

        #region API Calls

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Invalid Invoice ID" });
            }

            // Retrieve all related CustomerInvoiceLine records
            var invoiceLines = _unitOfWork.CustomerInvoiceLine.GetAll(u => u.customerInvoiceId == id).ToList();

            // Remove each invoice line individually if any exist
            if (invoiceLines.Any())
            {
                foreach (var line in invoiceLines)
                {
                    _unitOfWork.CustomerInvoiceLine.Remove(line);
                }
            }
            // Retrieve the CustomerInvoice itself
            var customerInvoice = _unitOfWork.Receipt.Get(u => u.Id == id);
            if (customerInvoice == null)
            {
                return Json(new { success = false, message = "Invoice Not Found" });
            }

            // Remove the invoice
            _unitOfWork.Receipt.Remove(customerInvoice);

            // Save changes
            _context.SaveChanges();

            return Json(new { success = true, message = "Invoice and related lines deleted successfully" });
        }
        #endregion
    
        public async Task<IActionResult> ShowInvoice(int id)
        {
            var customerInvoice = await _context.CustomerInvoices
                .Where(x => x.Id == id)
                .Include(x => x.customerInvoiceLine)
                    .ThenInclude(c => c.product)
                .FirstOrDefaultAsync();
            if (customerInvoice.customerInvoiceLine.Sum(line => line.SubAmount) != 0)
            {
                customerInvoice.Total = customerInvoice.customerInvoiceLine.Sum(line => line.SubAmount);
            }

            if (customerInvoice == null)
            {
                return NotFound();
            }

            // Map CustomerInvoice to CustomerInvoiceVM
            CustomerInvoiceVM customerInvoiceVM = new()
            {
                CustomerInvoice = customerInvoice,
                CustomerInvoiceList = customerInvoice.customerInvoiceLine
            };

            return View(customerInvoiceVM);
        }

        [HttpPost]
        public async Task<IActionResult> PrintInvoice(int id, decimal paid, decimal remaining)
        {
            // Save the data to session
            HttpContext.Session.SetString("PaidAmount", paid.ToString());
            HttpContext.Session.SetString("RemainingAmount", remaining.ToString());

            // Retrieve the saved data from session
            var paidAmount = HttpContext.Session.GetString("PaidAmount");
            var remainingAmount = HttpContext.Session.GetString("RemainingAmount");

            // Debug: Check if the session values are being retrieved
            Console.WriteLine($"Retrieved PaidAmount: {paidAmount}");
            Console.WriteLine($"Retrieved RemainingAmount: {remainingAmount}");

            // Pass the data to the view
            ViewBag.PaidAmount = paidAmount;
            ViewBag.RemainingAmount = remainingAmount;

            // Retrieve the CustomerInvoice from the database
            CustomerInvoice? obj = await _context.CustomerInvoices
                .Where(x => x.Id.Equals(id))
                .Include(x => x.customerInvoiceLine)
                .ThenInclude(c => c.product)
                .FirstOrDefaultAsync();

            // Check if obj is null
            if (obj == null)
            {
                return NotFound(); // Return a 404 Not Found response
            }

            // Calculate the total
            obj.Total = obj.customerInvoiceLine?.Sum(line => line.SubAmount) ?? 0;

            // Update the date
            obj.Date = DateTime.Now;

            // Save changes to the database
            _context.CustomerInvoices.Update(obj);
            _unitOfWork.save();

            return View(obj);
        }
    }
}
