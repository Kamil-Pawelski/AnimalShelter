using AnimalShelter.Constants;
using AnimalShelter.Domain;
using AnimalShelter.Domain.Repositores;
using AnimalShelter.Domain.UserEntities;
using AnimalShelter.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace AnimalShelter.App.Commands;

public class LoginCommand : IRequest<OperationResult<JwtSecurityToken>>
{
    public string Username { get; set; }
    public string Password { get; set; }
    public LoginCommand(string username, string password)
    {
        Username = username;
        Password = password;
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, OperationResult<JwtSecurityToken>>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly JWTService _jwtService;
    public LoginCommandHandler(IAccountRepository accountRepository, IPasswordHasher<User> passwordHasher, JWTService jwtService)
    {
        _accountRepository = accountRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<OperationResult<JwtSecurityToken>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _accountRepository.GetUserByUsername(request.Username);

            if (user == null)
            {
                return new OperationResult<JwtSecurityToken>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "User with this email already exists."
                };
            }

            var passwordVerify = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);

            if (passwordVerify != PasswordVerificationResult.Success)
            {
                return new OperationResult<JwtSecurityToken>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Incorrect password."
                };
            }

            var jwtToken = await _jwtService.GenerateJwtToken(user);

            return new OperationResult<JwtSecurityToken>
            {
                StatusCode = HttpStatusCode.OK,
                Result = jwtToken
            };
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex.Message);
            return new OperationResult<JwtSecurityToken>
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = ex.Message
            };
        }
    }
}



