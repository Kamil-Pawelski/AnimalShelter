using AnimalShelter.App.Commands;
using AnimalShelter.App.Queries;
using AnimalShelter.App.Routes;
using AnimalShelter.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace AnimalShelter.App.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnimalShelterController : ControllerBase
{
    private readonly IMediator _mediator;
    public AnimalShelterController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost(AnimalShelterRoutes.PostAnimal)]
    [Authorize(Roles = RolesConstants.Employee)]
    public async Task<IActionResult> PostAnimal(PostAnimalCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.StatusCode != HttpStatusCode.OK)
        {
            return StatusCode((int)result.StatusCode, result.Message);
        }

        return Ok(result.Message);
    }

    [HttpGet(AnimalShelterRoutes.GetAnimals)]
    [Authorize]
    public async Task<IActionResult> GetAnimals()
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        var result = await _mediator.Send(new GetAnimalsQuery(userRole));

        if (result.StatusCode != HttpStatusCode.OK)
        {
            return StatusCode((int)result.StatusCode, result.Message);
        }

        return Ok(result.Message);
    }
}
