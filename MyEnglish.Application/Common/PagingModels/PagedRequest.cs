namespace MyEnglish.Application.Common.PagingModels
{
    public class PagedRequest
    {
        public int? PageSize { get; set; }
        public int? PageNumberIndex { get; set; }
        public string? SearchText { get; set; }
        public Dictionary<string, SortDirection>? Sorted { get; set; }
    }

    public enum SortDirection
    {
        Ascending = 0,
        Descending = 1
    }
}
