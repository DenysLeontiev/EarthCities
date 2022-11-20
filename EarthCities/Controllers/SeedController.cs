using EarthCities.Data;
using EarthCities.Data.Folders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace EarthCities.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class SeedController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		private readonly IWebHostEnvironment _env;
		public SeedController(ApplicationDbContext context, IWebHostEnvironment env)
		{
			_context = context;
			_env = env;
		}

		[HttpGet]
		public async Task<ActionResult> Import()
		{
			if(_env.EnvironmentName != "Development")
			{
				throw new SecurityException("Not allowed");
			}

			var path = Path.Combine(_env.ContentRootPath, "Data/Source/worldcities.xlsx");
			using var stream = System.IO.File.OpenRead(path);
			using var excelPackage = new ExcelPackage(stream); //opens file if exists otherwise creates a new one

			//Get first worksheet
			var worksheet = excelPackage.Workbook.Worksheets[0];

			//How many rows we need to read data from
			int nEndRow = worksheet.Dimension.End.Row;

			//how many cities and countries we have
			int countriesAdded = 0;
			int citiesAdded = 0;

			//Added dictionary where key is countryName;Data is taken from the database
			var countriesByName = await _context.Countries
										  .AsNoTracking()
										  .ToDictionaryAsync(x => x.Name, StringComparer.OrdinalIgnoreCase);

			//iterates all the rows skipping the first one
			for (int nRow = 2; nRow <= nEndRow; nRow++)
			{
				var row = worksheet.Cells[nRow, 1, nRow, worksheet.Dimension.End.Column];

				var countryName = row[nRow, 5].GetValue<string>();
				var iso2 = row[nRow, 6].GetValue<string>();
				var iso3 = row[nRow, 7].GetValue<string>();

				if (countriesByName.ContainsKey(countryName))
					continue;

				var country = new Country
				{
					Name = countryName,
					ISO2 = iso2,
					ISO3 = iso3
				};

				await _context.Countries.AddAsync(country);

				countriesByName.Add(countryName, country);

				countriesAdded++;
			}

			if(countriesAdded > 0)
			{
				await _context.SaveChangesAsync();
			}

			var cities = await _context.Cities
								 .AsNoTracking()
								 .ToDictionaryAsync(x => (
									Name: x.Name,
									Latitude: x.Latitude,
									Longitude: x.Longitude,
									CountryId: x.CountryId
								 ));

			for (int nRow = 2; nRow <= nEndRow; nRow++)
			{
				var row = worksheet.Cells[nRow, 1, nRow, worksheet.Dimension.End.Column];

				var name = row[nRow, 1].GetValue<string>();
				var name_ascii = row[nRow, 2].GetValue<string>();
				var latitude = row[nRow, 3].GetValue<decimal>();
				var longitude = row[nRow, 4].GetValue<decimal>();
				var countryName = row[nRow, 5].GetValue<string>();

				// get countryId by countryName
				var countryId = countriesByName[countryName].Id;

				if (cities.ContainsKey((
					Name: name,
					Latitude: latitude,
					Longitude: longitude,
					CountryId: countryId
					)))
				{
					continue;
				}

				var city = new City
				{
					Name = name,
					Name_ASCII = name_ascii,
					Latitude = latitude,
					Longitude = longitude,
					CountryId = countryId
				};

				await _context.Cities.AddAsync(city);

				citiesAdded++;
			}

			if(citiesAdded > 0)
			{
				await _context.SaveChangesAsync();
			}

			return new JsonResult(new {
				CitiesAdded = citiesAdded,
				CountriesAdded = countriesAdded,
			});;
		}
	}
}
