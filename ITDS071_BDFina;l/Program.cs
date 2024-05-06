using System;
using System.Collections.Generic;
using System.Data.Common;
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
                FilaSeleccionada = 0;
            }

            public string Nombre {  get; set; }
            public string[] Titulos { get; set; }
            public List<String[]> Registros { get; set; }
            public string Ruta { get; set; }
            public int FilaSeleccionada { get; set; }
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

            while (true)
            {
                Console.Clear();
                dibujarTabla(Ingredientes, 1, 1);
                string tecla = Console.ReadLine();
                if (tecla == "x")
                {
                    eliminarRegistro(Ingredientes);
                    Ingredientes.FilaSeleccionada = seleccionarRegistro(Ingredientes, 0);
                    continue;
                }
                int contador = int.Parse(tecla);
                Ingredientes.FilaSeleccionada = seleccionarRegistro(Ingredientes, contador);
            }
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
            int indiceRegistro = 0;
            foreach (string[] registro in tabla.Registros)
            {
                Console.SetCursorPosition(x, filaActual);
                Console.Write("╠"); 
                for (int i = 0; i < tabla.Titulos.Length; i++)
                {
                    if (i == tabla.Titulos.Length - 1)
                    {
                        Console.Write("═══════════════╣");
                    }
                    else
                    {
                        Console.Write("═══════════════╬");
                    }
                }
                filaActual++;
                Console.SetCursorPosition(x, filaActual);
                if (indiceRegistro == tabla.FilaSeleccionada) Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.Write("║");
                foreach (string dato in registro)
                {
                    string datoFinal = dato;
                    if (datoFinal.Length >= 15) datoFinal = datoFinal.Remove(15); 
                    Console.Write($"{datoFinal.PadRight(15, ' ')}║");
                }
                filaActual++;
                indiceRegistro++;
                Console.ResetColor();
            }

            // Dibujar borde inferior
            Console.SetCursorPosition(x, filaActual);
            Console.Write(bordeInferior);
        }

        static string[] agregarRegistro(Tabla tabla, int x, int y, char v)
        {
            string[] datos = new string[tabla.Titulos.Length];
            for (int i = 0; i < tabla.Titulos.Length; i++)
            {
                if (tabla.Titulos[i] == "Costo")
                {
                    datos[i] = "0";
                }
                else
                {
                    Console.SetCursorPosition(x, y + i);
                    Console.Write($"{tabla.Titulos[i]}: ");
                    datos[i] = Console.ReadLine();
                }
            }
            string linea = "\n" + string.Join(",", datos);
            File.AppendAllText(tabla.Ruta, linea);
            return datos;
        }

        static int seleccionarRegistro(Tabla tabla, int contador)
        {
            int cantidadDeRegistros = tabla.Registros.Count();
            if (cantidadDeRegistros == 0) return 0;
            int filaSeleccionada = tabla.FilaSeleccionada;
            filaSeleccionada += contador;

            if (filaSeleccionada < 0) filaSeleccionada = cantidadDeRegistros - 1;
            filaSeleccionada = filaSeleccionada % cantidadDeRegistros;
            
            return filaSeleccionada;
        }

        static void eliminarRegistro(Tabla tabla)
        {
            tabla.Registros.RemoveAt(tabla.FilaSeleccionada);
        }
    }
}
