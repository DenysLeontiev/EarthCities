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
	public class MySeedController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly IWebHostEnvironment _env;

		public MySeedController(ApplicationDbContext context, IWebHostEnvironment env)
		{
			_context = context;
			_env = env;
		}

		[HttpGet]
		public async Task<ActionResult> Import()
		{
			if (_env.EnvironmentName != "Development")
			{
				throw new SecurityException("Not allowed");
			}

			var path = Path.Combine(_env.ContentRootPath, "Data/Source/worldcities.xlsx");
			using var stream = System.IO.File.OpenRead(path); // opens existing file for reading
			using ExcelPackage excelPackage = new ExcelPackage(stream);

			var worksheet = excelPackage.Workbook.Worksheets[0];

			int allRowsToTheEnd = worksheet.Dimension.End.Row; 

			int countriesAdded = 0;
			int citiesAdded = 0;

			var countriesDictionary = await _context.Countries.AsNoTracking().ToDictionaryAsync(x => x.Name, StringComparer.OrdinalIgnoreCase);

			for (int currentRow = 1; currentRow <= allRowsToTheEnd; currentRow++)
			{
				var row = worksheet.Cells[currentRow, 1, currentRow, worksheet.Dimension.End.Column];

				var countryName = row[currentRow, 5].GetValue<string>();
				var iso2 = row[currentRow, 6].GetValue<string>();
				var iso3 = row[currentRow, 7].GetValue<string>();

				if(countriesDictionary.ContainsKey(countryName))
				{
					continue;
				}

				var country = new Country
				{
					Name = countryName,
					ISO2 = iso2,
					ISO3 = iso3
				};

				await _context.Countries.AddAsync(country);

				countriesDictionary.Add(countryName, country);

				countriesAdded++;
			}

			if(countriesAdded > 0)
			{
				await _context.SaveChangesAsync();
			}

			var citiesDictionary = await _context.Cities.AsNoTracking().ToDictionaryAsync(x => (
				Name: x.Name,
				Latitude: x.Latitude,
				Longitude: x.Longitude,
				CountryId: x.CountryId
				));

			for (int currentRow = 2; currentRow <= allRowsToTheEnd; currentRow++)
			{
				var row = worksheet.Cells[currentRow, 1, currentRow, worksheet.Dimension.End.Column];

				var cityName = row[currentRow, 1].GetValue<string>();
				var cityNameASCII = row[currentRow, 2].GetValue<string>();
				var cityLat = row[currentRow, 3].GetValue<decimal>();
				var cityLon = row[currentRow, 4].GetValue<decimal>();
				var countryName = row[currentRow, 5].GetValue<string>();

				var countryId = countriesDictionary[countryName].Id;

				if (citiesDictionary.ContainsKey((
					Name: cityName,
					Latitude: cityLat,
					Longitude: cityLon,
					CountryId: countryId
					)))
				{
					continue;
				}

				var city = new City
				{ 
					Name = cityName,
					Name_ASCII = cityNameASCII,
					Latitude = cityLat,
					Longitude = cityLon,
					CountryId = countryId
				};

				await _context.Cities.AddAsync(city);
				citiesDictionary.Add((
					Name: cityName,
					Latitude: cityLat,
					Longitude: cityLon,
					CountryId: countryId
					), city);

				citiesAdded++;
			}

			if(citiesAdded > 0)
			{
				await _context.SaveChangesAsync();
			}

			return new JsonResult(new
			{
				CountriesAdded = countriesAdded,
				CitiesAdded = citiesAdded
			});
		}
	}
}
