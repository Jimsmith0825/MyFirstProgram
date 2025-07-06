using System;      // Provides base class library types like Console, String, etc.
using System.IO;   // Provides types for file input/output operations
using System.Linq; 

namespace PayrollApp
{ 
/////////////////////////////////////////////////////
// Employee Struct: Stores individual employee data//
////////////////////////////////////////////////////
struct Employee
{
    // Fields for each employee's details
    public int ID;
    public string FirstName;
    public string LastName;
    public double AnnualIncome;
    public double KiwiSaverRate;   // KiwiSaver contribution rate (e.g. 0.03 for 3%)
    public double FortnightlyPay; // Calculated pay every 2 weeks
    public double HourlyWage;     // Calculated hourly wage

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

//////////////////////////////////////////////////////////
// PayrollSystem Class: Manages overall payroll process//
////////////////////////////////////////////////////////
class PayrollSystem
{
    // Array of employee records
    static Employee[] employees;

    // State control variables
    static bool isDataLoaded = false;
    static bool isPayrollCalculated = false;

    // Entry point
    static void Main()
    {
        Console.WriteLine("Welcome to Kiwi Garage. Press Enter to Continue to the Main Menu");
        Console.ReadLine(); // Pause for user

        LoadPayrollData();  // Load employee data from file
        isDataLoaded = true;

        int option;
        do
        {
            // Display menu
            Console.WriteLine("\n--- NewKiwi Garage Payroll Menu ---");
            Console.WriteLine("1. Calculate Fortnightly Payroll");
            Console.WriteLine("2. Sort and Display Employees");
            Console.WriteLine("3. Search Employee by ID");
            Console.WriteLine("4. Save to File");
            Console.WriteLine("0. Exit");
            Console.Write("Enter your option: ");
            string input = Console.ReadLine();
            if (!int.TryParse(input, out option))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }

                // Switch based on user input
                switch (option)
            {
                case 1: CalculatePayroll(); isPayrollCalculated = true; break;
                case 2:
                    if (isDataLoaded)
                        SortAndDisplay();
                    else
                        Console.WriteLine("Please calculate payroll first.");
                    break;
                case 3:
                    if (isDataLoaded)
                        SearchEmployee();
                    else
                        Console.WriteLine("Please calculate payroll first.");
                    break;
                case 4:
                    if (isDataLoaded && isPayrollCalculated)
                        SaveToFile();
                    else
                        Console.WriteLine("Please calculate payroll and display employees first.");
                    break;
                case 0: Console.WriteLine("Goodbye!"); break;
                default: Console.WriteLine("Invalid option."); break;
            }
        } while (option != 0); // Loop until user exits
    }

    ////////////////////////////////////////////////////////////
    // LoadPayrollData: Reads employee records from text file//
    //////////////////////////////////////////////////////////
    static void LoadPayrollData()
    {
        string[] lines = File.ReadAllLines("employee.txt"); // Load all lines from file
        employees = new Employee[lines.Length]; // Allocate array based on line count

        for (int i = 0; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(','); // Split each line by comma
            employees[i] = new Employee
            {
                ID = int.Parse(parts[0].Trim()),
                FirstName = parts[1].Trim(),
                LastName = parts[2].Trim(),
                AnnualIncome = double.Parse(parts[3].Trim()),
                KiwiSaverRate = double.Parse(parts[4].Trim().TrimEnd('%')) / 100 // Convert % to decimal
            };
        }
    }

    //////////////////////////////////////////////////////////
    // CalculatePayroll: Computes tax, KiwiSaver, pay, etc.//
    ////////////////////////////////////////////////////////
    static void CalculatePayroll()
    {
        Console.WriteLine("\nCalculating Fortnightly Payroll...\n");
        Console.WriteLine(Employee.Header());

        for (int i = 0; i < employees.Length; i++)
        {
            double income = employees[i].AnnualIncome;
            double rate = employees[i].KiwiSaverRate;
            double kiwiSaver = income * rate;
            double tax = 0;

            // NZ Income Tax calculation (2025 rates)
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

            // Final pay and wage calculations
            employees[i].FortnightlyPay = (income - tax - kiwiSaver) / 26;
            employees[i].HourlyWage = income / 52 / 40;

            Console.WriteLine(employees[i].ToRowString());
        }

        Console.WriteLine("\nFortnightly Payroll Calculated\n");
    }

    ////////////////////////////////////////////////////////////////////
    // SortAndDisplay: Sorts employees by ID and shows formatted data//
    //////////////////////////////////////////////////////////////////
    static void SortAndDisplay()
    {
        // Bubble sort based on ID
        for (int i = 0; i < employees.Length - 1; i++)
        {
            for (int j = 0; j < employees.Length - i - 1; j++)
            {
                if (employees[j].ID > employees[j + 1].ID)
                {
                    var temp = employees[j];
                    employees[j] = employees[j + 1];
                    employees[j + 1] = temp;
                }
            }
        }

        Console.WriteLine("\nEmployee records sorted by ID:\n");
        Console.WriteLine(Employee.Header());

        foreach (var emp in employees)
        {
            Console.WriteLine(emp.ToRowString());
        }
    }

    //////////////////////////////////////////////////////////
    // SearchEmployee: Finds and shows employee by ID input//
    ////////////////////////////////////////////////////////
    static void SearchEmployee()
    {
        Console.Write("Enter employee ID to search: ");
        int id = int.Parse(Console.ReadLine());
        bool found = false;

        foreach (var emp in employees)
        {
            if (emp.ID == id)
            {
                Console.WriteLine(Employee.Header());
                Console.WriteLine(emp.ToRowString());
                found = true;
                break;
            }
        }

        if (!found)
            Console.WriteLine("Employee not found.");
    }

    ///////////////////////////////////////////////
    // SaveToFile: Writes payroll to a .txt file//
    /////////////////////////////////////////////
    static void SaveToFile()
    {
        using (StreamWriter writer = new StreamWriter("fortnightlypayroll.txt"))
        {
            writer.WriteLine(Employee.Header());
            foreach (var emp in employees)
            {
                writer.WriteLine(emp.ToRowString());
            }
        }

        Console.WriteLine("Data saved to fortnightlypayroll.txt");
    }
}
    }