// File: Repositories/QrCodeRepository.cs
using Microsoft.EntityFrameworkCore;
using scan2pay.Data;
using scan2pay.Models;
using scan2pay.Interfaces;
namespace scan2pay.Repositories;

public class QrCodeRepository : GenericRepository<QrCode>, IQrCodeRepository
{
    public QrCodeRepository(ApplicationDbContext context) : base(context) { }
    public async Task<QrCode?> GetByMarchandIdAsync(string marchandId) =>
        await _dbSet.FirstOrDefaultAsync(q => q.MarchandId == marchandId);
    public async Task<QrCode?> GetByQrCodeDataAsync(string qrCodeData) =>
        await _dbSet.FirstOrDefaultAsync(q => q.QrCodeData == qrCodeData);
}