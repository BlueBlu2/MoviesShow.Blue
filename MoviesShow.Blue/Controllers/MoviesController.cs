using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesShow.Blue.Helpers;
using MoviesShow.Blue.Models;
using MoviesShow.Blue.ViewModels;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesShow.Blue.Controllers
{
    public class MoviesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IToastNotification _toast;
        private List<string> _allowedExtentions = new List<string> { ".jpg", ".png" };
        private long _posterSize = 1048576;

        public MoviesController(AppDbContext context, IMapper mapper, IToastNotification toast)
        {
            _context = context;
            _mapper = mapper;
            _toast = toast;
        }
        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies.OrderByDescending(m=>m.Rate).ToListAsync();
            return View(movies);
        }

        public async Task<IActionResult> Create()
        {
            var movie = await new MovieFormViewModel().AddGenres(_context);
            return View("MovieForm", movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieFormViewModel movie)
        {
            await movie.AddGenres(_context);
            if (!ModelState.IsValid)
            {
                return View("MovieForm", movie);
            }
            var files = Request.Form.Files;
            var poster = files.FirstOrDefault();
            if (poster == null)
            {
                ModelState.AddModelError("Poster", "Please add movie poster");
                return View("MovieForm", movie);
            }

            if (!_allowedExtentions.Contains(Path.GetExtension(poster.FileName.ToLower())))
            {
                ModelState.AddModelError("Poster", "Only .jpg , .png are allowed!");
                return View("MovieForm", movie);
            }
            if (poster.Length > _posterSize)
            {
                ModelState.AddModelError("Poster", "poster cannot be more than 1MB!");
                return View("MovieForm", movie);
            }

            using var dataStream = new MemoryStream();
            await poster.CopyToAsync(dataStream);

            var validatedMovie = _mapper.Map<Movie>(movie);
            validatedMovie.Poster = dataStream.ToArray();

            _context.Movies.Add(validatedMovie);
            _context.SaveChanges();

            _toast.AddSuccessToastMessage("Movie created successfully");
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(Guid? Id)
        {
            if (Id == null)
                return BadRequest();

            var movie = await _context.Movies.FindAsync(Id);

            if (movie == null)
                return NotFound();
            var editingMovie = _mapper.Map<MovieFormViewModel>(movie);
            editingMovie.Id = movie.Id;
            await editingMovie.AddGenres(_context);
            return View("MovieForm",editingMovie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MovieFormViewModel movie)
        {
            await movie.AddGenres(_context);
            if (!ModelState.IsValid)
                return View("MovieForm", movie);
            
            var validatedMovie = await _context.Movies.FindAsync(movie.Id);
            if (validatedMovie == null)
                return NotFound();

            var files = Request.Form.Files;
            var poster = files.FirstOrDefault();
            if (poster != null)
            {
                using var dataStream = new MemoryStream();
                await poster.CopyToAsync(dataStream);
                movie.Poster = dataStream.ToArray();

                if (!_allowedExtentions.Contains(Path.GetExtension(poster.FileName.ToLower())))
                {
                    ModelState.AddModelError("Poster", "Only .jpg , .png are allowed!");
                    return View("MovieForm", movie);
                }
                if (poster.Length > _posterSize)
                {
                    ModelState.AddModelError("Poster", "poster cannot be more than 1MB!");
                    return View("MovieForm", movie);
                }

            }
            else
            {
                movie.Poster = validatedMovie.Poster;
            }

            _mapper.Map(movie, validatedMovie);
            _context.SaveChanges();

            _toast.AddSuccessToastMessage("Movie updated successfully");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(Guid? Id)
        {
            if(Id == null)
            {
                return BadRequest();
            }
            var movie = await _context.Movies.Include(m => m.Genre).SingleOrDefaultAsync(m =>m.Id == Id);
            if(movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid? Id)
        {
            if (Id == null)
            {
                return BadRequest();
            }
            var movie = await _context.Movies.FindAsync(Id);
            if (movie == null)
            {
                return NotFound();
            }

            _context.Movies.Remove(movie);
            _context.SaveChanges();
            return Ok();
        }

    }
}
