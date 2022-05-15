#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using important1.Models;
using Microsoft.AspNetCore.Identity;
using important1.Areas.Identity.Data;
using important1.Helpper;
using Microsoft.AspNetCore.Authorization;
using PagedList.Core;

namespace important1.Areas.Owner.Controllers
{
    [Area("Owner")]
    [Authorize(Roles = "Seller")]
    public class BooksController : Controller
    {
        private readonly int _recordsPerPage = 5;
        private readonly UserContext _context;
        private readonly UserManager<important1User> _userManager;

       

        public BooksController(UserContext context, UserManager<important1User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        

            public async Task<IActionResult> Index(int? categoryInt, int id = 0, string searchString = "")
       {
            ViewData["CategoryId"] = new SelectList(_context.Categories.ToList(), "CategoryId", "Name");
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentCate"] = categoryInt;
            var userid = _userManager.GetUserId(HttpContext.User);
            var books = from b in _context.Books 
                       select b;

            ViewBag.CurrentPage = id;

            if (categoryInt != null)
            {
                books = books.Include(b => b.Category)
                         .Include(b => b.IdNavigation)
                         .Where(c => c.Id == userid)
                         .Where(b => b.Title.Contains(searchString))
                         .Where(s => s.CategoryId == categoryInt)
                         .OrderBy(b => b.Title);
                
                List<Book> booksList = await books.Skip(id * _recordsPerPage)
                   .Take(_recordsPerPage).ToListAsync();
                int numOfFilteredBook = books.Count();
                ViewBag.NumberOfPages = (int)Math.Ceiling((double)numOfFilteredBook / _recordsPerPage);
              

                return View(booksList);
            }

            else
            {
                books = books.Include(b => b.Category)
                          .Include(b => b.IdNavigation)
                          .Where(c => c.Id == userid)
                          .Where(b => b.Title.Contains(searchString))
                          .OrderBy(b => b.Title);
                List<Book> booksList = await books.Skip(id * _recordsPerPage)
                  .Take(_recordsPerPage).ToListAsync();

                int numOfFilteredBook = books.Count();
                ViewBag.NumberOfPages = (int)Math.Ceiling((double)numOfFilteredBook / _recordsPerPage);

                return View(booksList);
            }   
        }


        // GET: Owner/Books/Details/5
        public async Task<IActionResult> Details(string id)
        {

            ViewData["CategoryId"] = new SelectList(_context.Categories.ToList(), "CategoryId", "Name");
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Category)
                .Include(b => b.IdNavigation)
                .FirstOrDefaultAsync(m => m.Isbn == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Owner/Books/Create
        public IActionResult Create()
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            ViewData["CategoryId"] = new SelectList(_context.Categories.ToList(), "CategoryId", "Name");
           /* ViewData["Id"] = _context.AspNetUsers.Where(c => c.Id == userid).FirstOrDefault().UserName;*/
            ViewData["Id"] = _context.Users.Where(c => c.Id == userid).FirstOrDefault().UserName;

            return View();
        }

        // POST: Owner/Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Isbn,Title,Descr,Author,Price,Thumb,Page,CategoryId")] Book book, IFormFile image)
        {
            if (image != null)
            {
                string imgName = book.Isbn + Path.GetExtension(image.FileName);
                string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", imgName);
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }
                book.Thumb = imgName;
            }
            else
            {
                return View(book);
            }
            var userid = _userManager.GetUserId(HttpContext.User);
            if (ModelState.IsValid)
            {

                book.Id = userid;
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories.ToList(), "CategoryId", "Name");
        /*    ViewData["Id"] = _context.AspNetUsers.Where(c => c.Id == userid).FirstOrDefault().UserName;*/
            ViewData["Id"] = _context.Users.Where(c => c.Id == userid).FirstOrDefault().UserName;

            return View(book);

        }

        // GET: Owner/Books/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            var userid = _userManager.GetUserId(HttpContext.User);
            ViewData["CategoryId"] = new SelectList(_context.Categories.ToList(), "CategoryId", "Name", book.CategoryId);
            ViewData["Id"] = new SelectList(_context.Users.Where(c => c.Id == userid), "Id", "UserName", book.Id);

            return View(book);
        }

        // POST: Owner/Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Isbn,Title,Descr,Author,Price,Thumb,Page,Id,CategoryId")] Book book, IFormFile image)
        {
         if (image == null)
            {

             
                Book thisProduct = _context.Books.Where(p => p.Isbn == book.Isbn).AsNoTracking().FirstOrDefault();
                book.Thumb = thisProduct.Thumb;
            }
           
            
            else
            {
                string imgName = book.Isbn + Path.GetExtension(image.FileName);
                string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", imgName);
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }
                book.Thumb = imgName;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Isbn))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            var userid = _userManager.GetUserId(HttpContext.User);
            ViewData["CategoryId"] = new SelectList(_context.Categories.ToList(), "CategoryId", "Name", book.CategoryId);
            ViewData["Id"] = new SelectList(_context.Users.Where(c => c.Id == userid), "Id", "UserName", book.Id);

            return View(book);
        }

        // GET: Owner/Books/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
           

            if (id == null)
            {
                return NotFound();
            }

            

            var book = await _context.Books
                .Include(b => b.Category)
                .Include(b => b.IdNavigation)
                .FirstOrDefaultAsync(m => m.Isbn == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Owner/Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var book = await _context.Books.FindAsync(id);

            string imgName = book.Thumb;
            string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", imgName);
            FileInfo file = new FileInfo(savePath);
            if (file.Exists)
            {
                file.Delete();
            }
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(string id)
        {
            return _context.Books.Any(e => e.Isbn == id);
        }
    }
}