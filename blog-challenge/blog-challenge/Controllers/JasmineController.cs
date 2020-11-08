using System;
using System.Web.Mvc;

namespace blog_challenge.Controllers
{
    public class JasmineController : Controller
    {
        public ViewResult Run()
        {
            return View("SpecRunner");
        }
    }
}
