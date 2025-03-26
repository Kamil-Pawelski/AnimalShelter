using AnimalShelter.App.DTO;
using AnimalShelter.Domain.AnimalShelterEntities;
using AnimalShelter.Domain.Common;
using AnimalShelter.Domain.Constants;
using AnimalShelter.Domain.Repositores;
using MediatR;
using Serilog;
using System.Net;

namespace AnimalShelter.App.Queries;

public class GetAnimalsQuery : IRequest<OperationResult<List<AnimalDTO>>>
{
    public string UserRole { get; set; }

    public GetAnimalsQuery(string userRole)
    {
        UserRole = userRole;
    }
}

public class GetAnimalsQueryHandler : IRequestHandler<GetAnimalsQuery, OperationResult<List<AnimalDTO>>>
{
    private readonly IAnimalShelterRepository _animalShelterRepository;

    public GetAnimalsQueryHandler(IAnimalShelterRepository animalShelterRepository)
    {
        _animalShelterRepository = animalShelterRepository;
    }

    public async Task<OperationResult<List<AnimalDTO>>> Handle(GetAnimalsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var animalList =  new List<Animal>();
            if (request.UserRole == RolesConstants.User)
            {
                animalList = await _animalShelterRepository.GetAllAnimalsByStatus(AdoptionStatus.Available);
            }
            else if (request.UserRole == RolesConstants.Employee)
            {
                animalList = await _animalShelterRepository.GetAllAnimals();
            }
            else 
            {
                return new OperationResult<List<AnimalDTO>>
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "Please log in."
                };
            }

            return new OperationResult<List<AnimalDTO>>
            {
                Result = animalList.Select(animal => new AnimalDTO(animal)).ToList(),
                StatusCode = HttpStatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex.Message);
            return new OperationResult<List<AnimalDTO>>
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = ex.Message
            };
        }
    }
}