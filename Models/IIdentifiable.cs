namespace GameManager.Models
{
    // Simple interface to show that every object has an Id.
    public interface IIdentifiable
    {
        Guid Id { get; }
    }
}
