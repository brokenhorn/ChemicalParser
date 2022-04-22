using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChemicalHierarchy;

namespace ChemicalParser
{
    class Parse
    {
        private string allowedSym = "1234567890-<>()[]-.ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz/";
        private string InputStr;
        private Dictionary<ChemicalElement, ChemicalQuantity> ChemList; //список элементов с их характеристикой

        //private struct elemQQ //Качество и кол-во элемента
       // {
         //   public string elemName;
           // public ChemicalQuantity ChemicalQuantity ;
      //  }

        public Parse(string chemStr)
        {
            InputStr = chemStr;
        }

        public void ParseStr()
        {
            string elemN = "";
            for (int i = 0; i < InputStr.Length; i++)
            {
                if (allowedSym.Contains(InputStr[i]) == false) // проверка валидности символа
                    return;

                if (InputStr[i] >= 60 & InputStr[i] <= 90) //блок формирования символа элемента
                {
                    elemN = elemN + InputStr[i];
                    i++;
                    while (InputStr[i] >= 97 & InputStr[i] <= 122)
                    {
                        elemN = elemN + InputStr[i];
                        i++;
                    }
                    //elemQQ el = new elemQQ {elemName = elemN, ChemicalQuantity = new ChemicalQuantity(1, 1)};
                    ChemList.Add(new ChemicalElement(elemN) , new ChemicalQuantity(1, 1));
                }

                if (InputStr[i] == '<')//блок проверки индекса
                {
                    
                }
            }
        }

    }
}
