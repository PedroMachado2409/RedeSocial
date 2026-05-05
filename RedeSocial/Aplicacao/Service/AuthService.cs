using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using RedeSocial.Aplicacao.Dto;
using RedeSocial.Domain.Abstractions;
using RedeSocial.Domain.Entities;
using RedeSocial.Exceptions;
using RedeSocial.Infraestrutura.Seguranca;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RedeSocial.Aplicacao.Service
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration       _configuration;
        private readonly IUsuarioRepository   _repository;
        private readonly IPostRepository      _postRepository;
        private readonly IAmizadeRepository   _amizadeRepository;
        private readonly IMapper              _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(
            IConfiguration       configuration,
            IUsuarioRepository   repository,
            IPostRepository      postRepository,
            IAmizadeRepository   amizadeRepository,
            IMapper              mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _configuration       = configuration;
            _repository          = repository;
            _postRepository      = postRepository;
            _amizadeRepository   = amizadeRepository;
            _mapper              = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Usuario> RegistrarUsuario(Usuario usuario)
        {
            var existente = await _repository.ObterUsuarioPorEmail(usuario.Email);
            if (existente != null)
                throw new BadRequestException(Messages.EmailCadastrado);

            usuario.Senha = PasswordHelper.CriptografarSenha(usuario.Senha);
            return await _repository.CadastrarUsuario(usuario);
        }

        public async Task<LoginResponseDTO> Autenticar(LoginRequestDTO dto)
        {
            var usuario = await _repository.ObterUsuarioPorEmail(dto.Email);
            if (usuario == null || !PasswordHelper.VerificarSenha(dto.Senha, usuario.Senha))
                throw new BadRequestException(Messages.CredenciaisInvalidas);

            return new LoginResponseDTO
            {
                Email = usuario.Email,
                Nome  = usuario.Nome,
                Token = GerarToken(usuario)
            };
        }

        public async Task<Usuario?> ObterUsuarioAutenticado()
        {
            var email = _httpContextAccessor.HttpContext?
                .User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            return await _repository.ObterUsuarioPorEmail(email!);
        }

        public async Task<List<UsuarioDTO>> ObterUsuarioPorNome(string nome)
        {
            var usuarios = await _repository.ObterUsuarioPorNome(nome);
            if (usuarios == null)
                throw new BadRequestException(Messages.UsuarioNotFound);
            return _mapper.Map<List<UsuarioDTO>>(usuarios);
        }

        public async Task<PerfilResponseDTO> ObterPerfil(int usuarioId)
        {
            var usuario = await _repository.ObterUsuarioPorId(usuarioId)
                ?? throw new NotFoundException(Messages.UsuarioNotFound);

            var perfil = _mapper.Map<PerfilResponseDTO>(usuario);

            var amigos = await _amizadeRepository.ObterIdsDosAmigos(usuarioId);
            var posts  = await _postRepository.ListarPostsDoUsuario(usuarioId);

            perfil.TotalAmigos = amigos.Count;
            perfil.TotalPosts  = posts.Count;

            return perfil;
        }

        public async Task<PerfilResponseDTO> AtualizarPerfil(AtualizarPerfilRequestDTO dto)
        {
            var usuario = await ObterUsuarioAutenticado()
                ?? throw new UnauthorizedException("Usuário não autenticado.");

            byte[]? fotoPerfil = null;
            byte[]? fotoBanner = null;

            if (!string.IsNullOrWhiteSpace(dto.FotoPerfilBase64))
                fotoPerfil = Convert.FromBase64String(dto.FotoPerfilBase64);

            if (!string.IsNullOrWhiteSpace(dto.FotoBannerBase64))
                fotoBanner = Convert.FromBase64String(dto.FotoBannerBase64);

            usuario.AtualizarPerfil(dto.DescricaoPerfil, fotoPerfil, fotoBanner);

            var atualizado = await _repository.AtualizarPerfil(usuario);

            var perfil = _mapper.Map<PerfilResponseDTO>(atualizado);
            var amigos = await _amizadeRepository.ObterIdsDosAmigos(usuario.Id);
            var posts  = await _postRepository.ListarPostsDoUsuario(usuario.Id);
            perfil.TotalAmigos = amigos.Count;
            perfil.TotalPosts  = posts.Count;

            return perfil;
        }

        public async Task TrocarSenha(TrocarSenhaRequestDTO dto)
        {
            var usuario = await ObterUsuarioAutenticado()
                ?? throw new UnauthorizedException("Usuário não autenticado.");

            if (!PasswordHelper.VerificarSenha(dto.SenhaAtual, usuario.Senha))
                throw new BadRequestException("Senha atual incorreta.");

            if (dto.NovaSenha.Length < 6)
                throw new BadRequestException("A nova senha deve ter pelo menos 6 caracteres.");

            usuario.TrocarSenha(PasswordHelper.CriptografarSenha(dto.NovaSenha));
            await _repository.TrocarSenha(usuario);
        }

        private string GerarToken(Usuario usuario)
        {
            var chave       = _configuration["Jwt:Key"]!;
            var chaveSim    = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave));
            var credenciais = new SigningCredentials(chaveSim, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Email),
                new Claim("nome", usuario.Nome),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer:             _configuration["Jwt:Issuer"],
                audience:           _configuration["Jwt:Audience"],
                claims:             claims,
                expires:            DateTime.UtcNow.AddHours(2),
                signingCredentials: credenciais
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
