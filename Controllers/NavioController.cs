using Microsoft.AspNetCore.Mvc;
using SistemaLogistico.Models;
using SistemaLogistico.Services;

namespace SistemaLogistico.Controllers;

[ApiController]
[Route("api/systemlog")]

public class NaviosController : ControllerBase
{

    private readonly iNavioService _navioController;

    public NaviosController(iNavioService navioController)
    {
        _navioController = navioController;
    }


    // Retorna a lista de navios
    [HttpGet("navios")]
    public ActionResult<List<Navio>> ListaNavios()
    {
        try
        {
         return Ok(_navioController.ListaNavios());   
        }
        catch (FileNotFoundException)
        {
            return StatusCode(404, "Nenhum navio encontrado");
        }
        catch (Exception)
        {
            return StatusCode(500, "Erro ao processar requisição");
        }
    }

    // Retorna a lista de containers
    [HttpGet("containers")]
    public ActionResult<List<Container>> ListaContainers()
    {
         try{
            return Ok(_navioController.ListaContainers());
        }
        catch (FileNotFoundException)
        {
            return StatusCode(404, "Nenhum container encontrado");
        }
        catch (Exception)
        {
            return StatusCode(500, "Erro ao processar requisição");
        }
        
    }

    // cadastroNavio
    [HttpPost("navios")]
    public ActionResult<List<Navio>> adicionarNavio(Navio navio)
    {
        try
        {
            if (navio != null)
            {
                return Ok(_navioController.adicionarNavio(navio));
            }
            return StatusCode(400, "Navio não informado no corpo da requisição");
        }
        catch (ArgumentException)
        {
            return StatusCode(400, "A Origem informada é invalida");
        }
        catch (System.Exception)
        {
            return Problem("Falha ao inserir navio na fila de embarque.");
        }
    }

    // alterarNavio
    [HttpPut("navio/{id}")]
    public ActionResult<bool> alterarNavio(int id, Navio navio)
    {
        try
        {
            if (navio == null)
            {
                return StatusCode(400, "Navio ou id não informado no corpo da requisição");
            }
            return Ok(_navioController.alterarNavio(id, navio));
        }
        catch (System.Exception)
        {
            return Problem("Falha ao alterar navio.");
        }

    }

    // filaEmbarque
    [HttpPost("embarque")]
    public ActionResult<NavioResponse> adicionarContainerFila(Container container)
    {
        try
        {
            if (container == null)
            {
                return StatusCode(400, "Container não informado no corpo da requisição");
            }
            if (container is null)
            {
                return StatusCode(204, "Valores inválidos!");
            }
            if (container.Carga <= 0)
            {
                return StatusCode(204, "Valores inválidos!");
            }
            if (container.Ponto == null)
            {
                return StatusCode(204, "Valores inválidos!");
            }
            return Ok(_navioController.adicionarContainerFila(container));
        }
        catch (System.Exception)
        {
            return Problem("Falha ao inserir container na fila de embarque.");
        }
    }

    // alfandega
    [HttpPut("alfandega/{id}")]
    public ActionResult<bool> alterarContainer(int id, Container container)
    {
        try
        {
            if ((id == null) || (container == null))
            {
                return StatusCode(400, "container ou id não informado no corpo da requisição");
            }
            return Ok(_navioController.alterarContainer(id, container));
        }
        catch (DriveNotFoundException)
        {
            return StatusCode(404, "Container não encontrado para alteraçao");
        }
        catch (Exception)
        {
            return StatusCode(500, "Falha ao alterar container.");
        }

    }

    // confisco
    [HttpDelete("confisco/{id}")]
    public ActionResult<bool> Confisco(int id)
    {
        try
        {
            return Ok(_navioController.Confisco(id));
        }
        catch (FileNotFoundException)
        {
            return StatusCode(404, "Container não encontrado para alteraçao");
        }
        catch (System.Exception)
        {
            return Problem("Falha ao confiscar container.");
        }
    }

    // carregamento
    [HttpGet("carregar")]
    public ActionResult<List<Navio>> Carregamento()
    {
        try
        {
            return Ok(_navioController.Carregamento());
        }
        catch (FileNotFoundException)
        {
            return StatusCode(404, "Nenhum navio encontrado");
        }
        catch (Exception)
        {
            return StatusCode(500, "Falha ao executar carregamento");
        }
    }

    // descarregamento
    [HttpGet("descarregamento/{id}")]
    public ActionResult<List<Container>> Descarregamento(int id)
    {
        try
        {
            return Ok(_navioController.Descarregamento(id));
        }
        catch (FileNotFoundException)
        {
            return StatusCode(404, "Nenhum container encontrado");
        }
        catch (Exception)
        {
            return StatusCode(500, "Falha ao executar o descarregamento");
        }

    }

    // Retorna a fila de containers
    [HttpGet("fila")]
    public ActionResult<List<int>> Fila()
    {
        try
        {
            return Ok(_navioController.Fila());
        }
        catch (FileNotFoundException)
        {
            return StatusCode(404, "Nenhum navio encontrado");
        }
        catch (Exception)
        {
            return StatusCode(500, "Falha ao executar o descarregamento");
        }
    }


}