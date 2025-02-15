using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace ClothingStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IUnitOfWork _unitOfWork;

		public CategoryController(ApplicationDbContext context, IUnitOfWork unitOfWork)
		{
			_context = context;
			_unitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();

            return View(objCategoryList);
        }
		public IActionResult Upsert(int? id) //Update Insert => Upsert
		{
			Category category = new();
			if (id == null || id == 0)
			{
				// Create
				return View(category);
			}
			else
			{
				// Update
				category = _unitOfWork.Category.Get(u => u.Id == id);
				return View(category);
			}
		}
		[HttpPost]
		public IActionResult Upsert(Category category)
		{
			if (category.Id == 0)
			{
				_unitOfWork.Category.Add(category);
			}
			else
			{
				_unitOfWork.Category.Update(category);
			}

			_unitOfWork.save();
			TempData["success"] = "Category Created Successfully";
			return RedirectToAction("Index");
		}
        #region API Calls

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var CategoryToBeDeleted = _unitOfWork.Category.Get(u => u.Id == id);
            if (CategoryToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }

            _unitOfWork.Category.Remove(CategoryToBeDeleted);
            _context.SaveChanges();

            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}
