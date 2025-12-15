using Microsoft.EntityFrameworkCore;

public class PaginatedList<T> : List<T> {
    public int PageIndex { get; private set; }
    public int TotalPages { get; private set; }
    public int TotalCount { get; private set; }
    public int PageSize { get; private set; }

    public List<int> PageNumbers { get; private set; }

    public PaginatedList(List<T> items, int count, int pageIndex, int pageSize) {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        PageSize = pageSize;

        GeneratePageNumbers();

        this.AddRange(items);
    }

    private void GeneratePageNumbers() {
        int current = PageIndex;
        int total = TotalPages;
        int delta = 2; 
        int left = current - delta;
        int right = current + delta + 1;

        var pages = new List<int>();
        var pageNumbers = new List<int>();

        for (int i = 1; i <= total; i++) {
            if (i == 1 || i == total || (i >= left && i < right)) {
                pages.Add(i);
            }
        }

        int? previous = null;
        foreach (var page in pages) {
            if (previous != null) {
                if (page - previous == 2) {
                    pageNumbers.Add(previous.Value + 1);
                } else if (page - previous != 1) {
                    pageNumbers.Add(-1); 
                }
            }
            pageNumbers.Add(page);
            previous = page;
        }

        PageNumbers = pageNumbers;
    }

    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public static PaginatedList<T> Create(List<T> source, int pageIndex, int pageSize) {
        var count = source.Count();
        var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize) {
        var count = await source.CountAsync();
        var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }
}