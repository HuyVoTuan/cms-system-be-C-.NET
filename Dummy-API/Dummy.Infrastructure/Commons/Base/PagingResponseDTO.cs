namespace Dummy.Infrastructure.Commons.Base
{
    public class PagingResponseDTO<T>
    {
        public int PageIndex { get; set; }
        public int PageLimit { get; set; }
        public int Total { get; set; }
        public List<T> Data { get; set; }
    }
}
