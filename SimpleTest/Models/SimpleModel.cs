namespace SimpleTest.Models;

public class SimpleModel
{
	public Guid Id { get; set; }
	public string Text { get; set; }
	public Weekday DayOfWeek { get; set; }
}

public enum Weekday
{
	Unknown = 0,
	Monday,
	Tuesday,
	Wednesday,
	Thursday,
	Friday,
	Saturday,
	Sunday
}