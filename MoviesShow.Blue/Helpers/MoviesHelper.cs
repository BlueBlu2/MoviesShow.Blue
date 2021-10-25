using Microsoft.EntityFrameworkCore;
using MoviesShow.Blue.Models;
using MoviesShow.Blue.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesShow.Blue.Helpers
{
    public static class MoviesHelper
    {
        public static async Task<MovieFormViewModel> AddGenres(this MovieFormViewModel model, AppDbContext context)
        {
            model.Genres = await context.Genres.OrderBy(g => g.Name).ToListAsync();
            return model;
        }
    }
}
