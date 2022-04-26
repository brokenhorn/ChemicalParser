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
        private struct elemQQ //Качество и кол-во элемента
       {
           public ChemicalElement elemName { get; set;}
           //public ChemicalQuantity quantity;
           public int min{ get; set ;}
           public int max{ get; set;}
           public int blockNbr { get; set;}
           
       }
       //private struct 
        private string allowedSym = "1234567890-<>()[]-.ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz/";
        private string InputStr;
        private List<elemQQ> ChemList; //список блоков элементов с их характеристикой
       // private List<
        private string outMsg;
        private int blockI;
        

        public Parse(string chemStr)
        {
            InputStr = chemStr;
            blockI = 0;
            ChemList = new List<elemQQ>();
        }

        public void ParseStr()
        {
            int i = 0;
            if (!(InputStr[i] >= 60 & InputStr[i] <= 90))
                {
                    outMsg = "Последовательность должна начинаться с заглавного символа";
                    return;
                }
            while(i < InputStr.Length)
            {
                 string elemN = "";
                if (allowedSym.Contains(InputStr[i]) == false) // проверка валидности символа
                   {
                    outMsg = "Located forbidden symbol";
                    return;
                   } 

                if (InputStr[i] >= 60 & InputStr[i] <= 90) //блок формирования символа элемента
                {
                    elemN = elemN + InputStr[i];
                    i++;
                    if (i < InputStr.Length)
                    {
                        while (i < InputStr.Length & InputStr[i] >= 97 & InputStr[i] <= 122)
                        {
                            elemN = elemN + InputStr[i];
                            i++;
                        }
                    }
                    
                    blockI++;
                    elemQQ el = new elemQQ {elemName = new ChemicalElement(elemN), min = 1, max = 1 , blockNbr = blockI}; //quantity = new ChemicalQuantity(1, 1)//
                    ChemList.Add(el);
                }

                if (i >= InputStr.Length)
                    break;
                if (InputStr[i] == '<') //блок проверки индекса
                {
                    if ((InputStr.Remove(0, i)).Substring(0, 5) != "<sub>")
                    {
                        outMsg = "Located forbidden symbol block";
                        return;
                    }
                    int min = 1;
                    int max = -1;         
                    i += 5;
                    while (InputStr[i] >= '0' & InputStr[i] <= '9') // Дописать для тире
                    {
                        min = InputStr[i] - '0';
                        if (InputStr[i + 1] >= 30 & InputStr[i + 1] <= 39)
                            min *= 10;
                        i++;
                    }
                    if ((InputStr.Remove(0, i)).Substring(0, 6) != "</sub>")
                    {
                        outMsg = "Located forbidden symbol block";
                        return;
                    }
                    if (max == -1)
                        max = min;
                    elemQQ tmp = ChemList.Last();
                    this.ChemList[blockI - 1] = new elemQQ {elemName = new ChemicalElement(elemN), min = tmp.min * min, max = tmp.max * max , blockNbr = blockI};
                    i += 6;
                }
            }
            return;
        }

    }
}
