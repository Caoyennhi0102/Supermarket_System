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
    
    public partial class PhieuTraHang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PhieuTraHang()
        {
            this.PhieuTraHangChiTiets = new HashSet<PhieuTraHangChiTiet>();
        }
    
        public string SoPhieuTra { get; set; }
        public string MaNV { get; set; }
        public System.DateTime NgayTao { get; set; }
        public Nullable<decimal> ThanhTien { get; set; }
    
        public virtual NhanVien NhanVien { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PhieuTraHangChiTiet> PhieuTraHangChiTiets { get; set; }
    }
}