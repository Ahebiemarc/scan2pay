// File: Services/NotificationService.cs
using AutoMapper;
using scan2pay.DTOs;
using scan2pay.Interfaces;
using scan2pay.Models;

namespace scan2pay.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(INotificationRepository notificationRepository, IMapper mapper, ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task CreateNotificationAsync(string userId, string message, NotificationType type, string? relatedEntityId = null)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(message))
        {
            _logger.LogWarning("Attempted to create notification with missing userId or message.");
            return;
        }

        var notification = new Notification
        {
            ApplicationUserId = userId,
            Message = message,
            Type = type,
            RelatedEntityId = relatedEntityId,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        await _notificationRepository.AddAsync(notification);
        var success = await _notificationRepository.SaveChangesAsync() > 0;
        if(success)
        {
            _logger.LogInformation($"Notification created for user {userId}, type {type}.");
            // TODO: Envoyer une notification push réelle ici si configuré (e.g., via Firebase Cloud Messaging, SignalR)
        }
        else
        {
            _logger.LogError($"Failed to save notification for user {userId}.");
        }
    }

    public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId, bool unreadOnly = false, int pageNumber = 1, int pageSize = 10)
    {
        var notifications = await _notificationRepository.GetNotificationsByUserIdAsync(userId, unreadOnly, pageNumber, pageSize);
        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public async Task<bool> MarkNotificationAsReadAsync(Guid notificationId, string userId)
    {
        await _notificationRepository.MarkAsReadAsync(notificationId, userId);
        return await _notificationRepository.SaveChangesAsync() > 0;
    }

    public async Task<bool> MarkAllUserNotificationsAsReadAsync(string userId)
    {
        await _notificationRepository.MarkAllAsReadAsync(userId);
         // ExecuteUpdateAsync effectue le SaveChanges.
         // Pour confirmer que quelque chose a été fait, on pourrait vérifier le nombre de lignes affectées,
         // mais la méthode du repo ne le retourne pas directement. On suppose que si pas d'erreur, c'est ok.
        return true;
    }
}