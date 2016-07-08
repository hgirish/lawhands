using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lawhands.Controllers
{
    [Authorize(Policy = PolicyNames.AdministratorsOnly)]
    public class AdminBaseController : Controller
    {
        
    }
}