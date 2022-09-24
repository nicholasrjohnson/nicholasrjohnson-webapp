namespace webapp.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    public class MembersController : Controller
    {
        [Authorize]
        public IActionResult MembersIndex(){

            return View();
        }
    }
}