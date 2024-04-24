using Assignment.Entity.Models;
using Assignment.Models;
using Assignment.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Assignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserData _userData;

        public HomeController(ILogger<HomeController> logger, IUserData userData)
        {
            _logger = logger;
            _userData = userData;
        }

        #region Index
        public IActionResult Index(PaginatedViewModel dataModel)
        {
            ViewBag.AllCity = _userData.AllCity();
            PaginatedViewModel data = _userData.IndexData(dataModel);
            return View(data);
        }
        #endregion

        #region AddUser
        [HttpPost]
        public IActionResult AddUser(AddUserModel user, int? GenderRadio)
        {
            if (GenderRadio == 1)
            {
                user.Gender = "Male";
            }
            else if (GenderRadio == 2)
            {
                user.Gender = "Female";
            }
            else
            {
                user.Gender = "Other";
            }
            _userData.AddUser(user);
            return RedirectToAction("Index");
        }
        #endregion

        #region GetUserData
        public IActionResult GetData(int id)
        {
            AddUserModel aum = _userData.GetData(id);
            return Json(aum);
        }
        #endregion

        #region EditUser
        [HttpPost]
        public IActionResult EditUser(AddUserModel user, int? GenderRadio)
        {
            if (GenderRadio == 1)
            {
                user.Gender = "Male";
            }
            else if (GenderRadio == 2)
            {
                user.Gender = "Female";
            }
            else
            {
                user.Gender = "Other";
            }
            _userData.EditUser(user);
            return RedirectToAction("Index");
        }
        #endregion

        #region DeleteUser
        public IActionResult DeleteUser(int id)
        {
            _userData.DeleteUser(id);
            return RedirectToAction("Index");
        }
        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}