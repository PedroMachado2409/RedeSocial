using AutoMapper;
using RedeSocial.Aplicacao.Dto;
using RedeSocial.Domain.Entities;

namespace RedeSocial.Aplicacao.Mapping
{
    public class UsuarioProfile : Profile
    {
        public UsuarioProfile()
        {
            CreateMap<UsuarioDTO, Usuario>()
             .ReverseMap();
        }
    }
}
