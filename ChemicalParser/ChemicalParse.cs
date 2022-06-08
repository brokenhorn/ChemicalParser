using System;
using System.Collections.Generic;
using System.Linq;
using ChemicalHierarchy;
using System.Text.Json;

namespace ChemicalParserValidator
{
    public class ChemicalParser
    {
        [Serializable]
        private class ChemicalDescription //Описательный класс
        {
            public ChemicalSubstance SubstanceDescription { get; set; }
            public ChemicalSystem SystemDescrition { get; set; }

            public ChemicalDescription(ChemicalSystem system)
            {
                SystemDescrition = system;
                //SubstanceDescription = null;
            }

            public ChemicalDescription(ChemicalSubstance substance)
            {
                //SystemDescrition = null;
                SubstanceDescription = substance;
            }
        }

        private class ElemQnQ //Блок для хранения качества и кол-ва элемента
        {
            public ChemicalElement elemName { get; set; }
            public double min { get; set; }
            public double max { get; set; }
            public int blockNbr { get; set; }

            public void SetMinMax(double NewMin, double NewMax)
            {
                min = NewMin * min;
                max = NewMax * max;
            }
        }

        public  String outMsg { get; set; }
		private String allowedSym = "1234567890-<>()[]-.ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz/";
        private String exceptionSym = ".abcdefghijklmnopqrstuvwxyz/";
        private String InputStr;
        private List<ElemQnQ> ChemList; //список блоков элементов с их характеристикой
        private int blockI;
        private int ChemSystemFlag; //0 - Substance; 1 - System
        private ChemicalSubstance Substance { get; set; }
        private ChemicalSystem System { get; set; }
        private String JsonDesc { get; set; }

        /// <summary>
        /// Возвращает объект типа ChemicalSystem
        /// </summary>
        public ChemicalSystem GetChemicalSystem()
        {
            return System;
        }

        /// <summary>
        /// Возвращает объект типа ChemicalSubstance, если исходная строка представляет вещество, иначе возвращает пустой объект
        /// </summary>
        public ChemicalSubstance GetChemicalSubstance()
        {
            if (ChemSystemFlag == 0)
                return Substance;
            throw new ApplicationException("Химическое вещество отсутствует");
        }

        /// <summary>
        /// Возвращает объект строку с JSON описание химической формулы
        /// </summary>
        public String GetJsonDescription()
        {
            return JsonDesc;
        }

        /// <summary>
        /// Конструктор, получает строку, обрабатывает, создает объект типа ChemicalSubstance или ChemicalSystem
        /// </summary>
        public ChemicalParser(string chemStr) // конструктор
        {
            int i = 0;
            blockI = 0;
            ChemSystemFlag = 0;
            outMsg = "0";
            ChemList = new List<ElemQnQ>();

            if (chemStr == null | chemStr == "")
            {
	            outMsg = "Обнаружена пустая строка";
				throw new ApplicationException(outMsg);
            }


            InputStr = String.Concat(chemStr.Where(c => !Char.IsWhiteSpace(c))); // Удаление всех пробелов в строке

            ParseStr(ref i, ' ', 0);

            ChemicalElement[] ElementArray   = new ChemicalElement[blockI];
            ChemicalQuantity[] QuantityArray = new ChemicalQuantity[blockI];
            ChemicalDescription description;
            for (int j = 0; j < blockI; j++)
            {
                ElementArray[j]  = ChemList[j].elemName;
                QuantityArray[j] = new ChemicalQuantity(ChemList[j].min, ChemList[j].max);
            }
            System = new ChemicalSystem(false, ElementArray);
            if (ChemSystemFlag == 0)
            {
                Substance   = new ChemicalSubstance(false, System, QuantityArray);
                description = new ChemicalDescription(Substance);
            }
            else
                description = new ChemicalDescription(System);

            JsonDesc = JsonSerializer.Serialize(description);
        }

        private void ParseMinMax(ref int i, ref String MinOrMax) // блок обработки максимального или минимального значения
        {
            while (InputStr[i] >= '0' & InputStr[i] <= '9' | InputStr[i] == '.')
            {
                MinOrMax += InputStr[i];
                i++;
                if (i >= InputStr.Length)
                    return;
            }
        }

