using System;
using System.Collections.Generic;

namespace JohanHansson_SUT24_Slutprojekt_Databaser.Models;

public partial class Staff
{
    public int StaffId { get; set; }

    public string? StaffName { get; set; }

    public string? Occupation { get; set; }

    public int? OccupationId { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual Occupation? OccupationNavigation { get; set; }
}
