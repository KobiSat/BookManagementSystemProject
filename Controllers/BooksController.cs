using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookManagementSystem.Models;
using Microsoft.AspNetCore.Http.HttpResults;


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
         /// <summary>
         /// Lists all books with pagination as recieved, or first page of 10 books.
         /// If page is to high, returns last existing page and update IsOverLastPage in reponse.
         /// </summary>
         /// <param name="page"></param>
         /// <param name="pageSize"></param>
         /// <returns>Response with {TotalItems, TotalPages, IsOverLastPage, CurrentPage, PageSize, Books}</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksList(int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Page number and page size must be greater than 0.");
            }

            var totalItems = await _context.Books.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            bool isOverLastPage = false;

            if (page > totalPages && totalPages > 0)
            {
                page = totalPages;
                isOverLastPage = true;
            }

            var books = await _context.Books
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                TotalItems = totalItems,
                TotalPages = totalPages,
                IsOverLastPage = isOverLastPage,
                CurrentPage = page,
                PageSize = pageSize,
                Books = books
            });
        }

        // GET: api/books/{id}
        /// <summary>
        /// Retrieves book information by recieved id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>book</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBookById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Id");
            }

            var book = await _context.Books.FindAsync(id);
            
            if (book == null)
            {
                return NotFound();
            }
            
            return Ok(book);
        }

        // POST: api/books
        /// <summary>
        /// Creates new book entry
        /// </summary>
        /// <param name="book"></param>
        /// <returns>Book added details</returns>
        [HttpPost]
        public async Task<ActionResult<Book>> CreateNewBookEntry([FromBody] Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBookById", new { id = book.Id }, book);
        }    
    
        // PUT: api/books/{id}
        /// <summary>
        /// Updates an existing book by it's id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="book"></param>
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
        /// <summary>
        /// Deletes a book by it's id
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Id");
            }

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
        /// <summary>
        /// Search by title or author.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="author"></param>
        /// <returns>A list of Books contain the recieved strings.</returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Book>>> SearchBooks([FromQuery] string title="", [FromQuery] string author="")
        {
            if (title.Length > 200 || author.Length > 200)
            {
                return BadRequest("Title or Author are invalid");
            }

            var books = await _context.Books
                .Where(b => (string.IsNullOrEmpty(title) || b.Title.StartsWith(title)) &&
                            (string.IsNullOrEmpty(author) || b.Author.Contains(author)))
                .ToListAsync();

            return Ok(books);
        }

        /// <summary>
        /// Checks if the recieved id exists in DB. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if exists, otherwise False</returns>
        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}