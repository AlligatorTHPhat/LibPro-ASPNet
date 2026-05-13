/****
Tran Hoang Phat - 49.01.104.107
****/

using System;
using LibraryDomain.Exceptions;

namespace LibraryDomain.ValueObjects
{
    public record Address
    {
        public string Street { get; init; }  
        public string Ward { get; init; }    
        public string District { get; init; } 
        public string City { get; init; }     

        public Address(string street, string ward, string district, string city)
        {
            Street = street;
            Ward = ward;
            District = district;
            City = city;
        }

        public override string ToString()
        {
            return $"{Street}, {Ward}, {District}, {City}";
        }

        public bool IsInCity(string cityName)
        {
            return string.Equals(City, cityName, StringComparison.OrdinalIgnoreCase);
        }
    }
}