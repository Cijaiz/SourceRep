namespace C2C.BusinessEntities.C2CEntities
{
    public class RolePermission : Audit
    {
        public int Id { get; set; }
        public short RoleId { get; set; }
        public int PermissionId { get; set; }
        public bool Permission { get; set; }
    }
}
