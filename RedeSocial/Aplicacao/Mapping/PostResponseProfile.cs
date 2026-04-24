using AutoMapper;
using RedeSocial.Aplicacao.Dto;
using RedeSocial.Domain.Entities;

namespace RedeSocial.Aplicacao.Mapping
{
    public class PostResponseProfile : Profile
    {
        public PostResponseProfile()
        {
            CreateMap<Comentario, ComentarioResponseDTO>()
                .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => src.Usuario.Id))
                .ForMember(dest => dest.UsuarioNome, opt => opt.MapFrom(src => src.Usuario.Nome))
                .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.Post.Id));

            CreateMap<Post, PostResponseDTO>()
                .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => src.Usuario.Id))
                .ForMember(dest => dest.UsuarioNome, opt => opt.MapFrom(src => src.Usuario.Nome))
                .ForMember(dest => dest.Comentarios, opt => opt.MapFrom(src => src.Comentarios));
        }
    }
}
