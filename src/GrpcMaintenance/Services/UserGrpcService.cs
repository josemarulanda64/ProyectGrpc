using Grpc.Core;
using GrpcMaintenance.Data;
using GrpcMaintenance.Models;
using GrpcMaintenance.Protos.User;
using Microsoft.EntityFrameworkCore;

namespace GrpcMaintenance.Services;

public class UserGrpcService : UserService.UserServiceBase
{
    private readonly AppDbContext _context;

    public UserGrpcService(AppDbContext context)
    {
        _context = context;
    }

    // ✅ CREATE
    public override async Task<UserResponse> CreateUser(
        CreateUserRequest request,
        ServerCallContext context)
    {
        var user = new User
        {
            Name = request.Name,
            Role = request.Role
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Role = user.Role
        };
    }

    // ✅ GET BY ID
    public override async Task<UserResponse> GetUserById(
        GetUserByIdRequest request,
        ServerCallContext context)
    {
        var user = await _context.Users.FindAsync(request.Id);

        if (user == null)
        {
            throw new RpcException(
                new Status(StatusCode.NotFound, "Usuario no encontrado"));
        }

        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Role = user.Role
        };
    }

    // ✅ GET ALL
    public override async Task<UsersResponse> GetAllUsers(
        Google.Protobuf.WellKnownTypes.Empty request,
        ServerCallContext context)
    {
        var users = await _context.Users.ToListAsync();

        var response = new UsersResponse();
        response.Users.AddRange(users.Select(u => new UserResponse
        {
            Id = u.Id,
            Name = u.Name,
            Role = u.Role
        }));

        return response;
    }

    public override async Task<DeleteUserResponse> DeleteUser(
        DeleteUserRequest request,
        ServerCallContext context)
    {
        var user = await _context.Users.FindAsync(request.Id);

        if (user == null)
        {
            throw new RpcException(
                new Status(StatusCode.NotFound, "User not found"));
        }

        if (request.Id < 0)
        {
            throw new RpcException(
                new Status(StatusCode.NotFound, $"User with id : {request.Id} not exist" )
            );
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return await Task.FromResult(
            new DeleteUserResponse
            {
                Id = user.Id
            }
        );
    }
}
