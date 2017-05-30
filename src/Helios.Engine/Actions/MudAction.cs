namespace Helios.Engine.Actions
{
    public class MudAction
    {
        public string[] Args { get; }
        public int SenderId { get; }
        public int ReceiverId { get; }
        public int OtherEntity1 { get; }
        public int OtherEntity2 { get; }
        public string Type { get; }
        public string Value { get; set; }

        public MudAction() { }
        public MudAction(string type, int senderId, params string[] args) : this(type, senderId, 0, 0, 0, args) { }
        public MudAction(string type, int senderId, int receiverId, params string[] args) : this(type, senderId, receiverId, 0, 0, args) { }
        public MudAction(string type, int senderId, int receiverId, int other1, params string[] args) : this(type, senderId, receiverId, other1, 0, args) { }
        public MudAction(string type, int senderId, int receiverId, int other1, int other2, params string[] args)
        {
            Args = args;
            SenderId = senderId;
            ReceiverId = receiverId;
            Type = type;
            OtherEntity1 = other1;
            OtherEntity2 = other2;
        }
    }
}