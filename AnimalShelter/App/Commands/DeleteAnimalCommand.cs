using AnimalShelter.Domain;
using AnimalShelter.Domain.Repositores;
using MediatR;
using Serilog;
using System.Net;

namespace AnimalShelter.App.Commands;

public class DeleteAnimalCommand : IRequest<OperationResult>
{
    public int Id { get; private set; }
    public DeleteAnimalCommand(int id)
    {
        Id = id;
    }
}

public class DeleteAnimalCommandHandler : IRequestHandler<DeleteAnimalCommand, OperationResult>
{
    private readonly IAnimalShelterRepository _animalShelterRepository;

    public DeleteAnimalCommandHandler(IAnimalShelterRepository animalShelterRepository)
    {
        _animalShelterRepository = animalShelterRepository;
    }

    public async Task<OperationResult> Handle(DeleteAnimalCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var animal = await _animalShelterRepository.GetAnimalById(request.Id);
            if (animal == null)
            {
                return new OperationResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "The animal with the given id does not exist."
                };
            }    
            
            await _animalShelterRepository.DeleteAnimal(animal);

            return new OperationResult
            {
                StatusCode = HttpStatusCode.OK,
                Message = $"Animal with id {request.Id} has been removed from the shelter."
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
