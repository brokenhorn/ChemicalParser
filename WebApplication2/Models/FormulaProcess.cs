using System;
using ChemicalParserValidator;


namespace WebApplication2.Models
{

	public class FormulaProcess
	{

		public String Str { set; get; }
		public String Answer { set; get; }

		public FormulaProcess(string nStr, string nAnswer)
		{
			Str = nStr;
			Answer = nAnswer;
		}

		public FormulaProcess()
		{
			Str = "";
			Answer = "";
		}

		public void ProcessParseThread()
		{
			try
			{
				ChemicalParser parser = new ChemicalParser(Str);
				Answer = parser.GetJsonDescription();
			}
			catch (Exception e)
			{
				Answer = e.Message;
			}
		}
	}
}