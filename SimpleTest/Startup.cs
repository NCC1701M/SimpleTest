using System;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Authentication;
using System.Linq;
#if DEBUG
using Microsoft.OpenApi.Models;
#endif

namespace SimpleTest;

public interface IStartup
{
	/// <summary>
	/// Adds additional services to this web application.
	/// </summary>
	/// <param name="services">A collection of services.</param>
	void ConfigureServices(IServiceCollection services);

	/// <summary>
	/// This method is used to specify how the app responds to HTTP requests.
	/// </summary>
	/// <param name="app">The current instance of <see cref="IApplicationBuilder" />.</param>
	/// <param name="provider">The current instance of <see cref="IServiceProvider" />.</param>
	void Configure(IApplicationBuilder app, IServiceProvider provider);
}

/// <summary>
/// Startup class for configuring the server.
/// </summary>
public class Startup : IStartup
{
	#region Variables
	private IConfiguration _configuration;
	private IWebHostEnvironment _env;
	private readonly string _allowSpecificOrigins = "_allowSpecificOrigins";
	#endregion

	#region Constructors
	/// <summary>
	/// Creates a new instance of the class.
	/// </summary>
	/// <param name="config">An instance of IConfiguration.</param>
	/// <param name="env">The hosting environment.</param>
	public Startup(IConfiguration config, IWebHostEnvironment env)
	{
		_configuration = config;
		_env = env;
	}
	#endregion

	#region Methods
	/// <summary>
	/// This method gets called by the runtime. Use this method to add services to the container.
	/// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
	/// </summary>
	/// <param name="services">A collection of services.</param>
	public void ConfigureServices(IServiceCollection services)
	{

		services.AddCors(options =>
		{
			options.AddPolicy(_allowSpecificOrigins,
				builder =>
				{
					builder.AllowAnyOrigin();
				}
			);
		});

		services.Configure<CookiePolicyOptions>(options =>
		{
			options.MinimumSameSitePolicy = SameSiteMode.None;
			options.Secure = CookieSecurePolicy.Always;
			options.HandleSameSiteCookieCompatibility();
		});

		services.AddOptions();

		services.AddControllers()
				  .AddNewtonsoftJson(options =>
				  {
					  options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
				  });

		services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

		if (_env.IsProduction())
			services.Configure<MvcOptions>(options =>
			{
				options.Filters.Add(new RequireHttpsAttribute());
			});

		services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo { Title = "SimpleTest", Version = "v1" });
		});
	}

	/// <summary>
	/// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
	/// </summary>
	/// <param name="app">The application builder.</param>
	/// <param name="loggerFactory">The logger factory.</param>
	/// <param name="provider">The service provider.</param>
	/// <param name="logger">The logger.</param>
	public void Configure(IApplicationBuilder app, IServiceProvider provider)
	{
		if (_env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}
		else
		{
			app.UseHsts();
		}

		if (_env.IsProduction())
			app.UseHttpsRedirection();

		app.UseSwagger();
		app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SimpleTest v1"));

		app.UseCors(_allowSpecificOrigins)
			.UseRequestLocalization(new RequestLocalizationOptions
			{
				DefaultRequestCulture = new RequestCulture("de-DE"),
				SupportedCultures = CultureInfo.GetCultures(CultureTypes.AllCultures),
				SupportedUICultures = CultureInfo.GetCultures(CultureTypes.AllCultures)
			})
			.UseRouting()
			.Use(async (ctx, next) =>
			{
				var isAuthenticated = true;

				if (!isAuthenticated && !ctx.User.Identity.IsAuthenticated)
				{
					var scheme = OpenIdConnectDefaults.AuthenticationScheme;
					var redirectUrl = ctx.Request.Path.Value.ToLower();
					await ctx.ChallengeAsync(
					scheme,
					new AuthenticationProperties { RedirectUri = redirectUrl }
					);
				}
				else
					await next();
			})
			.Use(async (context, next) =>
			{
				if (context.Response.Headers.Any(e => e.Key.ToLower() == "cache-control"))
				{
					context.Response.Headers.Remove("cache-control");
				}
				context.Response.Headers.Append("cache-control", "no-cache, no-store, must-revalidate");
				await next();
			})
			.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapControllerRoute(
					name: "Api with action",
					pattern: "api/{controller}/{action}/{id?}"
				);
				endpoints.MapControllerRoute(
					name: "DefaultApi",
					pattern: "api/{controller}/{id?}"
				);
			})
			.Use(async (context, next) =>
			{
				await next();

				// If no file is found the url might be an angular route which should be handled by the client.
				if (context.Response.StatusCode == 404)
				{
					context.Response.Redirect("/swagger/index.html");
				}
			})
			.UseDefaultFiles()
			.UseStaticFiles();

	}

	#endregion
}