using System;

namespace CoreEscuela.Entidades
{
    public class Alumno : ObjetoEscuelaBase
    {
       
        public List <Evaluacion> Evaluaciones { get; set; }= new List<Evaluacion>();
        
         public string NombreCurso { get; set; } = "";
    }
}