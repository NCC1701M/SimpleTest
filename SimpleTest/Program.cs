using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;

namespace SimpleTest;
/// <summary>
/// Contains the main method of this application.
/// </summary>
public class Program
{
	/// <summary>
	/// The main method of the application.
	/// </summary>
	/// <param name="args">The command line arguments.</param>
	public static void Main(string[] args)
	{
		BuildWebHost(args).Run();
	}

	/// <summary>
	/// Builds the web host of the application.
	/// </summary>
	/// <param name="args">The arguments for building the web host.</param>
	/// <returns></returns>
	public static WebApplication BuildWebHost(string[] args) =>
		CreateWebApplication(args)
			.Build<Startup>();


	// Located in an external library
	private static WebApplicationBuilder CreateWebApplication(string[] args)
	{
		var result = WebApplication.CreateBuilder();

		result.WebHost
				.UseKestrel(options => options.AddServerHeader = false)
				.UseIISIntegration()
				.UseDefaultServiceProvider((cbo, options) => options.ValidateScopes = cbo.HostingEnvironment.IsDevelopment())
				// .UseWebRoot("wwwroot") // Does not work with or without explicit setting of Web Root.
				.UseContentRoot(Directory.GetCurrentDirectory());

		result.Configuration
				.AddCommandLine(args)
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("serversettings.json", true)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{result.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.local.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables();

		return result;
	}
}
