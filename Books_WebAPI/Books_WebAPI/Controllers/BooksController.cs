using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Books_WebAPI.Models;
using Books_WebAPI.DTOs;
using System.Linq.Expressions;

namespace Books_WebAPI.Controllers
{
    [RoutePrefix("api/books")]
    public class BooksController : ApiController
    {
        private Books_WebAPIContext db = new Books_WebAPIContext();

        // Expression for returning DTO instances of Book
        private static readonly Expression<Func<Book, BookDto>> AsBookDto =
            x => new BookDto
            {
                Title = x.Title,
                Author = x.Author.Name,
                Genre = x.Genre
            };

        private static readonly Expression<Func<Book, BookDetailDto>> AsBookDetailDto =
            x => new BookDetailDto
            {
                Title = x.Title,
                Genre = x.Genre,
                publishDate = x.PublishDate,
                Description = x.Description,
                Price = x.Price,
                Author = x.Author.Name
            };

        // GET: api/Books
        [Route("")]
        public IQueryable<BookDto> GetBooks()
        {
            return db.Books.Include(b => b.Author).Select(AsBookDto);
        }

        // GET: api/Books/5
        [Route("{id:int}")]
        [ResponseType(typeof(Book))]
        public async Task<IHttpActionResult> GetBook(int id)
        {
            BookDto book = await db.Books.Include(x => x.Author).Where(x => x.BookId == id).Select(AsBookDto).FirstOrDefaultAsync();
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        // The async mehtod for getting the book details
        [Route("{id:int}/details")]
        [ResponseType(typeof(BookDetailDto))]
        public async Task<IHttpActionResult> GetBookDetail (int id)
        {
            BookDetailDto book = await db.Books.Include(x => x.Author).Where(x => x.BookId == id).Select(AsBookDetailDto).FirstOrDefaultAsync();

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        // Get books by Genre
        [Route("{genre}")]
        public IQueryable<BookDto> GetBooksByGenre (string genre)
        {
            return db.Books.Include(x => x.Author).Where(x => x.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase)).Select(AsBookDto);
        }

        // Get books by Author
        [Route("date/{pubdate:datetime:regex(\\d{4}-\\d{2}-\\d{2})}")]
        [Route("date/{*pubdate:datetime:regex(\\d{4}/\\d{2}/\\d{2})}")]  // new
        public IQueryable<BookDto> GetBooksByAuthor (int authorID)
        {
            return db.Books.Include(x => x.Author).Where(x => x.AuthorId == authorID).Select(AsBookDto);
        }

        // Get books by publication date
        [Route("date/{pdate:datetime}")]
        public IQueryable<BookDto> GetBooksByDate (DateTime pdate)
        {
            return db.Books.Include(x => x.Author).Where(x => DbFunctions.TruncateTime(pdate) == DbFunctions.TruncateTime(x.PublishDate)).Select(AsBookDto);
        }

        // PUT: api/Books/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutBook(int id, Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != book.BookId)
            {
                return BadRequest();
            }

            db.Entry(book).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Books
        [ResponseType(typeof(Book))]
        public async Task<IHttpActionResult> PostBook(Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Books.Add(book);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = book.BookId }, book);
        }

        // DELETE: api/Books/5
        [ResponseType(typeof(Book))]
        public async Task<IHttpActionResult> DeleteBook(int id)
        {
            Book book = await db.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            db.Books.Remove(book);
            await db.SaveChangesAsync();

            return Ok(book);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BookExists(int id)
        {
            return db.Books.Count(e => e.BookId == id) > 0;
        }
    }
}