using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using RedeSocial.Api.Middlware;
using RedeSocial.Aplicacao.Mapping;
using RedeSocial.Aplicacao.Service;
using RedeSocial.Aplicacao.Validators;
using RedeSocial.Domain.Abstractions;
using RedeSocial.Infraestrutura.Data;
using RedeSocial.Infraestrutura.Repositorios;
using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ===================== CONTROLLERS =====================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// ===================== SWAGGER =====================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ===================== HTTP CONTEXT =====================
builder.Services.AddHttpContextAccessor();

// ===================== DB =====================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ===================== JWT =====================
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

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

// ===================== CORS =====================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ===================== REPOSITORIES =====================
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IAmizadePendenteRepository, AmizadePendenteRepository>();
builder.Services.AddScoped<IAmizadeRepository, AmizadeRepository>();

// ❌ ERRO SEU (faltava implementação)
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IComentarioRepository, ComentarioRepository>();

// ===================== SERVICES =====================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AmizadePendenteService>();
builder.Services.AddScoped<AmizadeService>();
builder.Services.AddScoped<PostService>();

// ===================== AUTOMAPPER =====================
builder.Services.AddAutoMapper(typeof(UsuarioProfile).Assembly);

// ===================== FLUENT VALIDATION (.NET 8) =====================
builder.Services.AddValidatorsFromAssemblyContaining<UsuarioValidator>();
builder.Services.AddFluentValidationAutoValidation();

// ===================== APP =====================
var app = builder.Build();

// ===================== PIPELINE =====================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// Middleware global de erro
app.UseMiddleware<Middleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();