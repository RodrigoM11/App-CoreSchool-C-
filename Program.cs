
using System;
using System.Collections.Generic;
using CoreEscuela.Entidades;
using CoreEscuela.Util;
using static System.Console;
using CoreEscuela.App;

namespace CoreEscuela
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += AccionDelEvento;

            var engine = new EscuelaEngine();
            engine.Inicializar();
            Printer.WriteTitle("BIENVENIDOS A LA ESCUELA");
            Console.WriteLine(engine.Escuela.ToString());
            EscuelaEngine.ImpimirCursosEscuela(engine.Escuela);

            var dic = engine.GetDiccionarioObjetos();
            var reporteador = new Reporteador(dic);
            // var evalList = reporteador.GetListaEvaluaciones();
            // var asignaturas = reporteador.GetListaAsignaturas();
            // var asigList = reporteador.GetDicEvaluaXAsig();

            string opcion;
            do
            {
                Printer.WriteTitle("\n|Seleccione una Opción :");
                Console.WriteLine("1. Evaluaciones");
                Console.WriteLine("2. Escuela");
                Console.WriteLine("3. Alumnos");
                Console.WriteLine("4. Cursos");
                Console.WriteLine("5. Asignaturas");
                Console.WriteLine("6. Cargar una Nueva Evaluacion");
                Console.WriteLine("6.1 Editar una Evaluacion");
                Console.WriteLine("7. Imprimir Promedio por Asignatura");
                Console.WriteLine("8. Busqueda por Alumno");
                Console.WriteLine("9. Salir");
                Console.Write("Opción: ");
                opcion = Console.ReadLine() ?? string.Empty;

                switch (opcion)
                {
                    case "1":
                        engine.ImprimirDiccionario(dic, LlaveDiccionario.Evaluaciones);
                        break;
                    case "2":
                        engine.ImprimirDiccionario(dic, LlaveDiccionario.Escuela);
                        break;
                    case "3":
                        engine.ImprimirDiccionario(dic, LlaveDiccionario.Alumnos);
                        break;
                    case "4":
                        engine.ImprimirDiccionario(dic, LlaveDiccionario.Cursos);
                        break;
                    case "5":
                        engine.ImprimirDiccionario(dic, LlaveDiccionario.Asignaturas);
                        break;
                    case "6":
                        EscuelaEngine.CargarrEvaluaciones(engine);
                        dic = engine.GetDiccionarioObjetos();
                        break;
                    case "6.1":
                        Reporteador.EditarEvaluaciones(dic);
                        dic = engine.GetDiccionarioObjetos();
                        break;
                    case "7":
                        var listaPromXAsig = reporteador.GetPromeAlumnPorAsignatura();
                        Reporteador.ImprimirPromediosPorAsignatura(listaPromXAsig);
                        break;
                    case "8":
                        EscuelaEngine.BuscarYMostrarEvaluacionesPorAlumno(engine);
                        break;
                    case "9":
                        Console.WriteLine("Saliendo del programa...");
                        break;

                    default:
                        Console.WriteLine("Opción no válida. Por favor, seleccione una opción del 1 al 5.");
                        break;
                }

            } while (opcion != "9");
        }


        private static void AccionDelEvento(object? sender, EventArgs e)
        {
            Printer.WriteTitle("Para volver a ingresar ejecutar dotnet run en la cli");
        }


    }

}



// var listaObjetos = engine.GetObjetosEscuela(
//     out int conteoEvaluaciones,
//     out int conteoCursos
// out int conteoAsignaturas,
// out int conteoAlumnos
// );

// var ListaILugar = from obj in listaObjetos where obj is Ilugar select (Ilugar)obj;

// engine.Escuela.LimpiarLugar();


// if (listaObjetos != null && listaObjetos.Any())
// {
//     var ListaILugar = from obj in listaObjetos
//                       where obj is Ilugar
//                       select (Ilugar)obj;

//     if (ListaILugar.Any())
//     {
//         foreach (var lugar in ListaILugar)
//         {
//             Console.WriteLine(lugar);
//         }
//     }
//     else
//     {
//         Console.WriteLine("No se encontraron objetos que implementen la interfaz Ilugar.");
//     }
// }            
// else
// {
//     Console.WriteLine("listaObjetos está vacío o es null.");
// }
