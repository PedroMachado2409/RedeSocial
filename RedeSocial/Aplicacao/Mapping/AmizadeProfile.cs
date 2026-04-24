using AutoMapper;
using RedeSocial.Aplicacao.Dto;
using RedeSocial.Domain.Entities;

namespace RedeSocial.Aplicacao.Mapping
{
    public class AmizadeProfile : Profile
    {
        public AmizadeProfile()
        {
            CreateMap<AmizadeDTO, Amizade>().ReverseMap();
            CreateMap<AmizadePendenteDTO, AmizadePendente>().ReverseMap();
        }
    }
}
