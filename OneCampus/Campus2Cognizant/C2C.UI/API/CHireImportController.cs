using C2C.BusinessEntities;
using C2C.BusinessEntities.C2CEntities;
using C2C.BusinessEntities.NotificationEntities;
using C2C.BusinessLogic;
using C2C.Core.Extensions;
using C2C.Core.Logger;
using System;
using System.Web.Http;

namespace C2C.UI.API
{
  
    public class CHireImportController : ApiController
    {
        public ProcessResult CandidateImport(CHireData chireData)
        {
            ProcessResult response = null;
            var user = UserManager.Get(chireData.CandidateId);
            try
            {
                if (user == null)
                {
                    User userData = new User();
                    userData.UserName = chireData.CandidateId;
                    userData.IsLocked = false;
                    userData.UpdatedBy = C2C.Core.Constants.C2CWeb.DefaultValue.DEFAULT_USER_ID;
                    userData.Profile.FirstName = chireData.FirstName;
                    userData.Profile.LastName = chireData.LastName;
                    userData.Profile.Email = chireData.EmailAddress;
                    userData.Group.Title = chireData.College;
                    userData.Group.IsCollege = true;
                    userData.Password = "Password-1";
                    userData.UpdatedOn = DateTime.UtcNow;
                    var result = UserManager.UserImport(userData);

                    response = new ProcessResult() 
                    {
                        CandidateId = chireData.CandidateId, 
                        Message = C2C.Core.Constants.C2CWeb.Message.CREATED, 
                        Status = result.Status.ToString()
                    };
                }
                else
                {
                    response = new ProcessResult() 
                    {
                        CandidateId = chireData.CandidateId, 
                        Message = C2C.Core.Constants.C2CWeb.Message.USER_EXSISTS, 
                        Status = ResponseStatus.Failed.ToString() 
                    };
                }
            }
            catch(Exception ex)
            {
                Logger.Error(ex.ToFormatedString());
            }

            return response;
        }
    }
}
