using System;

namespace CoreEscuela.Entidades
{
    public class Evaluacion : ObjetoEscuelaBase
    {
        public required Alumno Alumno { get; set; }
        public required Asignatura Asignatura { get; set; }
        public float Nota { get; set; }
        public required string NombreCurso { get; set; }
         public required string CursoCargado { get; set; }
        public override string ToString()
        {
            return $"{Nota}  , {Alumno.Nombre}, {Asignatura.Nombre}                   {CursoCargado}";
        }
    }
}

