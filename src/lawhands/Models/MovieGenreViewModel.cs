using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace lawhands.Models
{
    public class MovieGenreViewModel
    {
        public List<Movie> movies;
        public SelectList genres;
        public string movieGenre { get; set; }
        public string searchString { get; set; }
    }
}