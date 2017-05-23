namespace Helios.Domain.Models
{
    public class Entity_Component : EntityBase
    {
        public int EntityId { get; set; }
        public string ComponentName { get; set; }

        public Entity Entity {get; set;}
    }
}