using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;
using ChemicalParserValidator;

namespace WebApplication2.Controllers
{

    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        // public ActionResult Index(FormulaProcess)
        [HttpGet]
        public ActionResult Index(string str)
        {
            FormulaProcess process = new FormulaProcess("", "ad");

            return View(process);
        }
        [HttpPost]
        public ActionResult Index(FormulaProcess formula)
        {
            try
            {
                ChemicalParser parser = new  ChemicalParser(formula.Str);
                formula.Answer = parser.GetJsonDescription();
            }
            catch (Exception e)
            {
                formula.Answer = e.Message;
            }
            //formula.Answer = "bbb!";
            FormulaProcess process = new FormulaProcess(formula.Str, formula.Answer);
            return View(process);
        }

    }
}