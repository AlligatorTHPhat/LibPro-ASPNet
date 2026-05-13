using System;
using LibraryDomain.Enums;

namespace LibraryDomain.Exceptions
{
    public class LoanLimitExceededException : DomainException
    {
        public ReaderType ReaderType { get; }
        public int MaxAllowed { get; }

        public LoanLimitExceededException(ReaderType readerType, int maxAllowed)
            : base($"Reader Type [{readerType}] has reached the maximum loan limit ({maxAllowed} books). Please return old books before borrowing new ones.")
        {
            ReaderType = readerType;
            MaxAllowed = maxAllowed;
        }
    }
}