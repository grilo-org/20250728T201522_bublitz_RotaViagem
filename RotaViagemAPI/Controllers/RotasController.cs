using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RotaViagemAPI.Data;
using RotaViagemAPI.Models;

namespace RotaViagemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RotasController : ControllerBase
    {
        private readonly RotaContext _context;

        public RotasController(RotaContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rota>>> GetRotas()
        {
            var rotas = await _context.Rotas.ToListAsync();
            return Ok(rotas);
        }

        [HttpPost]
        public async Task<ActionResult<Rota>> PostRota(Rota rota)
        {
            _context.Rotas.Add(rota);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRotas), new { id = rota.Id }, rota);
        }

        [HttpPut]
        public async Task<ActionResult<Rota>> PutRota(Rota rota)
        {
            var exist = _context.Rotas.FirstOrDefault(r => r.Origem == rota.Origem && r.Destino == rota.Destino);
            if (exist == null) return NotFound();
            
            _context.Entry(exist).Entity.Valor = rota.Valor;
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRotas), new { id = rota.Id }, rota);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRota(int id)
        {
            var rota = await _context.Rotas.FindAsync(id);
            if (rota == null) return NotFound();

            _context.Rotas.Remove(rota);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("melhor-rota")]
        public ActionResult<string> MelhorRota(string origem, string destino)
        {
            var rotas = _context.Rotas.ToList();
            var resultado = EncontrarMelhorRota(rotas, origem, destino);
            return resultado ?? "Rota não encontrada.";
        }

        private string? EncontrarMelhorRota(List<Rota> rotas, string origem, string destino)
        {
            var caminhos = new List<List<Rota>>();
            BuscarRotas(rotas, origem, destino, new List<Rota>(), caminhos);

            if (!caminhos.Any()) return null;

            var melhorCaminho = caminhos.OrderBy(c => c.Sum(r => r.Valor)).First();
            var melhorRota = string.Join(" -> ", melhorCaminho.Select(r => r.Origem)) + $" -> {destino} (Custo Total: {melhorCaminho.Sum(r => r.Valor)})";
            return melhorRota;
        }

        private void BuscarRotas(List<Rota> rotas, string origem, string destino, List<Rota> caminhoAtual, List<List<Rota>> caminhos)
        {
            var rotasPossiveis = rotas.Where(r => r.Origem == origem).ToList();

            foreach (var rota in rotasPossiveis)
            {
                var novoCaminho = new List<Rota>(caminhoAtual) { rota };

                if (rota.Destino == destino)
                {
                    caminhos.Add(novoCaminho);
                }
                else
                {
                    BuscarRotas(rotas, rota.Destino, destino, novoCaminho, caminhos);
                }
            }
        }
    }
}
