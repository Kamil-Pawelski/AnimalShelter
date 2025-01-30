using AnimalShelter.App.Commands;
using AnimalShelter.App.Routes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AnimalShelter.App.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IMediator _mediator;
    public AccountController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost(AccountRoutes.Register)]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        var result = await _mediator.Send(command);

        if(result.StatusCode != HttpStatusCode.OK) 
        {
            return StatusCode((int)result.StatusCode, result.Message);
        }

        return Ok(result.Message);
    }

    [HttpPost(AccountRoutes.Login)]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.StatusCode != HttpStatusCode.OK)
        {
            return StatusCode((int)result.StatusCode, result.Message);
        }

        return Ok(result.Result);
    }
}
