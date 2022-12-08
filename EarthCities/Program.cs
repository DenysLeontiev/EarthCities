using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EarthCities
{
	public class Program
	{
		public static void Main(string[] args)
		{
			//var configuration = new ConfigurationBuilder()
			//	.SetBasePath(Directory.GetCurrentDirectory())
			//	.AddJsonFile(appsettings.json, optional: true, reloadOnChange: true)
			//	.AddUserSecrets<Startup>(optional: true, reloadOnChange: true)
			//	.Build();
			//Log.logger = new LoggerConfiguration()
			//	.WriteTo.MSSqlServer(
			//	connectionString: configuration.GetConnecntionString("DefaultConnection")),
			//	restrictedToMinimumLevel: LogEventLevel.Information,
			//	sinkOptions: new MSSqlServerSinkOptions
			//	{
			//		TableName = "LogsEvent",
			//		AutoCreateSqlTable = true,
			//	}
			//	.WriteTo.Console()
			//	.CreateLogger();

			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddUserSecrets<Startup>(optional: true, reloadOnChange: true)
				.Build();


			Log.Logger = new LoggerConfiguration()
				.WriteTo.MSSqlServer(
					configuration.GetConnectionString("DefaultConnection"),
					restrictedToMinimumLevel: LogEventLevel.Information,
					sinkOptions: new MSSqlServerSinkOptions
						{
							TableName = "LogEvents",
							AutoCreateSqlTable = true
						})
				.WriteTo.Console().CreateLogger();

			CreateHostBuilder(args).UseSerilog().Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
