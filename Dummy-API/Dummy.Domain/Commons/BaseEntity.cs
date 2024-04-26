
namespace Dummy.Domain.Commons
{
    public class BaseEntity<T> : IAuditEntity
    {
        public T Id { get; set; }
        public DateTime CreatedDate { get; set ; }
        public DateTime UpdatedDate { get ; set ; }
    }
}
