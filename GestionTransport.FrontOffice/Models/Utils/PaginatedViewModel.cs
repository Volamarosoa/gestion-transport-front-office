namespace GestionTransport.FrontOffice.Models.Utils
{
    public class PaginatedViewModel<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        public int StartItem => (CurrentPage - 1) * PageSize + 1;
        public int EndItem => Math.Min(CurrentPage * PageSize, TotalItems);

        // Pour générer les numéros de page à afficher
        public List<int> GetPageNumbers(int maxPagesToShow = 5)
        {
            var pages = new List<int>();
            
            int startPage = Math.Max(1, CurrentPage - maxPagesToShow / 2);
            int endPage = Math.Min(TotalPages, startPage + maxPagesToShow - 1);
            
            // Ajuster le startPage si on est près de la fin
            if (endPage - startPage < maxPagesToShow - 1)
            {
                startPage = Math.Max(1, endPage - maxPagesToShow + 1);
            }

            for (int i = startPage; i <= endPage; i++)
            {
                pages.Add(i);
            }

            return pages;
        }
    }
}