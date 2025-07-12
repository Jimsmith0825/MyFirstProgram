using System;      // Provides base class library types like Console, String, etc. Zhenqiao He
using System.IO;   // Provides types for file input/output operations , Zhenqiao He
using System.Linq; // Provides LINQ extension methods for arrays and collections

namespace PayrollApp
{
    // Employee Struct: Stores individual employee data
    struct Employee
    {
        // Fields for each employee's details
        public int ID;
        public string FirstName;
        public string LastName;
        public double AnnualIncome;
        public double KiwiSaverRate;   // KiwiSaver contribution rate (e.g. 0.03 for 3%)
        public double FortnightlyPay;  // Calculated pay every 2 weeks
        public double HourlyWage;      // Calculated hourly wage

        // Returns the header for displaying employee table
        public static string Header()
        {
            return string.Format("{0,-5} {1,-10} {2,-10} {3,10} {4,10} {5,12} {6,18}",
                                   "ID", "FirstName", "LastName", "Income", "KiwiSaver%", "HourlyWage", "FortnightlyPayroll");
        }

        // Formats employee details into a row string for display
        public string ToRowString()
        {
            return string.Format("{0,-5} {1,-10} {2,-10} {3,10:F2} {4,10:F0} {5,12:F2} {6,18:F2}",
                                 ID, FirstName, LastName, AnnualIncome, KiwiSaverRate * 100, HourlyWage, FortnightlyPay);
        }
    }

    // PayrollSystem Class: Manages overall payroll process
    class PayrollSystem
    {
        // Array of employee records
        static Employee[] employees;

        // State control variables
        static bool isDataLoaded = false;          // Flag to indicate if data is loaded
        static bool isPayrollCalculated = false;   // Flag to indicate if payroll is calculated

        // Entry point method of the program
        static void Main()
        {
            Console.WriteLine("Welcome to Kiwi Garage. Press Enter to Continue to the Main Menu");
            Console.ReadLine(); // Pause for user input

            LoadPayrollData();  // Load employee data from file
            isDataLoaded = true; // Mark data as loaded

            int option;
            do
            {
                // Display menu options to the user
                Console.WriteLine("\n--- NewKiwi Garage Payroll Menu ---");
                Console.WriteLine("1. Calculate Fortnightly Payroll");
                Console.WriteLine("2. Sort and Display Employees");
                Console.WriteLine("3. Search Employee by ID");
                Console.WriteLine("4. Save to File");
                Console.WriteLine("0. Exit");
                Console.Write("Enter your option: ");
                string input = Console.ReadLine(); // Get user input

                // Input validation
                if (!int.TryParse(input, out option))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                // Handle menu option
                switch (option)
                {
                    case 1:
                        CalculatePayroll(); // Compute payroll
                        isPayrollCalculated = true;
                        break;
                    case 2:
                        if (isDataLoaded)
                            SortAndDisplay(); // Display sorted employee list
                        else
                            Console.WriteLine("Please calculate payroll first.");
                        break;
                    case 3:
                        if (isDataLoaded)
                            SearchEmployee(); // Search for employee by ID
                        else
                            Console.WriteLine("Please calculate payroll first.");
                        break;
                    case 4:
                        if (isDataLoaded && isPayrollCalculated)
                            SaveToFile(); // Save payroll to file
                        else
                            Console.WriteLine("Please calculate payroll and display employees first.");
                        break;
                    case 0:
                        Console.WriteLine("Goodbye!"); // Exit program
                        break;
                    default:
                        Console.WriteLine("Invalid option."); // Handle unexpected input
                        break;
                }
            } while (option != 0); // Repeat until user chooses to exit
        }

