namespace WebApplicationMVC.Models.Database
{
    public class Visiting
	{
		public int Id { get; set; }
		public int StatusId { get; set; }
		public Status Status { get; set; }
		public int ScheduleEntryId { get; set; }
		public Lesson ScheduleEntry { get; set; }
	}

}
