﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohanHansson_SUT24_Slutprojekt_Databaser;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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
    public static void StudentInformation()
    {
        using var context = CreateContext();

        var query = context.Students
            .Include(student => student.Class)
            .Select(student => new
            {
                student.FirstName,
                student.LastName,
                student.Class.ClassName,
                student.DoB,
                student.ClassId
            })
            .ToList();
        foreach (var stud in query)
        {
            Console.WriteLine($"Student name: {stud.FirstName} {stud.LastName} - DoB: {stud.DoB} - Class: {stud.ClassName} - ClassID: {stud.ClassId}");
        }
        Console.WriteLine();
    }
    public static void AvalibleSubjects()
    {
        using var context = CreateContext();

        var query = context.Subjects
            .Select(subject => new
            {
                subject.SubjectName
            })
            .ToList();
        Console.WriteLine("Here are all avalible subjects this semester:\n");
        foreach (var sub in query)
        {

            Console.WriteLine($"- {sub.SubjectName}");
        }
        Console.WriteLine();
    }
    internal class ADOService
    {
        private static readonly string _connectionString = "Data Source=localhost;Database=SchoolDB;Integrated Security=True;Trust Server Certificate=true;";

        public static void StaffAdmin()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))//ANVÄND TRY CATCH
            {
                connection.Open();
                string query = @"SELECT StaffName, Occupation, StartDate FROM Staff";
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string staffName = reader["StaffName"].ToString();
                            string occupation = reader["Occupation"].ToString();
                            int startDate = (int)reader["StartDate"];
                            int employmentTime = DateTime.Now.Year - startDate;
                            Console.WriteLine($"Staff name: {staffName} - Occupation: {occupation} - Employmentyears: {employmentTime}");
                        }
                        Console.WriteLine();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error! {ex.Message}");
                }
                Console.WriteLine("Do you wish to add a new staff menmber? Y/N");
                string userInput = Console.ReadLine().ToUpper();
                if(userInput == "Y".ToUpper())
                {
                    AddStaffMember();
                }
                else
                {
                    Console.WriteLine("Thank you for using our StaffAdmin-database. Until next time");
                }
                Console.WriteLine();
            }
        }
        public static void AddStaffMember()
        {
            Console.WriteLine("Enter full name: ");
            string staffName = Console.ReadLine();
            Console.WriteLine("Enter occupation: ");
            string occupation = Console.ReadLine();
            Console.WriteLine("Enter startyear(20XX): ");
            string startYear = Console.ReadLine();
            Console.WriteLine("Enter salary: ");
            decimal salary = decimal.Parse(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"INSERT INTO Staff (StaffName, Occupation, StartDate, Salary)
                            VALUES (@StaffName, @Occupation, @StartDate, @Salary)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@StaffName", staffName);
                command.Parameters.AddWithValue("@Occupation", occupation);
                command.Parameters.AddWithValue("@StartDate", startYear);
                command.Parameters.AddWithValue("@Salary", salary);
                try
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("New memeber added to the staff.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error! {ex.Message}");
                }

            }
        }
    }



}