        // LoadPayrollData: Reads employee records from text file
        static void LoadPayrollData()
        {
            string[] lines = File.ReadAllLines("employee.txt"); // Load all lines from file
            employees = new Employee[lines.Length]; // Create array with correct size

            for (int i = 0; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(','); // Split each line into fields
                
                employees[i] = new Employee
                {
                    ID = int.Parse(parts[0].Trim()),
                    FirstName = parts[1].Trim(),
                    LastName = parts[2].Trim(),
                    AnnualIncome = double.Parse(parts[3].Trim()),
                    KiwiSaverRate = double.Parse(parts[4].Trim().TrimEnd('%')) / 100    // Convert percentage to decimal
                };
            }
        }

        // CalculatePayroll: Computes tax, KiwiSaver, and net pay
        static void CalculatePayroll()
        {
            Console.WriteLine("\nCalculating Fortnightly Payroll...\n");
            Console.WriteLine(Employee.Header()); // Print header

            for (int i = 0; i < employees.Length; i++)
            {
                double income = employees[i].AnnualIncome;
                double rate = employees[i].KiwiSaverRate;
                double kiwiSaver = income * rate;
                double tax = 0;

                // Calculate tax based on NZ 2025 tax brackets
                if (income <= 15600)
                    tax = income * 0.105;
                else if (income <= 53500)
                    tax = 15600 * 0.105 + (income - 15600) * 0.175;
                else if (income <= 78100)
                    tax = 15600 * 0.105 + (53500 - 15600) * 0.175 + (income - 53500) * 0.30;
                else if (income <= 180000)
                    tax = 15600 * 0.105 + (53500 - 15600) * 0.175 +
                          (78100 - 53500) * 0.30 + (income - 78100) * 0.33;
                else
                    tax = 15600 * 0.105 + (53500 - 15600) * 0.175 +
                          (78100 - 53500) * 0.30 + (180000 - 78100) * 0.33 +
                          (income - 180000) * 0.39;

                // Calculate fortnightly pay and hourly wage
                employees[i].FortnightlyPay = (income - tax - kiwiSaver) / 26;
                employees[i].HourlyWage = income / 52 / 40;

                Console.WriteLine(employees[i].ToRowString()); // Output result
            }

            Console.WriteLine("\nFortnightly Payroll Calculated\n");
        }

        // SortAndDisplay: Sorts employees by ID and displays data
        static void SortAndDisplay()
        {
            // Bubble sort employees by ID
            for (int i = 0; i < employees.Length - 1; i++)
            {
                for (int j = 0; j < employees.Length - i - 1; j++)
                {
                    if (employees[j].ID > employees[j + 1].ID)
                    {
                        var temp = employees[j]; // Swap elements
                        employees[j] = employees[j + 1];
                        employees[j + 1] = temp;
                    }
                }
            }

            Console.WriteLine("\nEmployee records sorted by ID:\n");
            Console.WriteLine(Employee.Header()); // Print table header

            foreach (var emp in employees)
            {
                Console.WriteLine(emp.ToRowString()); // Display each employee
            }
        }

        // SearchEmployee: Searches for an employee by ID and displays result
        static void SearchEmployee()
        {
            Console.Write("Enter employee ID to search: ");
            int id = int.Parse(Console.ReadLine());
            bool found = false;

            foreach (var emp in employees)
            {
                if (emp.ID == id)
                {
                    Console.WriteLine(Employee.Header()); // Print header
                    Console.WriteLine(emp.ToRowString()); // Print matching employee
                    found = true;
                    break;
                }
            }

            if (!found)
                Console.WriteLine("Employee not found.");
        }

        // SaveToFile: Writes employee payroll data to an output file
        static void SaveToFile()
        {
            using (StreamWriter writer = new StreamWriter("fortnightlypayroll.txt"))
            {
                writer.WriteLine(Employee.Header()); // Write header first
                foreach (var emp in employees)
                {
                    writer.WriteLine(emp.ToRowString()); // Write each record
                }
            }

            Console.WriteLine("Data saved to fortnightlypayroll.txt"); // Confirmation
        }
    }
}