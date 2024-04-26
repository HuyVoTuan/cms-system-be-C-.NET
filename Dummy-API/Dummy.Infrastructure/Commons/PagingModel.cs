namespace Dummy.Infrastructure.Commons
{
    public class PagingModel<T>
    {
        public IEnumerable<T> ItemSource { get; set; }
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageLimit { get; set; }
        public PagingModel(IEnumerable<T> itemSource, int totalCount, int pageIndex, int pageLimit)
        {
            ItemSource = itemSource;
            TotalCount = totalCount;
            PageIndex = pageIndex;
            PageLimit = pageLimit;
        }
    }
}
