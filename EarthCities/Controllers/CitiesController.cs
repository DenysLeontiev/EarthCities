using EarthCities.Data;
using EarthCities.Data.Folders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EarthCities.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CitiesController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public CitiesController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<ApiResult<CityDto>>> GetCities(int pageIndex = 0,int pageSize = 10, string sortColumn = null, string sortOrder = null, string filterColumn = null, string filterQuery = null)
		{
			return await ApiResult<CityDto>.CreateAsync(
				_context.Cities.Select(c => new CityDto()
				{ 
					Name = c.Name,
					Name_ASCII = c.Name_ASCII,
					Latitude = c.Latitude,
					Longitude = c.Longitude,
					CountryId = c.CountryId,
					CountryName = c.Country.Name
				}),
				pageIndex,
				pageSize,
				sortColumn,
				sortOrder,
				filterColumn,
				filterQuery);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<City>> GetCity(int id)
		{
			var city = await _context.Cities.FindAsync(id);

			if(city == null)
			{
				return NotFound($"City with id - {id} is not found");
			}

			return city;
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> PutCity(int id, City city)
		{
			if(id != city.Id)
			{
				return BadRequest();
			}

			_context.Entry(city).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException ex)
			{
				if(!CityExists(id))
				{
					return NotFound($"City is not found with id - {id}.Details: {ex.Message}");
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		[HttpPost]
		public async Task<ActionResult<City>> PostCity(City city)
		{
			if (city == null)
				return NotFound("Error occured while addind city");

			await _context.Cities.AddAsync(city);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetCity", new { id = city.Id }, city);
		}

		[HttpDelete("id")]
		public async Task<ActionResult> DeleteCity(int id)
		{
			var city = await _context.Cities.FindAsync(id);

			if(city == null)
			{
				return NotFound();
			}

			_context.Cities.Remove(city);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		[HttpPost]
		[Route("isDupeCity")]
		public bool IsDupeCity(City city)
		{
			return _context.Cities.Any(c =>
			c.Name == city.Name &&
			c.Latitude == city.Latitude &&
			c.Longitude == city.Longitude &&
			c.CountryId == city.CountryId &&
			c.Id != city.Id
			);
		}

		private bool CityExists(int id)
		{
			return _context.Cities.Any(c => c.Id == id);
		}
	}
}
