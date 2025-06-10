// File: Interfaces/INotificationRepository.cs
using scan2pay.Models;
namespace scan2pay.Interfaces;
public interface INotificationRepository : IGenericRepository<Notification>
{
    Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(string userId, bool unreadOnly = false, int pageNumber = 1, int pageSize = 10);
    Task MarkAsReadAsync(Guid notificationId, string userId);
    Task MarkAllAsReadAsync(string userId);
}