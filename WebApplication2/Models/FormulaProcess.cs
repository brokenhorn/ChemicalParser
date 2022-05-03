using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
            Answer = "Поле вывода";
        }
    }
}