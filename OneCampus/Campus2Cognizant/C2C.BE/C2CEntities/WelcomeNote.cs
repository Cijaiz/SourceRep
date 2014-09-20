namespace C2C.BusinessEntities.C2CEntities
{
    #region Reference
    using C2C.Core.Constants.C2CWeb;
    using System;
    using System.ComponentModel.DataAnnotations;
using C2C.Core.DataAnnotations;
    #endregion

    public class WelcomeNote : Audit
    {
        public int Id { get; set; }

        [Required]
        public string Note { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public System.DateTime OfferExtendedStartDate { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [CompareDate("OfferExtendedStartDate",true,CompareDate.Type.GreaterOrEqual)]
        public System.DateTime OfferExtendedEndDate { get; set; }
        public Status Status { get; set; }

    }
}
