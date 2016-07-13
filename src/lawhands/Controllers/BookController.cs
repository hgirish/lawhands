using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;

namespace lawhands.Controllers
{
    public class BookController : Controller
    {
        private readonly IHtmlLocalizer<BookController> _localizer;
        private readonly IStringLocalizer _localizer1;
        private readonly IStringLocalizer _localizer2;
        public BookController(IHtmlLocalizer<BookController> localizer, IStringLocalizerFactory factory)
        {
            _localizer = localizer;
            _localizer1 = factory.Create(typeof (SharedResource));
            _localizer2 = factory.Create("SharedResource", location: null);
        }

        public IActionResult Hello(string name)
        {
            ViewData["Message"] = _localizer["<b>Hello</b><i> {0}</i>", name];
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = _localizer1["Your application description page."]
                + " loc 2: " + _localizer2["Your application description page."];

            return View("Hello");
        }
    }

    public class SharedResource
    {
    }
}