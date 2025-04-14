using System.ComponentModel.DataAnnotations.Schema;
using WebApplicationMVC.Models.Database;

namespace WebApplicationMVC.Models.Database;

public class Group
{
	public int Id { get; set; }
    public GroupType GroupType { get; set; }

	[NotMapped]
    public string Name {
		get
		{
			if (GroupType == GroupType.Regular)
				return _name;

			var res =  "Индив " + 
				Students.FirstOrDefault()?.FirstName + " " +
				Students.FirstOrDefault()?.LastName;

			return res;
        }
		set
		{
			_name = value;
		} 
	}

	private string _name;
	public int CourseId { get; set; }
	public Course Course { get; set; }
	public int TeacherId { get; set; }
    public User Teacher { get; set; }
    public List<Student> Students { get; set; } = new();


}

public enum GroupType
{
	Regular = 1, 
	Personal = 2
}