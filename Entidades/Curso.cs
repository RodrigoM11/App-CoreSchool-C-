using System;
using System.Collections.Generic;
using CoreEscuela.Util;

namespace CoreEscuela.Entidades
{
    public class Curso : ObjetoEscuelaBase, Ilugar
    {
        public TiposJornada Jornada { get; set; }
        public required List<Asignatura> Asignaturas { get; set; }
        public required List<Alumno> Alumnos { get; set; }

        public required string Direccion { get; set; }

        public void LimpiarLugar()
        {
            Printer.DrawLinea();
            Console.WriteLine("Limpiando Establecimiento...");
            Console.WriteLine($"Curso {Nombre} Limpio");
        }
    }
}