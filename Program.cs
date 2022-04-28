using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChemicalParser;

namespace diplom
{
    class Program
    {
        static void Main(string[] args)
        {
            // string chemStr = "H<sub>2-3.7</sub>O";
             string chemStr = "H2O3Cl";
            Console.WriteLine(chemStr);
           Parse d = new  Parse(chemStr) ;
           //d.ParseStr();
        }
    }
}
