using System;
using System.IO;
using System.Linq;

class Employee
{
    public int ID;
    public string FirstName;
    public string LastName;
    public double AnnualIncome;
    public double KiwiSaverRate; // Stored as decimal (e.g., 0.03 for 3%)
    public double FortnightlyPay;
    public double HourlyWage;

    // Converts employee details to string (not used for printing table)
    public override string ToString()
    {
        return $"{ID}, {FirstName} {LastName}, ${FortnightlyPay:F2}";
    }
}

class PayrollSystem
{
    static Employee[] employees;

    static void Main()
    {
        LoadPayrollData();  // Load employee details from the file at startup

        int option;
        do
        {
            // Display user options in a menu
            Console.WriteLine("\n--- NewKiwi Garage Payroll Menu ---");
            Console.WriteLine("1. Calculate Fortnightly Payroll");
            Console.WriteLine("2. Sort and Display Employees");
            Console.WriteLine("3. Search Employee by ID");
            Console.WriteLine("4. Save to File");
            Console.WriteLine("0. Exit");
            Console.Write("Enter your option: ");
            option = int.Parse(Console.ReadLine());

            // Call method based on selected option
            switch (option)
            {
                case 1: CalculatePayroll(); break;
                case 2: SortAndDisplay(); break;
                case 3: SearchEmployee(); break;
                case 4: SaveToFile(); break;
                case 0: Console.WriteLine("Goodbye!"); break;
                default: Console.WriteLine("Invalid option."); break;
            }
        } while (option != 0);  // Loop until user chooses to exit
    }

    static void LoadPayrollData()
    {
        // Read all lines from the text file (employee.txt)
        string[] lines = File.ReadAllLines("employee.txt");
        employees = new Employee[lines.Length / 5]; // 5 lines per employee record

        // Parse each set of 5 lines into Employee objects
        for (int i = 0, j = 0; i < lines.Length; i += 5, j++)
        {
            employees[j] = new Employee
            {
                ID = int.Parse(lines[i]),
                FirstName = lines[i + 1],
                LastName = lines[i + 2],
                AnnualIncome = double.Parse(lines[i + 3]),
                KiwiSaverRate = double.Parse(lines[i + 4].Trim('%')) / 100
            };
        }
    }

    static void CalculatePayroll()
    {
        Console.WriteLine("\nCalculating Fortnightly Payroll...\n");

        // Print header row for the payroll table
        Console.WriteLine("{0,-5} {1,-10} {2,-10} {3,10} {4,10} {5,12} {6,18}",
                          "ID", "FirstName", "LastName", "Income", "KiwiSaver%", "HourlyWage", "FortnightlyPayroll");

        // Loop through each employee to calculate payroll
        foreach (var emp in employees)
        {
            double annualIncome = emp.AnnualIncome;
            double kiwiSaverRate = emp.KiwiSaverRate * 100;  // Convert back to percentage for display
            double kiwiSaver = annualIncome * (kiwiSaverRate / 100);

            // Calculate tax based on income brackets
            double tax = 0;
            if (annualIncome <= 15600)
                tax = annualIncome * 0.105;
            else if (annualIncome <= 53500)
                tax = 15600 * 0.105 + (annualIncome - 15600) * 0.175;
            else if (annualIncome <= 78100)
                tax = 15600 * 0.105 + (53500 - 15600) * 0.175 + (annualIncome - 53500) * 0.30;
            else if (annualIncome <= 180000)
                tax = 15600 * 0.105 + (53500 - 15600) * 0.175 +
                      (78100 - 53500) * 0.30 + (annualIncome - 78100) * 0.33;
            else
                tax = 15600 * 0.105 + (53500 - 15600) * 0.175 +
                      (78100 - 53500) * 0.30 + (180000 - 78100) * 0.33 +
                      (annualIncome - 180000) * 0.39;

            // Final payroll values
            emp.FortnightlyPay = (annualIncome / 26) - (tax / 26) - (kiwiSaver / 26);  // 26 fortnights in a year
            emp.HourlyWage = emp.AnnualIncome / 52 / 40;  // 80 hours per fortnight

            // Output formatted results
            Console.WriteLine("{0,-5} {1,-10} {2,-10} {3,10:F2} {4,10:F0} {5,12:F2} {6,18:F2}",
                              emp.ID, emp.FirstName, emp.LastName, emp.AnnualIncome,
                              kiwiSaverRate, emp.HourlyWage, emp.FortnightlyPay);
        }

        Console.WriteLine("\nFortnightly Payroll Calculated\n");
    }
    static void SortAndDisplay()
    {
        // Sort employees using Bubble Sort by ID
        for (int i = 0; i < employees.Length - 1; i++)
        {
            for (int j = 0; j < employees.Length - i - 1; j++)
            {
                if (employees[j].ID > employees[j + 1].ID)
                {
                    // Swap employee records
                    var temp = employees[j];
                    employees[j] = employees[j + 1];
                    employees[j + 1] = temp;
                }
            }
        }

        // Confirm that sorting was completed
        Console.WriteLine("\nEmployee records sorted by ID:\n");

        // Print the sorted employee details
        foreach (var emp in employees)
        {
            Console.WriteLine($"{emp.ID}: {emp.FirstName} {emp.LastName}, Income: ${emp.AnnualIncome:F2}");
        }
    }

    static void SearchEmployee()
    {
        Console.Write("Enter employee ID to search: ");
        int id = int.Parse(Console.ReadLine());
        bool found = false;

        // Look for the employee by ID
        foreach (var emp in employees)
        {
            if (emp.ID == id)
            {
                // Display employee payroll details if found
                Console.WriteLine("{0,-5} {1,-10} {2,-10} {3,10:F2} {4,10:F0} {5,12:F2} {6,18:F2}",
                                  emp.ID, emp.FirstName, emp.LastName, emp.AnnualIncome,
                                  emp.KiwiSaverRate * 100, emp.HourlyWage, emp.FortnightlyPay);
                found = true;
                break;
            }
        }

        // Show message if not found
        if (!found)
            Console.WriteLine("Employee not found.");
    }
    static void SaveToFile()
    {
        using (StreamWriter writer = new StreamWriter("fortnightlypayroll.txt"))
        {
            // Write column headers to the file
            writer.WriteLine("{0,-5} {1,-10} {2,-10} {3,10} {4,10} {5,12} {6,18}",
                             "ID", "FirstName", "LastName", "Income", "KiwiSaver%", "HourlyWage", "FortnightlyPayroll");

            // Write each employee’s payroll data to the file
            foreach (var emp in employees)
            {
                double kiwiSaverRate = emp.KiwiSaverRate * 100;
                writer.WriteLine("{0,-5} {1,-10} {2,-10} {3,10:F2} {4,10:F0} {5,12:F2} {6,18:F2}",
                                 emp.ID, emp.FirstName, emp.LastName, emp.AnnualIncome,
                                 kiwiSaverRate, emp.HourlyWage, emp.FortnightlyPay);
            }
        }

        // Confirmation of save action
        Console.WriteLine("Data saved to fortnightlypayroll.txt");
    }
}