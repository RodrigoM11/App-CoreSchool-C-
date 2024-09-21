using System;
using System.Collections.Generic;
using System.Linq;
using CoreEscuela.Entidades;
using CoreEscuela.Util;

namespace CoreEscuela
{
    public sealed class EscuelaEngine
    {
        public Escuela Escuela { get; set; }

        public EscuelaEngine()
        {
            Escuela = new Escuela
        (
           "Default",
            "",
            "",
            "",
             0,
            TiposEscuela.Primaria
        );
        }

        public void Inicializar()
        {

            Escuela = new Escuela
        (
            "Academy",
            "Argentina",
            "Cordoba",
            "Av.Colon 1500",
              2024,
            TiposEscuela.Secundaria
        );

            CargarCursos();
            CargarAsignaturas();
            CargarEvaluaciones();

        }

        public void ImprimirDiccionario(Dictionary<LlaveDiccionario, IEnumerable<ObjetoEscuelaBase>> dic,
                                LlaveDiccionario keyDicc)
        {
            if (dic.TryGetValue(keyDicc, out IEnumerable<ObjetoEscuelaBase>? elementos))
            {
                IEnumerable<IGrouping<string, Evaluacion>>? evaluacionesPorCurso = null;

                if (keyDicc == LlaveDiccionario.Evaluaciones)
                {
                    evaluacionesPorCurso = elementos.Cast<Evaluacion>()
                                                     .GroupBy(e => e.NombreCurso)
                                                     .OrderBy(g => g.Key);
                }

                Printer.WriteTitle(keyDicc.ToString());

                foreach (var val in elementos)
                {
                    switch (keyDicc)
                    {
                        case LlaveDiccionario.Evaluaciones:
                            if (evaluacionesPorCurso != null)
                            {
                                foreach (var grupo in evaluacionesPorCurso)
                                {
                                    Console.WriteLine();
                                    Printer.WriteTitle($"Curso: {grupo.Key}");
                                    foreach (var evaluacion in grupo)
                                    {
                                        Console.WriteLine(evaluacion);
                                    }
                                    Console.WriteLine();

                                }
                            }

                            return;
                        case LlaveDiccionario.Escuela:
                            Console.WriteLine("Escuela: " + val);
                            break;
                        case LlaveDiccionario.Alumnos:
                            if (val is Alumno alumnoTmp)
                            {
                                Console.WriteLine($"Alumno: {alumnoTmp.Nombre}, Curso: {alumnoTmp.NombreCurso}");
                            }
                            break;
                        case LlaveDiccionario.Cursos:
                            if (val is Curso curtmp)
                            {
                                int count = curtmp.Alumnos.Count;
                                Console.WriteLine("Curso: " + val.Nombre + " Cantidad Alumnos: " + count);
                            }
                            break;
                        case LlaveDiccionario.Asignaturas:
                            Console.WriteLine(val);
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine($"No se encontró la clave {keyDicc} en el diccionario.");
            }
        }



        public Dictionary<LlaveDiccionario, IEnumerable<ObjetoEscuelaBase>> GetDiccionarioObjetos()
        {
            var diccionario = new Dictionary<LlaveDiccionario, IEnumerable<ObjetoEscuelaBase>>();

            diccionario.Add(LlaveDiccionario.Escuela, new[] { Escuela });
            diccionario.Add(LlaveDiccionario.Cursos, Escuela.Cursos.Cast<ObjetoEscuelaBase>());

            var listatmp = new List<Evaluacion>();
            var listatmpas = new List<Asignatura>();
            var listatmpal = new List<Alumno>();

            foreach (var cur in Escuela.Cursos)
            {
                listatmpas.AddRange(cur.Asignaturas);
                listatmpal.AddRange(cur.Alumnos);

                foreach (var alum in cur.Alumnos)
                {
                    listatmp.AddRange(alum.Evaluaciones);
                }

            }

            diccionario.Add(LlaveDiccionario.Evaluaciones, listatmp.Cast<ObjetoEscuelaBase>());
            diccionario.Add(LlaveDiccionario.Asignaturas, listatmpas.Cast<ObjetoEscuelaBase>());
            diccionario.Add(LlaveDiccionario.Alumnos, listatmpal.Cast<ObjetoEscuelaBase>());

            return diccionario;
        }


        public static void ImpimirCursosEscuela(Escuela escuela)
        {

            Printer.WriteTitle("Cursos de la Escuela");


            if (escuela?.Cursos != null)
            {
                foreach (var curso in escuela.Cursos)
                {
                    Console.WriteLine($"Nombre {curso.Nombre}, Id  {curso.UniqueId}, Dirección {curso.Direccion}");
                }
            }
        }


        public static void BuscarYMostrarEvaluacionesPorAlumno(EscuelaEngine engine)
        {

            Printer.WriteTitle("Ingrese el Nombre del curso según los cursos disponibles (ej. 101, 201..): ");
            string nombreCurso = Console.ReadLine() ?? string.Empty;


            var curso = engine.Escuela.Cursos.FirstOrDefault(c => c.Nombre.Equals(nombreCurso, StringComparison.OrdinalIgnoreCase));

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

            var evaluacionesAlumno = from evaluacion in alumno.Evaluaciones
                                     select evaluacion;

            if (!evaluacionesAlumno.Any())
            {
                Console.WriteLine($"No se encontraron evaluaciones para el alumno {alumno.Nombre} en el curso {curso.Nombre}.");
            }
            else
            {

                var notasPorAsignatura = from eval in evaluacionesAlumno
                                         group eval by eval.Asignatura.Nombre into grupoAsignaturas
                                         select new
                                         {
                                             Asignatura = grupoAsignaturas.Key,
                                             Notas = grupoAsignaturas.Select(e => e.Nota).ToList(),
                                             Promedio = grupoAsignaturas.Average(e => e.Nota)
                                         };


                var promedioTotal = evaluacionesAlumno.Average(e => e.Nota);
                Console.WriteLine();
                Printer.WriteTitle($"Notas de {alumno.Nombre} en el curso {curso.Nombre}:");
                Console.WriteLine();
                foreach (var asignatura in notasPorAsignatura)
                {
                    Printer.WriteTitle($"Asignatura: {asignatura.Asignatura}");
                    Console.WriteLine("Notas: " + string.Join(", ", asignatura.Notas));
                    Console.WriteLine($"Promedio: {asignatura.Promedio:F2}");
                    Console.WriteLine();
                    Console.WriteLine();
                }

                Console.WriteLine($"Promedio total: {promedioTotal:F2}");
                Console.WriteLine();
                Console.WriteLine();
            }
        }



        public IReadOnlyList<ObjetoEscuelaBase> GetObjetosEscuela(

              bool traeEvaluaciones = true,
              bool traeAlumnos = true,
              bool traeAsignaturas = false,
              bool traeCursos = true

              )
        {
            return GetObjetosEscuela(out int dummy, out dummy, out dummy, out dummy);
        }
        public IReadOnlyList<ObjetoEscuelaBase> GetObjetosEscuela(
                  out int conteoEvaluaciones,
                  out int conteoCursos,
                  bool traeEvaluaciones = true,
                  bool traeAlumnos = true,
                  bool traeAsignaturas = false,
                  bool traeCursos = true

                  )
        {
            return GetObjetosEscuela(out conteoEvaluaciones, out conteoCursos, out int dummy, out dummy);
        }
        public IReadOnlyList<ObjetoEscuelaBase> GetObjetosEscuela(
              out int conteoEvaluaciones,
              out int conteoCursos,
              out int conteoAsignaturas,
              out int conteoAlumnos,
              bool traeEvaluaciones = true,
              bool traeAlumnos = true,
              bool traeAsignaturas = false,
              bool traeCursos = true
              )
        {
            conteoAlumnos = conteoAsignaturas = conteoEvaluaciones = 0;

            var listaObj = new List<ObjetoEscuelaBase>();
            listaObj.Add(Escuela);

            if (traeCursos)
                listaObj.AddRange(Escuela.Cursos);

            conteoCursos = Escuela.Cursos.Count;

            foreach (var curso in Escuela.Cursos)
            {
                conteoAsignaturas += curso.Asignaturas.Count;
                conteoAlumnos += curso.Alumnos.Count;

                if (traeAsignaturas)
                    listaObj.AddRange(curso.Asignaturas);

                if (traeAlumnos)
                    listaObj.AddRange(curso.Alumnos);

                if (traeEvaluaciones)
                {
                    foreach (var alumno in curso.Alumnos)
                    {

                        listaObj.AddRange(alumno.Evaluaciones);
                        conteoEvaluaciones += alumno.Evaluaciones.Count;
                    }
                }
            }

            return listaObj.AsReadOnly();
        }



        public void CargarEvaluaciones()
        {
            foreach (var curso in Escuela.Cursos)
            {
                var rnd = new Random();

                foreach (var asignatura in curso.Asignaturas)
                {
                    foreach (var alumno in curso.Alumnos)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            var ev = new Evaluacion
                            {
                                CursoCargado = "",
                                NombreCurso = curso.Nombre,
                                Asignatura = asignatura,
                                Nombre = $"{asignatura.Nombre} Ev#{i + 1}",
                                Nota = MathF.Round((float)(5 * rnd.NextDouble()), 2),
                                Alumno = alumno
                            };
                            alumno.Evaluaciones.Add(ev);
                        }
                    }
                }
            }
        }

