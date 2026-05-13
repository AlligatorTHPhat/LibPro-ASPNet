/****
Tran Hoang Phat - 49.01.104.107
****/

using System;
using LibraryDomain.Exceptions;

namespace LibraryDomain.ValueObjects
{
    public record Money
    {
        public decimal Amount { get; init; }
        public string Currency { get; init; }

        public Money(decimal amount, string currency = "VND")
        {
            if (amount < 0)
            {
                throw new ValidationException("Amount", "Money amount cannot be negative!");
            }

            Amount = amount;
            Currency = currency;
        }

        public static Money Zero() => new Money(0);

        public static Money FromVnd(decimal amount) => new Money(amount, "VND");

        public static Money operator *(Money m, int multiplier)
        {
            return new Money(m.Amount * multiplier, m.Currency);
        }

        public static Money operator *(int multiplier, Money m) => m * multiplier;

        public override string ToString() => $"{Amount:N0} {Currency}";
    }
}