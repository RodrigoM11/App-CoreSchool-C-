using System;
using System.Linq;
using System.Security.Cryptography;
using CoreEscuela.Entidades;
using CoreEscuela.Util;

namespace CoreEscuela.App
{
    public class Reporteador
    {
        Dictionary<LlaveDiccionario, IEnumerable<ObjetoEscuelaBase>> _diccionario;
        public Reporteador(Dictionary<LlaveDiccionario, IEnumerable<ObjetoEscuelaBase>> dicObsEsc)
        {
            if (dicObsEsc == null)
                throw new ArgumentNullException(nameof(dicObsEsc));

            _diccionario = dicObsEsc;
        }

        public IEnumerable<Evaluacion> GetListaEvaluaciones()
        {
            // IEnumerable<Escuela> rta;

            if (_diccionario.TryGetValue(LlaveDiccionario.Evaluaciones, out IEnumerable<ObjetoEscuelaBase>? lista))
            {
                return lista!.Cast<Evaluacion>();
            }
            else
            {
                return new List<Evaluacion>();
            }
        }

        public IEnumerable<string> GetListaAsignaturas()
        {
            return GetListaAsignaturas(out var dummy);
        }

        public IEnumerable<string> GetListaAsignaturas(out IEnumerable<Evaluacion> listaEvaluaciones)
        {
            listaEvaluaciones = GetListaEvaluaciones();

            return (from Evaluacion ev in listaEvaluaciones select ev.Asignatura.Nombre).Distinct();
        }


        public Dictionary<string, IEnumerable<Evaluacion>> GetDicEvaluaXAsig()
        {
            var dictaRta = new Dictionary<string, IEnumerable<Evaluacion>>();

            var listaAsig = GetListaAsignaturas(out var listaEval);

            foreach (var asig in listaAsig)
            {
                var evalsAsig = from eval in listaEval
                                where eval.Asignatura.Nombre == asig
                                select eval;

                dictaRta.Add(asig, evalsAsig);
            }

            return dictaRta;
        }


        public Dictionary<string, IEnumerable<object>> GetPromeAlumnPorAsignatura()
        {
            var rta = new Dictionary<string, IEnumerable<object>>();
            var dicEvalXAsig = GetDicEvaluaXAsig();

            foreach (var asigConEval in dicEvalXAsig)
            {
                var promsAlumn = from eval in asigConEval.Value
                                 group eval by new
                                 {
                                     eval.Alumno.UniqueId,
                                     eval.Alumno.Nombre,
                                     eval.NombreCurso
                                 }
                            into grupoEvalsAlumno
                                 select new AlumnoPromedio
                                 {
                                     alumnoid = grupoEvalsAlumno.Key.UniqueId,
                                     alumnoNombre = grupoEvalsAlumno.Key.Nombre,
                                     promedio = grupoEvalsAlumno.Average(evaluacion => evaluacion.Nota),
                                     curso = grupoEvalsAlumno.Key.NombreCurso
                                 };

                rta.Add(asigConEval.Key, promsAlumn);
            }

            return rta;
        }

        public static void ImprimirPromediosPorAsignatura(Dictionary<string, IEnumerable<object>> listaPromXAsig)
        {
            foreach (var asignatura in listaPromXAsig)
            {
                Printer.WriteTitle($"Asignatura: {asignatura.Key}");

                foreach (var item in asignatura.Value)
                {
                    if (item is AlumnoPromedio alumnoPromedio)
                    {
                        Console.WriteLine($"\tAlumno: {alumnoPromedio.alumnoNombre}, Curso: {alumnoPromedio.curso}, Promedio: {alumnoPromedio.promedio:F2}");
                    }
                }
                Console.WriteLine();
            }
        }


        public static void EditarEvaluaciones(Dictionary<LlaveDiccionario, IEnumerable<ObjetoEscuelaBase>> dic)
        {

            Printer.WriteTitle("Ingrese el Nombre del curso según los cursos disponibles (ej. 101, 201..): ");
            string nombreCurso = Console.ReadLine() ?? string.Empty;

            var curso = dic[LlaveDiccionario.Cursos]
                .Cast<Curso>()
                .FirstOrDefault(c => c.Nombre.Equals(nombreCurso, StringComparison.OrdinalIgnoreCase));

            if (curso == null)
            {
                Printer.WriteTitle("Curso no encontrado. Asegúrese de ingresar un nombre válido.");
                return;
            }

            Printer.WriteTitle("Seleccione un Alumno según su número de orden:");
            for (int i = 0; i < curso.Alumnos.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {curso.Alumnos[i].Nombre}");
            }

            Console.Write("Opción: ");
            string alumnoSeleccionado = Console.ReadLine() ?? string.Empty;

            if (!int.TryParse(alumnoSeleccionado, out int alumnoIndex) || alumnoIndex < 1 || alumnoIndex > curso.Alumnos.Count)
            {
                Printer.WriteTitle("Opción no válida.");
                return;
            }

            var alumno = curso.Alumnos[alumnoIndex - 1];


            var evaluacionesAlumno = alumno.Evaluaciones;

            if (!evaluacionesAlumno.Any())
            {
                Console.WriteLine($"No se encontraron evaluaciones para el alumno {alumno.Nombre} en el curso {curso.Nombre}.");
                return;
            }

            Printer.WriteTitle("Seleccione la Evaluación que desea Editar, según su número de orden :");
            for (int i = 0; i < evaluacionesAlumno.Count; i++)
            {
                Console.WriteLine($"{i + 1} . {evaluacionesAlumno[i].Nombre} - Nota: {evaluacionesAlumno[i].Nota}");
            }

            Console.Write("Opción: ");
            string evaluacionSeleccionada = Console.ReadLine() ?? string.Empty;

            if (!int.TryParse(evaluacionSeleccionada, out int evaluacionIndex) || evaluacionIndex < 1 || evaluacionIndex > evaluacionesAlumno.Count)
            {
                Printer.WriteTitle("Opción no válida.");
                return;
            }

            var evaluacion = evaluacionesAlumno[evaluacionIndex - 1];

            Console.Write("Ingrese la nueva nota (0-5): ");
            string nuevaNotaString = Console.ReadLine() ?? string.Empty;

            if (!float.TryParse(nuevaNotaString, out float nuevaNota) || nuevaNota < 0 || nuevaNota > 5)
            {
                Printer.WriteTitle("Nota no válida.");
                return;
            }

            evaluacion.Nota = nuevaNota;
            evaluacion.CursoCargado = "EDITADO";

            Printer.WriteTitle("Evaluación editada correctamente.");
        }


    }
}