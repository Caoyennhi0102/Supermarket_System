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
    
    public partial class PhieuTraHangChiTiet
    {
        public string MaHangTra { get; set; }
        public string SoPhieuTra { get; set; }
        public int SoLuongTra { get; set; }
        public Nullable<decimal> ThanhTien { get; set; }
        public string LyDoTraHang { get; set; }
    
        public virtual HangHoa HangHoa { get; set; }
        public virtual PhieuTraHang PhieuTraHang { get; set; }
    }
}