using static System.Console;

namespace CoreEscuela.Util
{
    public static class Printer
    {
        public static void DrawLinea(int tam = 10)
        {
            WriteLine("".PadLeft(tam, '='));
        }

        public static void WriteTitle(string titulo)
        {
            var tamaño = titulo.Length + 4;
            DrawLinea(tamaño);
            WriteLine($"| {titulo} |");
            DrawLinea(tamaño);
        }

        public static void PresioneENTER()
        {
            WriteLine("Presione ENTER para continuar");
        }

        // public static void Beep(int hz = 2000, int tiempo=500, int cantidad =1)
        // {
        //     while (cantidad-- > 0)
        //     {
        //         System.Console.Beep(hz, tiempo);
        //     }
        // }
    }
}