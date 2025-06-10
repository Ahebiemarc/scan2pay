// File: Interfaces/INotificationService.cs
using scan2pay.DTOs;
using scan2pay.Models;
namespace scan2pay.Interfaces;
public interface INotificationService
{
    Task CreateNotificationAsync(string userId, string message, NotificationType type, string? relatedEntityId = null);
    Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId, bool unreadOnly = false, int pageNumber = 1, int pageSize = 10);
    Task<bool> MarkNotificationAsReadAsync(Guid notificationId, string userId);
    Task<bool> MarkAllUserNotificationsAsReadAsync(string userId);
}