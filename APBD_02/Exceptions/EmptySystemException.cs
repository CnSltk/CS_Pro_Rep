using System;

namespace APBD_02.Exceptions
{
    public class EmptySystemException : Exception
    {
        public EmptySystemException() 
            : base("The computer cannot be started because there is no operating system installed.") { }

        public EmptySystemException(string message) 
            : base(message) { }
    }
}