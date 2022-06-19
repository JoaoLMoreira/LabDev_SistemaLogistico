using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;

namespace SistemaLogistico.Controllers;

[ApiController]
[Route("[controller]")]

public class NavioController : ControllerBase
{
    private double ValidaFloat(string valor)
    {
        valor = valor.Replace(",", ".");
        double retorno = double.Parse(valor, CultureInfo.InvariantCulture.NumberFormat);
        return retorno;
    }

    private static List<Navio> navios = new List<Navio>();
    private static List<Container> containers = new List<Container>();
    private static List<string> pontos = new List<string>();
    private static List<int> fila = new List<int>();
    private static List<int> fila1 = new List<int>();
    private static List<int> fila2 = new List<int>();
    private static List<int> fila3 = new List<int>();
    private void adicionarNavio(double carga, List<string> pontos)
    {
        navios.Add(new Navio(carga, pontos));
    }

    private void adicionarContainer(string ponto, double carga)
    {
        containers.Add(new Container(ponto, carga));
    }

    private void adicionarContainerFila(int idContainer)
    {
        int limite = 3;
        if (fila1.Count < limite)
        {
            fila1.Add(idContainer);
        }
        else if (fila2.Count < limite)
        {
            fila2.Add(idContainer);
        }
        else if (fila3.Count < limite)
        {
            fila3.Add(idContainer);
        }
    }

    // Retorna a lista de navios
    [HttpGet("ListaNavios")]
    public ActionResult<List<Navio>> ListaNavios()
    {
        return Ok(navios);
    }

    // Retorna a lista de containers
    [HttpGet("ListaContainers")]
    public ActionResult<List<Container>> ListaContainers()
    {
        return Ok(containers);
    }

    // cadastroNavio
    [HttpPost("cadastroNavio")]
    public ActionResult<List<Navio>> adicionarNavio(Navio navio)
    {
        if (navio is null)
        {
            return StatusCode(204, "Valores inválidos!");
        }
        if (navio.CargaMaxima <= 0)
        {
            return StatusCode(204, "Valores inválidos!");
        }
        if (!navio.ListaPontos.Any())
        {
            return StatusCode(204, "Valores inválidos!");
        }
        foreach (string i in navio.ListaPontos)
        {
            i.ToUpper();
            if (i != "A" && i != "B" && i != "C" && i != "D")
            {
                return StatusCode(204, "Valores inválidos!");
            }
        }
        adicionarNavio(navio.CargaMaxima, navio.ListaPontos);
        return Ok(navios);
    }

    // alterarNavio
    [HttpPut("alterarNavio/{id}")]
    public ActionResult<bool> alterarNavio(int id, Navio navio)
    {
        Navio aux = null;
        try
        {
            aux = getNavioById(id);
        }
        catch (System.Exception)
        {
            return Problem("500", "Erro desconhecido.");
        }
        if (aux == null)
        {
            return NotFound();
        }
        if (navio.CargaMaxima > 0)
        {
            aux.CargaMaxima = navio.CargaMaxima;
        }
        else
        {
            return StatusCode(204, "Valores inválidos!");
        }
        return Ok(navios);
    }

     // filaEmbarque
    [HttpPost("filaEmbarque")]
    public ActionResult<(List<Navio>, List<Container>, List<int>, List<int>, List<int>)> adicionarContainerFila(Container container)
    {
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
        adicionarContainer(container.Ponto.ToUpper(), container.Carga);
        adicionarContainerFila(containers.Last().Id);
        return (navios, containers, fila1, fila2, fila3);
    }

    // alfandega
    [HttpPut("alfandega/{id}")]
    public ActionResult<bool> alterarContainer(int id, Container container)
    {
        Container aux = null;
        try
        {
            aux = getContainerById(id);
        }
        catch (System.Exception)
        {
            return Problem("500", "Erro desconhecido.");
        }
        if (aux == null)
        {
            return NotFound();
        }
        if (container.Carga > 0)
        {
            aux.Carga = container.Carga;
        }
        else
        {
            return StatusCode(204, "Valores inválidos!");
        }

        if (container.Ponto != null)
        {
            container.Ponto = container.Ponto.ToUpper();
            if (container.Ponto == "A" || container.Ponto == "B" || container.Ponto == "C" || container.Ponto == "D")
            {
                aux.Ponto = container.Ponto;
            }
            else
            {
                return StatusCode(204, "Valores inválidos!");
            }
        }
        else
        {
            return StatusCode(204, "Valores inválidos!");
        }
        return Ok(containers);
    }

    // confisco
    [HttpDelete("confisco/{id}")]
    public ActionResult<Boolean> Confisco(int id)
    {
        Container x = null;
        foreach (Container i in containers)
        {
            if (i.Id == id)
            {
                x = i;
            }
        }
        if (x != null)
        {
            containers.Remove(x);
            return Ok();
        }
        return false;
    }

    // carregamento
    [HttpGet("carregamento")]
    public ActionResult<List<Navio>> Carregamento()
    {
        try
        {
            if (containers == null || !containers.Any())
            {
                return StatusCode(400, "Não existem containers cadastrados na lista!");
            }
            double cargaTemp = 0;

            foreach (Navio n in navios)
            {
                double cargaAtual = 0;
                foreach (Container c in n.ListaContainers)
                {
                    cargaAtual += c.Carga;
                }
                if (containers != null && containers.Any())
                {
                    foreach (Container c in containers)
                    {
                        if (n.ListaPontos.Contains(c.Ponto))
                        {
                            if (cargaTemp + c.Carga <= n.CargaMaxima && (cargaAtual + c.Carga) < n.CargaMaxima)
                            {
                                n.ListaContainers.Add(c);
                                cargaTemp += c.Carga;
                            }
                        }
                    }
                    foreach (Container c in n.ListaContainers)
                    {
                        containers.Remove(c);
                    }
                }
                n.ListaContainers = n.ListaContainers.OrderBy(x => x.Ponto).ToList();
            }
            return Ok(navios);
        }
        catch (System.Exception)
        {
            return Problem("Erro ao mostar a lista ordenada dos containers!");
        }
    }

    // descarregamento
    [HttpGet("descarregamento/{id}")]
    public ActionResult<List<Container>> Descarregamento(int id)
    {
        try
        {
            Container x = null;
            Navio y = null;
            foreach (Navio n in navios)
            {
                foreach (Container c in n.ListaContainers)
                {
                    if (c.Id == id)
                    {
                        x = c;
                        y = n;
                        break;
                    }
                    else
                    {
                        return StatusCode(400, "Não existem esse id de container cadastrado!");
                    }
                }
            }

            if (x != null && y != null)
            {
                y.ListaContainers.Remove(x);
                y.ListaContainers = y.ListaContainers.OrderBy(x => x.Ponto).ToList();
                return Ok(navios);
            }
            else
            {
                return NotFound(navios);
            }
        }
        catch (System.Exception)
        {
            return Problem("problema ao descarregar o container");
        }

    }

    // Retorna a fila de containers
    [HttpGet("Fila")]
    public ActionResult<List<int>> Fila()
    {
        return Ok(fila);
    }

    private Navio getNavioById(int id)
    {
        foreach (Navio i in navios)
        {
            if (i.Id == id)
            {
                return i;
            }
        }
        return null;
    }

    private Container getContainerById(int id)
    {
        foreach (Container i in containers)
        {
            if (i.Id == id)
            {
                return i;
            }
        }
        return null;
    }
}