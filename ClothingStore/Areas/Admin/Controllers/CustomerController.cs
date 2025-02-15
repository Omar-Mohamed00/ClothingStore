using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccess.Data;
using Models;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;

namespace ClothingStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerController(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Customer> objCustomerList = _unitOfWork.Customer.GetAll().ToList();
            return View(objCustomerList);
        }
        public IActionResult Upsert(int? id) //Update Insert => Upsert
        {
            Customer customer = new();
            if (id == null || id == 0)
            {
                // Create
                return View(customer);
            }
            else
            {
				// Update
				customer = _unitOfWork.Customer.Get(u => u.Id == id);
                return View(customer);
            }
        }
        [HttpPost]
        public IActionResult Upsert(Customer customer)
        {
            if (customer.Id == 0)
            {
                _unitOfWork.Customer.Add(customer);
            }
            else
            {
                _unitOfWork.Customer.Update(customer);
            }

            _unitOfWork.save();
            TempData["success"] = "Customer Created Successfully";
            return RedirectToAction("Index");
        }
        #region API Calls

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var CustomerToBeDeleted = _unitOfWork.Customer.Get(u => u.Id == id);
            if (CustomerToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }

            _unitOfWork.Customer.Remove(CustomerToBeDeleted);
            _context.SaveChanges();

            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}
