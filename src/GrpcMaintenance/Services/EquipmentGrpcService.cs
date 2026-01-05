using Grpc.Core;
using GrpcMaintenance.Data;
using GrpcMaintenance.Models;
using GrpcMaintenance.Protos.Equipment;
using Microsoft.EntityFrameworkCore;

namespace GrpcMaintenance.Services;

public class EquipmentGrpcService : EquipmentService.EquipmentServiceBase
{
    private readonly AppDbContext _context;

    public EquipmentGrpcService(AppDbContext context)
    {
        _context = context;
    }

    // ✅ CREATE
    public override async Task<EquipmentResponse> CreateEquipment(
        CreateEquipmentRequest request,
        ServerCallContext context)
    {
        var equipment = new Equipment
        {
            Name = request.Name,
            SerialNumber = request.SerialNumber
        };

        _context.Equipments.Add(equipment);
        await _context.SaveChangesAsync();

        return new EquipmentResponse
        {
            Id = equipment.Id,
            Name = equipment.Name,
            SerialNumber = equipment.SerialNumber
        };
    }

    // ✅ GET BY ID
    public override async Task<EquipmentResponse> GetEquipmentById(
        GetEquipmentByIdRequest request,
        ServerCallContext context)
    {
        var equipment = await _context.Equipments.FindAsync(request.Id);

        if (equipment == null)
        {
            throw new RpcException(
                new Status(StatusCode.NotFound, "Equipo no encontrado"));
        }

        return new EquipmentResponse
        {
            Id = equipment.Id,
            Name = equipment.Name,
            SerialNumber = equipment.SerialNumber
        };
    }

    // ✅ GET ALL
    public override async Task<EquipmentsResponse> GetAllEquipments(
        Google.Protobuf.WellKnownTypes.Empty request,
        ServerCallContext context)
    {
        var equipments = await _context.Equipments.ToListAsync();

        var response = new EquipmentsResponse();
        response.Equipments.AddRange(
            equipments.Select(e => new EquipmentResponse
            {
                Id = e.Id,
                Name = e.Name,
                SerialNumber = e.SerialNumber
            })
        );

        return response;
    }
}
