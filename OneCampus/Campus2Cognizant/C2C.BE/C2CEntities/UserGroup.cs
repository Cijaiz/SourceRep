#region References
using C2C.Core.Constants.C2CWeb;
#endregion

namespace C2C.BusinessEntities.C2CEntities
{
    public class UserGroup : Audit
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int GroupId { get; set; }

        public Status Status { get; set; }
    }
}
