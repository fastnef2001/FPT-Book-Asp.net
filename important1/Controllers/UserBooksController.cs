#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using important1.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using important1.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;

namespace important1.Controllers
{
    public class UserBooksController : Controller
    {
        private readonly int _recordsPerPage = 1;
        private readonly UserContext _context;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<UserBooksController> _logger;
        private readonly UserManager<important1User> _userManager;

        public UserBooksController(
            ILogger<UserBooksController> logger, 
            IEmailSender emailSender, 
            UserManager<important1User> userManager, 
            UserContext context)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        [Authorize(Roles = "Customer")]
        public IActionResult ForCustomerOnly()
        {
            ViewBag.message = "This is for Customer only! Hi " + _userManager.GetUserName(HttpContext.User);
            return View("Views/Home/Index.cshtml");
        }

        [Authorize(Roles = "Seller")]
        public IActionResult ForSellerOnly()
        {
            ViewBag.message = "This is for Store Owner only!";
            return View("Areas/Owner/Views/Home/Index.cshtml");
        }


        [Authorize(Roles = "Admin")]
        public IActionResult ForAdmin()
        {
            ViewBag.message = "This is for Store Admin only!";
            return View("Areas/Admin/Views/Home/Index.cshtml");
        }


        // GET: UserBooks
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
                         
                          .Where(b => b.Title.Contains(searchString))
                          .OrderBy(b => b.Title);
                List<Book> booksList = await books.Skip(id * _recordsPerPage)
                  .Take(_recordsPerPage).ToListAsync();

                int numOfFilteredBook = books.Count();
                ViewBag.NumberOfPages = (int)Math.Ceiling((double)numOfFilteredBook / _recordsPerPage);

                return View(booksList);
            }
        }
 


        // GET: UserBooks/Details/5
        public async Task<IActionResult> Details(string id, int quantity)
        {
            ViewData["quantitytest"] = quantity;
            if (id == null)
            {
                return NotFound();
            }
            var book = await _context.Books
                .Include(b => b.Carts)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Isbn == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

    

        public async Task<IActionResult> AddToCart(string isbn, int quantity = 1)
        {
            
                var thisUserId = _userManager.GetUserId(HttpContext.User);
                var userCart = await _context.Books
                        .FindAsync(isbn);

                Cart myCart = new Cart() 
                { 
                    UId = thisUserId,
                    BookIsbn = isbn, Quantity = quantity, 
                    TotalPerCart = userCart.Price * quantity 
                };
                Cart fromDb = _context.Carts.FirstOrDefault(c => c.UId == thisUserId && c.BookIsbn == isbn);
                //if not existing (or null), add it to cart. If already added to Cart before, ignore it.
                if (fromDb == null)
                {
                    _context.Add(myCart);
                    await _context.SaveChangesAsync();

                }
                return RedirectToAction("Index");
                    
            




        }

        public async Task<IActionResult> Checkout()
        {
            var thisUserId = _userManager.GetUserId(HttpContext.User);
            List<Cart> myDetailsInCart = await _context.Carts
                .Where(c => c.UId == thisUserId)
                .Include(c => c.Book)
                .ToListAsync();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Order myOrder = new Order();
                    myOrder.Id = thisUserId;
                    myOrder.OrderDate = DateTime.Now;
                    myOrder.Total = myDetailsInCart.Select(c => c.TotalPerCart)
                        .Aggregate((c1, c2) => c1 + c2);

                    _context.Add(myOrder);
                    await _context.SaveChangesAsync();

                    foreach (var item in myDetailsInCart)
                    {
                        OrderDetail detail = new OrderDetail()
                        {
                            OrderId = myOrder.OrderId,
                            Isbn = item.BookIsbn,
                            Quantity = item.Quantity,
                        };
                        _context.Add(detail);
                    }
                    await _context.SaveChangesAsync();

                    _context.Carts.RemoveRange(myDetailsInCart);
                    await _context.SaveChangesAsync();
                    transaction.Commit();

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Error occurred in Checkout" + ex);
                }
            }

            return RedirectToAction("Index", "UserCarts");
        }

        // GET: UserBooks/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId");
            ViewData["Id"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

       
    }
}
