using System;

namespace Theater
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== ТЕАТР 'МЕЛОДИЯ' ===\n");

            TheaterMenu menu = new TheaterMenu();
            menu.ShowMainMenu();

            Console.WriteLine("\nСпасибо за посещение театра! До новых встреч!");
            Console.ReadKey();
        }
    }
}