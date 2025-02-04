namespace WebApplicationMVC.Models.Database
{
    public class Student
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateOnly DateOfBirth { get; set; }
		public int CourseId { get; set; }
		public Course Course { get; set; }
		public int GroupId { get; set; }
		public Group Group { get; set; }
		public int ParentId { get; set; }
		public User Parent { get; set; }
		public int RoleId { get; set; }
		public Role Role { get; set; }

	}

}
