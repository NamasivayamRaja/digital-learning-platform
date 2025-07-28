using Microsoft.AspNetCore.Http;

namespace DigitalLearningPlatform.BuildingBlocks.Common.Exceptions
{
    public class LearningPlatformException : Exception
    {
        public int StatusCode { get; private set; }
        public LearningPlatformException() { }

        public LearningPlatformException(string message, int statusCode = StatusCodes.Status400BadRequest) : base(message) 
        {
            StatusCode = statusCode;
        }

        public LearningPlatformException(string message, Exception innerException) : base(message, innerException)
        {
        }   
    }
}
