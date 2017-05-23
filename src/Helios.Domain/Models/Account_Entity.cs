namespace Helios.Domain.Models
{
    public class Account_Entity
    {
        public int AccountId { get; set; }
        public int EntityId { get; set; }

        public Account Account { get; set; }
        public Entity Entity { get; set; }
    }
}