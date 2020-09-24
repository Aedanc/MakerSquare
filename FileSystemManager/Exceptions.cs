using System;

namespace MakerSquare
{
    namespace FileSystem
    {
        public class CannotCreateProjectDirException : Exception
        {
            public CannotCreateProjectDirException(){}
            public CannotCreateProjectDirException(string message) : base(message){}
            public CannotCreateProjectDirException(string message, Exception inner): base(message, inner){}
        }

        public class CannotCreateDefaultManifestException : Exception
        {
            public CannotCreateDefaultManifestException() { }
            public CannotCreateDefaultManifestException(string message) : base(message) { }
            public CannotCreateDefaultManifestException(string message, Exception inner) : base(message, inner) { }
        }
    }
}