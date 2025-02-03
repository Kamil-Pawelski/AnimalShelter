using AnimalShelter.Domain;
using AnimalShelter.Domain.AnimalShelterEntities;
using AnimalShelter.Domain.Repositores;
using MediatR;
using Serilog;
using System.Net;

namespace AnimalShelter.App.Commands;

public class PostAdoptAnimalCommand : IRequest<OperationResult>
{
    public PostAdoptAnimalCommand(int userId, int animalId)
    {
        UserId = userId;
        AnimalId = animalId;
    }

    public int UserId { get; set; }
    public int AnimalId { get; set; } 
}

public class PostAdoptAnimalCommandHandler : IRequestHandler<PostAdoptAnimalCommand, OperationResult>
{
    private readonly IAnimalShelterRepository _animalShelterRepository;

    public PostAdoptAnimalCommandHandler(IAnimalShelterRepository animalShelterRepository)
    {
        _animalShelterRepository = animalShelterRepository;
    }

    public async Task<OperationResult> Handle(PostAdoptAnimalCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var animal = await _animalShelterRepository.GetAnimalById(request.AnimalId);
            if (animal == null)
            {
                return new OperationResult
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "The animal with the given ID does not exist."
                };
            }

            if (animal.AdoptionStatus == AdoptionStatus.Adopted)
            {
                return new OperationResult
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Message = "The animal is already adopted."
                };
            }

            var animalAdopted = new AnimalAdopted
            {
                AnimalId = request.AnimalId,
                UserId = request.UserId,
                AdoptionDate = DateTime.UtcNow
            };

            await _animalShelterRepository.AdoptAnimal(animalAdopted);

            return new OperationResult
            {
                StatusCode = HttpStatusCode.OK,
                Message = $"Animal {animal.Name} has been adopted by user {request.UserId}."
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