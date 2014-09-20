using C2C.Core.Helper;
using NotificationHub.DataModel;
using System.Linq;

namespace Cognizant.Octane.OrchardSTS
{
    public class OrchardMembershipValidator
    {
        #region Public Methods

        public bool ValidateOneTimeUser(string userName, string enteredPassword)
        {
            bool isUserValid = false;
            C2CDataEntities oneCampusEntity = new C2CDataEntities(GetConnectionString());

            var matchedUser = oneCampusEntity.Users.Where(user => user.UserName.Equals(userName)).FirstOrDefault();

            if (matchedUser != null)
                isUserValid = matchedUser.Password.Equals(CryptoHelper.HashData(matchedUser.PasswordSalt, enteredPassword));
            
            return isUserValid;
        }

        #endregion Public Methods

        #region Private Methods
        public static string GetConnectionString()
        {
            return CommonHelper.GetConfigSetting("C2CDataEntities");
        }
        #endregion Private Methods
    }
}