using System;

namespace TerraSketch.FluentBuilders
{
    public class InvalidBuildStateException : Exception
    {
        public InvalidBuildStateException(string message) : base(message)
        {

        }
    }
}