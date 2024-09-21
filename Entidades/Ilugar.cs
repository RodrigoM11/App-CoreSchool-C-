using System;

namespace CoreEscuela.Entidades
{
    public interface Ilugar
    {
        string Direccion { get; set; }

        void LimpiarLugar();
    } 
}