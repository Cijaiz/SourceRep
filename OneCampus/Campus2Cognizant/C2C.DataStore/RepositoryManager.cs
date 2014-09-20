namespace C2C.DataStore
{
    #region Reference
    using C2C.Core.Helper;
    #endregion

    public static class RepositoryManager
    {
        internal static readonly string connectionString = string.Empty;
        static RepositoryManager()
        {
            connectionString = CommonHelper.GetConfigSetting(C2C.Core.Constants.C2CWeb.Key.C2C_DB_CONNECTION);
        }

        public static C2CStoreEntities GetStoreEntity()
        {
            return new C2CStoreEntities(connectionString);
        }
    }
}
