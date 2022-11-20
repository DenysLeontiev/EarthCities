using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Threading.Tasks;

namespace EarthCities.Data.Folders
{
	public class MyApiResult<T>
	{
		private MyApiResult(List<T> data, int count, int pageIndex, int pageSize, string sortColumn, string sortOrder, string filterColumn,string filterQuery)
		{
			Data = data;
			TotalCount = count;
			TotalPages = (int)Math.Ceiling(count / (double)pageSize);
			PageIndex = pageIndex;
			PageSize = pageSize;
			SortColumn = sortColumn;
			SortOrder = sortOrder;
			FilterColumn = filterColumn;
			FilterQuery = filterQuery;
		}

		public async Task<MyApiResult<T>> CreateAsync(IQueryable<T> source,int pageIndex, int pageSize, string sortColumn = null, string sortOrder = null, string filterColumn = null, string filterQuery = null)
		{
			if(!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterQuery) && IsValidProperty(filterColumn))
			{
				//source = source.Where(string.Format("{0}.Contains(@0)", filterColumn), filterQuery);
				source = source.Where(filterColumn, filterQuery);
			}

			var count = await source.CountAsync();

			if(IsValidProperty(sortOrder) && sortOrder != null)
			{
				sortOrder = sortOrder == "ASC" && !string.IsNullOrEmpty(sortOrder) ? "ASC" : "DESC";

				source = source.OrderBy<T>(sortColumn, sortOrder);
			}

			source = source.Skip(pageIndex * pageSize).Take(pageSize);

			var data = await source.ToListAsync();

			return new MyApiResult<T>(data, count, pageIndex, pageSize, sortColumn, sortOrder, filterColumn, filterQuery);

		}

		public bool IsValidProperty(string propertyName, bool ExceptionIfNotFound = true)
		{
			var prop = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

			if (prop == null && ExceptionIfNotFound)
			{
				throw new NotSupportedException(string.Format($"Error:Property {propertyName} does not exist"));
			}

			return prop != null;
		}

		public List<T> Data { get; set; }
		public int TotalCount { get; set; }
		public int TotalPages { get; set; }
		public int PageIndex { get; set; }
		public int PageSize { get; set; }
		public string SortColumn { get; set; }
		public string SortOrder { get; set; }
		public string FilterColumn { get; set; }
		public string FilterQuery { get; set; }

		public bool HasPreviousPage
		{
			get
			{
				return (PageIndex > 1);
			}
		}

		public bool HasNextPage
		{
			get
			{
				return ((PageIndex + 1) > TotalPages);
			}
		}
	}
}
