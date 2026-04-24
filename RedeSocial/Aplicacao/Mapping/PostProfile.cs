using AutoMapper;
using RedeSocial.Aplicacao.Dto;
using RedeSocial.Domain.Entities;

public class PostProfile : Profile
{
    public PostProfile()
    {
        CreateMap<PostRequestDTO, Post>()
            .ForMember(dest => dest.Titulo, opt => opt.MapFrom(src => src.Titulo))
            .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => src.UsuarioId))
            .ForMember(dest => dest.Comentarios, opt => opt.MapFrom(src => src.Comentarios)).ReverseMap();

        CreateMap<ComentarioRequestDTO, Comentario>()
            .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => src.UsuarioId))
            .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.PostId))
            .ForMember(dest => dest.DtComentario, opt => opt.MapFrom(src => src.DtComentario))
            .ForMember(dest => dest.Descricao, opt => opt.MapFrom(src => src.Descricao)).ReverseMap();

       
    }
}