		/* private void ParseIndexBlockHtml(ref int i, ref String elemN, ref int blockIStart, ref int blockIEnd) // Блок обработки HTML индекса 
		 {
			 String str_min = "";
			 String str_max = "";
			 double min = -1;
			 double max = -1;

			 if ((InputStr.Remove(0, i)).Substring(0, 5) != "<sub>")
			 {
				 outMsg = "1.Обнаружен запрещенный блок символ";
				 throw new ApplicationException(outMsg);
			 }
			 i += 5;

			 if (!(i >= InputStr.Length))
				 ParseMinMax(ref i, ref str_min);

			 if (!(i >= InputStr.Length))
			 {
				 if (InputStr[i] == '-')
				 {
					 i++;
					 ParseMinMax(ref i, ref str_max);
				 }
			 }

			 if ((InputStr.Remove(0, i)).Substring(0, 6) != "</sub>")
			 {
				 outMsg = "1.Обнаружен запрещенный блок символ";
				 throw new ApplicationException(outMsg);
			 }

			 min = Convert.ToDouble(str_min);
			 if (str_max != "")
				 max = Convert.ToDouble(str_max);
			 if (max == -1)
				 max = min;

			 ElemQnQ tmp = ChemList.Last();
			 if (blockIEnd == 0)
				 ChemList[blockI - 1].SetMinMax(min, max);
			 else
			 {
				 foreach (var block in ChemList)
				 {
					 if (block.blockNbr >= blockIStart & block.blockNbr <= blockIEnd)
						 ChemList[block.blockNbr - 1].SetMinMax(min, max);
				 }
				 blockIStart = 0;
				 blockIEnd = 0;
			 }
			 i += 6;
		 }*/

		private void ParseIndexBlockHtml(ref int i, ref String elemN, ref int blockIStart, ref int blockIEnd) // Блок обработки HTML индекса 
		{
			String str_min = "";
			String str_max = "";
			double min = -1;
			double max = -1;


			if ((InputStr.Remove(0, i)).Substring(0, 5) != "<sub>")
			{
				outMsg = "1.Обнаружен запрещенный блок символ";
				throw new ApplicationException(outMsg);
			}
			i += 5;

			ParseMinMax(ref i, ref str_min);

			if (!(i >= InputStr.Length))
			{
				if (InputStr[i] == '-')
				{
					i++;
					ParseMinMax(ref i, ref str_max);
				}
			}

			if ((InputStr.Remove(0, i)).Substring(0, 6) != "</sub>")
			{
				outMsg = "1.Обнаружен запрещенный блок символ";
				throw new ApplicationException(outMsg);
			}
			i += 6;

			min = Convert.ToDouble(str_min);
			if (str_max != "")
				max = Convert.ToDouble(str_max);
			if (max == -1)
				max = min;
			if (blockIEnd == 0)
				ChemList[blockI - 1].SetMinMax(min, max);
			else
			{
				for (int j = blockIStart; j <= blockIEnd; j++)
					ChemList[j - 1].SetMinMax(min, max); // why j -1??

				blockIStart = 0;
				blockIEnd = 0;
			}
		}

		private void ParseIndexBlock(ref int i, ref String elemN, ref int blockIStart, ref int blockIEnd) //Блок обработки индекса
        {
            String str_min = "";
            String str_max = "";
            double min = -1;
            double max = -1;

            ParseMinMax(ref i, ref str_min);

            if (!(i >= InputStr.Length))
            {
                if (InputStr[i] == '-')
                {
                    i++;
                    ParseMinMax(ref i, ref str_max);
                }
            }

            min = Convert.ToDouble(str_min);
            if (str_max != "")
                max = Convert.ToDouble(str_max);
            if (max == -1)
                max = min;
            if (blockIEnd == 0)
                ChemList[blockI - 1].SetMinMax(min, max);
            else
            {
                for (int j = blockIStart; j <= blockIEnd; j++)
                    ChemList[j - 1].SetMinMax(min, max); // why j -1??

                blockIStart = 0;
                blockIEnd = 0;
            }
        }
        private void ParseChemElName(ref int i, ref String elemN, ref int blockIStart, ref int bracketFlagOpen) //блок формирования символа элемента
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

