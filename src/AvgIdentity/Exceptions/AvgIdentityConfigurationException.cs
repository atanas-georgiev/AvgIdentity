namespace AvgIdentity.Exceptions
{
    using System;

    public class AvgIdentityConfigurationException : Exception
    {
        public AvgIdentityConfigurationException(string message)
            : base(message)
        {
        }
    }
}