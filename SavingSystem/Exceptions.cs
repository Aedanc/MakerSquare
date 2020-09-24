using System;

namespace MakerSquare
{
    namespace SavingSystem
    {
       
        public class CannotSaveFileException : Exception
        {
            public CannotSaveFileException() { }
            public CannotSaveFileException(string message) : base(message) { }
            public CannotSaveFileException(string message, Exception inner) : base(message, inner) { }
        }

        public class CannotLoadFileException : Exception
        {
            public CannotLoadFileException() { }
            public CannotLoadFileException(string message) : base(message) { }
            public CannotLoadFileException(string message, Exception inner) : base(message, inner) { }
        }
    }
}