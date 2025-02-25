using AnimalShelter.App.DTO;
using AnimalShelter.Domain.AnimalShelterEntities;
using AnimalShelter.Domain.Common;
using AnimalShelter.Domain.Repositores;
using MediatR;
using Serilog;
using System.Net;

namespace AnimalShelter.App.Commands;

public class PostAnimalCommand : IRequest<OperationResult<AnimalDTO>>
{
    public PostAnimalCommand(string name, string species, string breed, int age, int weight)
    {
        Name = name;
        Species = species;
        Breed = breed;
        Age = age;
        Weight = weight;
    }

    public string Name { get; set; }
    public string Species { get; set; }
    public string Breed { get; set; }
    public int Age { get; set; }
    public int Weight { get; set; }
}

public class PostAnimalCommandHandler : IRequestHandler<PostAnimalCommand, OperationResult<AnimalDTO>>
{
    private readonly IAnimalShelterRepository _animalShelterRepository;

    public PostAnimalCommandHandler(IAnimalShelterRepository animalShelterRepository)
    {
        _animalShelterRepository = animalShelterRepository;
    }

    public async Task<OperationResult<AnimalDTO>> Handle(PostAnimalCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var animal = new Animal()
            {
                Name = request.Name,
                Species = request.Species,
                Breed = request.Breed,
                Age = request.Age,
                Weight = request.Weight,
                AdoptionStatus = AdoptionStatus.Available
            };

            await _animalShelterRepository.AddAnimal(animal);

            return new OperationResult<AnimalDTO>
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