namespace WebApplicationMVC.Models.Database
{
    public class ScheduleEntry
	{
		public int Id { get; set; }
		public int GroupId { get; set; }
		public Group Group { get; set; }
		public DateTimeOffset Start { get; set; }
		public DateTimeOffset End { get; set; }
	}

}
