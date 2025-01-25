
using Bogus;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace WebApplicationMVC.Models.Database
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions options) : base(options)
		{
			//Database.EnsureDeleted();
			Database.EnsureCreated();
		}

		public DbSet<User> Users { get; set; }
		public DbSet<Student> Students { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<Course> Courses { get; set; }
		public DbSet<Group> Groups { get; set; }
		public DbSet<Status> Statuses { get; set; }
		public DbSet<ScheduleEntry> ScheduleEntries { get; set; }
		public DbSet<Visiting> Visitings { get; set; }



		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

			modelBuilder
				.AddFakeData();
		}
	}


	static class ModelBuilderExtensions
	{
		//public static string Pow(Faker f, User u)
		//{
		//    return u.Name = f.Person.UserName;
		//}

		public static ModelBuilder AddFakeData(this ModelBuilder builder)
		{
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

			var usersFaker = new Faker<User>()
				.RuleFor(u => u.Name, (f, u) => u.Name = f.Person.FullName)
				.RuleFor(u => u.Id, (f, u) => u.Id = f.IndexFaker)
				.RuleFor(u => u.Login, (f, u) => u.Login = f.Person.UserName)
				.RuleFor(u => u.Password, (f, u) => u.Password = f.Internet.Password())
				.RuleFor(u => u.Number, (f, u) => u.Number = f.Phone.PhoneNumber())
				.RuleFor(u => u.DateOfBirth, (f, u) => u.DateOfBirth = DateOnly.FromDateTime(f.Date.Past()))
				.RuleFor(s => s.RoleId, (f, s) => s.RoleId = f.PickRandom(roles).Id);
			_ = usersFaker.Generate(1);
			var users = usersFaker.Generate(5);
			builder.Entity<User>().HasData(users);

			var coursesFaker = new Faker<Course>()
							.RuleFor(u => u.Name, (f, u) => u.Name = f.Lorem.Word())
							.RuleFor(u => u.Id, (f, u) => u.Id = f.IndexFaker + 1)
							.RuleFor(u => u.Description, (f, u) => u.Description = f.Lorem.Paragraph())
							.RuleFor(u => u.TeacherId, (f, u) => u.TeacherId = f.PickRandom(users.Where(u => u.RoleId == 2)).Id);
			var courses = coursesFaker.Generate(5);
			builder.Entity<Course>().HasData(courses);

			var groupsFaker = new Faker<Group>()
				.RuleFor(u => u.Id, (f, u) => u.Id = f.IndexFaker + 1)
				.RuleFor(u => u.Name, (f, u) => u.Name = f.Lorem.Word())
				.RuleFor(u => u.CourseId, (f, u) => u.CourseId = f.PickRandom(courses).Id);
			var groups = groupsFaker.Generate();
			builder.Entity<Group>().HasData(groups);


			var studentsFaker = new Faker<Student>()
				.RuleFor(s => s.Name, (f, s) => s.Name = f.Person.FullName)
				.RuleFor(s => s.Id, (f, s) => s.Id = f.IndexFaker + 1)
				.RuleFor(s => s.DateOfBirth, (f, s) => s.DateOfBirth = DateOnly.FromDateTime(f.Date.Past()))
				.RuleFor(s => s.CourseId, (f, s) => s.CourseId = f.PickRandom(courses).Id)
				.RuleFor(s => s.GroupId, (f, s) => s.GroupId = f.PickRandom(groups).Id)
				.RuleFor(s => s.ParentId, (f, s) => s.ParentId = f.PickRandom(users).Id)
				.RuleFor(s => s.RoleId, (f, s) => s.RoleId = f.PickRandom(roles).Id);
			var students = studentsFaker.Generate(5);
			builder.Entity<Student>().HasData(students);


			return builder;
		}


	}

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

	}
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

	public class Role
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}


	public class Course
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }

		public int TeacherId { get; set; }
		public User Teacher { get; set; }
		public List<Group> Groups { get; set; }
	}
	public class Group
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int CourseId { get; set; }
		public Course Course { get; set; }
		public List<Student> Students { get; set; }
	}
	public class ScheduleEntry
	{
		public int Id { get; set; }
		public int GroupId { get; set; }
		public Group Group { get; set; }
		public DateTimeOffset Start { get; set; }
		public DateTimeOffset End { get; set; }
	}
	public class Status
	{
		public int Id { get; set; }
		public bool Value { get; set; }
	}
	public class Visiting
	{
		public int Id { get; set; }
		public int StatusId { get; set; }
		public Status Status { get; set; }
		public int ScheduleEntryId { get; set; }
		public ScheduleEntry ScheduleEntry { get; set; }
	}

}
