using ApiDemo.Models;

namespace ApiDemo.Services
{
    public class ModelService
    {
        Model model { get; set; }
        public ModelService(Model model) {
            this.model = model;
        }

        public bool getAllMovies(ref Model model)
        {
            if (this.model == null) return false;
            else
            {
                model = this.model;
                return true;
            }
        }

        public bool GetMovie(int id, ref Movie _movie)
        {
            if (model.movies != null)
            {
                foreach (var movie in model.movies)
                {
                    if (movie.Id == id)
                    {
                        _movie = movie;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool AddMovie(ref Movie movie)
        {
            foreach (var _movie in model.movies)
            {
                if (_movie.Id == movie.Id || _movie.title.Equals(movie.title))
                {
                    movie = _movie;
                    return false;
                }
            }
            model.movies.Add(movie);
            model.movies = model.movies.OrderBy(movie => movie.Id).ToList();
            return true;
        }

        public bool UpdateMovie(int id, ref Movie movie)
        {
            for (int i = 0; i < model.movies.Count(); i++)
            {
                if (model.movies[i].Id == id)
                {
                    model.movies[i] = movie;
                    return true;
                }
            }
            return false;
        }

        public bool DeleteMovie(int id)
        {
            for (int i = 0; i < model.movies.Count(); i++)
            {
                if (model.movies[i].Id == id)
                {
                    model.movies.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
    }
}
