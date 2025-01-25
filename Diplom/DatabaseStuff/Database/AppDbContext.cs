using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseStuff.Database
{
	public class AppDbContext : DbContext
	{
		public DbSet <User> Users { get; set; }
		public DbSet <Student> Students { get; set; }
		public DbSet <Role> Roles { get; set; }	
		public DbSet <Course> Courses { get; set; }
		public DbSet<Group> Groups { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			
		}
	}

	public class Student
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateOnly DateOfBirth { get; set; }
		public Course Course { get; set; }
		public Group Group { get; set; }
		public User Parent { get; set; }
		public Role Role { get; set; }

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
		public Role Role { get; set; }
		public List<Student> Student { get; set; }

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
		public User Teacher { get; set; }
		public List <Group> Groups { get; set; }
	}
	public class Group
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public Course Course { get; set; }
		public List <Student> Students { get; set; }
	}
	
}
