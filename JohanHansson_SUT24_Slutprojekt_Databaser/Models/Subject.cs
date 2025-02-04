using System;
using System.Collections.Generic;

namespace JohanHansson_SUT24_Slutprojekt_Databaser.Models;

public partial class Subject
{
    public int SubjectId { get; set; }

    public string? SubjectName { get; set; }

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
}
