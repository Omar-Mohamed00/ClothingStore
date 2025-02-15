using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using Models;
using Models.ViewModels;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace ClothingStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomerInvoiceLineController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerInvoiceLineController(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        // GET: CustomerInvoiceLine
        public IActionResult Index()
        {
            List<CustomerInvoiceLine> CustomerInvoiceLineList = _unitOfWork.CustomerInvoiceLine
                .GetAll(includeProperties: "customerInvoice,medicine").ToList();
            return View(CustomerInvoiceLineList);
        }
        public IActionResult Upsert(int? id) //Update Insert => Upsert
        {
            CustomerInvoiceVM customerInvoiceVM = new()
            {
                MedicineList = _unitOfWork.Product.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
				CustomerList = _unitOfWork.Receipt.GetAll().Select(u => new SelectListItem
				{
					Text = u.invoiceNumber,
					Value = u.Id.ToString()
				}),
                CustomerInvoice = new CustomerInvoice(),
                CustomerInvoiceLine = new CustomerInvoiceLine()
            };
            var firstInvoiceNumber = customerInvoiceVM.CustomerList.FirstOrDefault();

            if (firstInvoiceNumber != null)
            {
                customerInvoiceVM.CustomerList = customerInvoiceVM.CustomerList.Take(1);

            }
            if (id == null || id == 0)
            {
                // Create
                //return View(customerInvoiceVM);
                return PartialView("~/Views/Shared/_UpsertPartialView.cshtml", customerInvoiceVM);

            }
            else
            {
                // Update
                customerInvoiceVM.CustomerInvoiceLine = _unitOfWork.CustomerInvoiceLine.Get(u => u.customerInvoiceLineId == id);
                //return View(customerInvoiceVM);
                return PartialView("~/Views/Shared/_UpsertPartialView.cshtml", customerInvoiceVM);

            }
        }
        [HttpPost]
        public IActionResult Upsert(CustomerInvoiceVM customerInvoiceVM)
        {

            if (ModelState.IsValid)
            {
                var medicine = _unitOfWork.Product.Get(m => m.Id == customerInvoiceVM.CustomerInvoiceLine.ProductId);
                if (medicine is null)
                {
                    return RedirectToAction("Details", "CustomerInvoice",new { customerInvoiceVM.CustomerInvoice.Id});
                }
                customerInvoiceVM.CustomerInvoiceLine.Price = medicine.PurchasePrice;
                
                customerInvoiceVM.CustomerInvoiceLine.SubAmount =
                    (customerInvoiceVM.CustomerInvoiceLine.Quantity * customerInvoiceVM.CustomerInvoiceLine.Price) -
                    customerInvoiceVM.CustomerInvoiceLine.Discount;


                customerInvoiceVM.CustomerInvoiceLine.customerInvoiceId = customerInvoiceVM.CustomerInvoice.Id;
                if (customerInvoiceVM.CustomerInvoiceLine.customerInvoiceLineId == 0)
                {
                    _unitOfWork.CustomerInvoiceLine.Add(customerInvoiceVM.CustomerInvoiceLine);
                }
                else
                {
                    _unitOfWork.CustomerInvoiceLine.Update(customerInvoiceVM.CustomerInvoiceLine);
                }
                _unitOfWork.save();
                return RedirectToAction("Details", "CustomerInvoice",new { customerInvoiceVM.CustomerInvoice.Id});

            }
            return PartialView("~/Views/Shared/_UpsertPartialView.cshtml", customerInvoiceVM);
        }

        #region API Calls

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var CustomerInvoiceLineToBeDeleted = _unitOfWork.CustomerInvoiceLine.Get(u => u.customerInvoiceLineId == id);
            if (CustomerInvoiceLineToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }

            _unitOfWork.CustomerInvoiceLine.Remove(CustomerInvoiceLineToBeDeleted);
            _context.SaveChanges();

            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}
