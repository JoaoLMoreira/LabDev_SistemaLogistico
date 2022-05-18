using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;

namespace SistemaLogistico.Controllers;

[ApiController]
[Route("[controller]")]

public class NavioController : ControllerBase {

    private double ValidaFloat(string valor) {

        valor = valor.Replace(",", ".");

        double retorno = double.Parse(valor, CultureInfo.InvariantCulture.NumberFormat);

        return retorno;

    }

    private static List<Navio> navios = new List<Navio>();

    private static List<Container> containers = new List<Container>();

    private static List<int> fila = new List<int>();
    private static List<Container> filaprioridade = new List<Container>();

    private void adicionarNavio(string ponto, double carga, List<Container> container) {
        
        navios.Add(new Navio(ponto, carga, container));

    }

    private void adicionarContainer(string ponto, double carga) {
        
        containers.Add(new Container(ponto, carga));

    }

    private void adicionarContainerFila(int idContainer) {

        fila.Add(idContainer);

    }

    // cadastroNavio
    [HttpPost("cadastroNavio")]
    public ActionResult<List<Navio>> adicionarNavio(Navio navio) {

        if (navio is null) {
            return StatusCode(204, "Valores inválidos!");
        }

        if (navio.CargaMaxima <= 0) {
            return StatusCode(204, "Valores inválidos!");
        }

        if (navio.Ponto == null) {
            return StatusCode(204, "Valores inválidos!");
        }

        adicionarNavio(navio.Ponto, navio.CargaMaxima, navio.ContainersLista);

        return Ok(navios);

    }

    // alterarNavio
    [HttpPut("alterarNavio/{id}")]
    public ActionResult<bool> alterarNavio(int id, Navio navio) {

        Navio aux = null;

        try {

            aux = getNavioById(id);

        }

        catch (System.Exception) {
            
            return Problem("500", "Erro desconhecido.");

        }

        if (aux == null) {
            return NotFound();
        }

        if (navio.CargaMaxima > 0) {
            aux.CargaMaxima = navio.CargaMaxima;
        }

        else {
            return StatusCode(204, "Valores inválidos!");
        }

        if (navio.Ponto != null) {

            navio.Ponto = navio.Ponto.ToUpper();

            if (navio.Ponto == "A" || navio.Ponto == "B" || navio.Ponto == "C" || navio.Ponto == "D") {

                aux.Ponto = navio.Ponto;

            }

            else {

                return StatusCode(204, "Valores inválidos!");

            }
            
        }

        else {
            return StatusCode(204, "Valores inválidos!");
        }

        return Ok(navios);

    }

    // filaEmbarque
    [HttpPost("filaEmbarque")]
    public ActionResult<(List<Navio>, List<Container>, List<int>)> adicionarContainerFila(Container container) {

        if (container is null) {
            return StatusCode(204, "Valores inválidos!");
        }

        if (container.Carga <= 0) {
            return StatusCode(204, "Valores inválidos!");
        }

        if (container.Ponto == null) {
            return StatusCode(204, "Valores inválidos!");
        }

        adicionarContainer(container.Ponto, container.Carga);

        adicionarContainerFila(containers.Last().Id);

        return (navios, containers, fila);

    }

    // alfandega
    [HttpPut("alfandega/{id}")]
    public ActionResult<bool> alterarContainer(int id, Container container) {

        Container aux = null;

        try {

            aux = getContainerById(id);

        }

        catch (System.Exception) {
            
            return Problem("500", "Erro desconhecido.");

        }

        if (aux == null) {
            return NotFound();
        }

        if (container.Carga > 0) {
            aux.Carga = container.Carga;
        }

        else {
            return StatusCode(204, "Valores inválidos!");
        }

        if (container.Ponto != null) {

            container.Ponto = container.Ponto.ToUpper();

            if (container.Ponto == "A" || container.Ponto == "B" || container.Ponto == "C" || container.Ponto == "D") {

                aux.Ponto = container.Ponto;

            }

            else {

                return StatusCode(204, "Valores inválidos!");

            }
            
        }

        else {
            return StatusCode(204, "Valores inválidos!");
        }

        return Ok(containers);

    }

    // Retorna a lista de navios
    [HttpGet("ListaNavios")]
    public ActionResult<List<Navio>> ListaNavios() {

        return Ok(navios);

    }

    // Retorna a lista de containers
    [HttpGet("ListaContainers")]
    public ActionResult<List<Container>> ListaContainers() {

        return Ok(containers);

    }


     [HttpDelete("{id}")]
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

    // carregamento do containers 
    [HttpGet]
    public ActionResult<List<Container>> Carregamento()
    {
        try
        {
            if(containers == null){
                return StatusCode(400, "Não existem containers cadastrados na lista");
            }

            var filaprioridade = containers.OrderBy(a => a.Carga).ToList();

            return filaprioridade;
        }
        catch (System.Exception)
        {
            return Problem("Erro ao mostar a lista ordenada dos containers");
        }
    }

    [HttpGet("{id}")]
    public ActionResult<List<Container>> Descarregamento(int id){
        try {
        Container x = null;
        foreach (Container i in containers)
        {
            if (i.Id == id)
            { 
                x = i; 
            }
            else{
                return StatusCode(400, "Não existem esse id de container cadastrado");
            }

        }
        if (x != null)
        {
            containers.Remove(x);
            var filaprioridade = containers.OrderBy(a => a.Carga).ToList();
            return filaprioridade;
        }
        else{
           return containers;
        }
        }
        catch (System.Exception)
        {
            return Problem("problema ao descarregar o container");
        }
    }

    // Retorna a fila de containers
    [HttpGet("Fila")]
    public ActionResult<List<int>> Fila() {

        return Ok(fila);

    }

    private Navio getNavioById(int id) {

        foreach (Navio i in navios) {

            if (i.Id == id) {
                return i;
            }

        }

        return null;

    }

    private Container getContainerById(int id) {

        foreach (Container i in containers) {

            if (i.Id == id) {
                return i;
            }

        }

        return null;

    }




}