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
    
    public partial class ContentType
    {
        public ContentType()
        {
            this.ContentComments = new HashSet<ContentComment>();
            this.ContentLikes = new HashSet<ContentLike>();
            this.ContentShares = new HashSet<ContentShare>();
            this.Permissions = new HashSet<Permission>();
        }
    
        public short Id { get; set; }
        public string Name { get; set; }
    
        public virtual ICollection<ContentComment> ContentComments { get; set; }
        public virtual ICollection<ContentLike> ContentLikes { get; set; }
        public virtual ICollection<ContentShare> ContentShares { get; set; }
        public virtual ICollection<Permission> Permissions { get; set; }
    }
}