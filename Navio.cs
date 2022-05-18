namespace SistemaLogistico;

public class Navio {

    public Navio() {
        ContainersLista = new List<Container>();
    }

    public Navio (string ponto, double carga, List<Container> containers) {

        Id = id + 1;
        Ponto = ponto;
        CargaMaxima = carga;
        ContainersLista = new List<Container>();
        id += 1;

    }

    static int id = 0;
    public int Id {get; set;}
    public string? Ponto {get; set;}
    public double CargaMaxima {get; set;}
    public List<Container> ContainersLista {get; set;}

}
