//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DogsEntities
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserEventInterested
    {
        public int Id { get; set; }
        public Nullable<int> EventId { get; set; }
        public Nullable<int> Interested { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> UserId { get; set; }
    }
}