using AutoMapper;
using Set.Frontend.Models;
using Set.Backend.Models;

namespace Set.Frontend
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
