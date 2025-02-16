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

        public IActionResult Index(int customerInvoiceId, string invoiceNumber)
        {
            List<CustomerInvoiceLine> customerInvoiceLineList = _unitOfWork.CustomerInvoiceLine
                .GetAll(u => u.customerInvoiceId == customerInvoiceId && u.customerInvoice.invoiceNumber == invoiceNumber,
                        includeProperties: "customerInvoice,medicine")
                .ToList();

            return View(customerInvoiceLineList);
        }

        public IActionResult Upsert(int? id)
        {
            // Fetch the latest invoice to auto-assign
            var latestInvoice = _unitOfWork.Receipt.GetAll()
                .OrderByDescending(i => i.Id)
                .FirstOrDefault();

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
                CustomerInvoice = latestInvoice ?? new CustomerInvoice(),
                CustomerInvoiceLine = new CustomerInvoiceLine()
            };

            if (id == null || id == 0)
            {
                // Create Mode: Auto-set `customerInvoiceId` to the latest invoice
                if (latestInvoice != null)
                {
                    customerInvoiceVM.CustomerInvoiceLine.customerInvoiceId = latestInvoice.Id;
                }

                return PartialView("~/Views/Shared/_UpsertPartialView.cshtml", customerInvoiceVM);
            }
            else
            {
                // Update Mode: Retrieve existing `CustomerInvoiceLine`
                var existingInvoiceLine = _unitOfWork.CustomerInvoiceLine.Get(
                    u => u.customerInvoiceLineId == id,
                    includeProperties: "customerInvoice");

                if (existingInvoiceLine == null)
                {
                    return NotFound(); // Handle invalid ID case
                }

                customerInvoiceVM.CustomerInvoiceLine = existingInvoiceLine;
                customerInvoiceVM.CustomerInvoice = existingInvoiceLine.customerInvoice; // Keep the correct invoice

                return PartialView("~/Views/Shared/_UpsertPartialView.cshtml", customerInvoiceVM);
            }
        }
        [HttpPost]
        public IActionResult Upsert(CustomerInvoiceVM customerInvoiceVM)
        {
            if (ModelState.IsValid)
            {
                var product = _unitOfWork.Product.Get(m => m.Id == customerInvoiceVM.CustomerInvoiceLine.ProductId);
                if (product is null)
                {
                    return RedirectToAction("Upsert", "CustomerInvoice", new { id = customerInvoiceVM.CustomerInvoice.Id });
                }

                // Check if the requested quantity is available
                if (customerInvoiceVM.CustomerInvoiceLine.Quantity > product.Quantity || product.Quantity == 0)
                {
                    return RedirectToAction("Upsert", "CustomerInvoice", new { id = customerInvoiceVM.CustomerInvoice.Id });

                }
                if (customerInvoiceVM.CustomerInvoiceLine.Quantity == 0)
                {
                    return RedirectToAction("Upsert", "CustomerInvoice", new { id = customerInvoiceVM.CustomerInvoice.Id });

                }
                // Update the product quantity
                product.Quantity -= customerInvoiceVM.CustomerInvoiceLine.Quantity;
                _unitOfWork.Product.Update(product);

                customerInvoiceVM.CustomerInvoiceLine.Price = product.SellingPrice;
                customerInvoiceVM.CustomerInvoiceLine.SubAmount =
                    (customerInvoiceVM.CustomerInvoiceLine.Quantity * customerInvoiceVM.CustomerInvoiceLine.Price) -
                    customerInvoiceVM.CustomerInvoiceLine.Discount;


                if (customerInvoiceVM.CustomerInvoiceLine.customerInvoiceId == 0)
                {
                    customerInvoiceVM.CustomerInvoiceLine.customerInvoiceId = customerInvoiceVM.CustomerInvoice.Id;
                }

                if (customerInvoiceVM.CustomerInvoiceLine.customerInvoiceLineId == 0)
                {
                    _unitOfWork.CustomerInvoiceLine.Add(customerInvoiceVM.CustomerInvoiceLine);
                }
                else
                {
                    _unitOfWork.CustomerInvoiceLine.Update(customerInvoiceVM.CustomerInvoiceLine);
                }

                _unitOfWork.save();
                return RedirectToAction("Upsert", "CustomerInvoice", new { id = customerInvoiceVM.CustomerInvoice.Id });
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
            var product = _unitOfWork.Product.Get(m => m.Id == CustomerInvoiceLineToBeDeleted.ProductId);

            // Update the product quantity
            product.Quantity += CustomerInvoiceLineToBeDeleted.Quantity;
            _unitOfWork.Product.Update(product);

            _context.SaveChanges();

            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}