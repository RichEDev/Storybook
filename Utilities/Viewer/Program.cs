namespace Utilities.Viewer
{
    using System;
    using System.Windows.Forms;

    using Utilities.Cryptography;

    internal class Program
    {
        #region Methods

        private static void EncryptString()
        {
            Console.Clear();
            Console.WriteLine("\n Please enter the string you wish to encrypt:\n");
            Console.Write("     ");

            string encryptMe = Console.ReadLine();
            string encryptedText = ExpensesCryptography.Encrypt(encryptMe);

            Clipboard.SetText(encryptedText);

            Console.WriteLine("\n     {0}", encryptedText);
            Console.WriteLine("\n This has been entered into your clipboard.");

            Console.Write("\n Press ENTER to continue");
            Console.ReadLine();

            ShowMenu();
        }

        [STAThread]
        private static void Main(string[] args)
        {
            ShowMenu();
        }

        private static void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("\n Choose 1 of the following utilities:\n");
            Console.WriteLine("    1) Encrypt a string");
            Console.WriteLine("    Esc) Exit");
            Console.Write("\n Please press the number key corresponding to the utility you require: ");

            ReadKey();
        }

        private static void ReadKey()
        {
            ConsoleKeyInfo key = Console.ReadKey();

            switch (key.Key)
            {
                case ConsoleKey.Escape:
                    break;
                case ConsoleKey.D1:
                    EncryptString();
                    break;
                default:
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine(" Invalid number key pressed, please try again (or press Escape to exit): ");
                    ReadKey();
                    break;
            }
        }

        #endregion
    }
}