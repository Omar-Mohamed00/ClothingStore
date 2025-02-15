using DataAccess.Data;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
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
            var invoices = _context.CustomerInvoices
                .Include(c => c.Customer);
            return View(await invoices.ToListAsync());
        }


        // GET: CustomerInvoice/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0) // Changed `id == null` to check against 0 since `id` is an int
            {
                return NotFound();
            }

            var customerInvoice = await _context.CustomerInvoices
                .Include(c => c.Customer)
                .Include(c => c.customerInvoiceLine)
                .ThenInclude(line => line.product) // Assuming Item is related to CustomerInvoiceLine
                .SingleOrDefaultAsync(m => m.Id == id);
            
            customerInvoice.Total = customerInvoice.customerInvoiceLine.Sum(line => line.SubAmount);

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

        public IActionResult Upsert(int? id) //Update Insert => Upsert
        {
            CustomerInvoiceVM customerInvoiceVM = new()
            {
                CustomerList = _unitOfWork.Customer.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                MedicineList = _unitOfWork.Product.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                CustomerInvoice = id == null || id == 0
          ? new CustomerInvoice()
          : _unitOfWork.Receipt.Get(u => u.Id == id, includeProperties: "customerInvoiceLine") ?? new CustomerInvoice()
            };

            return View(customerInvoiceVM);
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
				//else
				//{
				//	// Update logic
				//	_unitOfWork.Receipt.Update(customerInvoiceVM.CustomerInvoice);
				//}

				_unitOfWork.save();
				return RedirectToAction("Index");
			}

			// Reload dropdown data in case of error
			customerInvoiceVM.CustomerList = _unitOfWork.Customer.GetAll().Select(u => new SelectListItem
			{
				Text = u.Name,
				Value = u.Id.ToString()
			});

			customerInvoiceVM.MedicineList = _unitOfWork.Product.GetAll().Select(u => new SelectListItem
			{
				Text = u.Name,
				Value = u.Id.ToString()
			});

			return View(customerInvoiceVM);
		}
        #region API Calls

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var ReceiptToBeDeleted = _unitOfWork.Receipt.Get(u => u.Id == id);
            if (ReceiptToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }

            _unitOfWork.Receipt.Remove(ReceiptToBeDeleted);
            _context.SaveChanges();

            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    
    private bool CustomerInvoiceExists(int id)
        {
            return _context.CustomerInvoices.Any(e => e.Id == id);
        }

        public async Task<IActionResult> ShowInvoice(int id)
        {
            var customerInvoice = await _context.CustomerInvoices
                .Where(x => x.Id == id)
                .Include(x => x.customerInvoiceLine)
                    .ThenInclude(c => c.product)
                .Include(x => x.Customer)
                .FirstOrDefaultAsync();

            customerInvoice.Total = customerInvoice.customerInvoiceLine.Sum(line => line.SubAmount);

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
                .Include(x => x.Customer)
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
