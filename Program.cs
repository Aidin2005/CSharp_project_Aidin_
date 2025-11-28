using System;
using System.IO;

namespace AttendanceProject
{
    class AttendanceSystem
    {
        string[] names;
        bool[,] att;
        bool[] dayOk;
        int days;

        public void Run()
        {
            Console.WriteLine("=== Student Attendance System v1.5 ===");

            Init();

            while (true)
            {
                Menu();
                Console.Write("Choose an option: ");
                var x = Console.ReadLine();

                if (x == "1")
                {
                    Console.WriteLine("\n--- Marking attendance for a selected day ---");
                    MarkDay();
                }
                else if (x == "2")
                {
                    Console.WriteLine("\n--- Displaying full attendance table ---");
                    ShowT();
                }
                else if (x == "3")
                {
                    Console.WriteLine("\n--- Generating attendance report ---");
                    Rep();
                }
                else if (x == "4")
                {
                    Console.WriteLine("\n--- Saving attendance report to file ---");
                    SaveTxt();
                }
                else if (x == "5")
                {
                    Console.WriteLine("\n--- Loading saved report from file ---");
                    LoadTxt();
                }
                else if (x == "6")
                {
                    Console.WriteLine("\n--- Searching for student by name ---");
                    FindName();
                }
                else if (x == "7")
                {
                    Console.WriteLine("\n--- Sorting students by attendance percentage ---");
                    SortIt();
                }
                else if (x == "0")
                {
                    Console.WriteLine("Exiting the program...");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid option, try again.");
                }

                Console.WriteLine();
            }
        }

        void Init()
        {
            Console.Write("Enter number of students: ");
            int n = int.Parse(Console.ReadLine());

            Console.Write("Enter number of days: ");
            days = int.Parse(Console.ReadLine());

            names = new string[n];
            att = new bool[n, days];
            dayOk = new bool[days];

            Console.WriteLine("Enter student names:");
            for (int i = 0; i < n; i++)
            {
                Console.Write($"Student {i + 1}: ");
                var s = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(s))
                    s = "Student" + (i + 1);
                names[i] = s;
            }
        }

        void Menu()
        {
            Console.WriteLine();
            Console.WriteLine("============== MAIN MENU ==============");
            Console.WriteLine("1 - Mark attendance for a specific day");
            Console.WriteLine("2 - Show full attendance table for all students");
            Console.WriteLine("3 - Generate attendance report (percent results)");
            Console.WriteLine("4 - Save report into a text file");
            Console.WriteLine("5 - Load previously saved report from file");
            Console.WriteLine("6 - Search for a student by name");
            Console.WriteLine("7 - Sort students by attendance percentage");
            Console.WriteLine("0 - Exit the program");
            Console.WriteLine("========================================");
        }

        void MarkDay()
        {
            Console.Write($"Enter day number (1 - {days}): ");
            int d = int.Parse(Console.ReadLine()) - 1;

            Console.WriteLine("Use P for Present or A for Absent:");

            for (int i = 0; i < names.Length; i++)
            {
                Console.Write($"{names[i]}: ");
                string input = Console.ReadLine().ToUpper();

                if (input == "P") att[i, d] = true;
                else att[i, d] = false;
            }

            dayOk[d] = true;
            Console.WriteLine("Attendance successfully recorded.");
        }

        void ShowT()
        {
            Console.Write("\t");
            for (int d = 0; d < days; d++)
                Console.Write($"Day{d + 1}\t");
            Console.WriteLine();

            for (int i = 0; i < names.Length; i++)
            {
                Console.Write(names[i] + "\t");
                for (int d = 0; d < days; d++)
                {
                    if (!dayOk[d]) Console.Write("-\t");
                    else Console.Write(att[i, d] ? "P\t" : "A\t");
                }
                Console.WriteLine();
            }
        }

        void Rep()
        {
            int done = 0;
            for (int d = 0; d < days; d++)
                if (dayOk[d]) done++;

            if (done == 0)
            {
                Console.WriteLine("No attendance has been entered yet.");
                return;
            }

            double[] arr = new double[names.Length];
            double sum = 0;

            Console.WriteLine("Attendance Report:");
            for (int i = 0; i < names.Length; i++)
            {
                int count = 0;
                for (int d = 0; d < days; d++)
                    if (dayOk[d] && att[i, d]) count++;

                arr[i] = (double)count / done * 100;
                sum += arr[i];

                string warning = arr[i] < 75 ? " <-- LOW ATTENDANCE" : "";
                Console.WriteLine($"{names[i]}: {arr[i]:F1}%{warning}");
            }

            Console.WriteLine($"\nClass Average: {(sum / names.Length):F1}%");
        }

        void FindName()
        {
            Console.Write("Enter name to search: ");
            string q = Console.ReadLine().ToLower();
            bool found = false;

            int done = 0;
            for (int d = 0; d < days; d++)
                if (dayOk[d]) done++;

            for (int i = 0; i < names.Length; i++)
            {
                if (names[i].ToLower().Contains(q))
                {
                    found = true;
                    int c = 0;

                    for (int d = 0; d < days; d++)
                        if (dayOk[d] && att[i, d]) c++;

                    double p = done == 0 ? 0 : (double)c / done * 100;

                    Console.WriteLine($"Student found: {names[i]}");
                    Console.WriteLine($"Attendance: {p:F1}%");
                    break;
                }
            }

            if (!found)
                Console.WriteLine("No matching student found.");
        }

        void SortIt()
        {
            double[] res = new double[names.Length];
            int done = 0;

            for (int d = 0; d < days; d++)
                if (dayOk[d]) done++;

            for (int i = 0; i < names.Length; i++)
            {
                int c = 0;
                for (int d = 0; d < days; d++)
                    if (dayOk[d] && att[i, d]) c++;

                res[i] = done > 0 ? (double)c / done * 100 : 0;
            }

            for (int i = 0; i < names.Length - 1; i++)
                for (int j = i + 1; j < names.Length; j++)
                    if (res[j] > res[i])
                    {
                        var t1 = res[i]; res[i] = res[j]; res[j] = t1;
                        var t2 = names[i]; names[i] = names[j]; names[j] = t2;
                    }

            Console.WriteLine("Sorted by attendance:");
            for (int i = 0; i < names.Length; i++)
                Console.WriteLine($"{names[i]} - {res[i]:F1}%");
        }

        void SaveTxt()
        {
            using (var w = new StreamWriter("attendance_report.txt"))
            {
                w.WriteLine("Student Attendance Report");
                w.WriteLine($"Generated at: {DateTime.Now}");
                w.WriteLine();

                int done = 0;
                for (int d = 0; d < days; d++) if (dayOk[d]) done++;

                for (int i = 0; i < names.Length; i++)
                {
                    int c = 0;
                    for (int d = 0; d < days; d++)
                        if (dayOk[d] && att[i, d]) c++;

                    double p = done == 0 ? 0 : (double)c / done * 100;
                    w.WriteLine($"{names[i]}: {p:F1}%");
                }
            }

            Console.WriteLine("Report successfully saved.");
        }

        void LoadTxt()
        {
            if (!File.Exists("attendance_report.txt"))
            {
                Console.WriteLine("No saved report found.");
                return;
            }

            Console.WriteLine(File.ReadAllText("attendance_report.txt"));
        }
    }

    class Program
    {
        static void Main()
        {
            var app = new AttendanceSystem();
            app.Run();
        }
    }
}
