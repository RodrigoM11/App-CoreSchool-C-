using System;
using System.Collections.Generic;
using CoreEscuela.Util;

namespace CoreEscuela.Entidades
{
    public class Escuela : ObjetoEscuelaBase, Ilugar
    {

        public int AñoDeCreación { get; set; }
        public string Pais { get; set; }
        public string Ciudad { get; set; }
        public string Direccion { get; set; }
        public TiposEscuela TipoEscuela { get; set; }
        public List<Curso> Cursos { get; set; }

        public Escuela 
        (
            string nombre = "",
            string pais = "",
            string ciudad = "",
            string direccion = "",
            int año = 0,
            TiposEscuela tipo = TiposEscuela.Secundaria
        ) : base()
        
        {
            Nombre = nombre;
            AñoDeCreación = año;
            TipoEscuela = tipo;
            Pais = pais;
            Ciudad = ciudad;
            Direccion = direccion;
            Cursos = new List<Curso>();
        }
        public override string ToString()
        {
            return $"Nombre: \"{Nombre}\", Tipo: {TipoEscuela} {System.Environment.NewLine} Pais: {Pais}, Ciudad:{Ciudad}, \n Dirección:{Direccion}";
        }

        public void LimpiarLugar()
        {
             Printer.DrawLinea();
             Console.WriteLine("Limpiando Escuela...");
             foreach (var curso in Cursos)
             {
                curso.LimpiarLugar();
             }
             Printer.WriteTitle($"Escuela {Nombre} Limpia");
        }

    }
}
