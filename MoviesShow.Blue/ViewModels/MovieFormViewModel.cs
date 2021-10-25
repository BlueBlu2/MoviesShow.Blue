using MoviesShow.Blue.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesShow.Blue.ViewModels
{
    public class MovieFormViewModel
    {
        public Guid Id { get; set; }
        [Required, StringLength(250)]
        public string Title { get; set; }
        public string Year { get; set; }
        [Range(1,10)]
        public decimal Rate { get; set; }
        [Required, StringLength(2500)]
        [Display(Name ="Story Line")]
        public string StoryLine { get; set; }
        public byte[] Poster { get; set; }
        [Display(Name ="Genre")]
        public byte GenreId { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
    }
}
