using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesShow.Blue.Helpers;
using MoviesShow.Blue.Models;
using MoviesShow.Blue.ViewModels;
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

        public MoviesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies.ToListAsync();
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
            await movie.AddGenres(_context); //= await AddGenres(movie);
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
            var allowedExtentions = new List<string> { ".jpg", ".png" };

            if (!allowedExtentions.Contains(Path.GetExtension(poster.FileName.ToLower())))
            {
                ModelState.AddModelError("Poster", "Only .jpg , .png are allowed!");
                return View("MovieForm", movie);
            }
            if (poster.Length > 1048576)
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
    }
}
