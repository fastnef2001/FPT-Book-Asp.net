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
    public class UserCartsController : Controller
    {
        private readonly UserContext _context;
        private readonly UserManager<important1User> _userManager;

        public UserCartsController(UserContext context, UserManager<important1User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: UserCarts
        public async Task<IActionResult> Index()
        {
            var userid = _userManager.GetUserId(HttpContext.User);

            var userContext = _context.Carts.Include(c => c.Book)
                                            .Include(c => c.User)
                                            .Where(c => c.UId == userid);
            if (userContext.Count() == 0)
            {
                return View("/Views/UserCarts/CartEmpty.cshtml");
            }

            return View(await userContext.ToListAsync());
        }

      
        // GET: UserCarts/Edit/5
        public async Task<IActionResult> Edit(string uid, string bid)
        {
            if (uid == null || bid == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .FirstOrDefaultAsync(m => m.UId == uid && m.BookIsbn == bid);
            return View(cart);
        }

        // POST: UserCarts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("UId,BookIsbn,Quantity,TotalPerCart")] Cart cart, int quantity)
        {
            try
            {
                var book = _context.Books.FirstOrDefault(c => c.Isbn == cart.BookIsbn);
                cart.TotalPerCart = book.Price * quantity;
                _context.Carts.Update(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Edit", new { uid = cart.UId, bid = cart.BookIsbn });
            }

        }


        // GET: UserCarts/Delete/5
        public async Task<IActionResult> Delete(string uid, string bid)
        {
            if (uid == null || bid == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.Book)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.UId == uid && m.BookIsbn == bid);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }


        // POST: UserCarts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Cart cart)
        {
            /*var cart = await _context.Carts.FindAsync(id);
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));*/

            try
            {
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { uid = cart.UId, bid = cart.BookIsbn });
            }
        }


        private bool CartExists(string id)
        {
            return _context.Carts.Any(e => e.UId == id);
        }
    }
}
