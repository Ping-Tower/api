using Domain.Common.Interfaces;

namespace Domain.Entities;

public class Request : BaseEntity
{
    public string? Query { get; set; }
    public string? ServerId { get; set; }
    public Server? ServerRef { get; set; }
}