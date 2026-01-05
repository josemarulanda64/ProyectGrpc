using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcMaintenance.Data;
using GrpcMaintenance.Models;
using GrpcMaintenance.Protos;
using GrpcMaintenance.Protos.Maintenance;
using Microsoft.EntityFrameworkCore;

namespace GrpcMaintenance.Services
{
    public class MaintenanceGrpcService : Protos.Maintenance.MaintenanceService.MaintenanceServiceBase
    {
        private readonly AppDbContext _context;

        public MaintenanceGrpcService(AppDbContext context)
        {
            _context = context;
        }

        public override async Task<OrderResponse> CreateOrder(
            CreateOrderRequest request,
            ServerCallContext context)
        {
            var order = new MaintenanceOrder
            {
                UserId = request.UserId,
                EquipmentId = request.EquipmentId,
                DateCreated = DateTime.UtcNow,
                State = "OPEN"
            };

            _context.MaintenanceOrders.Add(order);
            await _context.SaveChangesAsync();

            return new OrderResponse
            {
                Id = order.Id,
                UserId = order.UserId,
                EquipmentId = order.EquipmentId,
                State = order.State,
                DateCreated = order.DateCreated.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }

        public override async Task<OrdersResponse> GetOrders(
     Empty request,
     ServerCallContext context)
        {
            var orders = await _context.MaintenanceOrders
                .Include(o => o.User)
                .Include(o => o.Equipment)
                .ToListAsync();

            var response = new OrdersResponse();

            response.Orders.AddRange(
                orders.Select(o => new OrderResponse
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    EquipmentId = o.EquipmentId,
                    State = o.State,
                    DateCreated = o.DateCreated.ToString("yyyy-MM-dd HH:mm:ss"),
                    UserName = o.User?.Name ?? "",
                    EquipmentName = o.Equipment?.Name ?? ""
                })
            );

            return response;
        }

    }
}
