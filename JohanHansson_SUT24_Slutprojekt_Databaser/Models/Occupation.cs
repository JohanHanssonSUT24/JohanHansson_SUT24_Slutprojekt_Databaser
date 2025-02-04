using System;
using System.Collections.Generic;

namespace JohanHansson_SUT24_Slutprojekt_Databaser.Models;

public partial class Occupation
{
    public int OccupationId { get; set; }

    public string? OccupationName { get; set; }

    public int? DepartmentId { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
}
