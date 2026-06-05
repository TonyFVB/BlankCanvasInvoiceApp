namespace BlankCanvasApp.Domain.Common
{
    public class BaseEntity
    {
        public int Id { get; set; }

        public DateTimeOffset CreationTime { get; set; }

        public DateTimeOffset? LastModificationTime { get; set; }

        public bool IsDeleted { get; set; }

    }
}
