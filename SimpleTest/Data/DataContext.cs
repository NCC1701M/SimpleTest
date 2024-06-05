using Microsoft.EntityFrameworkCore;
using SimpleTest.Models;

namespace SimpleTest.Data;

public class DataContext : DbContext
{
	public DataContext(DbContextOptions<DataContext> options)
		: base(options)
	{
	}
	protected override void OnModelCreating(ModelBuilder builder)
	{
		builder.Entity<SimpleModel>(entity =>
		{
			entity.HasKey(e => e.Id);

			entity.Property(e => e.Text)
					.IsRequired()
					.HasMaxLength(100);

			entity.Property(e => e.DayOfWeek)
					.IsRequired()
					.HasDefaultValue(Weekday.Monday)
					.HasConversion(
						v => v.ToString(),
						v => (Weekday)Enum.Parse(typeof(Weekday), v)
					);

			entity.HasData(
				new SimpleModel { Id = new("4282b76f-daed-4292-9f24-4392a9f2f854"), Text = "I hate Mondays", DayOfWeek = Weekday.Monday },
				new SimpleModel { Id = new("929de995-0b35-45fe-b39b-8ab2075ae84f"), Text = "They are not better", DayOfWeek = Weekday.Tuesday },
				new SimpleModel { Id = new("c8655719-fb15-4eab-aa75-3d07696ff118"), Text = "Halftime", DayOfWeek = Weekday.Wednesday },
				new SimpleModel { Id = new("6f050ba8-d44f-425a-8de1-3b54cb42036b"), Text = "Still not over", DayOfWeek = Weekday.Thursday },
				new SimpleModel { Id = new("c5adf0dd-368b-462d-9ddd-c91e78965f0f"), Text = "Weekend is only a few hours away", DayOfWeek = Weekday.Friday },
				new SimpleModel { Id = new("e5b04963-b82f-4bb0-a52d-dd019620eafa"), Text = "YEAH!!!", DayOfWeek = Weekday.Saturday },
				new SimpleModel { Id = new("007000fb-d473-4deb-b8e4-846c30601c9f"), Text = "Why is already sunday?", DayOfWeek = Weekday.Sunday }
			);
		});
	}

	public virtual DbSet<SimpleModel> SimpleModels { get; set; }
}