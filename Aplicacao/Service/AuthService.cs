using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using RedeSocial.Aplicacao.Dto;
using RedeSocial.Domain.Entities;
using RedeSocial.Exceptions;
using RedeSocial.Infraestrutura.Repositorios;
using RedeSocial.Infraestrutura.Seguranca;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RedeSocial.Aplicacao.Service
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UsuarioRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public AuthService(IConfiguration configuration, UsuarioRepository repository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _repository = repository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Usuario> RegistrarUsuario(Usuario usuario)
        {
            var usuarioExistente = await _repository.ObterUsuarioPorEmail(usuario.Email);
            if(usuarioExistente != null)
            {
                throw new Exception(Messages.EmailCadastrado);
            }

            usuario.Senha = PasswordHelper.CriptografarSenha(usuario.Senha);
            return await _repository.CadastrarUsuario(usuario);
        }

        public async Task<LoginResponseDTO> Autenticar(LoginRequestDTO dto)
        {
            
            var usuario = await _repository.ObterUsuarioPorEmail(dto.Email);
            if (usuario == null || !PasswordHelper.VerificarSenha(dto.Senha, usuario.Senha))
            {
                throw new Exception(Messages.CredenciaisInvalidas);
            }

            var token = GerarToken(usuario);
            return new LoginResponseDTO
            {
                Email = usuario.Email,
                Nome = usuario.Nome,
                Token = token
            };
        }

        public async Task<Usuario?> ObterUsuarioAutenticado()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var email = httpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            return await _repository.ObterUsuarioPorEmail(email);
        }

        public async Task<List<UsuarioDTO>> ObterUsuarioPorNome(string nome)
        {
            var usuario = await _repository.ObterUsuarioPorNome(nome);
            if(usuario == null)
            {
                throw new Exception(Messages.UsuarioNotFound);
            }

            return _mapper.Map<List<UsuarioDTO>>(usuario);
        }


        //************************** METODOS PRIVADOS **********************************

        private string GerarToken(Usuario usuario)
        {
            var chaveSecreta = _configuration["Jwt:Key"];
            var chaveSimetricar = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveSecreta));
            var credenciais = new SigningCredentials(chaveSimetricar, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Email),
                new Claim("nome", usuario.Nome),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credenciais
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
