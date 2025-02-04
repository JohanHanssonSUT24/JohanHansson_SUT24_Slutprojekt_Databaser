using System;
using System.Collections.Generic;

namespace JohanHansson_SUT24_Slutprojekt_Databaser.Models;

public partial class Department
{
    public int DepartmentId { get; set; }

    public string? DepartmentName { get; set; }

    public virtual ICollection<Occupation> Occupations { get; set; } = new List<Occupation>();
}
