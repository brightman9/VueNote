using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http.Extensions;
using VueNote.Core.Util;
using VueNote.Core.UserManagement;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VueNote.WebApi.Controllers
{

    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return Json(new
            {
                success = true,
                msg = "hello, this is VueNote"
            });
        }
    }
}
