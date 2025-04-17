using Microsoft.EntityFrameworkCore;

namespace WebApplicationMVC.Models.Database;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions options) : base(options)
	{
		Database.Migrate();
	}

	public DbSet<User> Users { get; set; }
	public DbSet<Student> Students { get; set; }
	public DbSet<Course> Courses { get; set; }
	public DbSet<Group> Groups { get; set; }
	public DbSet<Status> Statuses { get; set; }
	public DbSet<Lesson> Lessons { get; set; }
	public DbSet<Visiting> Visitings { get; set; }
	public DbSet<Attendance> Attendances { get; set; }
	 


	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{

	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<User>()
			.Property(o => o.AppRole)
			.HasConversion<int>();
        
		modelBuilder.Entity<Group>()
			.Property(o => o.GroupType)
			.HasConversion<int>();

        modelBuilder.Entity<Group>()
			.Property<string>("_name") // tells EF to map the private field
			.HasColumnName("Name");

        modelBuilder.Entity<Student>()
			.HasOne(s => s.User)
			.WithMany(u => u.Students)
			.HasForeignKey(s => s.UserId)
			.OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Student>()
			.HasOne(s => s.Group)
			.WithMany(g => g.Students)
			.HasForeignKey(s => s.GroupId)
			.OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<User>().HasData(
             new User()
             {
                 Id = 1,
                 FirstName = "Админ",
                 LastName = "Админов",
                 Number = "123",
                 Login = "admin@mail.ru",
                 Password = "admin",
                 AppRole = AppRole.Admin,
             }
		);
        
		//     modelBuilder
		//.AddFakeData();
	}
}


//static class ModelBuilderExtensions
//{
//	public static ModelBuilder AddFakeData(this ModelBuilder builder)
//	{

//        Randomizer.Seed = new Random(8675309);

//		var statuses = new List<Status>
//		{
//			new Status { Id = 1, Value = true },
//			new Status { Id = 2, Value = false }
//		};
//		builder.Entity<Status>().HasData(statuses);

//		var usersFaker = new Faker<User>("ru")
//			.RuleFor(u => u.FirstName, (f, u) => u.FirstName = f.Person.FirstName)
//			.RuleFor(u => u.LastName, (f, u) => u.LastName = f.Person.LastName)
//			.RuleFor(u => u.Id, (f, u) => u.Id = f.IndexFaker)
//			.RuleFor(u => u.Login, (f, u) => u.Login = f.Person.UserName)
//			.RuleFor(u => u.Password, (f, u) => u.Password = f.Internet.Password())
//			.RuleFor(u => u.Number, (f, u) => u.Number = f.Phone.PhoneNumber())
//			.RuleFor(u => u.DateOfBirth, (f, u) => u.DateOfBirth = DateOnly.FromDateTime(f.Date.Past()));
//		_ = usersFaker.Generate(1);
//		var users = usersFaker.Generate(5);

//            users.Add(new User()
//            {
//				Id = users.Max(x => x.Id) + 1,
//				FirstName = "Админ",
//				LastName = "Админов",
//				Number = "123",
//				Login = "admin@mail.ru",
//				Password = "admin",
//				AppRole = AppRole.Admin,
//            });

//            users.Add(new User()
//            {
//                Id = users.Max(x => x.Id) + 1,
//                FirstName = "Мария",
//				LastName = "Учитель",
//                Number = "456",
//                Login = "teacher@mail.ru",
//                Password = "teacher",
//				AppRole = AppRole.Teacher
//            });

//            users.Add(new User()
//            {
//                Id = users.Max(x => x.Id) + 1,
//                FirstName = "Виталик",
//                LastName = "Учитель",
//                Number = "789",
//                Login = "vitalik_teacher@mail.ru",
//                Password = "teacher",
//                AppRole = AppRole.Teacher
//            });

//			users.Add(new User()
//			{
//			    Id = users.Max(x => x.Id) + 1,
//			    FirstName = "Борис",
//			    LastName = "Родитель",
//			    Number = "000",
//			    Login = "parent@mail.ru",
//			    Password = "parent",
//			    AppRole = AppRole.User
//			});

//			users.Add(new User()
//			{
//				Id = users.Max(x => x.Id) + 1,
//				FirstName = "Анна",
//				LastName = "Родитель",
//				Number = "000",
//				Login = "anna_parent@mail.ru",
//				Password = "parent",
//				AppRole = AppRole.User
//			});

//        builder.Entity<User>().HasData(users);

//		var coursesFaker = new Faker<Course>("ru")
//						.RuleFor(u => u.Name, (f, u) => u.Name = f.Lorem.Word())
//						.RuleFor(u => u.Id, (f, u) => u.Id = f.IndexFaker + 1)
//						.RuleFor(u => u.Description, (f, u) => u.Description = f.Lorem.Paragraph());
//		var courses = coursesFaker.Generate(5);
//		builder.Entity<Course>().HasData(courses);

//		var groupsFaker = new Faker<Group>("ru")
//			.RuleFor(u => u.Id, (f, u) => u.Id = f.IndexFaker + 1)
//			.RuleFor(u => u.GroupType, f => GroupType.Regular)
//			.RuleFor(u => u.Name, (f, u) => u.Name = f.Lorem.Word())
//			.RuleFor(u => u.CourseId, (f, u) => u.CourseId = f.PickRandom(courses).Id)
//			.RuleFor(u => u.TeacherId, (f, u) => u.TeacherId = f.PickRandom(users.Where(x => x.AppRole == AppRole.Teacher)).Id);
//		var groups = groupsFaker.Generate(3);

//		var group = new Group()
//		{
//			Id = 150,
//			Name = "Индив",
//			CourseId = courses[0].Id,
//			GroupType = GroupType.Personal,
//			TeacherId = users.FirstOrDefault(u => u.FirstName == "Мария" && u.AppRole == AppRole.Teacher).Id
//		};

//        groups.Add(group);

//		builder.Entity<Group>().HasData(groups);

//		var studentsFaker = new Faker<Students>("ru")
//			.RuleFor(s => s.FirstName, (f, s) => s.FirstName = f.Person.FirstName)
//			.RuleFor(s => s.LastName, (f, s) => s.LastName = f.Person.LastName)
//			//.RuleFor(s => s.MiddleName, (f, s) => s.MiddleName = f.Person.)
//			.RuleFor(s => s.Id, (f, s) => s.Id = f.IndexFaker + 1)
//			.RuleFor(s => s.DateOfBirth, (f, s) => s.DateOfBirth = DateOnly.FromDateTime(f.Date.Past()))
//			.RuleFor(s => s.GroupId, (f, s) => s.GroupId = f.PickRandom(groups.Where(g => g.GroupType != GroupType.Personal)).Id)
//			.RuleFor(s => s.UserId,
//				(f, s) => s.UserId = f.PickRandom(users.Where(u => u.AppRole == AppRole.User)).Id);
//		var students = studentsFaker.Generate(5);

//		var student = students[0];

//		student.GroupId = group.Id;

//        builder.Entity<Students>().HasData(students);

//		var lessonsFaker = new Faker<Lesson>("ru")
//			.RuleFor(u => u.Id, (f, u) => u.Id = f.IndexFaker + 1)
//			.RuleFor(l => l.GroupId, (f, l) => l.GroupId = f.PickRandom(groups).Id)
//			.RuleFor(l => l.Start, f => f.Date.Between(DateTime.Now.AddMonths(-3), DateTime.Now.AddMonths(+3)));
//		var lessons = lessonsFaker.Generate(60);
//        builder.Entity<Lesson>().HasData(lessons);

//        return builder;
//	}
//}