﻿using System;
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
                new string[] {"ID", "Nombre", "Contenido", "Cantidad"},
                new List<string[]> {},
                Path.GetFullPath("..\\..\\Datos\\BD_Ingredientes.txt")
                );
            LeerArchivo(Ingredientes);

            Tabla Platillos = new Tabla(
                "Platillos",
                new string[] { "ID", "Nombre", "Precio", "Costo" },
                new List<string[]> { },
                Path.GetFullPath("..\\..\\Datos\\BD_Platillos.txt")
                );
            LeerArchivo(Platillos);

            Tabla Recetas = new Tabla(
                "Recetas",
                new string[] {"Platillo_ID", "Ingrediente_ID", "Cantidad"},
                new List<string[]> { },
                Path.GetFullPath("..\\..\\Datos\\BD_Recetas.txt")
                );
            LeerArchivo(Recetas);
            Console.ReadKey();
            Console.WriteLine(Console.WindowWidth);
            Console.ReadKey();
        }

        static void LeerArchivo(Tabla tabla)
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

        
    }
}
