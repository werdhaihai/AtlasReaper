using System;

namespace AtlasReaper
{
    class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                // Create an instance of ArgHandler
                ArgHandler argHandler = new ArgHandler();

                // Invoke the HandleArgs method
                argHandler.HandleArgs(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured: " + ex.Message);
            }

        }
    }
}
