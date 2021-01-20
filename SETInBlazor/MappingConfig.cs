﻿using AutoMapper;
using SETInBlazor.Models;
using Set.Backend.Models;

namespace SETInBlazor
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<SetCard, SetCardUiModel>();
            CreateMap<SetCardUiModel, SetCard>()
                .ForSourceMember(source => source.BackGroundColor, act => act.DoNotValidate());
        }
    }
}
