using System;
using System.Web.Mvc;
using WebApplication2.Models;
using System.Threading;


namespace WebApplication2.Controllers
{

    public class HomeController : Controller
    {
	    [ValidateInput(false)]
		[HttpGet]
        public ActionResult Index()
        {
            FormulaProcess process = new FormulaProcess("", "");

            return View(process);
        }

        [ValidateInput(false)]
		[HttpPost]
        public ActionResult Index(FormulaProcess formula)
        {
	        Thread parseThread = new Thread(formula.ProcessParseThread);

	        parseThread.Start();
	        parseThread.Join();
	        FormulaProcess process = new FormulaProcess(formula.Str, formula.Answer);

            return View(process);
        }

    }
}