using System.Net;

namespace AnimalShelter.Domain.Common;

public class OperationResult
{
    public HttpStatusCode StatusCode { get; set; }
    public string? Message { get; set; }
}

public class OperationResult<T>
{
    public HttpStatusCode StatusCode { get; set; }
    public string? Message { get; set; }
    public T? Result { get; set; } 
}
