// File: Repositories/NotificationRepository.cs
using Microsoft.EntityFrameworkCore;
using scan2pay.Data;
using scan2pay.Models;
using scan2pay.Interfaces;
namespace scan2pay.Repositories;
public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    public NotificationRepository(ApplicationDbContext context) : base(context) { }
    public async Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(string userId, bool unreadOnly = false, int pageNumber = 1, int pageSize = 10)
    {
        var query = _dbSet.Where(n => n.ApplicationUserId == userId);
        if (unreadOnly) query = query.Where(n => !n.IsRead);
        return await query.OrderByDescending(n => n.CreatedAt)
                          .Skip((pageNumber - 1) * pageSize)
                          .Take(pageSize)
                          .ToListAsync();
    }
    public async Task MarkAsReadAsync(Guid notificationId, string userId)
    {
        var notification = await _dbSet.FirstOrDefaultAsync(n => n.Id == notificationId && n.ApplicationUserId == userId);
        if (notification != null)
        {
            notification.IsRead = true;
            _dbSet.Update(notification);
        }
    }
    public async Task MarkAllAsReadAsync(string userId)
    {
        await _dbSet.Where(n => n.ApplicationUserId == userId && !n.IsRead)
                    .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
    }
}
