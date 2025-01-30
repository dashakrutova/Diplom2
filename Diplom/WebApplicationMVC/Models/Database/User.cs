using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace WebApplicationMVC.Models.Database
{
    public class User
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Login { get; set; }
		public string Password { get; set; }
		public string Number { get; set; }
		public DateOnly DateOfBirth { get; set; }
		public List<Course> Courses { get; set; }
		public int RoleId { get; set; }
		public Role Role { get; set; }
		public List<Student> Student { get; set; }


		[NotMapped]
		public List<Claim> Claims { get; set; }

	}

}
