using System;
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
    public class ADOService
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
            int startYear = int.Parse(Console.ReadLine());
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
        public static void ListOfStudents()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"SELECT StudId, FirstName, LastName FROM Students";
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Console.WriteLine("Students: ");
                        List<int> studentID = new List<int>();

                        while (reader.Read())
                        {
                            int studentId = (int)reader["StudId"];
                            string firstName = reader["FirstName"].ToString();
                            string lastName = reader["LastName"].ToString();
                            Console.WriteLine($"Name: {firstName} {lastName}. StudentID: {studentId}");
                            studentID.Add(studentId);
                        }
                        Console.WriteLine("Please enter a studentID:");
                        int studId = int.Parse(Console.ReadLine());
                        ADOService.StudentGrades(studId);
                        
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error {ex.Message}");
                }
            }
        }
        public static void StudentGrades(int studentId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"SELECT grades.GradeChar, grades.GradeDate, staff.StaffName, subjects.SubjectName
                             FROM Grades grades
                             JOIN Staff staff ON grades.TeacherId = staff.StaffId
                             JOIN Subjects subjects ON grades.SubjectId = subjects.SubjectId
                             WHERE grades.StudentId = @StudentId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@StudentId", studentId);
                try
                {
                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        Console.WriteLine($"All the grades for student: {studentId}");
                        while (reader.Read())
                        {
                            string grade = reader["GradeChar"].ToString();
                            DateTime gradeDate = Convert.ToDateTime(reader["GradeDate"]);
                            string teacherName = reader["StaffName"].ToString();
                            string subjectName = reader["SubjectName"].ToString();

                            Console.WriteLine($"Subject: {subjectName} - Grade: {grade} - Grading teacher: {teacherName} - Date of Grade: {gradeDate}");
                        }
                        Console.WriteLine();
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error! {ex.Message}");
                }
            }
        }
        public static void DepartmentWages()
        {
            using(SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"SELECT Department.DepartmentName, SUM(Staff.Salary) AS TotalSalary
                                FROM Staff
                                JOIN Occupation ON Staff.OccupationId = Occupation.OccupationId
                                JOIN Department ON Occupation.DepartmentId = Department.DepartmentId
                                GROUP BY Department.DepartmentName";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        Console.WriteLine("Total wages for each department: ");
                        while (reader.Read())
                        {
                            string departmentName = reader["DepartmentName"].ToString();
                            decimal totalSalary = Convert.ToDecimal(reader["TotalSalary"]);
                            Console.WriteLine($"Department: {departmentName} - Total Salary: {totalSalary:C}");
                        }
                        Console.WriteLine();
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error! {ex.Message}");
                }
                
            }
        }
        public static void AverageWage()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"SELECT department.DepartmentName, AVG(staff.Salary) AS AverageSalary,COUNT(staff.StaffId) AS TotalEmployees
                                FROM Staff staff
                                JOIN Occupation occupation ON staff.OccupationId = occupation.OccupationId
                                JOIN Department department ON occupation.DepartmentId = department.DepartmentId
                                GROUP BY department.DepartmentName
                                ORDER BY department.DepartmentName";
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        Console.WriteLine($"Average salary per Department: ");
                        while (reader.Read())
                        {
                            string departmentName = reader["DepartmentName"].ToString();
                            decimal averageSalary = Convert.ToDecimal(reader["AverageSalary"]);
                            int totalEmployees = Convert.ToInt32(reader["TotalEmployees"]);

                            Console.WriteLine($"Department: {departmentName} - Number of Employees in Department: {totalEmployees} - Average salary per employee: {averageSalary:C}");
                        }
                        Console.WriteLine();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error! {ex.Message}");
                }
            }
        }
        public static void GetStudentInfo()
        {
            Console.WriteLine("Please enter student ID: ");
            int studentId = Int32.Parse(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("GetStudentInfo", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@StudentId", studentId);

                try
                {
                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string firstName = reader["FirstName"].ToString();
                            string lastName = reader["LastName"].ToString();
                            string className = reader["ClassName"].ToString();
                            string dob = reader["DoB"].ToString();

                            Console.WriteLine($"First name: {firstName}");
                            Console.WriteLine($"Last name: {lastName}");
                            Console.WriteLine($"Class: {className}");
                            Console.WriteLine($"Date of birth: {dob}");
                        }
                        Console.WriteLine();
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error! {ex.Message}");
                }
            }
        }
        public static void AddGrades()
        {
            Console.WriteLine("Enter student Id: ");
            int studentId = Int32.Parse(Console.ReadLine());

            Console.WriteLine("Enter subject Id: ");
            int subjectId = Int32.Parse(Console.ReadLine());

            Console.WriteLine("Enter grade, A-F: ");
            string grade = Console.ReadLine();
           

            Console.WriteLine("Enter teacher Id: ");
            int teacherId = Int32.Parse(Console.ReadLine());

            Console.WriteLine("Enter grade date(YYYY-MM-DD: ");
            DateTime gradeDate = DateTime.Parse(Console.ReadLine());

            using( SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        SqlCommand command = new SqlCommand(@"INSERT INTO Grades (StudentId, SubjectId, GradeChar, TeacherId, GradeDate)
                                                            VALUES(@StudentId,@SubjectId,@GradeChar,@TeacherId,@GradeDate)", connection);

                        command.Transaction = transaction;
                        command.Parameters.AddWithValue("@StudentId", studentId);
                        command.Parameters.AddWithValue("@SubjectId", subjectId);
                        command.Parameters.AddWithValue("@GradeChar", grade);
                        command.Parameters.AddWithValue("@TeacherId", teacherId);
                        command.Parameters.AddWithValue("@GradeDate", gradeDate);

                        try
                        {
                            command.ExecuteNonQuery();
                            transaction.Commit();
                            Console.WriteLine("New grade added.");
                        }
                        catch(Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine($"Error! {ex.Message}");
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error! {ex.Message}");
                }
            }
        }
    }



}
