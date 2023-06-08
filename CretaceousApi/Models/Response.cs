namespace CretaceousApi.Models;

    public class Response
    {
        public List<Animal> Animals { get; set; } = new List<Animal>();
        public int Pages { get; set; }
        public int CurrentPage { get; set; }    
        public int PageSize { get; set; }
    }