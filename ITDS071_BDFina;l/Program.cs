using Microsoft.Win32;
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

            public string Nombre { get; set; }
            public string[] Titulos { get; set; }
            public List<String[]> Registros { get; set; }
            public string Ruta { get; set; }
            public int FilaSeleccionada { get; set; }
        }

        public struct Opciones
        {
            public Opciones(string[] lista)
            {
                Lista = lista;
                FilaSeleccionada = 0;
            }
            public string[] Lista { get; set; }
            public int FilaSeleccionada { get; set; }
        }


        static void Main(string[] args)
        {
            Tabla Ingredientes = new Tabla(
                "Ingredientes",
                new string[] { "Ingrediente_ID", "Nombre", "Contenido", "Cantidad", "Costo/Cantidad" },
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
                new string[] { "Platillo_ID", "Ingrediente_ID", "Cantidad" },
                new List<string[]> { },
                Path.GetFullPath("..\\..\\Datos\\BD_Recetas.txt")
                );
            leerArchivo(Recetas);

            Tabla[] Bases = new Tabla[] { Ingredientes, Platillos, Recetas };
            Opciones Contenido = new Opciones(new string[] {"Kilo", "Litro"});
            Opciones Tablas = new Opciones(new string[] { Ingredientes.Nombre, Platillos.Nombre, Recetas.Nombre });

            while (true)
            {
                // Menu Principal
                Console.Clear();
                Console.SetCursorPosition(1, 1);
                Console.Write("Bienvenido");
                Console.SetCursorPosition(1, 2);
                Console.Write("Escoja una base de datos a la que se quiera conectar");
                Console.SetCursorPosition(1, Console.WindowHeight - 1);
                Console.Write("[Up-Arrow] Seleccionar [Down-Arrow] Seleccionar [Enter] Entrar [Escape] Salir");

                Tabla tablaSeleccionada = Ingredientes;
                ConsoleKeyInfo tecla;
                Console.SetCursorPosition(40, 10);
                string tabla = dibujarTabla(Tablas);

                
                switch (tabla)
                {
                    case "Ingredientes":
                        tablaSeleccionada = Bases[0];
                        break;
                    case "Platillos":
                        tablaSeleccionada = Bases[1];
                        break;
                    case "Recetas":
                        tablaSeleccionada= Bases[2];
                        break;
                    case "":
                        return;
                }

                Opciones campos = new Opciones(new string[tablaSeleccionada.Titulos.Count()]);
                for (int i = 0; i < tablaSeleccionada.Titulos.Length; i++)
                {
                    campos.Lista[i] = tablaSeleccionada.Titulos[i];
                }

                // Menu de tabla
                while (true)
                {
                    Opciones Platillo = actualizarOpciones(Platillos);
                    Opciones Ingrediente = actualizarOpciones(Ingredientes);
                    Console.Clear();
                    calcularCostos(Ingredientes, Recetas, Platillos);
                    dibujarTabla(tablaSeleccionada, 1, 1);
                    Console.SetCursorPosition(1, Console.WindowHeight - 1);
                    Console.Write("[Up-Arrow] Seleccionar [Down-Arrow] Seleccionar [+] Agregar [E] Editar [Backspace] Eliminar [Escape] Salir");
                    tecla = Console.ReadKey(true);
                    if (tecla.Key == ConsoleKey.Escape) break;
                    switch (tecla.Key)
                    {
                        case ConsoleKey.UpArrow:
                            tablaSeleccionada.FilaSeleccionada = seleccionarRegistro(tablaSeleccionada, -1);
                            break;
                        case ConsoleKey.DownArrow:
                            tablaSeleccionada.FilaSeleccionada = seleccionarRegistro(tablaSeleccionada, 1);
                            break;
                        case ConsoleKey.Add:
                            if (tablaSeleccionada.Nombre == "Recetas") agregarRegistro(tablaSeleccionada, 83, 1, Platillo, Ingrediente, Platillos, Ingredientes);
                            else agregarRegistro(tablaSeleccionada, 83, 1, Contenido, Platillo);
                            break;
                        case ConsoleKey.E:
                            Console.SetCursorPosition(83, 1);
                            string campo = dibujarTabla(campos);
                            int campoID = 0;
                            for (int i = 0; i < tablaSeleccionada.Titulos.Length; i++)
                            {
                                if(campo == tablaSeleccionada.Titulos[i])
                                {
                                    campoID = i; break;
                                }
                            }
                            Console.SetCursorPosition(83, 2);
                            Console.Write("Inserte el nuevo dato");
                            string nuevoDato = Console.ReadLine();
                            editarRegistro(tablaSeleccionada, campoID, nuevoDato);
                            break;
                        case ConsoleKey.Backspace:
                            eliminarRegistro(tablaSeleccionada);
                            break;
                        default:
                            Console.SetCursorPosition(80, 1);
                            Console.Write("Inserte una tecla valida");
                            break;

                    }
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
                    if (datoFinal.Length > 15) datoFinal = datoFinal.Remove(15);
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

        static string dibujarTabla(Opciones opciones)
        {
            int x = Console.CursorLeft;
            int y = Console.CursorTop;
            int filaActual;
            ConsoleKeyInfo tecla;

            while (true)
            {
                filaActual = y;

                // Borde superior
                Console.SetCursorPosition(x, filaActual);
                Console.Write("╔════════════════════╗");
                filaActual++;
                // Opciones
                int indiceOpcion = 0;
                foreach (string dato in opciones.Lista)
                {
                    if (indiceOpcion == opciones.FilaSeleccionada) Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.SetCursorPosition(x, filaActual);
                    string datoFinal = dato;
                    if (datoFinal.Length >= 20) datoFinal = datoFinal.Remove(20);
                    Console.Write($"║{datoFinal.PadRight(20, ' ')}║");
                    filaActual++;
                    indiceOpcion++;
                    Console.ResetColor();
                }
                // Borde inferior
                Console.SetCursorPosition(x, filaActual);
                Console.Write("╚════════════════════╝");

                tecla = Console.ReadKey();

                if (tecla.Key == ConsoleKey.Enter) break;
                if (tecla.Key == ConsoleKey.Escape) return "";
                switch(tecla.Key)
                {
                    case ConsoleKey.UpArrow:
                        opciones.FilaSeleccionada = seleccionarRegistro(opciones, -1);
                        break;
                    case ConsoleKey.DownArrow:
                        opciones.FilaSeleccionada = seleccionarRegistro(opciones, 1);
                        break;
                }
            }

            // Limpiar espacio en tabla
            filaActual = y;
            Console.SetCursorPosition(x, filaActual);
            Console.Write("                      ");
            filaActual++;
            foreach(string dato in opciones.Lista)
            {
                Console.SetCursorPosition(x, filaActual);
                Console.Write("                      ");
                filaActual++;
            }
            Console.SetCursorPosition(x, filaActual);
            Console.Write("                      ");
            // Escribir registro agregado en la seccion de utilidades
            filaActual = y;
            Console.SetCursorPosition(x, filaActual);
            Console.Write($"");

            return opciones.Lista[opciones.FilaSeleccionada];
        }

        static void agregarRegistro(Tabla tabla, int x, int y, Opciones opciones1, Opciones opciones2, Tabla platillosAuxiliar = new Tabla(), Tabla ingredientesAuxiliar = new Tabla())
        {
            string[] datos = new string[tabla.Titulos.Length];

            if(tabla.Nombre == "Recetas")
            {
                for(int i = 0; i < tabla.Titulos.Length; i++)
                {
                    if (tabla.Titulos[i] == "Platillo_ID")
                    {
                        Console.SetCursorPosition(x, y + i);
                        string dato = dibujarTabla(opciones1);
                        foreach (string[] platillo in platillosAuxiliar.Registros)
                        {
                            if(dato == platillo[1])
                            {
                                datos[i] = platillo[0];
                                break;
                            }
                        }
                        Console.Write($"{tabla.Titulos[i]}: {datos[i]}");
                    }
                    else if (tabla.Titulos[i] == "Ingrediente_ID")
                    {
                        Console.SetCursorPosition(x, y + i);
                        string dato = dibujarTabla(opciones2);
                        foreach (string[] ingrediente in ingredientesAuxiliar.Registros)
                        {
                            if(dato == ingrediente[1])
                            {
                                datos[i] = ingrediente[0];
                            }
                        }
                        Console.Write($"{tabla.Titulos[i]}: {datos[i]}");
                    }
                    else
                    {
                        while (true)
                        {
                            Console.SetCursorPosition(x, y + i);
                            Console.Write($"{tabla.Titulos[i]}: ");
                            string dato = Console.ReadLine();
                            double datoNumerico;
                            if (double.TryParse(dato, out datoNumerico))
                            {
                                datos[i] = dato;
                                break;
                            }
                            else
                            {
                                Console.SetCursorPosition(x, 20);
                                Console.Write("Inserte unicamente valores numericos");
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < tabla.Titulos.Length; i++)
                {
                    if (tabla.Titulos[i] == "Costo")
                    {
                        datos[i] = "0";
                    }
                    else if (tabla.Titulos[i] == "Contenido")
                    {
                        Console.SetCursorPosition(x, y + i);
                        datos[i] = dibujarTabla(opciones1);
                        Console.Write($"{tabla.Titulos[i]}: {datos[i]}");
                    }
                    else if (tabla.Titulos[i] == "Costo/Cantidad")
                    {
                        while(true)
                        {
                            Console.SetCursorPosition(x, y + i);
                            Console.Write($"{tabla.Titulos[i]}: ");
                            string dato = Console.ReadLine();
                            double datoNumerico;
                            if (double.TryParse(dato, out datoNumerico))
                            {
                                datos[i] = dato;
                                break;
                            }
                            else
                            {
                                Console.SetCursorPosition(x, 20);
                                Console.Write("Inserte unicamente valores numericos");
                            }
                        }
                    }
                    else
                    {
                        Console.SetCursorPosition(x, y + i);
                        Console.Write($"{tabla.Titulos[i]}: ");
                        datos[i] = Console.ReadLine();
                    }
                }
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

        static int seleccionarRegistro(Opciones opciones, int contador)
        {
            int cantidadDeRegistros = opciones.Lista.Length;
            if (cantidadDeRegistros == 0) return 0;
            int filaSeleccionada = opciones.FilaSeleccionada;
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

        static void calcularCostos(Tabla Ingredientes, Tabla Recetas, Tabla Platillos)
        {
            // registro = [platillo_id, nombre, precio, costo]
            // ingrediente = [ingrediente_id, nombre, contenido, cantidad, precio/cantidad]
            // receta = [platillo_id, ingrediente_id, cantidad]
            foreach (string[] registro in Platillos.Registros)
            {
                double costo = 0;
                foreach (string[] receta in Recetas.Registros)
                {
                    if (registro[0] == receta[0])
                    {
                        foreach(string[] ingrediente in Ingredientes.Registros)
                        {
                            if (receta[1] == ingrediente[0])
                            {
                                double costoXCantidad = double.Parse(ingrediente[4]);
                                double cantidad = double.Parse(receta[2]);
                                costo += costoXCantidad * cantidad;
                            }
                        }
                    }
                }
                registro[3] = costo.ToString();
            }
        }

        static Opciones actualizarOpciones(Tabla tabla)
        {
            Opciones opciones = new Opciones(new string[tabla.Registros.Count]);
            for(int i = 0; i < tabla.Registros.Count; i++)
            {
                opciones.Lista[i] = tabla.Registros[i][1];
            }
            return opciones;
        }
    }
}

