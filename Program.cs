using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using RedeSocial.Aplicacao.Mapping;
using RedeSocial.Aplicacao.Service;
using RedeSocial.Infraestrutura.Data;
using RedeSocial.Infraestrutura.Repositorios;
using System.Text;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // 👇 Converte enums para strings no JSON
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],

        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,

        NameClaimType = JwtRegisteredClaimNames.Sub
    };
});

// Registre seus serviços personalizados
builder.Services.AddScoped<UsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<AmizadePendenteRepository, AmizadePendenteRepository>();
builder.Services.AddScoped<AmizadePendenteService>();

builder.Services.AddScoped<AmizadeRepository, AmizadeRepository>();
builder.Services.AddScoped<AmizadeService>();

builder.Services.AddScoped<PostRepository>();
builder.Services.AddScoped<ComentarioRepository>();
builder.Services.AddScoped<PostService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(UsuarioProfile).Assembly);
builder.Services.AddAutoMapper(typeof(AmizadeProfile).Assembly);
builder.Services.AddAutoMapper(typeof(PostProfile).Assembly);
builder.Services.AddAutoMapper(typeof(PostResponseProfile).Assembly);


var app = builder.Build();

// Configure o pipeline HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
