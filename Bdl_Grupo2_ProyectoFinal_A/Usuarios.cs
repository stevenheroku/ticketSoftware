//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Bdl_Grupo2_ProyectoFinal_A
{
    using System;
    using System.Collections.Generic;
    
    public partial class Usuarios
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Usuarios()
        {
            this.Equipos = new HashSet<Equipos>();
            this.Tickets = new HashSet<Tickets>();
        }
    
        public int Usr_Id { get; set; }
        public string Usr_CodigoUsuario { get; set; }
        public int Cant_TicketGenerados { get; set; }
        public int enviado { get; set; }
        
        public string Usr_NombreUsuario { get; set; }
        public string Usr_Correo { get; set; }
        public string Usr_Password { get; set; }
        public Nullable<int> Dpt_Id { get; set; }
    
        public virtual Departamentos Departamentos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Equipos> Equipos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tickets> Tickets { get; set; }
    }
}
