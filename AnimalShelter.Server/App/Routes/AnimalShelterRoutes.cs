using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AnimalShelter.App.Routes;

public static class AnimalShelterRoutes
{
    public const string PostAnimal = "/animals";
    public const string GetAnimals = "/animals";
    public const string GetAnimal = "/animals/{id}";
    public const string PutAnimal = "/animals/{id}";
    public const string DeleteAnimal = "/animals/{id}";

    public const string PostAdoptAnimal = "/adopt";
    public const string GetAdoptedAnimals = "/adopted";
}
