// File: Controllers/NotificationsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using scan2pay.DTOs;
using scan2pay.Interfaces;
using System.Security.Claims;

namespace scan2pay.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(INotificationService notificationService, ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    [HttpGet("my-notifications")]
    [ProducesResponseType(typeof(IEnumerable<NotificationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyNotifications(
        [FromQuery] bool unreadOnly = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        pageSize = Math.Min(pageSize, 50); // Limiter la taille de page
        var notifications = await _notificationService.GetUserNotificationsAsync(userId, unreadOnly, pageNumber, pageSize);
        return Ok(notifications);
    }

    [HttpPut("{id}/mark-read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkNotificationAsRead(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var success = await _notificationService.MarkNotificationAsReadAsync(id, userId);
        if (!success) return NotFound(new { message = "Notification non trouvée ou non autorisée." });
        return NoContent();
    }

    [HttpPut("mark-all-read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkAllNotificationsAsRead()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        await _notificationService.MarkAllUserNotificationsAsReadAsync(userId);
        return NoContent();
    }
}
