#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using important1.Areas.Identity.Data;
using important1.Models;
using Microsoft.AspNetCore.Identity;

namespace important1.Controllers
{
    public class UserOrderDetailsController : Controller
    {
        private readonly UserContext _context;

        private readonly UserManager<important1User> _userManager;
       


        public UserOrderDetailsController(UserContext context, UserManager<important1User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? id)
        {
            var thisUserId = _userManager.GetUserId(HttpContext.User);
            var userContext = _context.OrderDetails.Include(o => o.IsbnNavigation).Include(o => o.Order)
                .Where(u => u.OrderId == id);
            return View(await userContext.ToListAsync());
        }
    }
}
