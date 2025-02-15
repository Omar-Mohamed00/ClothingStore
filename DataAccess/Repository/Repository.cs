using DataAccess.Data;
using DataAccess.Repository.IRepository;
using System.Linq.Expressions;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class Repository<T> : IRepository<T> where T : class
{
	private readonly ApplicationDbContext _db;
	internal DbSet<T> dbSet;
	public Repository(ApplicationDbContext db)
	{
		_db = db;
		this.dbSet = _db.Set<T>();
	}

	public void Add(T entity)
	{
		dbSet.Add(entity);
	}

	public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
	{
		IQueryable<T> query;
		if (tracked)
		{
			query = dbSet;
		}
		else
		{
			query = dbSet.AsNoTracking();
		}

		query = query.Where(filter);
		if (!string.IsNullOrEmpty(includeProperties))
		{
			// Dynamically add include properties to the query
			foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
			{
				query = query.Include(includeProp);
			}
		}
		return query.FirstOrDefault();
	}

	public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
	{
		IQueryable<T> query = dbSet;
		if (filter != null)
		{
			query = query.Where(filter);
		}
		if (!string.IsNullOrEmpty(includeProperties))
		{
			// Dynamically add include properties to the query
			foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
			{
				query = query.Include(includeProp);
			}
		}
		return query.ToList();
	}

	public void Remove(T entity)
	{
		dbSet.Remove(entity);
	}
}
