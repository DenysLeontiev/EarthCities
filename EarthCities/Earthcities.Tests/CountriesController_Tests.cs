using EarthCities.Data;
using EarthCities.Data.Folders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldCities.Controllers;
using Xunit;

namespace EarthCities.Earthcities.Tests
{
	public class CountriesController_Tests
	{
		[Fact]
		public async void GetCountry_Test()
		{

			//Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase("WorldCities").Options;

			using(var context = new ApplicationDbContext(options))
			{
				await context.Countries.AddAsync(new Country
				{ 
					Id = 1,
					Name = "TestCountry1",
					ISO2 = "iso2",
					ISO3 = "iso3",
				});

				await context.SaveChangesAsync();
			}

			Country country_exists;
			Country country_notExists;
			//Act

			using(var context = new ApplicationDbContext(options))
			{
				var countryController = new CountriesController(context);

				country_exists = (await countryController.GetCountry(1)).Value;
				country_notExists = (await countryController.GetCountry(2)).Value;
			}

			//Assert

			Assert.NotNull(country_exists);
			Assert.Null(country_notExists);
			Assert.Equal<Country>(country_exists, country_exists);

		}
	}
}
