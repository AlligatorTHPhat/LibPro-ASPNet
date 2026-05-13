using AutoMapper;
using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryApplication.DTOs.Categories
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryResponse>();
        }
    }
}
