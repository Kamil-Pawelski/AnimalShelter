using AnimalShelter.App.DTO;
using AnimalShelter.Domain.Common;
using AnimalShelter.Domain.Repositores;
using MediatR;
using Serilog;
using System.Net;

namespace AnimalShelter.App.Queries;

public class GetAdoptedAnimalsQuery : IRequest<OperationResult<List<AdoptedAnimalDTO>>>
{
}

public class GetAdoptedAnimalsQueryHandler : IRequestHandler<GetAdoptedAnimalsQuery, OperationResult<List<AdoptedAnimalDTO>>>
{
    private readonly IAnimalShelterRepository _animalShelterRepository;

    public GetAdoptedAnimalsQueryHandler(IAnimalShelterRepository animalShelterRepository)
    {
        _animalShelterRepository = animalShelterRepository;
    }

    public async Task<OperationResult<List<AdoptedAnimalDTO>>> Handle(GetAdoptedAnimalsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var adoptedAnimals = await _animalShelterRepository.GetAdoptedAnimals();

            return new OperationResult<List<AdoptedAnimalDTO>>
            {
                StatusCode = HttpStatusCode.OK,
                Result = adoptedAnimals
            };
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex.Message);
            return new OperationResult<List<AdoptedAnimalDTO>>
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = ex.Message,
            };
        }
    }
}
