using System;
using System.Collections.Generic;

namespace JohanHansson_SUT24_Slutprojekt_Databaser.Models;

public partial class Class
{
    public int Id { get; set; }

    public string? ClassName { get; set; }

    public int TeacherStaffId { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual Staff TeacherStaff { get; set; } = null!;
}
