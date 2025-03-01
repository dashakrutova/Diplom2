namespace WebApplicationMVC.Models.Database
{
    public class Group
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int CourseId { get; set; }
		public Course Course { get; set; }
		public int TeacherId { get; set; }
        public User Teacher { get; set; }
        public List<Student> Students { get; set; }
	}
}