using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChemicalParserValidator;

namespace diplom
{
    class Program
    {
        static void Main(string[] args)
        {
            // string chemStr = "H<sub>2-3.7</sub>O";
             //string chemStr = "Ho2(Cl3(H3O2[Zn3He2]3)4)2";
            string chemStr = "Sn-Te";
            Console.WriteLine(chemStr);
           ChemicalParser d = new  ChemicalParser(chemStr) ;
           d.GetChemicalSystem();
           //d.ParseStr();
        }
    }
}
