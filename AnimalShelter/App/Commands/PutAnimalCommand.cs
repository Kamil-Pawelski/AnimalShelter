using AnimalShelter.App.DTO;
using AnimalShelter.Domain;
using AnimalShelter.Domain.AnimalShelterEntities;
using AnimalShelter.Domain.Repositores;
using MediatR;
using Serilog;
using System.Net;

namespace AnimalShelter.App.Commands;

public class PutAnimalCommand : IRequest<OperationResult<AnimalDTO>>
{
    public int Id { get; private set; }
    public string Name { get; set; }
    public string Species { get; set; }
    public string Breed { get; set; }
    public int Age { get; set; }
    public int Weight { get; set; }
    public AdoptionStatus AdoptionStatus { get; set; }

    public PutAnimalCommand(string name, string species, string breed, int age, int weight, AdoptionStatus adoptionStatus)
    {
        Name = name;
        Species = species;
        Breed = breed;
        Age = age;
        Weight = weight;
        AdoptionStatus = adoptionStatus;
    }

    public void SetId(int id) => Id = id;
}

public class PutAnimalCommandHandler : IRequestHandler<PutAnimalCommand, OperationResult<AnimalDTO>>
{
    private readonly IAnimalShelterRepository _animalShelterRepository;

    public PutAnimalCommandHandler(IAnimalShelterRepository animalShelterRepository)
    {         
        _animalShelterRepository = animalShelterRepository;
    }


    public async Task<OperationResult<AnimalDTO>> Handle(PutAnimalCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var animal = await _animalShelterRepository.GetAnimalById(request.Id);

            if (animal == null)
            {
                return new OperationResult<AnimalDTO>()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = $"The animal with the given id does not exist."
                };
            }

            animal.Name = request.Name;
            animal.Species = request.Species;
            animal.Breed = request.Breed;
            animal.Age = request.Age;
            animal.Weight = request.Weight;
            animal.AdoptionStatus = request.AdoptionStatus;

            var updatedAnimal =  await _animalShelterRepository.UpdateAnimal(animal);

            return new OperationResult<AnimalDTO>
            {
                Result = new AnimalDTO(animal),
                StatusCode = HttpStatusCode.OK,
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
