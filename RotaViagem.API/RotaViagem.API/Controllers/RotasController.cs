using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Application.Services;
using Domain.Entities;

namespace SeuProjeto.Controllers
{
    [ApiController]
    [Route("rotas")]
    public class RotasController : ControllerBase
    {
        private readonly RotaService _service;

        public RotasController(RotaService service)
        {
            _service = service;
        }


        // GET /rotas
        [HttpGet]
        public async Task<IActionResult> ListarRotas()
        {
            var rotas = await _service.ListarRotasAsync();
            return Ok(rotas);
        }


        [HttpGet("mais-barata")]
        public async Task<IActionResult> ConsultarRotaMaisBarata([FromQuery] string origem, [FromQuery] string destino)
        {
            var (caminho, custoTotal) = await _service.ConsultarRotaMaisBarata(origem.ToUpper(), destino.ToUpper());

            if (custoTotal == -1 || caminho.Count == 0)
                return NotFound("Rota não encontrada.");

            return Ok(new
            {
                Origem = origem,
                Destino = destino,
                CustoTotal = custoTotal,
                Trechos = caminho.Select(r => new
                {
                    r.origem,
                    r.destino,
                    r.valor
                })
            });
        }


        // GET /rotas/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPorId(string id)
        {
            var rota = await _service.BuscarPorIdAsync(id);
            return rota is null ? NotFound() : Ok(rota);
        }

        // POST /rotas
        [HttpPost]  
        public async Task<IActionResult> CriarRota([FromBody] Rota rota)
        {
            await _service.CriarRotaAsync(rota);
            return CreatedAtAction(nameof(BuscarPorId), new { id = rota.id }, rota);
        }

        // PUT /rotas/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarRota(string id, [FromBody] Rota rota)
        {
            rota.id = id;
            await _service.AtualizarRotaAsync(rota);
            return NoContent();
        }

        // DELETE /rotas/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoverRota(string id)
        {
            await _service.RemoverRotaAsync(id);
            return NoContent();
        }



    }
}
