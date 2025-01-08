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

        // GET: api/books/{id}
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

        // POST: api/books
        [HttpPost]
        public async Task<ActionResult<Book>> CreateNewBookEntry(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            Console.WriteLine($"post {book.Title}");

            return CreatedAtAction("GetBookById", new { id = book.Id }, book);
        }    
    
        // PUT: api/books/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, [FromBody] Book book)
        {
            //Verifing id in params with id in body
            if (id != book.Id)
            {
                return BadRequest("Book ID mismatch");
            }

             _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //Throw a specific message for not existing Id
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/books/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // GET: api/books/search?title=example&author=author
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Book>>> SearchBooks([FromQuery] string title="", [FromQuery] string author="")
        {
            var books = await _context.Books
                .Where(b => (string.IsNullOrEmpty(title) || b.Title.StartsWith(title)) &&
                            (string.IsNullOrEmpty(author) || b.Author.Contains(author)))
                .ToListAsync();

            return Ok(books);
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}