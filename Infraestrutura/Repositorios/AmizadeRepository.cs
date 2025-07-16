using Microsoft.EntityFrameworkCore;
using RedeSocial.Domain.Entities;
using RedeSocial.Infraestrutura.Data;

namespace RedeSocial.Infraestrutura.Repositorios
{
    public class AmizadeRepository
    {
        private readonly AppDbContext _context;

        public AmizadeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Amizade>> ListarAmigosDoUsuario(int usuarioId)
        {
            return await _context.Amizades.Include(a => a.Usuario).Include(a => a.Amigo)
                .Where(a => a.AmigoId == usuarioId || a.UsuarioId == usuarioId).ToListAsync();
        }

        public async Task<bool> JaSaoAmigos(int usuario1Id, int usuario2Id)
        {
            return await _context.Amizades.AnyAsync(a =>
                (a.UsuarioId == usuario1Id && a.AmigoId == usuario2Id) ||
                (a.UsuarioId == usuario2Id && a.AmigoId == usuario1Id));
        }

        public async Task<Amizade> FazerAmizade(Amizade amizade, AmizadePendente pendente)
        {
            await _context.Amizades.AddAsync(amizade);
             _context.AmizadePendentes.Remove(pendente);
            await _context.SaveChangesAsync();
            return amizade;
        }

        public async Task<Amizade?> ObterAmizadePorId(int id)
        {
            return await _context.Amizades.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task RemoverAmizade(Amizade amizade)
        {
            _context.Amizades.Remove(amizade);
            await _context.SaveChangesAsync();
        }
    }
}
