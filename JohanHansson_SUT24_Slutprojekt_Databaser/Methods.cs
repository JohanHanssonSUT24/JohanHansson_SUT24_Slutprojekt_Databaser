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
    //Method to create a "temporary" context
    private static SchoolDbContext CreateContext()
    {
        return new SchoolDbContext();
    }
    public static void Teachers()//Method to show all teachers in department
    {
        Console.Clear();
        using var context = CreateContext();
        
        var query = context.Staff
            .Where(staff => staff.OccupationNavigation.OccupationName == "Teacher") //Show only Teachers"
            .GroupBy(staff => staff.OccupationNavigation.Department.DepartmentName) //Order Teachers by department
            .Select(group => new
            {
                DepartmmentName = group.Key,
                Teacher = group.Count() //Number of teachers in given department
            })
            .ToList();
        foreach (var staff in query)//Print result
        {
            Console.WriteLine($"Department {staff.DepartmmentName}\nHas {staff.Teacher} Teachers working there.");
        }
        Console.WriteLine();
        Console.WriteLine("Press any key to return to main menu.");
        Console.ReadKey();
        Console.Clear();
    }
    public static void StudentInformation()//Method to show all student info
    {
        Console.Clear();
        using var context = CreateContext();

        var query = context.Students
            .Include(student => student.Class) //Add classinfo
            .Select(student => new
            {
                student.FirstName,
                student.LastName,
                student.Class.ClassName,
                student.DoB,
                student.ClassId
            })
            .ToList();
        Console.WriteLine("STUDENTS ENROLLED: ");
        Console.WriteLine();
        foreach (var stud in query) //Print result
        {
            Console.WriteLine($"Student name: {stud.FirstName} {stud.LastName} - DoB: {stud.DoB} - Class: {stud.ClassName} - ClassID: {stud.ClassId}");
        }
        Console.WriteLine();
        Console.WriteLine("Press any key to return to main menu.");
        Console.ReadKey();
        Console.Clear();
    }
    public static void AvalibleSubjects() //Method to show all classes avalible
    {
        Console.Clear();
        using var context = CreateContext();
        var query = context.Subjects
            .Select(subject => new
            {
                subject.SubjectName
            })
            .ToList();
        Console.WriteLine("Here are all avalible subjects this semester:\n");//Display all subject names
        foreach (var sub in query)
        {

            Console.WriteLine($"- {sub.SubjectName}");
        }
        Console.WriteLine();
        Console.WriteLine("Press any key to return to main menu."); //End methods with return option to main menu.
        Console.ReadKey();
        Console.Clear();
    }
    public class ADOService //Class for ADO
    {
        //Connection string to DB
        private static readonly string _connectionString = "Data Source=localhost;Database=SchoolDB;Integrated Security=True;Trust Server Certificate=true;";

        public static void StaffAdmin() //Method to show all staff info
        {
            Console.Clear();
            using (SqlConnection connection = new SqlConnection(_connectionString))//Connection to DB
            {
                Console.WriteLine("Staff avalible on campus: \n");
                connection.Open();
                string query = @"SELECT StaffName, Occupation, StartDate FROM Staff"; //Query
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    using (SqlDataReader reader = command.ExecuteReader())// SqlReader to read the result
                    {
                        while (reader.Read()) //Read each row in result
                        {
                            string staffName = reader["StaffName"].ToString();
                            string occupation = reader["Occupation"].ToString();
                            int startDate = reader["StartDate"] != DBNull.Value ? Convert.ToInt32(reader["StartDate"]) : 0;
                            int employmentTime = DateTime.Now.Year - startDate;
                            Console.WriteLine($"Staff name: {staffName} - Occupation: {occupation} - Employmentyears: {employmentTime}");
                        }
                        Console.WriteLine();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error! {ex.Message}"); //Error message
                }
                Console.WriteLine("Do you wish to add a new staff menmber? Y/N"); //Adding option to add new teacher. Call on method - AddStaffMember
                string userInput = Console.ReadLine().ToUpper();
                if (userInput == "Y".ToUpper())
                {
                    AddStaffMember();
                }
                else
                {
                    Console.WriteLine("Thank you for using our StaffAdmin-database. Until next time");
                }
                Console.WriteLine();
                Console.WriteLine("Press any key to return to main menu.");
                Console.ReadKey();
                Console.Clear();
            }
        }
        public static void AddStaffMember()//Method to add new staff
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
                //Get OccupationId
                string occupationId = @"SELECT OccupationId FROM Occupation WHERE OccupationName = @Occupation";

                SqlCommand occupationIdCommand = new SqlCommand(occupationId, connection);
                occupationIdCommand.Parameters.AddWithValue("@Occupation", occupation);

                int occupationIdValue = -1;
                try
                {
                    object result = occupationIdCommand.ExecuteScalar();

                    if (result != null)
                    {
                        occupationIdValue = Convert.ToInt32(result);
                    }
                    else
                    {
                        Console.WriteLine("Occupation not found.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error! {ex.Message}");
                    return;
                }
                string query = @"INSERT INTO Staff (StaffName, Occupation, StartDate, Salary, OccupationId)
                            VALUES (@StaffName, @Occupation, @StartDate, @Salary, @OccupationId)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@StaffName", staffName);
                command.Parameters.AddWithValue("@Occupation", occupation);
                command.Parameters.AddWithValue("@StartDate", startYear);
                command.Parameters.AddWithValue("@Salary", salary);
                command.Parameters.AddWithValue("@OccupationId", occupationIdValue);
                try
                {
                    command.ExecuteNonQuery(); //Execute INSERT to add new member to staff
                    Console.WriteLine("New memeber added to the staff.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error! {ex.Message}");
                }
            }
        }
        public static void ListOfStudents() //Method to list all students
        {
            Console.Clear();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"SELECT StudId, FirstName, LastName FROM Students";//Query
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    using (SqlDataReader reader = command.ExecuteReader())//Execute query and show result
                    {
                        Console.WriteLine("All Students enrolled: ");//Show all students
                        List<int> studentID = new List<int>();

                        while (reader.Read())
                        {
                            int studentId = (int)reader["StudId"];
                            string firstName = reader["FirstName"].ToString();
                            string lastName = reader["LastName"].ToString();
                            Console.WriteLine($"Name: {firstName} {lastName}. StudentID: {studentId}");
                            studentID.Add(studentId);
                        }
                        Console.WriteLine();
                        int studId = 0;
                        bool validNumber = false;
                        while (!validNumber)
                        {
                            Console.WriteLine("Please enter a studentID:");//Choose studentId
                            string userInput = Console.ReadLine();
                            if (int.TryParse(userInput, out studId))
                            {
                                if (studId > 0 && studentID.Contains(studId))//If statement to verify user input compared to students in DB
                                {
                                    validNumber = true;
                                }
                                else
                                {
                                    Console.WriteLine("Invalid StudentID. Please choose from the list.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Enter studentID");
                                Console.WriteLine();
                            }
                        }
                        ADOService.StudentGrades(studId);//Show grades for student
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error {ex.Message}");
                }
            }
        }
        public static void StudentGrades(int studentId)//Method to show student grades
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                //Query to show subject, grade, grading teacher and grade date
                string query = @"SELECT grades.GradeChar, grades.GradeDate, staff.StaffName, subjects.SubjectName
                             FROM Grades grades
                             JOIN Staff staff ON grades.TeacherId = staff.StaffId
                             JOIN Subjects subjects ON grades.SubjectId = subjects.SubjectId
                             WHERE grades.StudentId = @StudentId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@StudentId", studentId);
                try
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Console.WriteLine($"All the grades for studentID: {studentId}");

                        while (reader.Read())
                        {
                            string grade = reader["GradeChar"].ToString();
                            DateTime gradeDate = Convert.ToDateTime(reader["GradeDate"]);
                            string teacherName = reader["StaffName"].ToString();
                            string subjectName = reader["SubjectName"].ToString();

                            Console.WriteLine($"Subject: {subjectName} - Grade: {grade} - Grading teacher: {teacherName} - Date of Grade: {gradeDate}");
                        }
                        Console.WriteLine();
                        Console.WriteLine("Press any key to return to main menu.");
                        Console.ReadKey();
                        Console.Clear();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error! {ex.Message}");
                }
            }
        }
        public static void DepartmentWages()//Method to show sum of wages in each department
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                Console.Clear();
                connection.Open();
                string query = @"SELECT Department.DepartmentName, SUM(Staff.Salary) AS TotalSalary
                                FROM Staff
                                JOIN Occupation ON Staff.OccupationId = Occupation.OccupationId
                                JOIN Department ON Occupation.DepartmentId = Department.DepartmentId
                                GROUP BY Department.DepartmentName";
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    using (SqlDataReader reader = command.ExecuteReader())
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
                catch (Exception ex)
                {
                    Console.WriteLine($"Error! {ex.Message}");
                }
            }
        }
        public static void AverageWage()//Method for average wage in each department
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
                    using (SqlDataReader reader = command.ExecuteReader())
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
                Console.WriteLine("Press any key to return to main menu.");
                Console.ReadKey();
                Console.Clear();
            }
        }
        public static void GetStudentInfo()//Method to show certain student information
        {
            Console.Clear();
            using var context = CreateContext();

            var query = context.Students
                .Include(student => student.Class)
                .Select(student => new
                {
                    student.FirstName,
                    student.LastName,
                    student.StudId
                })
                .ToList();
            Console.WriteLine("STUDENTS ENROLLED: ");
            Console.WriteLine();
            foreach (var stud in query)
            {
                Console.WriteLine($"Student name: {stud.FirstName} {stud.LastName} - StudentId: {stud.StudId}");
            }
            Console.WriteLine();
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
                    using (SqlDataReader reader = command.ExecuteReader())
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
                catch (Exception ex)
                {
                    Console.WriteLine($"Error! {ex.Message}");
                }
                Console.WriteLine("Press any key to return to main menu.");
                Console.ReadKey();
                Console.Clear();
            }
        }
        public static void AddGrades()
        {
            using var context = CreateContext();
            var query = context.Students
                .Include(student => student.Class)
                .Select(student => new
                {
                    student.FirstName,
                    student.LastName,
                    student.StudId
                })
                .ToList();
            Console.WriteLine();
            foreach (var stud in query)
            {
                Console.WriteLine($"Student name: {stud.FirstName} {stud.LastName} - StudentId: {stud.StudId}");
            }
            Console.WriteLine("Enter student Id: ");
            int studentId = Int32.Parse(Console.ReadLine());

            using var contextSub = CreateContext();
            var querySub = context.Subjects
                
                .Select(subjects => new
                {
                    subjects.SubjectId,
                    subjects.SubjectName,
                })
                .ToList();
            Console.WriteLine();
            foreach (var sub in querySub)
            {
                Console.WriteLine($"SubjectId: {sub.SubjectId} Subject: {sub.SubjectName}");
            }
            Console.WriteLine("Enter subject Id: ");
            int subjectId = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Enter grade, A-F: ");
            string grade = Console.ReadLine();

            using var contextTeach = CreateContext();
            var queryStaff = context.Staff
                .Select(staff => new
                {
                    staff.StaffName,
                    staff.StaffId
                })
                .ToList();
            Console.WriteLine();
            foreach (var staff in queryStaff)
            {
                Console.WriteLine($"StaffId: {staff.StaffId} Staff name: {staff.StaffName}");
            }
            Console.WriteLine("Enter teacher Id: ");
            int teacherId = Int32.Parse(Console.ReadLine());

            Console.WriteLine("Enter grade date(YYYY-MM-DD): ");
            DateTime gradeDate = DateTime.Parse(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(_connectionString))
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
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine($"Error! {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error! {ex.Message}");
                }
                Console.WriteLine("Press any key to return to main menu.");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}
