// File: DTOs/NotificationDtos.cs
namespace scan2pay.DTOs;
public class NotificationDto
{
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? RelatedEntityId { get; set; }
}
