namespace LibraryDomain.Exceptions
{
    public class EntityAlreadyExistsException : DomainException
    {
        public string EntityName { get; }
        public object ConflictingValue { get; }

        public EntityAlreadyExistsException(string name, object value)
            : base($"Entity \"{name}\" with value [{value}] already exists in the system.")
        {
            EntityName = name;
            ConflictingValue = value;
        }
    }
}