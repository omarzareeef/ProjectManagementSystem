namespace PMS.Domain.Entities;

public class BaseEntity
{
    public Guid Id { get; set; }

    public Guid CreatedById { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid? ModifiedById { get; set; }

    public DateTime? ModifiedAt { get; set; }
}