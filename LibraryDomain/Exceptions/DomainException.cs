using System;

namespace LibraryDomain.Exceptions
{
    /// <summary>
    /// Class cơ sở cho tất cả các ngoại lệ thuộc về quy tắc nghiệp vụ (Domain Rules).
    /// </summary>
    public class DomainException : Exception
    {
        public string? ErrorCode { get; }
        public DomainException(string message, string? errorCode = null)
        : base(message)
        {
            ErrorCode = errorCode;
        }

        public DomainException()
        {
        }

        public DomainException(string message)
            : base(message)
        {
        }

        public DomainException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}