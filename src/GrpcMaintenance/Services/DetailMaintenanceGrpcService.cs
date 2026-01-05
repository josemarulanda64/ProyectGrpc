using Grpc.Core;
using GrpcMaintenance.Data;
using GrpcMaintenance.Models;
using GrpcMaintenance.Protos.DetailMaintenance;
using Microsoft.EntityFrameworkCore;

namespace GrpcMaintenance.Services;

public class DetailMaintenanceGrpcService
    : DetailMaintenanceService.DetailMaintenanceServiceBase
{
    private readonly AppDbContext _context;

    public DetailMaintenanceGrpcService(AppDbContext context)
    {
        _context = context;
    }

    // ✅ CREATE
    public override async Task<DetailResponse> CreateDetail(
        CreateDetailRequest request,
        ServerCallContext context)
    {
        var orderExists = await _context.MaintenanceOrders
            .AnyAsync(o => o.Id == request.MaintenanceOrderId);

        if (!orderExists)
        {
            throw new RpcException(
                new Status(StatusCode.NotFound, "La orden de mantenimiento no existe"));
        }

        var detail = new DetailMaintenance
        {
            MaintenanceOrderId = request.MaintenanceOrderId,
            Description = request.Description,
            Date = DateTime.Parse(request.Date)
        };

        _context.DetailMaintenances.Add(detail);
        await _context.SaveChangesAsync();

        return new DetailResponse
        {
            Id = detail.Id,
            MaintenanceOrderId = detail.MaintenanceOrderId,
            Description = detail.Description,
            Date = detail.Date.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    // ✅ GET BY ORDER (🔥 CORREGIDO)
    public override async Task<DetailsResponse> GetDetailsByOrder(
        GetDetailsByOrderRequest request,
        ServerCallContext context)
    {
        var details = await _context.DetailMaintenances
            .Where(d => d.MaintenanceOrderId == request.MaintenanceOrderId)
            .ToListAsync();

        var response = new DetailsResponse();

        response.Details.AddRange(details.Select(d => new DetailResponse
        {
            Id = d.Id,
            MaintenanceOrderId = d.MaintenanceOrderId,
            Description = d.Description,
            Date = d.Date.ToString("yyyy-MM-dd HH:mm:ss")
        }));

        return response;
    }
}
