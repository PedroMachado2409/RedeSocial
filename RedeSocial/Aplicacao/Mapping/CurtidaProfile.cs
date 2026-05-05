using AutoMapper;
using RedeSocial.Aplicacao.Dto;
using RedeSocial.Domain.Entities;

namespace RedeSocial.Aplicacao.Mapping
{
    public class CurtidaProfile : Profile
    {
        public CurtidaProfile()
        {
            CreateMap<CurtidaDTO, Curtida> ().ReverseMap();
            CreateMap<CurtidaRequestDTO, Curtida> ().ReverseMap();

        }
    }
}
