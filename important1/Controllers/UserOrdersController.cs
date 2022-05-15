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
    public class UserOrdersController : Controller
    {
        private readonly UserContext _context;
        private readonly UserManager<important1User> _userManager;
        


        public UserOrdersController(UserContext context, UserManager<important1User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: UserOrders
        public async Task<IActionResult> Index(int id = 0)
        {
            var thisUserId = _userManager.GetUserId(HttpContext.User);
            var orders = from b in _context.Orders
                         select b;
            
            orders = orders.Include(u => u.IdNavigation).Include(c => c.OrderDetails)
                .Where(d => d.Id == thisUserId);

            List<Order> orderList = await orders.ToListAsync();
          
           
            return View(orderList);




        }
    }
}
