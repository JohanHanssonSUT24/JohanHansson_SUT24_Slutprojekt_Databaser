using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohanHansson_SUT24_Slutprojekt_Databaser;

namespace JohanHansson_SUT24_Slutprojekt_Databaser.Models;

public class Methods
{
    private static SchoolDbContext CreateContext()
    {
        return new SchoolDbContext();
    }
    public static void Teachers()
    {
        using var context = CreateContext();

        var query = context.Staff
            .Where(staff => staff.OccupationNavigation.OccupationName == "Teacher")
            .GroupBy(staff => staff.OccupationNavigation.Department.DepartmentName)
            .Select(grades => new
            {
                DepartmmentName = grades.Key,
                Teacher = grades.Count()
            })
            .ToList();
        foreach (var staff in query)
        {
            Console.WriteLine($"Department {staff.DepartmmentName}\nHas {staff.Teacher} Teachers working there.");
        }
        Console.WriteLine();
    }

}
