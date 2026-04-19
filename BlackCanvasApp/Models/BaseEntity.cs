namespace BlackCanvasApp.Models
{
    public class BaseEntity
    {
        public int Id { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public bool IsDeleted { get; set; }

    }
}
