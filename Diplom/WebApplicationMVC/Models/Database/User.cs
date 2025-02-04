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
		public int RoleId { get; set; }
		public Role Role { get; set; }
		public List<Student> Student { get; set; }

        public AppRole AppRole { get; set; }

		[NotMapped]
        public List<Claim> Claims
		{
			get
			{
                var claims = new List<Claim>();
				claims.Add(new Claim(ClaimTypes.Name, Name));

				if (AppRole == AppRole.Admin)
				{
                    claims.Add(new Claim(AppRole.Admin.ToString(), "true"));
                }

				return claims;
			}
		}
	}
}