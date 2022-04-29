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
        private class ElemQnQ //Качество и кол-во элемента
       {
           public ChemicalElement elemName { get; set;}
           //public ChemicalQuantity quantity;
           public double min{ get; set ;}
           public double max{ get; set;}
           public int blockNbr{ get; set;}
           
		   public void SetMinMax(double NewMin, double NewMax)
		   {
			   min = NewMin * min;
			   max = NewMax * max;
		   }
       }
        private String allowedSym = "1234567890-<>()[]-.ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz/";
        private String InputStr;
        private List<ElemQnQ> ChemList; //список блоков элементов с их характеристикой
        private String outMsg{ get; set;}
        private int blockI;
        

        public Parse(string chemStr) // конструктор
        {
            InputStr = String.Concat(chemStr.Where(c => !Char.IsWhiteSpace(c))); // Удаление всех пробелов в строке
            blockI = 0;
            ChemList = new List<ElemQnQ>();
			int i = 0;
			outMsg = "0";
			//ParseStr();
			ParseBracketBlock(ref InputStr, ref i, ' ', 0);
			//сериализовать
        }

        private void ParseMinMax(ref String InputStr, ref int i, ref String MinOrMax) // блок обработки максимального или минимального значения
        {
        	while (InputStr[i] >= '0' & InputStr[i] <= '9' | InputStr[i] == '.') 
        	{
            	MinOrMax += InputStr[i];
        		i++;
				if (i >= InputStr.Length)
					return;
        	}  
        }

		private void ParseIndexBlockHtml(ref String InputStr, ref int i, ref String elemN,  ref int blockIStart, ref int blockIEnd)
		{
			if ((InputStr.Remove(0, i)).Substring(0, 5) != "<sub>")
            {
                outMsg = "1.Обнаружен запрещенный блок символ";
                return;
            }
            String str_min = "";
            String str_max = "";         
            i += 5;
			ParseMinMax(ref InputStr, ref i, ref str_min);
            if (InputStr[i] == '-')
            {
                i++;
				ParseMinMax(ref InputStr, ref i, ref str_max);
            }
            if ((InputStr.Remove(0, i)).Substring(0, 6) != "</sub>")
            {
                outMsg = "1.Обнаружен запрещенный блок символ";
                return;
            }
            double min = -1;
            double max = -1;
            min = Convert.ToDouble(str_min);
            if (str_max != "")
                max = Convert.ToDouble(str_max);  
            if (max == -1)
                max = min;

            ElemQnQ tmp = ChemList.Last();
            if (blockIEnd == 0)
            	ChemList[blockI - 1].SetMinMax(min, max);//this.ChemList[blockI - 1] = new ElemQnQ {elemName = new ChemicalElement(elemN), min = tmp.min * min, max = tmp.max * max , blockNbr = blockI};
			else
			{
				foreach (var block in ChemList)
				{
					if (block.blockNbr >= blockIStart & block.blockNbr <= blockIEnd)
						ChemList[block.blockNbr - 1].SetMinMax(min, max);//this.ChemList[block.blockNbr - 1] = new ElemQnQ {elemName = new ChemicalElement(elemN), min = tmp.min * min, max = tmp.max * max , blockNbr = blockI};
				}
				blockIStart = 0; //Может не нужно
				blockIEnd 	= 0;
			}
            i += 6;
		}

        private void ParseIndexBlock(ref String InputStr, ref int i, ref String elemN,  ref int blockIStart, ref int blockIEnd)
        {
			String str_min = "";
            String str_max = "";

			ParseMinMax(ref InputStr, ref i, ref str_min);
			
			if (!(i >= InputStr.Length))
			{
				if (InputStr[i] == '-')
            	{
                	i++;
					ParseMinMax(ref InputStr, ref i, ref str_max);
            	}
			}

			double min = -1;
            double max = -1;
            min = Convert.ToDouble(str_min);
            if (str_max != "")
                 max = Convert.ToDouble(str_max);  
            if (max == -1)
                 max = min;
			ElemQnQ tmp = ChemList.Last();
			if (blockIEnd == 0)
            	ChemList[blockI - 1].SetMinMax(min, max);//this.ChemList[blockI - 1] = new ElemQnQ {elemName = new ChemicalElement(elemN), min = tmp.min * min, max = tmp.max * max , blockNbr = blockI};
			else
			{
				foreach (var block in ChemList)
				{
					if (block.blockNbr >= blockIStart & block.blockNbr <= blockIEnd)
						ChemList[block.blockNbr - 1].SetMinMax(min, max);//this.ChemList[block.blockNbr - 1] = new ElemQnQ {elemName = new ChemicalElement(elemN), min = tmp.min * min, max = tmp.max * max , blockNbr = blockI};
				}
				blockIStart = 0; //Может не нужно
				blockIEnd 	= 0;
			}
        }
		private void ParseChemElName(ref String InputStr, ref int i, ref String elemN, ref int blockIStart, ref int bracketFlagOpen) //блок формирования символа элемента
		{
    		elemN = elemN + InputStr[i];
    		i++;
    		if (i < InputStr.Length)
    		{
    		    while (InputStr[i] >= 97 & InputStr[i] <= 122)
    		    {
    		        elemN = elemN + InputStr[i];
    		        i++;
    		        if (i >= InputStr.Length)
    		            break;
    		    }
    		}    
    		blockI++;
			if (blockIStart == 0 & bracketFlagOpen != 0)
				blockIStart = blockI;
    		ElemQnQ el = new ElemQnQ {elemName = new ChemicalElement(elemN), min = 1, max = 1 , blockNbr = blockI}; //quantity = new ChemicalQuantity(1, 1)//
    		ChemList.Add(el);
		}

		private void ParseBracketBlock(ref String InputStr, ref int i, char BracketSym, int FlagOpen)
		{
			char 	BracketSymClose		= ' ';
			int 	bracketFlagOpen 	= FlagOpen; // 1 = '(', 2 = '['
			int 	bracketFlagClose 	= 0;
			int 	blockIStart 		= 0;
			int 	blockIEnd 			= 0;

			if (BracketSym != ' ')
			{
				if (BracketSym == '(')
					{
						BracketSymClose = ')';
						i++;
					}
				else
					{
						BracketSymClose = ']';
						i++;
					}
			}
			
			while (i < InputStr.Length/*InputStr[i] != BracketSym | InputStr[i] != BracketSymClose*/)
			{
				String elemN = "";

				if (allowedSym.Contains(InputStr[i]) == false) // проверка валидности символа // Может проверять всю строку??
                   {
                    outMsg = "1.Обнаружен запрещенный символ";
                    return;
                   } 

				if (InputStr[i] == '(')
				{
					ParseBracketBlock(ref InputStr, ref i, InputStr[i], 1);
					if (outMsg[0] == '1')
						return;
					if (i >= InputStr.Length)
                	{
						outMsg = "0.Проверка прошла успешно";
						return;
					}	 
				}
				else if (InputStr[i] == '[')
				{	
					ParseBracketBlock(ref InputStr, ref i, InputStr[i], 2);
					if (outMsg[0] == '1')
							return;
					if (i >= InputStr.Length)
                    	{
							outMsg = "0.Проверка прошла успешно";
							return;
						}	 
				}

				if (InputStr[i] >= 60 & InputStr[i] <= 90) // блок проверки элемента
					ParseChemElName(ref InputStr, ref i, ref elemN, ref blockIStart, ref bracketFlagOpen);

                if (i >= InputStr.Length)
                    break;

                if (InputStr[i] == '<') //блок проверки индекса Html
					ParseIndexBlockHtml(ref InputStr, ref i, ref elemN, ref blockIEnd, ref blockIStart);
                
                if (InputStr[i] >= '0' & InputStr[i] <= '9') //блок проверки индекса
					ParseIndexBlock(ref InputStr, ref i, ref elemN, ref blockIStart, ref blockIEnd);
				
				if (InputStr[i] == BracketSymClose) //блок обработки окончания выражения в скобках
				{
					blockIEnd = ChemList.Last().blockNbr;
					i++;
					if (InputStr[i] == '<') //блок проверки индекса Html
					ParseIndexBlockHtml(ref InputStr, ref i, ref elemN, ref blockIEnd, ref blockIStart);
                
                	if (InputStr[i] >= '0' & InputStr[i] <= '9') //блок проверки индекса
						ParseIndexBlock(ref InputStr, ref i, ref elemN, ref blockIStart, ref blockIEnd);
					bracketFlagClose = 1;
					return;
				}
			}
			if (BracketSym != ' ' & bracketFlagClose == 0)
					 outMsg = "1.Не найдена закрывающая скобка";	
			return;
		}
        // private void ParseStr()
        // {
        //     int i = 0;
		// //	int bracketFlagOpen = 0; // 1 = '(', 2 = '['
		// //	int bracketFlagClose = 0;
        //     if (!(InputStr[i] >= 60 & InputStr[i] <= 90) | InputStr[i] != '(' | InputStr[i] != '[' )
        //         {
        //             outMsg = "1.Последовательность должна начинаться c заглавного символа элемента";
        //             return;
        //         }
        //     while (i < InputStr.Length)
        //     {
        //         string elemN = "";

        //         if (allowedSym.Contains(InputStr[i]) == false) // проверка валидности символа
        //            {
        //             outMsg = "1.Обнаружен запрещенный символ";
        //             return;
        //            } 

		// 		//if (InputStr[i] == '(' | InputStr[i] = '[')

		// 		if (InputStr[i] >= 60 & InputStr[i] <= 90) // блок проверки элемента
		// 			ParseChemElName(ref InputStr, ref i, ref elemN);

        //         if (i >= InputStr.Length)
        //             break;

        //         if (InputStr[i] == '<') //блок проверки индекса Html
		// 			ParseIndexBlockHtml(ref InputStr, ref i, ref elemN);
                
        //         if (InputStr[i] >= '0' & InputStr[i] <= '9') //блок проверки индекса
		// 			ParseIndexBlock(ref InputStr, ref i, ref elemN);
        //     }
		// 	outMsg = "0.Проверка прошла успешно";//дописать про код ошибки
        //     return;
        // }

    }
}
