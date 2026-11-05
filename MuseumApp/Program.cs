using System;
using System.Text;

namespace Museum
{
    internal class Program
    {
        const int Capacity = 100;
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
                    case "1":
                        ShowAbout();
                        Pause();
                        break;
                    case "2":
                        Console.WriteLine("Поки не зроблено.");
                        Pause();
                        break;
                    case "3":
                        Console.WriteLine("Поки не зроблено.");
                        Pause();
                        break;
                    case "0":
                        running = false;
                        Console.WriteLine("До побачення.");
                        break;
                    default:
                        Console.WriteLine("Немає такого пункту. Спробуйте ще раз.");
                        Pause();
                        break;
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
    }
}