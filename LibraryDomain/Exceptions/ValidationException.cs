namespace LibraryDomain.Exceptions
{
    public class ValidationException : DomainException
    {
        public string PropertyName { get; }

        public ValidationException(string propertyName, string message)
            : base($"Data in [{propertyName}]: {message} is not valid!")
        {
            PropertyName = propertyName;
        }
    }
}