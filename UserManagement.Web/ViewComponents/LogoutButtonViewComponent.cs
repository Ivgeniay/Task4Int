using Microsoft.AspNetCore.Mvc;

namespace UserManagement.Web.ViewComponents
{
    public class LogoutButtonViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(bool? isAuthenticated)
        {
            return View(isAuthenticated);
        }
    }
}
