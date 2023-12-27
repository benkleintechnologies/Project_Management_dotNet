namespace Stage0
{
    partial class Program
    {
        private static void Main(string[] args)
        {
            Welcome8589();
            Welcome0954();
            Console.ReadKey();
        }
        static partial void Welcome0954();

        private static void Welcome8589()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();
            Console.WriteLine("{0}, welcome to my first console application.", name);
        }
    }
}