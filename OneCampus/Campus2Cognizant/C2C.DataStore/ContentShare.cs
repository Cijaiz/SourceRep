//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace C2C.DataStore
{
    using System;
    using System.Collections.Generic;
    
    public partial class ContentShare
    {
        public ContentShare()
        {
            this.ContentSharedUsers = new HashSet<ContentSharedUser>();
        }
    
        public int Id { get; set; }
        public int SharedBy { get; set; }
        public System.DateTime SharedOn { get; set; }
        public short ContentTypeId { get; set; }
        public int ContentId { get; set; }
        public string Description { get; set; }
    
        public virtual ContentType ContentType { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual ICollection<ContentSharedUser> ContentSharedUsers { get; set; }
    }
}
