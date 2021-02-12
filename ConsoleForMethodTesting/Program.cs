using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIMethods;

namespace ConsoleForMethodTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            var response = APIMethods.ClassroomAPI.ListActiveClasses();
            if (response.Courses != null && response.Courses.Count > 0)
            {
                foreach (var course in response.Courses)
                {
                    Console.WriteLine("{0} ({1})", course.Name, course.Id);
                }
            }
            else
            {
                Console.WriteLine("No courses found.");
            }
            Console.Read();

        }
    }
}
