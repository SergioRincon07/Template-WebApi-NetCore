﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.DataBaseMegaLinea.Models
{
    public partial class spLoginResult
    {
        public string IdProveedor { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaTerminacion { get; set; }
        public string RolName { get; set; }
    }
}
