using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Threading.Tasks;

namespace EarthCities.Data
{
	public class ApiResult<T>
	{
		private ApiResult(List<T> data, int count, int pageIndex, int pageSize,string sortColumn, string sortOrder, string filterColumn, string filterQuery)
		{
			Data = data;
			PageIndex = pageIndex;
			PageSize = pageSize;
			TotalCount = count;
			TotalPages = (int)Math.Ceiling(count / (double)pageSize);
			SortColumn = sortColumn;
			SortOrder = sortOrder;
			FilterColumn = filterColumn;
			FilterQuery = filterQuery;
		}

		public static async Task<ApiResult<T>> CreateAsync(IQueryable<T> source,int pageIndex, int pageSize, string sortColumn = null, string sortOrder = null, string filterColumn = null, string filterQuery = null)
		{
			if (!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterQuery) && IsValidProperty(filterColumn))
			{
				source = source.Where(string.Format("{0}.Contains(@0)", filterColumn), filterQuery);
				//source = source.Where(string.Format(filterColumn, filterQuery));
			}

			int count = await source.CountAsync();

			if (!string.IsNullOrEmpty(sortColumn) && IsValidProperty(sortColumn))
			{
				sortOrder = !string.IsNullOrEmpty(sortOrder)
				&& sortOrder.ToUpper() == "ASC"
				? "ASC"
				: "DESC";
				source = source.OrderBy<T>(
				string.Format(
				"{0} {1}",
				sortColumn,
				sortOrder));
			}


			source = source.Skip(pageIndex * pageSize).Take(pageSize);

			var data = await source.ToListAsync();

			return new ApiResult<T>(data, count, pageIndex, pageSize, sortOrder, sortColumn, filterColumn, filterQuery);
		}

		public static bool IsValidProperty(string propertyName, bool throwExceptionIfNotFound = true)
		{
			PropertyInfo prop = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

			if(prop == null && throwExceptionIfNotFound)
			{
				throw new NotSupportedException(string.Format($"Error:Property {propertyName} does not exist"));
			}

			return prop != null;
		}

		public List<T> Data { get; private set; } // data we return
		public int TotalCount { get; private set; } // all elements in all pages
		public int TotalPages { get; private set; } // all pages
		public int PageIndex { get; private set; }
		public int PageSize { get; private set; }
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

		public bool HasNext
		{
			get
			{
				return ((PageIndex + 1) < TotalPages);
			}
		}

	}
}
