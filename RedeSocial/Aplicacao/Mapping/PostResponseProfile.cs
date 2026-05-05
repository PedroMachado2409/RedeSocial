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
                .ForMember(dest => dest.UsuarioId,   opt => opt.MapFrom(src => src.Usuario.Id))
                .ForMember(dest => dest.UsuarioNome, opt => opt.MapFrom(src => src.Usuario.Nome))
                .ForMember(dest => dest.PostId,      opt => opt.MapFrom(src => src.Post.Id))
                .ForMember(dest => dest.UsuarioFotoPerfilBase64,
                    opt => opt.MapFrom(src =>
                        src.Usuario.FotoPerfil != null && src.Usuario.FotoPerfil.Length > 0
                            ? Convert.ToBase64String(src.Usuario.FotoPerfil)
                            : null));

            CreateMap<Post, PostResponseDTO>()
                .ForMember(dest => dest.UsuarioId,   opt => opt.MapFrom(src => src.Usuario.Id))
                .ForMember(dest => dest.UsuarioNome, opt => opt.MapFrom(src => src.Usuario.Nome))
                .ForMember(dest => dest.Comentarios, opt => opt.MapFrom(src => src.Comentarios))
                .ForMember(dest => dest.UsuarioFotoPerfilBase64,
                    opt => opt.MapFrom(src =>
                        src.Usuario.FotoPerfil != null && src.Usuario.FotoPerfil.Length > 0
                            ? Convert.ToBase64String(src.Usuario.FotoPerfil)
                            : null))
                .ForMember(dest => dest.ImagemBase64,
                    opt => opt.MapFrom(src =>
                        src.ImagemPost != null && src.ImagemPost.Length > 0
                            ? Convert.ToBase64String(src.ImagemPost)
                            : null));
        }
    }
}
