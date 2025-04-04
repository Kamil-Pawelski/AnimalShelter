# Animal Shelter

The Animal Shelter application is designed to manage and facilitate the adoption process of animals.

# Built with

- ASP.NET Core
- Entity Framework
- SQLServer
- XUnit
- Mvc.Testing
- Mediatr
- JWT
- Visual Studio 2022

# API Documentation

| **Method** | **Endpoint**      | **Parameters**                        | **Data**                                           | **Description**                  |
| ---------- | ----------------- | ------------------------------------- | -------------------------------------------------- | -------------------------------- |
| POST       | /api/register     | -                                     | username, email, password                          | Register a new user              |
| POST       | /api/login        | -                                     | username, password                                 | Log in an existing user          |
| GET        | /api/animals      | -                                     | -                                                  | Get a list of all animals        |
| GET        | /api/animals/{id} | (Integer) id                          | -                                                  | Get details of a specific animal |
| POST       | /api/animals      | (Integer) id                          | name, species, breed, age, weight                  | Create a new animal              |
| PUT        | /api/animals/{id} | (Integer) id                          | name, species, breed, age, weight, adopotionStatus | Update an existing animal        |
| DELETE     | /api/animals/{id} | (Integer) id                          | -                                                  | Delete a specific animal         |
| Post       | /api/adopt        | (Integer) UserId, (Integer) AnimalId, | -                                                  | Adopt an animal                  |
