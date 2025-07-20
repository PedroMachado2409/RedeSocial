using Microsoft.EntityFrameworkCore;
using RedeSocial.Domain.Entities;
using RedeSocial.Infraestrutura.Data;

namespace RedeSocial.Infraestrutura.Repositorios
{
    public class AmizadePendenteRepository
    {
        private readonly AppDbContext _context;

        public AmizadePendenteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AmizadePendente> EnviarSolicitacao(AmizadePendente amizadePendente)
        {
            await _context.AmizadePendentes.AddAsync(amizadePendente);
            await _context.SaveChangesAsync();
            return amizadePendente;
        }

        public async Task<List<AmizadePendente>> ListarPedidosRecebidos(int destinatarioId)
        {
            return await _context.AmizadePendentes.Include(a => a.Solicitante)
                .Where(a => a.DestinatarioId == destinatarioId).ToListAsync();
        }

        public async Task<AmizadePendente?> ObterPedidoDeAmizadePorId(int id)
        {
            return await _context.AmizadePendentes.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<AmizadePendente>> ListarPedidosEnviados(int solicitanteId)
        {
            return await _context.AmizadePendentes.Include(a => a.Destinatario)
                .Where(a => a.SolicitanteId == solicitanteId).ToListAsync();
        }

        public async Task<bool> ValidarSeJaTemPedido(int usuario1, int usuario2)
        {
            return await _context.AmizadePendentes.AnyAsync(a => (a.SolicitanteId == usuario1 && a.DestinatarioId == usuario2
            || a.DestinatarioId == usuario1 && a.SolicitanteId == usuario2));
        }

        public async Task RejeitarPedido(AmizadePendente pendente)
        {
             _context.AmizadePendentes.Remove(pendente);
             await _context.SaveChangesAsync();
        }
    }
}
