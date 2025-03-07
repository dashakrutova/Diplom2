using Bogus;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace WebApplicationMVC.Models.Database;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions options) : base(options)
	{
		//Database.EnsureDeleted();
		//Database.EnsureCreated();
	}

	public DbSet<User> Users { get; set; }
	public DbSet<Student> Students { get; set; }
	public DbSet<Role> Roles { get; set; }
	public DbSet<Course> Courses { get; set; }
	public DbSet<Group> Groups { get; set; }
	public DbSet<Status> Statuses { get; set; }
	public DbSet<Lesson> Lessons { get; set; }
	public DbSet<Visiting> Visitings { get; set; }



	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{

	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<User>()
			.Property(o => o.AppRole)
			.HasConversion<int>();

            modelBuilder
			.AddFakeData();
	}
}


static class ModelBuilderExtensions
{
	public static ModelBuilder AddFakeData(this ModelBuilder builder)
	{

        Randomizer.Seed = new Random(8675309);

        var roles = new List<Role>
		{
			new Role() { Id = 1, Name = "Admin" },
			new Role() { Id = 2, Name = "Teacher" },
			new Role() { Id = 3, Name = "Parent" }
		};
		builder.Entity<Role>().HasData(roles);

		var statuses = new List<Status>
		{
			new Status { Id = 1, Value = true },
			new Status { Id = 2, Value = false }
		};
		builder.Entity<Status>().HasData(statuses);

		var usersFaker = new Faker<User>("ru")
            .RuleFor(u => u.FirstName, (f, u) => u.FirstName = f.Person.FirstName)
            .RuleFor(u => u.LastName, (f, u) => u.LastName = f.Person.LastName)
            .RuleFor(u => u.Id, (f, u) => u.Id = f.IndexFaker)
			.RuleFor(u => u.Login, (f, u) => u.Login = f.Person.UserName)
			.RuleFor(u => u.Password, (f, u) => u.Password = f.Internet.Password())
			.RuleFor(u => u.Number, (f, u) => u.Number = f.Phone.PhoneNumber())
			.RuleFor(u => u.DateOfBirth, (f, u) => u.DateOfBirth = DateOnly.FromDateTime(f.Date.Past()))
			.RuleFor(s => s.RoleId, (f, s) => s.RoleId = f.PickRandom(roles).Id);
		_ = usersFaker.Generate(1);
		var users = usersFaker.Generate(5);

            users.Add(new User()
            {
			Id = users.Max(x => x.Id) + 1,
			FirstName = "Админ",
			LastName = "Админов",
			Number = "123",
			RoleId = roles.First().Id,
                Login = "admin@mail.ru",
                Password = "admin",
			AppRole = AppRole.Admin,
            });

            users.Add(new User()
            {
                Id = users.Max(x => x.Id) + 1,
                FirstName = "Мария",
				LastName = "Учитель",
                Number = "456",
                RoleId = roles.First().Id,
                Login = "teacher@mail.ru",
                Password = "teacher",
			AppRole = AppRole.Teacher
            });

            users.Add(new User()
            {
                Id = users.Max(x => x.Id) + 1,
                FirstName = "Виталик",
                LastName = "Учитель",
                Number = "789",
                RoleId = roles.First().Id,
                Login = "vitalik_teacher@mail.ru",
                Password = "teacher",
                AppRole = AppRole.Teacher
            });

			users.Add(new User()
			{
			    Id = users.Max(x => x.Id) + 1,
			    FirstName = "Борис",
			    LastName = "Родитель",
			    Number = "000",
			    RoleId = roles.First().Id,
			    Login = "parent@mail.ru",
			    Password = "parent",
			    AppRole = AppRole.Parent
			});

			users.Add(new User()
			{
				Id = users.Max(x => x.Id) + 1,
				FirstName = "Анна",
				LastName = "Родитель",
				Number = "000",
				RoleId = roles.First().Id,
				Login = "anna_parent@mail.ru",
				Password = "parent",
				AppRole = AppRole.Parent
			});

        builder.Entity<User>().HasData(users);

		var coursesFaker = new Faker<Course>("ru")
						.RuleFor(u => u.Name, (f, u) => u.Name = f.Lorem.Word())
						.RuleFor(u => u.Id, (f, u) => u.Id = f.IndexFaker + 1)
						.RuleFor(u => u.Description, (f, u) => u.Description = f.Lorem.Paragraph());
		var courses = coursesFaker.Generate(5);
		builder.Entity<Course>().HasData(courses);

		var groupsFaker = new Faker<Group>("ru")
			.RuleFor(u => u.Id, (f, u) => u.Id = f.IndexFaker + 1)
			.RuleFor(u => u.Name, (f, u) => u.Name = f.Lorem.Word())
			.RuleFor(u => u.CourseId, (f, u) => u.CourseId = f.PickRandom(courses).Id)
			.RuleFor(u => u.TeacherId, (f, u) => u.TeacherId = f.PickRandom(users.Where(x => x.AppRole == AppRole.Teacher)).Id);
		var groups = groupsFaker.Generate(3);
		builder.Entity<Group>().HasData(groups);
		
		var studentsFaker = new Faker<Student>("ru")
			.RuleFor(s => s.FirstName, (f, s) => s.FirstName = f.Person.FirstName)
            .RuleFor(s => s.LastName, (f, s) => s.LastName = f.Person.LastName)
            //.RuleFor(s => s.MiddleName, (f, s) => s.MiddleName = f.Person.)
            .RuleFor(s => s.Id, (f, s) => s.Id = f.IndexFaker + 1)
			.RuleFor(s => s.DateOfBirth, (f, s) => s.DateOfBirth = DateOnly.FromDateTime(f.Date.Past()))
			.RuleFor(s => s.GroupId, (f, s) => s.GroupId = f.PickRandom(groups).Id)
			.RuleFor(s => s.ParentId, 
				(f, s) => s.ParentId = f.PickRandom(users.Where(u => u.AppRole == AppRole.Parent)).Id)
			.RuleFor(s => s.RoleId, (f, s) => s.RoleId = f.PickRandom(roles).Id);
		var students = studentsFaker.Generate(5);
		builder.Entity<Student>().HasData(students);

		var lessonsFaker = new Faker<Lesson>("ru")
			.RuleFor(u => u.Id, (f, u) => u.Id = f.IndexFaker + 1)
			.RuleFor(l => l.GroupId, (f, l) => l.GroupId = f.PickRandom(groups).Id)
			.RuleFor(l => l.Start, f => f.Date.Between(DateTime.Now.AddMonths(-3), DateTime.Now.AddMonths(+3)));
		var lessons = lessonsFaker.Generate(60);
        builder.Entity<Lesson>().HasData(lessons);

        return builder;
	}
}