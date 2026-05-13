using System;

namespace LibraryDomain.Exceptions
{
    public class MembershipExpiredException : DomainException
    {
        public DateTime ExpiryDate { get; }

        public MembershipExpiredException(DateTime expiryDate)
            : base($"Library Card is expired as of {expiryDate:dd/MM/yyyy}. Please renew to continue using the service.")
        {
            ExpiryDate = expiryDate;
        }
    }
}