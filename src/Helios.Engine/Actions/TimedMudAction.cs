using System;

namespace Helios.Engine.Actions
{
    public class TimedMudAction : MudAction, IComparable<TimedMudAction>
    {
        public long DispatchTime { get; set; }
        public bool IsValid { get; set; }

        //public TimedMudAction() { }
        public TimedMudAction(uint dispatchTime, string type, int senderId, params string[] args) : this(dispatchTime, type, senderId, 0, 0, 0, args) { }
        public TimedMudAction(uint dispatchTime, string type, int senderId, int receiverId, params string[] args) : this(dispatchTime, type, senderId, receiverId, 0, 0, args) { }
        public TimedMudAction(uint dispatchTime, string type, int senderId, int receiverId, int other1, params string[] args) : this(dispatchTime, type, senderId, receiverId, other1, 0, args) { }
        public TimedMudAction(uint dispatchTime, string type, int senderId, int receiverId, int other1, int other2, params string[] args)
            : base(type, senderId, receiverId, other1, other2, args)
        {
            DispatchTime = DateTime.UtcNow.AddSeconds(dispatchTime).Ticks;
            IsValid = true;
        }


        public TimedMudAction(string dispatchTime, string type, int senderId, params string[] args) : this(dispatchTime, type, senderId, 0, 0, 0, args) { }
        public TimedMudAction(string dispatchTime, string type, int senderId, int receiverId, params string[] args) : this(dispatchTime, type, senderId, receiverId, 0, 0, args) { }
        public TimedMudAction(string dispatchTime, string type, int senderId, int receiverId, int other1, params string[] args) : this(dispatchTime, type, senderId, receiverId, other1, 0, args) { }
        public TimedMudAction(string dispatchTime, string type, int senderId, int receiverId, int other1, int other2, params string[] args)
            : base(type, senderId, receiverId, other1, other2, args)
        {
            try
            {
                var dt = DateTime.Parse(dispatchTime).ToUniversalTime();
                if (dt < DateTime.UtcNow)
                {
                    Console.WriteLine("Timed actions can't be in the past");
                    return;
                }
                DispatchTime = dt.Ticks;
            }

            catch
            {
                //Log entry - badly formatted date string
                return;
            }

            IsValid = true;
        }


        public void Hook()
        {
            Game.Instance.GetEntityById(SenderId).Components.AddHook(this);
            if (ReceiverId > 0)
                Game.Instance.GetEntityById(ReceiverId).Components.AddHook(this);
            if (OtherEntity1 > 0)
                Game.Instance.GetEntityById(OtherEntity1).Components.AddHook(this);
            if (OtherEntity2 > 0)
                Game.Instance.GetEntityById(OtherEntity2).Components.AddHook(this);
        }
        public void Unhook()
        {
            IsValid = false;
            Game.Instance.GetEntityById(SenderId).Components.RemoveHook(this);
            if (ReceiverId > 0)
                Game.Instance.GetEntityById(ReceiverId).Components.RemoveHook(this);
            if (OtherEntity1 > 0)
                Game.Instance.GetEntityById(OtherEntity1).Components.RemoveHook(this);
            if (OtherEntity2 > 0)
                Game.Instance.GetEntityById(OtherEntity2).Components.RemoveHook(this);
        }

        public int CompareTo(TimedMudAction other)
        {
            if (DispatchTime < other.DispatchTime) return -1;
            if (DispatchTime > other.DispatchTime) return 1;
            return 0;
        }
    }
}