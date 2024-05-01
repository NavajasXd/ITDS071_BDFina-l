using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace ITDS071_BDFina_l
{
    internal class Program
    {
        public struct Tabla
        {
            public Tabla(string nombre, string[] titulos, List<string> registros, string ruta)
            {
                Nombre = nombre;
                Titulos = titulos;
                Registros = registros;
                Ruta = ruta;
            }
            public string Nombre {  get; set; }
            public string[] Titulos { get; set; }
            public List<String> Registros { get; set; }
            public string Ruta { get; set; }
        };
            

        static void Main(string[] args)
        {
            Tabla Ingredientes = new Tabla(
                "Ingredientes",
                new string[] {"ID", "Nombre"},
                new List<string> {},
                "C:\\Users\\bmigu\\source\\repos\\ITDS071_BDFina;l\\ITDS071_BDFina;l\\BD_Ingredientes.txt"
                );

            Console.WriteLine(Ingredientes.Nombre);
            foreach (string titulo in Ingredientes.Titulos)
            {
                Console.WriteLine(titulo);
            }
        }

        static void IngresarRegistrosATabla(Tabla tabla, string ruta)
        {

        }
    }
}
