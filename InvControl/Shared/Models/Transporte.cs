﻿using System.ComponentModel.DataAnnotations;

namespace InvControl.Shared.Models
{
    public class Transporte : ICloneable
    {
        public int IdTransporte { get; set; }

        [Required(ErrorMessage = "Debe completar este campo"),
            StringLength(50, ErrorMessage = "Longitud máxima 50 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe completar este campo"),
            StringLength(7, ErrorMessage = "Longitud máxima 7 caracteres")]
        public string Patente { get; set; } = string.Empty;

        public bool Activo { get; set; }

        public string NombrePatente => $"{Nombre}: {Patente}";

        public object Clone() => MemberwiseClone();
    }
}
