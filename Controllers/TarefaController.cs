using Microsoft.AspNetCore.Mvc;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TarefaController : ControllerBase
    {
        private readonly OrganizadorContext _context;

        public TarefaController(OrganizadorContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {

            var tarefaBanco = _context.Find(id);

            if (tarefaBanco == null)
                return NotFound();

            return Ok(tarefaBanco);
        }

        [HttpGet("ObterTodos")]
        public IActionResult ObterTodos()
        {

            var tarefaBanco = _context.Tarefas.ToList();

            if (!tarefaBanco.Any())
            {
                return NotFound("Nenhuma tarefa encontrada.");
            }

            return Ok(tarefaBanco);
        }

        [HttpGet("ObterPorTitulo")]
        public IActionResult ObterPorTitulo(string titulo)
        {

            var tarefas = _context.Tarefas.Where(x => x.Titulo.ToUpper().Contains(titulo.ToUpper()))
                          .ToList();

            // var tarefa = _context.Tarefas                   Correspondência exata
            //           .Where(x => x.Titulo == titulo)
            //           .ToList();

            if (!tarefas.Any())
            {
                return NotFound("Nenhuma tarefa encontrada com o título especificado.");
            }

            return Ok(tarefas);
        }

        [HttpGet("ObterPorData")]
        public IActionResult ObterPorData(DateTime data)
        {
            var tarefa = _context.Tarefas.Where(x => x.Data.Date == data.Date);
            return Ok(tarefa);
        }

        [HttpGet("ObterPorStatus")]
        public IActionResult ObterPorStatus(EnumStatusTarefa status)
        {
            var tarefas = _context.Tarefas.Where(x => x.Status == status).ToList();
            if (!tarefas.Any())
            {
                return NotFound("Nenhuma tarefa encontrada com o título especificado.");
            }
            return Ok(tarefas);
        }

        [HttpPost]
        public IActionResult Criar(Tarefa tarefa)
        {
            var resultadoVerificacao = VerificaTarefa(tarefa);

            if (resultadoVerificacao is IActionResult erro)
            {
                return erro;
            }

            var tarefaExistente = _context.Tarefas
                                  .Any(t => t.Titulo.Equals(tarefa.Titulo, StringComparison.OrdinalIgnoreCase));

            if (tarefaExistente)
            {
                return Conflict(new { Erro = "Já existe uma tarefa com o mesmo título." });
            }

            _context.Tarefas.Add(tarefa);
            _context.SaveChanges();

            return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
        }


        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, Tarefa tarefa)
        {
            var tarefaBanco = _context.Tarefas.Find(id);

            var resultadoVerificacao = VerificaTarefa(tarefaBanco);

            if (resultadoVerificacao is IActionResult erro)
            {
                return erro;
            }

            _context.Tarefas.Update(tarefaBanco);

            _context.SaveChanges();

            return Ok(tarefaBanco);
        }

        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            var tarefaBanco = _context.Tarefas.Find(id);

            if (tarefaBanco == null)
                return NotFound();

            _context.Tarefas.Remove(tarefaBanco);

            return NoContent();
        }


        private IActionResult VerificaTarefa(Tarefa tarefa)
        {
            if (tarefa == null)
            {
                return BadRequest(new { Erro = "A tarefa não pode ser nula." });
            }
            if (string.IsNullOrWhiteSpace(tarefa.Titulo))
            {
                return BadRequest(new { Erro = "O título da tarefa é obrigatório." });
            }
            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            return Ok(tarefa);
        }
    }
}
