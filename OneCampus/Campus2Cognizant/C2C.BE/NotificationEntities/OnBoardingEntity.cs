using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C2C.BusinessEntities.NotificationEntities
{
    public class OnBoardingEntity
    {
        public string CandidateId { get; set; }
        public string CandidateName { get; set; }
       
        public OfferStatus OfferStatus { get; set; }
        
        public EUMSStatus EUMSRegisteredStatus { get; set; }
    }

    [Serializable]
    public enum EUMSStatus : short
    {
        REGISTERED = 1,
        NOT_REGISTERED = 2
    }

    [Serializable]
    public enum OfferStatus : short
    {
        UNKNOWN = 0,
        OFFER_ACCEPTED = 1,
        OFFER_REJECTED = 2
    }
}
