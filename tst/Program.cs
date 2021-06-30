using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tst
{
    class Program
    {
        static void Main(string[] args)
        {
            string str1 = "ab";
            string str2 = "abc";
            string str3 = "abd";

            double? number = null;

            double numberCasted = (double)number;


            Console.WriteLine(numberCasted + " was casted from" + number);
            Console.ReadLine();

        }
    }
}
