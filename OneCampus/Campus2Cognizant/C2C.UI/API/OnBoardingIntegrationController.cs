using C2C.BusinessEntities.C2CEntities;
using C2C.BusinessEntities.NotificationEntities;
using C2C.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace C2C.UI.API
{
    public class OnBoardingIntegrationController : ApiController
    {
        [HttpPost]
        public void IntegrateOnBoarding(OnBoardingEntity onBoardingEntity)
        {
            User user = UserManager.Get(Convert.ToInt32(onBoardingEntity.CandidateId));          
            //user.EUMSStatus = onBoardingEntity.EUMSRegisteredStatus;
            //user.OfferStatus = onBoardingEntity.OfferStatus;
        }
    }
}
