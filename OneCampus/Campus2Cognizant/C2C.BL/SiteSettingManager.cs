using C2C.BusinessEntities;
using C2C.BusinessEntities.C2CEntities;
using System.Web;
using System;
using DAL = C2C.DataAccessLogic;
using C2C.Core.Helper;
using C2C.Core.Constants.C2CWeb;

namespace C2C.BusinessLogic
{
    /// <summary>
    /// Create,Get,Update for stiesetting Manager
    /// </summary>
    public static class SiteSettingManager
    {
        /// <summary>
        ///Creates the SiteSetting
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Success/Failure Response with SiteSetting detail</returns>
        public static ProcessResponse<SiteSetting> Create(SiteSetting sitesetting)
        {
            var response = DAL.SiteSettingWorker.GetInstance().Create(sitesetting);
            return response;
        }

        /// <summary>
        /// Gets the SiteSetting Detail
        /// </summary>
        /// <returns>sitesetting</returns>
        public static SiteSetting Get()
        {
            SiteSetting sitesetting = null;
            HttpCookie cookie = HttpContext.Current.Request.Cookies["SiteSettings"];

            //Check if cookie exists and if so, load site settings from cookies.
            if (cookie != null)
            {
                DateTime expiryDate = Convert.ToDateTime(cookie["ExpiresBy"]);

                //Check for expiration of cookies.
                if (expiryDate < DateTime.Now)
                {
                    sitesetting = DAL.SiteSettingWorker.GetInstance().Get();
                    HttpCookie encryptCookie = HttpUtilityHelper.SetEncryptedCookie<SiteSetting>("SiteSettings", "SiteSettingsKey", sitesetting,DefaultValue.SITESETTINGS_EXPIRYMINUTES);
                }
                else
                {
                    sitesetting = HttpUtilityHelper.GetDecryptedCookie<SiteSetting>("SiteSettings", "SiteSettingsKey");
                }
            }
            else
            {
                sitesetting = DAL.SiteSettingWorker.GetInstance().Get();
                HttpCookie encryptCookie = HttpUtilityHelper.SetEncryptedCookie<SiteSetting>("SiteSettings", "SiteSettingsKey", sitesetting,DefaultValue.SITESETTINGS_EXPIRYMINUTES);
            }

            return sitesetting;
        }

        /// <summary>
        /// Updates the SiteSetting
        /// </summary>
        /// <param name="sitesetting">Updated Sitesetting</param>
        /// <returns>Success/Failure Response</returns>
        public static ProcessResponse<SiteSetting> Update(SiteSetting sitesetting)
        {
            ProcessResponse<SiteSetting> response = null;
            response = DAL.SiteSettingWorker.GetInstance().Update(sitesetting);
            HttpCookie encryptCookie = HttpUtilityHelper.SetEncryptedCookie<SiteSetting>("SiteSettings", "SiteSettingsKey", sitesetting, DefaultValue.SITESETTINGS_EXPIRYMINUTES);
            return response;
        }
    }
}
