using AnimalShelter.Domain.Common;
using AnimalShelter.Domain.Constants;
using AnimalShelter.Domain.Repositores;
using AnimalShelter.Domain.UserEntities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System.Net;

namespace AnimalShelter.App.Commands;

public class RegisterCommand : IRequest<OperationResult>
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }

    public RegisterCommand(string username, string password, string email)
    {
        Username = username;
        Password = password;
        Email = email;
    }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, OperationResult>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    public RegisterCommandHandler(IAccountRepository accountRepository, IPasswordHasher<User> passwordHasher)
    {
        _accountRepository = accountRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<OperationResult> Handle(RegisterCommand request, CancellationToken token)
    {
        try
        {
            var existingUser = await _accountRepository.GetUserByEmail(request.Email);

            if (existingUser != null)
            {
                return new OperationResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "User with this email already exists."
                };
            }

            existingUser = await _accountRepository.GetUserByUsername(request.Username);

            if (existingUser != null)
            {
                return new OperationResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "User with this username already exists."
                };
            }

            var newUser = new User
            {
                Username = request.Username,
                Email = request.Email,
            };

            newUser.Password = _passwordHasher.HashPassword(newUser, request.Password);

            await _accountRepository.AddUser(newUser);
            var userRole = await _accountRepository.GetRole(RolesConstants.User);

            if (userRole != null)
            {
                var userRoleAssignment = new UserRole
                {
                    UserId = newUser.Id,
                    RoleId = userRole.Id
                };
                await _accountRepository.AddDefaultRole(userRoleAssignment);
            }

            return new OperationResult
            {
                StatusCode = HttpStatusCode.OK,
                Message = "User registered successfully."
            };
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex.Message);
            return new OperationResult
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = ex.Message
            };
        }        
    }
}
