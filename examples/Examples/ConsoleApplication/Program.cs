using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication
{

    class Program
    {
        // Software entry point
        static void Main(string[] args)
        {
            // Creates the person object
            Person person = new Person("Antonio", "Blescia", 29);

            // Creates the output trace string
            string output = String.Format("{0} {1}:{2}", person.Name, person.Surname, person.Age);

            // Prints to stdout
            Console.WriteLine(output);

            // Waits
            Console.Read();
        }
    }
}
