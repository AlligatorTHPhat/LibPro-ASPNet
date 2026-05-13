/****
Tran Hoang Phat - 49.01.104.107
****/

using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryDomain.ValueObjects
{
    public record Gender
    {
        public string Name { get; }
        public int Value { get; }
        public Gender(string name, int value)
        {
            Name = name;
            Value = value;
        }
        public static readonly Gender Male = new Gender("Male", 1);
        public static readonly Gender Female = new Gender("Female", 0);
        public static Gender FromValue(int value)
        {
            return value switch
            {
                1 => Male,
                0 => Female,
                _ => throw new ArgumentException("Gender invalid! 1 for Male, 0 for Female!")
            };
        }
        public override string ToString() => Name;
    }
}