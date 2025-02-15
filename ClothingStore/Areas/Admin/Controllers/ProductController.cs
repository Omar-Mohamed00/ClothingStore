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
using Models.ViewModels;

namespace ClothingStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
		private readonly ApplicationDbContext _context;
		private readonly IUnitOfWork _unitOfWork;

		public ProductController(ApplicationDbContext context, IUnitOfWork unitOfWork)
		{
			_context = context;
			_unitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
			List<Product> objMedicineList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
			return View(objMedicineList);
		}
		public IActionResult Upsert(int? id) //Update Insert => Upsert
		{
			ProductVM medicineVM = new()
			{
				CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				}),
				Medicine = new Product()
			};
			if (id == null || id == 0)
			{
				// Create
				return View(medicineVM);
			}
			else
			{
				// Update
				medicineVM.Medicine = _unitOfWork.Product.Get(u => u.Id == id);
				return View(medicineVM);
			}
		}
		[HttpPost]
		public IActionResult Upsert(ProductVM medicineVm)
		{
			if (ModelState.IsValid)
			{
				if (medicineVm.Medicine.Id == 0)
				{
					_unitOfWork.Product.Add(medicineVm.Medicine);
				}
				else
				{
					_unitOfWork.Product.Update(medicineVm.Medicine);
				}
				_unitOfWork.save();
				return RedirectToAction("Index");
			}
			return View(medicineVm);
		}

		#region API Calls
		
		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			var MedicineToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
			if (MedicineToBeDeleted == null)
			{
				return Json(new { success = false, message = "Error While Deleting" });
			}

			_unitOfWork.Product.Remove(MedicineToBeDeleted);
			_context.SaveChanges();

			return Json(new { success = true, message = "Delete Successful" });
		}
		#endregion
	}
}
