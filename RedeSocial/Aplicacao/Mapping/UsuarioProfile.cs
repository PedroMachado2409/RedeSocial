using AutoMapper;
using RedeSocial.Aplicacao.Dto;
using RedeSocial.Domain.Entities;

namespace RedeSocial.Aplicacao.Mapping
{
    public class UsuarioProfile : Profile
    {
        public UsuarioProfile()
        {
            CreateMap<Usuario, UsuarioDTO>()
                .ForMember(dest => dest.FotoPerfilBase64,
                    opt => opt.MapFrom(src =>
                        src.FotoPerfil != null && src.FotoPerfil.Length > 0
                            ? Convert.ToBase64String(src.FotoPerfil)
                            : null));

            CreateMap<Usuario, PerfilResponseDTO>()
                .ForMember(dest => dest.FotoPerfilBase64,
                    opt => opt.MapFrom(src =>
                        src.FotoPerfil != null && src.FotoPerfil.Length > 0
                            ? Convert.ToBase64String(src.FotoPerfil)
                            : null))
                .ForMember(dest => dest.FotoBannerBase64,
                    opt => opt.MapFrom(src =>
                        src.FotoBanner != null && src.FotoBanner.Length > 0
                            ? Convert.ToBase64String(src.FotoBanner)
                            : null))
                .ForMember(dest => dest.TotalAmigos, opt => opt.Ignore())
                .ForMember(dest => dest.TotalPosts,  opt => opt.Ignore());
        }
    }
}
