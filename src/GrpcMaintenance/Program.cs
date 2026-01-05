using GrpcMaintenance.Services;
using GrpcMaintenance.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add Database

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=Mantenimiento.db"));

builder.Services.AddGrpc().AddJsonTranscoding();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
//Mapp GRPC services
app.MapGrpcService<MaintenanceGrpcService>();
app.MapGrpcService<DetailMaintenanceGrpcService>();
app.MapGrpcService<UserGrpcService>();
app.MapGrpcService<EquipmentGrpcService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
