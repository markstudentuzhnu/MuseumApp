using System;
using System.Globalization;
using System.Text;

namespace Museum
{
    internal class Program
    {
        const int Capacity = 100;
        const decimal MaxValue = 1000000000m;
        const int TitleLength = 24;
        const int AuthorLength = 18;
        const int SectionLength = 12;
        const int ConditionLength = 12;
        const double MaterialRate = 140.0;
        const double WorkRate = 400.0;

        static int[] ids = new int[Capacity];
        static string[] titles = new string[Capacity];
        static string[] authors = new string[Capacity];
        static int[] years = new int[Capacity];
        static string[] sections = new string[Capacity];
        static decimal[] values = new decimal[Capacity];
        static string[] conditions = new string[Capacity];
        static bool[] exhibited = new bool[Capacity];
        static int count = 0;
        static int nextId = 1;

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
                    case "1": ShowExhibits(); Pause(); break;
                    case "2": AddExhibit(); Pause(); break;
                    case "3": FillSampleData(); Pause(); break;
                    case "4": ShowAbout(count, Capacity); Pause(); break;
                    case "5": ShowCostTable(); Pause(); break;
                    case "6": ShowVisitorsTable(); Pause(); break;
                    case "7": ShowPassingDemo(); Pause(); break;
                    case "0": running = false; ShowGoodbye(); break;
                    default: ShowUnknownChoice(); Pause(); break;
                }
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("=== Музей ===");
            Console.WriteLine("1 — показати всі експонати");
            Console.WriteLine("2 — додати експонат");
            Console.WriteLine("3 — заповнити тестовими даними");
            Console.WriteLine("4 — довідка про програму");
            Console.WriteLine("5 — вартість реставрації залежно від площі");
            Console.WriteLine("6 — відвідуваність розділів за тиждень");
            Console.WriteLine("7 — дослід: значиме й посилальне");
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

