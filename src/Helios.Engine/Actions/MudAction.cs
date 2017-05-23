namespace Helios.Engine.Actions
{
    public class MudAction
    {
        public long DispatchTime { get; set; }
        public string[] Args { get; }
        public int SenderId { get; }
        public int ReceiverId { get; }
        public int OtherEntity1 { get; }
        public int OtherEntity2 { get; }
        public string Type { get; }
        public string Value { get; set; }

        public MudAction() { }
        public MudAction(string type, int senderId, long dispatchTime = 0, params string[] args) : this(type, senderId, 0, 0, 0, dispatchTime, args) { }
        public MudAction(string type, int senderId, int receiverId, long dispatchTime = 0, params string[] args) : this(type, senderId, receiverId, 0, 0, dispatchTime, args) { }
        public MudAction(string type, int senderId, int receiverId, int other1, long dispatchTime = 0, params string[] args) : this(type, senderId, receiverId, other1, 0, dispatchTime, args) { }
        public MudAction(string type, int senderId, int receiverId, int other1, int other2, long dispatchTime = 0, params string[] args)
        {
            Args = args;
            SenderId = senderId;
            ReceiverId = receiverId;
            DispatchTime = dispatchTime;
            Type = type;
            OtherEntity1 = other1;
            OtherEntity2 = other2;
        }

        public int CompareTo(MudAction other)
        {
            if (DispatchTime < other.DispatchTime) return -1;
            if (DispatchTime > other.DispatchTime) return 1;
            return 0;
        }
    }
}