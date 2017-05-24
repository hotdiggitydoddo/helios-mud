namespace Helios.Domain.Models
{
    public class Entity_Command : EntityBase
    {
        public int EntityId { get; set; }
        public string Command { get; set; }
    }
}