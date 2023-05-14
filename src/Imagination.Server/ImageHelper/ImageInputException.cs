using System;


namespace Imagination.ImageHelper
{
    public class ImageInputException : Exception
    {

        internal ImageInputException(string errorMessage)
            : base(errorMessage)
        {
        }

        internal ImageInputException(string errorMessage, Exception innerException)
            : base(errorMessage, innerException)
        {
        }
    }
}
