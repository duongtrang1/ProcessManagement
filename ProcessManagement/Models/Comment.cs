//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProcessManagement.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Comment
    {
        public int Id { get; set; }
        public string IdUser { get; set; }
        public int IdDirection { get; set; }
        public string Direction { get; set; }
        public string Content { get; set; }
        public System.DateTime Create_At { get; set; }
        public System.DateTime Update_At { get; set; }
        public bool isAction { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
    }
}