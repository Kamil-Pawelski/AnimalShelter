using AnimalShelter.App.DTO;
using AnimalShelter.Domain;
using AnimalShelter.Domain.Repositores;
using MediatR;
using Serilog;
using System.Net;

namespace AnimalShelter.App.Queries;

public class GetAnimalQuery : IRequest<OperationResult<AnimalDTO>>
{
    public int Id { get; set; }

    public GetAnimalQuery(int id)
    {
        Id = id;
    }
}

public class GetAnimalQueryHandler : IRequestHandler<GetAnimalQuery, OperationResult<AnimalDTO>>
{
    private readonly IAnimalShelterRepository _animalShelterRepository;

    public GetAnimalQueryHandler(IAnimalShelterRepository animalShelterRepository)
    {
        _animalShelterRepository = animalShelterRepository;
    }

    public async Task<OperationResult<AnimalDTO>> Handle(GetAnimalQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var animal = await _animalShelterRepository.GetAnimalById(request.Id);

            if (animal == null)
            {
                return new OperationResult<AnimalDTO>()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "The animal with the given ID does not exist."
                };
            }

            return new OperationResult<AnimalDTO>()
            {
                Result = new AnimalDTO(animal),
                StatusCode = HttpStatusCode.OK
            };
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex.Message);
            return new OperationResult<AnimalDTO>
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = ex.Message
            };
        }
    }
}