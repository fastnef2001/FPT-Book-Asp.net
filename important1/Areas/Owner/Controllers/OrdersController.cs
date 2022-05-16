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

namespace important1.Areas.Owner.Controllers
{
    [Area("Owner")]
    public class OrdersController : Controller
    {
        private readonly UserContext _context;
        private readonly UserManager<important1User> _userManager;
        public OrdersController(UserContext context, UserManager<important1User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Owner/Orders
        public async Task<IActionResult> Index(int id = 0, string searchString = "")
        {
            ViewData["CurrentFilter"] = searchString;
            var thisUserId = _userManager.GetUserId(HttpContext.User);
            var orders = from b in _context.Orders
                         select b;
            orders = orders.Include(u => u.IdNavigation).Include(c => c.OrderDetails).ThenInclude(c => c.IsbnNavigation)
               .Where(d => d.OrderDetails.Where(e => e.IsbnNavigation.Id == thisUserId).Any())
               .OrderByDescending(r => r.OrderDate);
            if (searchString != null)
            {
                orders = orders.Where(b => b.IdNavigation.UserName.Contains(searchString));
            }
            List<Order> orderList = await orders.ToListAsync();
            return View(orderList);
        }
    }
}
