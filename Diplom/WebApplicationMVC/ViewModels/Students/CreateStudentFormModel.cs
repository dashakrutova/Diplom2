﻿using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationMVC.ViewModels.Students;

public class CreateStudentFormModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? MiddleName { get; set; }

    [DataType(DataType.Date)]
    public DateOnly DateOfBirth { get; set; }
    public int GroupId { get; set; }
    public int ParentId { get; set; }
}