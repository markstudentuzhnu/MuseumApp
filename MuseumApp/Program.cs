using System;
using System.Globalization;
using System.Text;

namespace Museum
{
    internal class Program
    {
        const int Capacity = 100;
        const double MaterialRate = 140.0;
        const double WorkRate = 400.0;

        static int count = 0;

        static string[] fieldNames =
        {
            "Інвентарний номер", "Назва", "Автор", "Рік створення",
            "Розділ експозиції", "Оцінна вартість", "Стан", "Чи виставлений"
        };

        static string[] fieldTypes =
        {
            "int", "string", "string", "int", "string", "decimal", "string", "bool"
        };

        static string[] sectionNames =
        {
            "Живопис", "Скульптура", "Кераміка", "Графіка", "Нумізматика", "Тканини"
        };

        static int[] sectionBase = { 60, 40, 30, 25, 20, 35 };

        static string[] dayNames = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Нд" };

        static double[] dayFactor = { 0.8, 0.8, 0.9, 1.0, 1.2, 1.6, 1.5 };

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            bool running = true;

            while (running)
            {
                ShowMenu();
                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1": ShowAbout(); Pause(); break;
                    case "2": ShowCostTable(); Pause(); break;
                    case "3": ShowVisitorsTable(); Pause(); break;
                    case "0": running = false; ShowGoodbye(); break;
                    default: ShowUnknownChoice(); Pause(); break;
                }
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("=== Музей ===");
            Console.WriteLine("1 — довідка про програму");
            Console.WriteLine("2 — вартість реставрації залежно від площі");
            Console.WriteLine("3 — відвідуваність розділів за тиждень");
            Console.WriteLine("0 — вихід");
            Console.Write("Ваш вибір: ");
        }

        static void Pause()
        {
            Console.WriteLine("\nНатисніть будь-яку клавішу...");
            Console.ReadKey(true);
            Console.Clear();
        }

        static void ShowGoodbye()
        {
            Console.WriteLine("До побачення.");
        }

        static void ShowUnknownChoice()
        {
            Console.WriteLine("Немає такого пункту. Спробуйте ще раз.");
        }

        static void ShowAbout()
        {
            Console.WriteLine("Тема №28: Музей.");
            Console.WriteLine("Сутність: експонат. Полів: " + fieldNames.Length + ".");
            for (int i = 0; i < fieldNames.Length; i++)
            {
                Console.WriteLine($"  {i + 1}. {fieldNames[i]} — {fieldTypes[i]}");
            }
            Console.WriteLine($"Місткість сховища: {Capacity} записів, зайнято: {count}.");
        }

        static void ShowCostTable()
        {
            double start = ReadDouble("Початок проміжку, м² (0,1–20): ", 0.1, 20);
            double end = ReadDouble("Кінець проміжку, м² (від початку до 20): ", start, 20);
            double step = ReadDouble("Крок, м² (0,1–5): ", 0.1, 5);
            Console.WriteLine();
            PrintCostTable(start, end, step);
        }

        static double CalculateMaterialsCost(double area, double rate)
        {
            return rate * area;
        }

        static double CalculateWorkCost(double area, double rate)
        {
            return rate * Math.Sqrt(area);
        }

        static void PrintCostTable(double start, double end, double step)
        {
            if (step <= 0)
            {
                Console.WriteLine("Помилка: крок має бути додатним.");
                return;
            }
            if (end < start)
            {
                Console.WriteLine("Помилка: кінець проміжку менший за початок.");
                return;
            }

            Console.WriteLine("Вартість реставрації залежно від площі пошкодження");
            Console.WriteLine("Площа, м²".PadLeft(10) + "Матеріали, грн".PadLeft(16) +
                              "Робота, грн".PadLeft(14));
            Console.WriteLine(new string('-', 40));

            for (double area = start; area <= end + 1e-9; area += step)
            {
                double materials = CalculateMaterialsCost(area, MaterialRate);
                double work = CalculateWorkCost(area, WorkRate);
                Console.WriteLine($"{area,10:F2}{materials,16:F2}{work,14:F2}");
            }
        }

        static void ShowVisitorsTable()
        {
            int maxRows = sectionNames.Length;
            int maxCols = dayNames.Length;
            int rows = ReadInt("Скільки розділів показати (1–" + maxRows + "): ", 1, maxRows);
            int cols = ReadInt("Скільки днів показати (1–" + maxCols + "): ", 1, maxCols);
            Console.WriteLine();
            PrintVisitorsTable(rows, cols);
        }

        static int CalculateVisitors(int section, int day)
        {
            return (int)Math.Round(sectionBase[section] * dayFactor[day], MidpointRounding.AwayFromZero);
        }

        static int SumSectionVisitors(int section, int cols)
        {
            int sum = 0;
            for (int c = 0; c < cols; c++)
            {
                sum += CalculateVisitors(section, c);
            }
            return sum;
        }

        static int SumDayVisitors(int day, int rows)
        {
            int sum = 0;
            for (int r = 0; r < rows; r++)
            {
                sum += CalculateVisitors(r, day);
            }
            return sum;
        }

        static void PrintVisitorsTable(int rows, int cols)
        {
            Console.WriteLine("Кількість відвідувачів розділів за днями тижня");
            Console.Write("Розділ".PadRight(14));
            for (int c = 0; c < cols; c++)
            {
                Console.Write($"{dayNames[c],8}");
            }
            Console.WriteLine("Разом".PadLeft(8));

            int grand = 0;
            for (int r = 0; r < rows; r++)
            {
                Console.Write(sectionNames[r].PadRight(14));
                for (int c = 0; c < cols; c++)
                {
                    Console.Write($"{CalculateVisitors(r, c),8}");
                }
                int rowSum = SumSectionVisitors(r, cols);
                Console.WriteLine($"{rowSum,8}");
                grand += rowSum;
            }

            Console.Write("Разом".PadRight(14));
            for (int c = 0; c < cols; c++)
            {
                Console.Write($"{SumDayVisitors(c, rows),8}");
            }
            Console.WriteLine($"{grand,8}");
        }

        static double ReadDouble(string prompt, double min, double max)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? "";
                double value;
                bool isNumber = double.TryParse(input.Trim().Replace(',', '.'),
                    NumberStyles.Float, CultureInfo.InvariantCulture, out value);

                if (!isNumber || double.IsNaN(value) || double.IsInfinity(value))
                {
                    Console.WriteLine("Помилка: потрібне число.");
                    continue;
                }
                if (value < min || value > max)
                {
                    Console.WriteLine($"Помилка: число має бути від {min} до {max}.");
                    continue;
                }
                return value;
            }
        }

        static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? "";
                int value;
                bool isNumber = int.TryParse(input.Trim(), NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out value);

                if (!isNumber)
                {
                    Console.WriteLine("Помилка: потрібне ціле число.");
                    continue;
                }
                if (value < min || value > max)
                {
                    Console.WriteLine($"Помилка: число має бути від {min} до {max}.");
                    continue;
                }
                return value;
            }
        }
    }
}