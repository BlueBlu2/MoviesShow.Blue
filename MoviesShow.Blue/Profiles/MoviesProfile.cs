using AutoMapper;
using MoviesShow.Blue.Models;
using MoviesShow.Blue.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesShow.Blue.Profiles
{
    public class MoviesProfile: Profile
    {
        public MoviesProfile()
        {
            CreateMap<Movie, MovieFormViewModel>().ReverseMap();
        }
    }
}
