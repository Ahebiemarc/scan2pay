// File: Controllers/PaymentMethodsController.cs
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using scan2pay.DTOs;
using scan2pay.Interfaces;
using scan2pay.Models;
using System.Security.Claims;

namespace scan2pay.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PaymentMethodsController : ControllerBase
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PaymentMethodsController> _logger;

    public PaymentMethodsController(IPaymentMethodRepository paymentMethodRepository, IMapper mapper, ILogger<PaymentMethodsController> logger)
    {
        _paymentMethodRepository = paymentMethodRepository;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(PaymentMethodDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddPaymentMethod(CreatePaymentMethodDto createDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        // Simulation: Dans un vrai scénario, le TokenFromGateway serait validé avec la passerelle
        // et les détails (MaskedIdentifier, ExpiryDate) seraient récupérés de la passerelle.
        var paymentMethod = _mapper.Map<PaymentMethod>(createDto);
        paymentMethod.ApplicationUserId = userId;
        // Pour la simulation, on utilise des valeurs génériques si non fournies par un vrai token
        paymentMethod.MaskedIdentifier = $"{createDto.Provider.ToUpper()} **** {new Random().Next(1000, 9999)}";
        if (createDto.Type.ToLower() == "card" && string.IsNullOrEmpty(paymentMethod.ExpiryDate))
        {
            paymentMethod.ExpiryDate = "12/99"; // Placeholder
        }


        await _paymentMethodRepository.AddAsync(paymentMethod);
        await _paymentMethodRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMyPaymentMethods), new { id = paymentMethod.Id }, _mapper.Map<PaymentMethodDto>(paymentMethod));
    }

    [HttpGet("my-methods")]
    [ProducesResponseType(typeof(IEnumerable<PaymentMethodDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyPaymentMethods()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var methods = await _paymentMethodRepository.GetPaymentMethodsByUserIdAsync(userId);
        return Ok(_mapper.Map<IEnumerable<PaymentMethodDto>>(methods));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePaymentMethod(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var method = await _paymentMethodRepository.GetByIdAsync(id);
        if (method == null || method.ApplicationUserId != userId)
        {
            return NotFound(new { message = "Méthode de paiement non trouvée ou non autorisée." });
        }

        _paymentMethodRepository.Delete(method);
        await _paymentMethodRepository.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id}/set-default")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetDefaultPaymentMethod(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var methods = await _paymentMethodRepository.GetPaymentMethodsByUserIdAsync(userId);
        var targetMethod = methods.FirstOrDefault(m => m.Id == id);

        if (targetMethod == null)
        {
            return NotFound(new { message = "Méthode de paiement non trouvée." });
        }

        foreach (var method in methods)
        {
            method.IsDefault = (method.Id == id);
            _paymentMethodRepository.Update(method);
        }
        await _paymentMethodRepository.SaveChangesAsync();
        return NoContent();
    }
}