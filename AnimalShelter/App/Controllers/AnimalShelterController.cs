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

        return Ok(result.Result);
    }

    [HttpGet(AnimalShelterRoutes.GetAnimals)]
    [Authorize]
    public async Task<IActionResult> GetAnimals()
    {
        var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

        var result = await _mediator.Send(new GetAnimalsQuery(userRole));

        if (result.StatusCode != HttpStatusCode.OK)
        {
            return StatusCode((int)result.StatusCode, result.Message);
        }

        return Ok(result.Result);
    }

    [HttpGet(AnimalShelterRoutes.GetAnimal)]
    [Authorize]
    public async Task<IActionResult> GetAnimal([FromRoute] int id)
    {
        var command = new GetAnimalQuery(id);
        var result = await  _mediator.Send(command);

        if (result.StatusCode != HttpStatusCode.OK)
        {
            return StatusCode((int)result.StatusCode, result.Message);
        }

        return Ok(result.Result);
    }

    [HttpPut(AnimalShelterRoutes.PutAnimal)]
    [Authorize(Roles = RolesConstants.Employee)]
    public async Task<IActionResult> PutAnimal([FromRoute] int id, [FromBody] PutAnimalCommand command)
    {
        command.SetId(id);
        var result = await _mediator.Send(command);

        if (result.StatusCode != HttpStatusCode.OK)
        {
            return StatusCode((int)result.StatusCode, result.Message);
        }

        return Ok(result.Result);
    }


    [HttpDelete(AnimalShelterRoutes.DeleteAnimal)]
    [Authorize(Roles = RolesConstants.Employee)]
    public async Task<IActionResult> DeleteAnimal([FromRoute] int id)
    {
        var command = new DeleteAnimalCommand(id);

        var result = await _mediator.Send(command);

        if (result.StatusCode != HttpStatusCode.OK)
        {
            return StatusCode((int)result.StatusCode, result.Message);
        }

        return Ok(result.Message);
    }

    [HttpPost(AnimalShelterRoutes.PostAdoptAnimal)]
    [Authorize]
    public async Task<IActionResult> PostAdoptAnimal(PostAdoptAnimalCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.StatusCode != HttpStatusCode.OK)
        {
            return StatusCode((int)result.StatusCode, result.Message);
        }

        return Ok(result.Message);
    }

    [HttpGet(AnimalShelterRoutes.GetAdoptedAnimals)]
    [Authorize(Roles = RolesConstants.Employee)]
    public async Task<IActionResult> GetAdoptedAnimals()
    {
        var command =new GetAdoptedAnimalsQuery();

        var result = await _mediator.Send(command);

        if (result.StatusCode != HttpStatusCode.OK)
        {
            return StatusCode((int)result.StatusCode, result.Message);
        }

        return Ok(result.Result);
    }
}
