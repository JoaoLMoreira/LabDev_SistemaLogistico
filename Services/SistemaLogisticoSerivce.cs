
namespace SistemaLogistico.Services
{
    public class NavioController : iNavioController
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
}