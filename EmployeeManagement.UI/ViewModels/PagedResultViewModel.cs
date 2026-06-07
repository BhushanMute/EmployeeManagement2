namespace EmployeeManagement.UI.ViewModels
{
    /// <summary>
    /// Generic paged result wrapper for paginated API responses
    /// Used for: Team History, All Leave Requests, Employee Lists, etc.
    /// </summary>
    public class PagedResultViewModel<T>
    {
        // ===== Core Data =====
        public List<T> Items { get; set; } = new List<T>();

        // ===== Pagination Info =====
        public int TotalCount { get; set; }              // Total records in database
        public int PageNumber { get; set; } = 1;         // Current page (1-based)
        public int PageSize { get; set; } = 10;          // Records per page

        // ===== Computed Properties =====

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages =>
            PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

        /// <summary>
        /// Has previous page available
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// Has next page available
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// First page number
        /// </summary>
        public int FirstPage => 1;

        /// <summary>
        /// Last page number (same as TotalPages)
        /// </summary>
        public int LastPage => TotalPages;

        /// <summary>
        /// Previous page number
        /// </summary>
        public int PreviousPage => HasPreviousPage ? PageNumber - 1 : 1;

        /// <summary>
        /// Next page number
        /// </summary>
        public int NextPage => HasNextPage ? PageNumber + 1 : TotalPages;

        /// <summary>
        /// First item index on current page (1-based, for display)
        /// Example: "Showing 21 to 40 of 100"
        /// </summary>
        public int FirstItemIndex =>
            TotalCount == 0 ? 0 : ((PageNumber - 1) * PageSize) + 1;

        /// <summary>
        /// Last item index on current page (1-based, for display)
        /// </summary>
        public int LastItemIndex =>
            Math.Min(PageNumber * PageSize, TotalCount);

        /// <summary>
        /// Check if there's any data
        /// </summary>
        public bool HasItems => Items != null && Items.Any();

        /// <summary>
        /// Check if result is empty
        /// </summary>
        public bool IsEmpty => !HasItems;

        // ===== Helper Methods =====

        /// <summary>
        /// Get range of page numbers to display (for pagination UI)
        /// Example: If on page 5 with total 20 pages, returns [3,4,5,6,7]
        /// </summary>
        public IEnumerable<int> GetPageRange(int delta = 2)
        {
            if (TotalPages <= 1) return new List<int>();

            int start = Math.Max(1, PageNumber - delta);
            int end = Math.Min(TotalPages, PageNumber + delta);

            // Adjust to always show same number of pages if possible
            if (end - start < (delta * 2))
            {
                if (start == 1)
                    end = Math.Min(TotalPages, start + (delta * 2));
                else if (end == TotalPages)
                    start = Math.Max(1, end - (delta * 2));
            }

            var pages = new List<int>();
            for (int i = start; i <= end; i++)
            {
                pages.Add(i);
            }
            return pages;
        }

        /// <summary>
        /// Should show "First" page link (page 1)
        /// </summary>
        public bool ShouldShowFirstPage(int delta = 2) =>
            PageNumber - delta > 1;

        /// <summary>
        /// Should show "Last" page link (TotalPages)
        /// </summary>
        public bool ShouldShowLastPage(int delta = 2) =>
            PageNumber + delta < TotalPages;
    }
}