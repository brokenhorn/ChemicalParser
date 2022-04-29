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
             string chemStr = "Ho2(Cl3(H3O2)4)2";
            Console.WriteLine(chemStr);
           Parse d = new  Parse(chemStr) ;
           //d.ParseStr();
        }
    }
}