            ChemList.Add(new ElemQnQ { elemName = new ChemicalElement(elemN), min = 1, max = 1, blockNbr = blockI });
        }

        void SetSymClose(ref int i, ref char BracketSym, ref char BracketSymClose) // Блок Проверки нахождения в скобке, установка закрываюего символа скобки
        {
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
        }

        void SetChemSystemFlag(ref int i) // Блок проверки наличия признака хим. системы в строке
        {
            if (InputStr[i] == '-')
            {
                ChemSystemFlag = 1;
                i++;
            }
        }

        private void ParseStr(ref int i, char BracketSym, int FlagOpen) // Главный метод обработки строки (рекурсивный)
        {
            char BracketSymClose = ' ';
            int bracketFlagOpen = FlagOpen; // 1 = '(', 2 = '['
            int bracketFlagClose = 0;
            int blockIStart = 0;
            int blockIEnd = 0;

            SetSymClose(ref i, ref BracketSym, ref BracketSymClose);

            while (i < InputStr.Length)
            {
                String elemN = "";

                if (allowedSym.Contains(InputStr[i]) == false) // проверка валидности символа
                {
                    outMsg = "1.Обнаружен запрещенный символ";
                    throw new ApplicationException(outMsg);
                }

                if (InputStr[i] == '(')
                {
                    ParseStr(ref i, InputStr[i], 1);
                    if (i >= InputStr.Length)
                    {
                        outMsg = "0.Проверка прошла успешно";
                        return;
                    }
                }
                else if (InputStr[i] == '[')
                {
                    ParseStr(ref i, InputStr[i], 2);
                    if (i >= InputStr.Length)
                    {
                        outMsg = "0.Проверка прошла успешно";
                        return;
                    }
                }

                if (InputStr[i] >= 60 & InputStr[i] <= 90) // блок проверки элемента
                    ParseChemElName(ref i, ref elemN, ref blockIStart, ref bracketFlagOpen);

                if (i >= InputStr.Length)
                    break;

                if (InputStr[i] == '<') //блок проверки индекса Html
                    ParseIndexBlockHtml(ref i, ref elemN, ref blockIStart, ref blockIEnd);

                if (i >= InputStr.Length)
	                break;

				if (InputStr[i] >= '0' & InputStr[i] <= '9') //блок проверки индекса
                    ParseIndexBlock(ref i, ref elemN, ref blockIStart, ref blockIEnd);

                if (i >= InputStr.Length)
                    break;

                if (InputStr[i] == BracketSymClose) //блок обработки окончания выражения в скобках
                {
                    blockIEnd = ChemList.Last().blockNbr;
                    i++;

                    if (i >= InputStr.Length)
                    {
	                    bracketFlagClose = 1;
						break;
                    }

					if (InputStr[i] == '<') //блок проверки индекса Html
                        ParseIndexBlockHtml(ref i, ref elemN, ref blockIStart, ref blockIEnd);


					if (i >= InputStr.Length)
						break;

					if (InputStr[i] >= '0' & InputStr[i] <= '9') //блок проверки индекса
                        ParseIndexBlock(ref i, ref elemN, ref blockIStart, ref blockIEnd);

                    bracketFlagClose = 1;
                    return;
                }

                if (i >= InputStr.Length)
                    break;

                SetChemSystemFlag(ref i);

                if (exceptionSym.Contains(InputStr[i]) == true) // проверка допустимых символов, которые не могли дойти до этого блока
                {
                    outMsg = "1.Обнаружен недопустимый в данном блоке символ";
                    throw new ApplicationException(outMsg);
                }
            }
            if (BracketSym != ' ' & bracketFlagClose == 0)
            {
                outMsg = "1.Не найдена закрывающая скобка";
                throw new ApplicationException(outMsg);
            }
            
        }
    }
}
