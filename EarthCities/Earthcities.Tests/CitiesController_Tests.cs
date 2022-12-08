using EarthCities.Controllers;
using EarthCities.Data;
using EarthCities.Data.Folders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EarthCities.Earthcities.Tests
{
	public class CitiesController_Tests
	{
		[Fact]
		public async Task GetCity()
		{
			//Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase("WorldCities")
				.Options;

			using (var context = new ApplicationDbContext(options))
			{
				await context.Cities.AddAsync(new City
				{ 
					Id = 1,
					Latitude = 1,
					Longitude = 1,
					CountryId = 1,
					Name = "TestCity1",
				});

				await context.SaveChangesAsync();
			}

			City city_exists;
			City city_notExists;
			//Act
			using(var context = new ApplicationDbContext(options))
			{
				var citiesController = new CitiesController(context);

				city_exists = (await citiesController.GetCity(1)).Value;
				city_notExists = (await citiesController.GetCity(2)).Value;
			}
			//Assert

			Assert.NotNull(city_exists);
			Assert.Null(city_notExists);
		}
	}
}