        public static void CargarrEvaluaciones(EscuelaEngine engine)
        {
            Printer.WriteTitle("Cargar una Nueva Evaluación");


            Printer.WriteTitle("Ingrese el Nombre del curso segun los cursos de arriba (ej. 101, 201..): ");
            Console.Write("Opción: ");
            string nombreCurso = Console.ReadLine() ?? string.Empty;

            var curso = engine.Escuela.Cursos.FirstOrDefault(c => c.Nombre.Equals(nombreCurso, StringComparison.OrdinalIgnoreCase));

            if (curso == null)
            {
                Console.WriteLine("Curso no encontrado. Asegúrese de ingresar un nombre válido.");
                return;
            }

            Printer.WriteTitle("Seleccione un Alumno segun su numero de orden (ej. 1,2..):");


            for (int i = 0; i < curso.Alumnos.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {curso.Alumnos[i].Nombre}");
            }
            Console.Write("Opción: ");
            string alumnoSeleccionado = Console.ReadLine() ?? string.Empty;

            if (!int.TryParse(alumnoSeleccionado, out int alumnoIndex) || alumnoIndex < 1 || alumnoIndex > curso.Alumnos.Count)
            {
                Console.WriteLine("Opción no válida.");
                return;
            }

            var alumno = curso.Alumnos[alumnoIndex - 1];


            Console.WriteLine("Seleccione una asignatura:");
            for (int i = 0; i < curso.Asignaturas.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {curso.Asignaturas[i].Nombre}");
            }
            Console.Write("Opción: ");
            string asignaturaSeleccionada = Console.ReadLine() ?? string.Empty;

            if (!int.TryParse(asignaturaSeleccionada, out int asignaturaIndex) || asignaturaIndex < 1 || asignaturaIndex > curso.Asignaturas.Count)
            {
                Console.WriteLine("Opción no válida.");
                return;
            }

            var asignatura = curso.Asignaturas[asignaturaIndex - 1];


            Console.WriteLine("Ingrese el nombre de la evaluación:");
            string nombre = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(nombre))
            {
                Console.WriteLine("El nombre de la evaluación no puede estar vacío.");
                return;
            }


