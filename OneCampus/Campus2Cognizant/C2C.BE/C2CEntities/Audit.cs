using System;

namespace C2C.BusinessEntities.C2CEntities
{
    public abstract class Audit
    {
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
