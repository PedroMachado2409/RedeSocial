using AutoMapper;
using RedeSocial.Aplicacao.Dto;
using RedeSocial.Domain.Entities;

public class AmizadeProfile : Profile
{
    private static string? ToBase64(byte[]? bytes) =>
        bytes != null && bytes.Length > 0 ? Convert.ToBase64String(bytes) : null;

    public AmizadeProfile()
    {
        // ── AmizadePendente → DTO ─────────────────────────────────────────────
        CreateMap<AmizadePendente, AmizadePendenteDTO>()
            .ForMember(dest => dest.SolicitanteNome,
                opt => opt.MapFrom(src => src.Solicitante.Nome))
            .ForMember(dest => dest.SolicitanteFotoPerfilBase64,
                opt => opt.MapFrom(src => ToBase64(src.Solicitante.FotoPerfil)))
            .ForMember(dest => dest.DestinatarioNome,
                opt => opt.MapFrom(src => src.Destinatario.Nome))
            .ForMember(dest => dest.DestinatarioFotoPerfilBase64,
                opt => opt.MapFrom(src => ToBase64(src.Destinatario.FotoPerfil)));

        CreateMap<AmizadePendenteDTO, AmizadePendente>()
            .ForMember(dest => dest.Solicitante,  opt => opt.Ignore())
            .ForMember(dest => dest.Destinatario, opt => opt.Ignore());

        // ── Amizade → DTO ─────────────────────────────────────────────────────
        // AmigoId e AmigoNome dependem de quem é o usuário logado (context.Items)
        CreateMap<Amizade, AmizadeDTO>()
            .ForMember(dest => dest.AmigoId, opt => opt.MapFrom((src, _, _, context) =>
            {
                var uid = (int)context.Items["usuarioId"];
                return src.UsuarioId == uid ? src.AmigoId : src.UsuarioId;
            }))
            .ForMember(dest => dest.AmigoNome, opt => opt.MapFrom((src, _, _, context) =>
            {
                var uid = (int)context.Items["usuarioId"];
                return src.UsuarioId == uid ? src.Amigo.Nome : src.Usuario.Nome;
            }))
            .ForMember(dest => dest.AmigoFotoPerfilBase64, opt => opt.MapFrom((src, _, _, context) =>
            {
                var uid = (int)context.Items["usuarioId"];
                var foto = src.UsuarioId == uid ? src.Amigo.FotoPerfil : src.Usuario.FotoPerfil;
                return ToBase64(foto);
            }));
    }
}
