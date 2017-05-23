namespace Helios.Engine.Objects
{
    public class MudTrait
    {
        public string Name { get; }
        public string Value { get; set; }

        public MudTrait(string name)
        {
            Name = name;
        }

        public MudTrait(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}