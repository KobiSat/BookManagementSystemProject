using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookManagementSystem.Models;


namespace BookManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBookById(int id)
        {
            Console.WriteLine($"Searching for book with Id: {id}");  // Debug log


            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }

         // GET: api/books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksList(int page = 1, int pageSize = 10)
        {

            var books = await _context.Books
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            Console.WriteLine("get all");
            return Ok(books);
        }



        // POST: api/books
        [HttpPost]
        public async Task<ActionResult<Book>> CreateNewBookEntry(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            Console.WriteLine($"post {book.Title}");

            return CreatedAtAction("GetBookById", new { id = book.Id }, book);
        }    
    }
}