//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Hornet.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class RegexEntry
    {
        public int Id { get; set; }
        public string Pattern { get; set; }
        public string Remarks { get; set; }
        public int RegexGroupId { get; set; }
    
        public virtual RegexGroup RegexGroup { get; set; }
    }
}
