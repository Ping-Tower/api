namespace Domain.Common.Interfaces;

public interface IHasSoftDelete
{
    bool IsDeleted {get; set;}
}