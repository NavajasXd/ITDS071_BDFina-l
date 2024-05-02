using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace ITDS071_BDFina_l
{
    internal class Program
    {
        public struct Tabla
        {
            public Tabla(string nombre, string[] titulos, List<string[]> registros, string ruta)
            {
                Nombre = nombre;
                Titulos = titulos;
                Registros = registros;
                Ruta = ruta;
            }

            public string Nombre {  get; set; }
            public string[] Titulos { get; set; }
            public List<String[]> Registros { get; set; }
            public string Ruta { get; set; }
        };
            

        static void Main(string[] args)
        {
            Tabla Ingredientes = new Tabla(
                "Ingredientes",
                new string[] {"Ingrediente_ID", "Nombre", "Contenido", "Cantidad"},
                new List<string[]> { },
                Path.GetFullPath("..\\..\\Datos\\BD_Ingredientes.txt")
                );
            leerArchivo(Ingredientes);

            Tabla Platillos = new Tabla(
                "Platillos",
                new string[] { "Platillo_ID", "Nombre", "Precio", "Costo" },
                new List<string[]> { },
                Path.GetFullPath("..\\..\\Datos\\BD_Platillos.txt")
                );
            leerArchivo(Platillos);

            Tabla Recetas = new Tabla(
                "Recetas",
                new string[] {"Platillo_ID", "Ingrediente_ID", "Cantidad"},
                new List<string[]> { },
                Path.GetFullPath("..\\..\\Datos\\BD_Recetas.txt")
                );
            leerArchivo(Recetas);

            dibujarTabla(Recetas, 1, 1);
            Console.ReadKey(true);
        }

        static void leerArchivo(Tabla tabla)
        {
            if (!File.Exists(tabla.Ruta))
            {
                try
                {
                    FileStream BD = File.Create(tabla.Ruta);
                    BD.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al crear el archivo: " + ex.ToString());
                }
                return;
            }
            // Si ya existe el archivo, actualizar tabla.Registros con toda la informacion del archivo
            string[] lineas = File.ReadAllLines(tabla.Ruta);
            foreach (string linea in lineas)
            {
                string[] datos = linea.Split(',');
                tabla.Registros.Add(datos);
            }
        }

        static void dibujarTabla(Tabla tabla, int x, int y)
        {
            // Dibujar Encabezado
            int filaActual = y;
            Console.SetCursorPosition(x, filaActual);
            Console.Write($"{tabla.Nombre}");
            filaActual++;

            string bordeSuperior = "╔";
            string titulosEnTabla = "║";
            string bordeInferior = "╚";

            string ultimo = tabla.Titulos.Last();
            foreach (string titulo in tabla.Titulos)
            {
                if (titulo == ultimo)
                {
                    bordeSuperior += "═══════════════╗";
                    if (titulo.Length >= 15) titulosEnTabla += titulo.Remove(15);
                    else titulosEnTabla += titulo.PadRight(15, ' ');
                    bordeInferior += "═══════════════╝";
                }
                else
                {
                    bordeSuperior += "═══════════════╦";
                    if (titulo.Length >= 15) titulosEnTabla += titulo.Remove(15);
                    else titulosEnTabla += titulo.PadRight(15, ' ');
                    bordeInferior += "═══════════════╩";
                }
                titulosEnTabla += "║";
            }
            Console.SetCursorPosition(x, filaActual);
            Console.Write(bordeSuperior);
            filaActual++;
            Console.SetCursorPosition(x, filaActual);
            Console.Write(titulosEnTabla);
            filaActual++;

            // Dibujar registros
            if (tabla.Registros.Count > 0)
            {
                string bordeRegistro = "╠";
                string lineaRegistro = "║";
                foreach (string[] registro in tabla.Registros)
                {
                    ultimo = registro.Last();
                    foreach (string dato in registro)
                    {
                        if (dato == ultimo)
                        {
                            bordeRegistro += "═══════════════╣";
                            if (dato.Length >= 15) lineaRegistro += dato.Remove(15);
                            else lineaRegistro += dato.PadRight(15, ' ');
                        }
                        else
                        {
                            bordeRegistro += "═══════════════╬";
                            if (dato.Length >= 15) lineaRegistro += dato.Remove(15);
                            else lineaRegistro += dato.PadRight(15, ' ');
                        }
                        lineaRegistro += "║";
                    }
                }
                Console.SetCursorPosition(x, filaActual);
                Console.Write(bordeRegistro);
                filaActual++;
                Console.SetCursorPosition(x, filaActual);
                Console.Write(lineaRegistro);
                filaActual++;
            }

            // Dibujar borde inferior
            Console.SetCursorPosition(x, filaActual);
            Console.Write(bordeInferior);
        }
    }
}