        static void ShowAbout(int used, int capacity)
        {
            Console.WriteLine("Тема №28: Музей.");
            Console.WriteLine("Сутність: експонат. Полів: " + fieldNames.Length + ".");
            for (int i = 0; i < fieldNames.Length; i++)
            {
                Console.WriteLine($"  {i + 1}. {fieldNames[i]} — {fieldTypes[i]}");
            }
            Console.WriteLine($"Місткість сховища: {capacity} записів, зайнято: {used}.");
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

        static string ReadText(string prompt, int maxLength)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = (Console.ReadLine() ?? "").Trim();

                if (input.Length == 0)
                {
                    Console.WriteLine("Помилка: рядок не може бути порожнім.");
                    continue;
                }
                if (input.Length > maxLength)
                {
                    Console.WriteLine($"Помилка: не більше {maxLength} символів.");
                    continue;
                }
                return input;
            }
        }

        static decimal ReadDecimal(string prompt, decimal min, decimal max)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? "";
                decimal value;
                bool isNumber = decimal.TryParse(input.Trim().Replace(',', '.'),
                    NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture, out value);

                if (!isNumber)
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

        static bool ReadYesNo(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string answer = (Console.ReadLine() ?? "").Trim().ToLower();

                if (answer == "так")
                {
                    return true;
                }
                if (answer == "ні")
                {
                    return false;
                }
                Console.WriteLine("Помилка: введіть «так» або «ні».");
            }
        }

        static void StoreExhibit(string title, string author, int year, string section,
            decimal value, string condition, bool isExhibited)
        {
            ids[count] = nextId;
            titles[count] = title;
            authors[count] = author;
            years[count] = year;
            sections[count] = section;
            values[count] = value;
            conditions[count] = condition;
            exhibited[count] = isExhibited;
            count++;
            nextId++;
        }

        static void AddExhibit()
        {
            if (count == Capacity)
            {
                Console.WriteLine("Сховище заповнене, видаліть щось зайве.");
                return;
            }

            string title = ReadText("Назва: ", TitleLength);
            string author = ReadText("Автор: ", AuthorLength);
            int year = ReadInt($"Рік створення (1–{DateTime.Today.Year}): ", 1, DateTime.Today.Year);
            string section = ReadText("Розділ експозиції: ", SectionLength);
            decimal value = ReadDecimal("Оцінна вартість, грн (0–1000000000): ", 0m, MaxValue);
            string condition = ReadText("Стан (добрий, задовільний, реставрація): ", ConditionLength);
            bool isExhibited = ReadYesNo("Виставлений зараз (так/ні): ");

            StoreExhibit(title, author, year, section, value, condition, isExhibited);
            Console.WriteLine($"Додано експонат №{ids[count - 1]}.");
        }

        static void FillSampleData()
        {
            string[] sampleTitles =
            {
                "Козак із бандурою", "Бронзова статуетка", "Глечик з розписом", "Гравюра «Старий Київ»",
                "Срібна монета", "Рушник весільний", "Пейзаж з млином"
            };
            string[] sampleAuthors =
            {
                "Невідомий автор", "Остап Левицький", "Невідомий автор", "Ірина Мельник",
                "Невідомий автор", "Невідомий автор", "Олена Гнатюк"
            };
            int[] sampleYears = { 1780, 1925, 1890, 1958, 1648, 1910, 1972 };
            string[] sampleSections =
            {
                "Живопис", "Скульптура", "Кераміка", "Графіка", "Нумізматика", "Тканини", "Живопис"
            };
            decimal[] sampleValues = { 350000m, 120500.50m, 18000m, 42000m, 96000m, 27300.75m, 88000m };
            string[] sampleConditions =
            {
                "добрий", "задовільний", "реставрація", "добрий", "добрий", "задовільний", "добрий"
            };
            bool[] sampleExhibited = { true, true, false, true, false, true, true };

            int added = 0;
            for (int i = 0; i < sampleTitles.Length; i++)
            {
                if (count == Capacity)
                {
                    Console.WriteLine("Сховище заповнене, решту записів не додано.");
                    break;
                }
                StoreExhibit(sampleTitles[i], sampleAuthors[i], sampleYears[i], sampleSections[i],
                    sampleValues[i], sampleConditions[i], sampleExhibited[i]);
                added++;
            }
            Console.WriteLine($"Додано тестових експонатів: {added}. Усього: {count}.");
        }

        static void ShowExhibits()
        {
            if (count == 0)
            {
                Console.WriteLine("Експонатів поки немає.");
                return;
            }

            Console.WriteLine($"{"№",4}  {"Назва",-26}{"Автор",-20}{"Рік",6}  {"Розділ",-14}" +
                              $"{"Вартість, грн",15}  {"Стан",-14}{"Виставл.",9}");
            Console.WriteLine(new string('-', 114));
            for (int i = 0; i < count; i++)
            {
                string mark = exhibited[i] ? "так" : "ні";
                Console.WriteLine($"{ids[i],4}  {titles[i],-26}{authors[i],-20}{years[i],6}  " +
                                  $"{sections[i],-14}{values[i],15:F2}  {conditions[i],-14}{mark,9}");
            }

            Console.WriteLine($"\nЕкспонатів: {count}.");
            Console.WriteLine($"Загальна оцінна вартість: {CalculateTotalValue(values, count):F2} грн");
            Console.WriteLine($"Середня оцінна вартість: {CalculateAverageValue(values, count):F2} грн");
        }

        static decimal CalculateTotalValue(decimal[] valueArray, int used)
        {
            decimal sum = 0;
            for (int i = 0; i < used; i++)
            {
                sum += valueArray[i];
            }
            return sum;
        }

        static decimal CalculateAverageValue(decimal[] valueArray, int used)
        {
            if (used == 0)
            {
                return 0;
            }
            return CalculateTotalValue(valueArray, used) / used;
        }

        static void IncreaseValue(int value)
        {
            value = value + 100;
            Console.WriteLine($"  усередині методу: value = {value}");
        }

        static void IncreaseValueRef(ref int value)
        {
            value = value + 100;
            Console.WriteLine($"  усередині методу: value = {value}");
        }

        static void IncreaseElement(int[] prices)
        {
            prices[0] = prices[0] + 100;
            Console.WriteLine($"  усередині методу: prices[0] = {prices[0]}");
        }

        static void ReplaceArray(int[] prices)
        {
            prices = new int[] { 1, 2, 3 };
            Console.WriteLine($"  усередині методу: prices[0] = {prices[0]}");
        }

        static void ShowPassingDemo()
        {
            int price = 50;
            int[] prices = { 50, 70, 90 };

            Console.WriteLine($"До: price = {price}, prices[0] = {prices[0]}");
            Console.WriteLine();

            Console.WriteLine("1. IncreaseValue(price) — значиме без ref:");
            IncreaseValue(price);
            Console.WriteLine($"  у Main: price = {price}");

            Console.WriteLine("2. IncreaseElement(prices) — елемент масиву:");
            IncreaseElement(prices);
            Console.WriteLine($"  у Main: prices[0] = {prices[0]}");

            Console.WriteLine("3. ReplaceArray(prices) — підміна масиву цілком:");
            ReplaceArray(prices);
            Console.WriteLine($"  у Main: prices[0] = {prices[0]}, довжина {prices.Length}");

            Console.WriteLine("4. IncreaseValueRef(ref price) — значиме з ref:");
            IncreaseValueRef(ref price);
            Console.WriteLine($"  у Main: price = {price}");
        }

    }
}
