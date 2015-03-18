using System;

namespace HighFlyers.Various
{
    public class ToDoElement
    {
        public ToDoElement(string description, Func<bool> checkMethod, Action fixmeMethod = null)
        {
            FixmeMethod = fixmeMethod;
            CheckMethod = checkMethod;
            Description = description;
        }

        public string Description { get; private set; }
        public Func<bool> CheckMethod { get; private set; }
        public Action FixmeMethod { get; private set; } 
    }
}
