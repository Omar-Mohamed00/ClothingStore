using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(Product obj)
		{
			var objFromDb = _db.Products.FirstOrDefault(u => u.Id == obj.Id);
			if (objFromDb != null)
			{
				objFromDb.Color = obj.Color;
				objFromDb.Size = obj.Size;
				objFromDb.Name = obj.Name;
				objFromDb.Quantity = obj.Quantity;
				objFromDb.PurchasePrice = obj.PurchasePrice;
				objFromDb.SellingPrice = obj.SellingPrice;
				objFromDb.CategoryId = obj.CategoryId;
			}
		}
	}
}
