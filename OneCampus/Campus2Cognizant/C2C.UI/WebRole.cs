namespace C2C.UI
{
    #region Reference
    using C2C.Core.Logger;
    using Microsoft.WindowsAzure.ServiceRuntime;
    #endregion

    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            Logger.Debug("Instance OnStart.");
            return base.OnStart();
        }
    }
}