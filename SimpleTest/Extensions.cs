using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleTest;

public static class Extensions
{
	public static WebApplication Build<T>(this WebApplicationBuilder builder)
			where T : class, IStartup
	{
		var createStartup = (object[] parameters) =>
		{
			var types = parameters?.Select(p => p?.GetType())?.ToArray() ?? Type.EmptyTypes;
			var ctor = typeof(T).GetConstructor(types);

			if (ctor == null)
				return null;

			return ctor.Invoke(parameters) as T;
		};

		T startup = createStartup(new object[] { builder.Configuration, builder.Environment });

		if (startup == null)
			startup = createStartup(new object[] { builder.Configuration });

		if (startup == null)
			startup = createStartup(null);

		if (startup == null)
			throw new ApplicationException($"No constructor found for type {typeof(T).Name}");

		return builder.Build(startup);
	}


	/// <summary>
	/// Builds the web application and uses the type <typeparamref name="T" /> for the start up configuration.
	/// </summary>
	/// <param name="builder">The current <see cref="WebApplicationBuilder" />.</param>
	/// <param name="startup">An instance of <see cref="IStartup" /> for a start up configuration of the web application.
	/// <returns>The created <see cref="WebApplication" />.</returns>
	public static WebApplication Build(this WebApplicationBuilder builder, IStartup startup)
	{
		if (builder == null)
			throw new NullReferenceException(nameof(builder));

		if (startup != null)
			startup.ConfigureServices(builder.Services);

		var app = builder.Build();

		if (startup != null && builder.Environment.ApplicationName != "ef")
			startup.Configure(app, app.Services.CreateScope().ServiceProvider);

		return app;
	}
}