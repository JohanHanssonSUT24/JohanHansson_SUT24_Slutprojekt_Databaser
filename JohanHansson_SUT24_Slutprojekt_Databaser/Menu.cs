using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohanHansson_SUT24_Slutprojekt_Databaser;

namespace JohanHansson_SUT24_Slutprojekt_Databaser.Models;

public class Menu
{
    public static void MainMenu()
    {
        bool menuBool = true;

        while (menuBool)
        {
            Console.WriteLine("WELCOME TO SCHOOL OF FUTURE DEVELOPERS");
            Console.WriteLine("[1] Department of Teachers");
            Console.WriteLine("[2] Student information");
            Console.WriteLine("[3] Courses");
            Console.WriteLine("[4] Staff-admin");
            Console.WriteLine("[5] Student-admin");
            Console.WriteLine("[6] Payroll");
            Console.WriteLine("[7] VIP-students");
            Console.WriteLine("[8] Grades");
            Console.WriteLine("[9] EXIT");
            string userInput = Console.ReadLine();

            switch (userInput)
            {
                case "1":
                    Methods.Teachers();
                    break;
                case "2":
                    Methods.StudentInformation();
                    break;
                case "3":
                    Methods.AvalibleSubjects();
                    break;
                case "4":
                    Methods.ADOService.StaffAdmin();
                    break;
                case "5":
                    Methods.ADOService.ListOfStudents();
                    break;
                case "6":
                    break;
                case "7":
                    break;
                case "8":
                    break;
                case "9":
                    menuBool = false;
                    break;
                default:
                    Console.WriteLine("Wrong input. Pleace choose between 1-9.");
                    Console.ReadKey();
                    Console.Clear();
                    break;
                    
            }
            
        }
    }
}