            Console.WriteLine("Ingrese la nota de la evaluación (0-5):");
            string notastring = Console.ReadLine() ?? string.Empty;

            if (!float.TryParse(notastring, out float nota) || nota < 0 || nota > 5)
            {
                Console.WriteLine("Nota no válida. Debe estar entre 0 y 5.");
                return;
            }

            var evaluacion = new Evaluacion
            {
                CursoCargado = "CARGADO",
                NombreCurso = curso.Nombre,
                Nombre = nombre,
                Nota = nota,
                Alumno = alumno,
                Asignatura = asignatura
            };

            alumno.Evaluaciones.Add(evaluacion);
            Printer.WriteTitle("Evaluación cargada correctamente.");
        }

        private void CargarAsignaturas()
        {
            foreach (var curso in Escuela.Cursos)
            {
                var listaAsignaturas = new List<Asignatura>(){
                            new Asignatura{Nombre="Matemáticas"} ,
                            new Asignatura{Nombre="Educación Física"},
                            new Asignatura{Nombre="Castellano"},
                            new Asignatura{Nombre="Ciencias Naturales"}
                };
                curso.Asignaturas = listaAsignaturas;
            }
        }

        private List<Alumno> GenerarAlumnosAlAzar(int cantidad)
        {
            string[] nombre1 = { "Alba", "Felipa", "Eusebio", "Farid", "Donald", "Alvaro", "Nicolás" };
            string[] apellido1 = { "Ruiz", "Sarmiento", "Uribe", "Maduro", "Trump", "Toledo", "Herrera" };
            string[] nombre2 = { "Freddy", "Anabel", "Rick", "Murty", "Silvana", "Diomedes", "Nicomedes", "Teodoro" };

            var listaAlumnos = from n1 in nombre1
                               from n2 in nombre2
                               from a1 in apellido1
                               select new Alumno { Nombre = $"{n1} {n2} {a1}" };

            return listaAlumnos.OrderBy((al) => al.UniqueId).Take(cantidad).ToList();
        }

        private void CargarCursos()
        {
            Escuela.Cursos = new List<Curso>(){
        new Curso(){ Nombre = "101", Jornada = TiposJornada.Mañana, Asignaturas = new List<Asignatura>(), Alumnos = new List<Alumno>(), Direccion = "Av. Colon 1500" },
        new Curso(){ Nombre = "201", Jornada = TiposJornada.Mañana, Asignaturas = new List<Asignatura>(), Alumnos = new List<Alumno>(), Direccion = "Av. Colon 1500" },
        new Curso(){ Nombre = "301", Jornada = TiposJornada.Mañana, Asignaturas = new List<Asignatura>(), Alumnos = new List<Alumno>(), Direccion = "Av. Colon 1500" },
        new Curso(){ Nombre = "401", Jornada = TiposJornada.Tarde, Asignaturas = new List<Asignatura>(), Alumnos = new List<Alumno>(), Direccion = "Av. Colon 1000" },
        new Curso(){ Nombre = "501", Jornada = TiposJornada.Tarde, Asignaturas = new List<Asignatura>(), Alumnos = new List<Alumno>(), Direccion = "Av. Colon 1000" }
        };


            Random rnd = new Random();
            foreach (var c in Escuela.Cursos)
            {
                int cantRandom = rnd.Next(5, 20);
                var alumnos = GenerarAlumnosAlAzar(cantRandom);
                foreach (var alumno in alumnos)
                {
                    alumno.NombreCurso = c.Nombre;
                }
                c.Alumnos = alumnos;
            }


        }
    }
}