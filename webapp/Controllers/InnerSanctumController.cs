namespace webapp.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    public class InnerSanctumController : Controller
    {
        [Authorize]
        public IActionResult InnerSanctumIndex(){

            return View();
        }
    }
}