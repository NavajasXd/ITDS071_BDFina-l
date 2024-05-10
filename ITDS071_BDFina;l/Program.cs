using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using static ITDS071_BDFina_l.Program;

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

            Tabla Tablas = new Tabla(
                "Tablas",
                new string[] { "#", "Tabla" },
                new List<string[]> { new string[] { "1", "Ingredientes" },
                                     new string[] { "2", "Platillos"},
                                     new string[] { "3", "Recetas"}
                },
                ""
                );

            while (true)
            {
                Console.Clear();
                int tablaSeleccionada = menuPrincipal(Tablas, 40, 8);
                if (tablaSeleccionada == -1) break;
                Console.Clear();
                switch (tablaSeleccionada)
                {
                    case 0:
                        menuTablas(Ingredientes);
                        break;
                    case 1:
                        menuTablas(Platillos);
                        break;
                    case 2:
                        menuTablas(Recetas);
                        break;
                    default:
                        break;
                }
            
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

        static void agregarRegistro(Tabla tabla, int x, int y)
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
            for (int i = 0; i < tabla.Registros.Count; i++)
            {
                if (datos[0] == tabla.Registros[i][0]) return;
            }
            string linea = string.Join(",", datos) + "\n";
            File.AppendAllText(tabla.Ruta, linea);
            tabla.Registros.Add(datos);
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
            if (tabla.Registros.Count == 0) return;
            // Elimina el registro seleccionado del archivo
            string[] lineas = File.ReadAllLines(tabla.Ruta);
            List<string> nuevasLineas = new List<string>();
            for (int i = 0; i < lineas.Length; i++)
            {
                if (i != tabla.FilaSeleccionada)
                {
                    nuevasLineas.Add(lineas[i]);
                }
            }
            File.WriteAllLines(tabla.Ruta, nuevasLineas);

            // Elimina el registro seleccionado de la tabla en memoria
            tabla.Registros.RemoveAt(tabla.FilaSeleccionada);
        }

        static void editarRegistro(Tabla tabla, int datoViejo, string datoNuevo)
        {
            tabla.Registros[tabla.FilaSeleccionada][datoViejo] = datoNuevo;
            string[] lineas = File.ReadAllLines(tabla.Ruta);
            string[] lineaNueva = lineas[tabla.FilaSeleccionada].Split(',');
            lineaNueva[datoViejo] = datoNuevo;
            lineas[tabla.FilaSeleccionada] = string.Join(",", lineaNueva);
            File.WriteAllLines(tabla.Ruta, lineas);
        }

        static int menuPrincipal(Tabla Tablas, int x, int y)
        {
            Console.SetCursorPosition(1, 1);
            Console.Write("Bienvenido careverga");
            Console.SetCursorPosition(1, 2);
            Console.Write("Escoja una base de datos a la que se quiera conectar");

            ConsoleKeyInfo tecla;
            while (true)
            {
                dibujarTabla(Tablas, 40, 10);
                tecla = Console.ReadKey(true);
                switch (tecla.Key)
                {
                    case ConsoleKey.UpArrow:
                        Tablas.FilaSeleccionada = seleccionarRegistro(Tablas, -1);
                        break;
                    case ConsoleKey.DownArrow:
                        Tablas.FilaSeleccionada = seleccionarRegistro(Tablas, 1);
                        break;
                    case ConsoleKey.Escape:
                        return -1;
                    case ConsoleKey.Enter:
                        return Tablas.FilaSeleccionada;
                    default:
                        Console.SetCursorPosition(80, 1);
                        Console.Write("Inserte una tecla válida");
                        break;
                }
            }
        }

        static int menuTablas(Tabla tabla)
        {
            ConsoleKeyInfo tecla;

            while(true)
            {
                Console.Clear();
                dibujarTabla(tabla, 1, 1);
                tecla = Console.ReadKey(true);
                switch (tecla.Key)
                {
                    case ConsoleKey.Add:
                        agregarRegistro(tabla, 80, 1);
                        break;
                    case ConsoleKey.Backspace:
                        eliminarRegistro(tabla);
                        break;
                    case ConsoleKey.UpArrow:
                        tabla.FilaSeleccionada = seleccionarRegistro(tabla, -1);
                        break;
                    case ConsoleKey.DownArrow:
                        tabla.FilaSeleccionada = seleccionarRegistro(tabla, 1);
                        break;
                    case ConsoleKey.E:
                        editarRegistro(tabla, 1, "1");
                        break;
                    case ConsoleKey.Escape:
                        return 0;
                    default:
                        Console.SetCursorPosition(80, 1);
                        Console.Write("Inserte una tecla válida");
                        break;
                }
            }
        }
    }
}
