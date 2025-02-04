using System;
using System.Collections.Generic;

namespace JohanHansson_SUT24_Slutprojekt_Databaser.Models;

public partial class Student
{
    public int StudId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateOnly? DoB { get; set; }

    public int? ClassId { get; set; }

    public virtual Class? Class { get; set; }

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
}
