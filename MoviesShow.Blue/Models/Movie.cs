using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesShow.Blue.Models
{
    public class Movie
    {
        public Guid Id { get; set; }
        [Required,MaxLength(250)]
        public string Title { get; set; }
        public string Year { get; set; }
        public decimal Rate { get; set; }
        [Required, MaxLength(2500)]
        public string StoryLine { get; set; }
        [Required]
        public byte[] Poster { get; set; }
        public byte GenreId { get; set; }
        public Genre Genre { get; set; }
    }
}
