//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Supermarket_System.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PhanCa
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PhanCa()
        {
            this.GiaoCas = new HashSet<GiaoCa>();
        }
    
        public int MaPhanCa { get; set; }
        public int MaCa { get; set; }
        public Nullable<System.DateTime> NgayLamViec { get; set; }
        public string MaNV { get; set; }
        public string GhiChu { get; set; }
    
        public virtual CaLamViec CaLamViec { get; set; }
        public virtual CaLamViec CaLamViec1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GiaoCa> GiaoCas { get; set; }
        public virtual NhanVien NhanVien { get; set; }
    }
}