using C2C.BusinessEntities;
using C2C.Core.Constants.C2CWeb;
using C2C.DataStore;
using System;
using System.Linq;
using BE = C2C.BusinessEntities.C2CEntities;

namespace C2C.DataAccessLogic
{
    public class SiteSettingWorker
    {
        /// <summary>
        /// Creates a new instance for SiteSettingWorker
        /// </summary>
        /// <returns>Newly created Instance</returns>
        public static SiteSettingWorker GetInstance()
        {
            return new SiteSettingWorker();
        }

        public ProcessResponse<BE.SiteSetting> Create(BE.SiteSetting newSiteSetting)
        {
            ProcessResponse<BE.SiteSetting> response = null;
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                //Transform the Business entity to Database Entity for Repository operations.
                var sitesetting = new SiteSetting()
                {
                    Name = newSiteSetting.Name,
                    Realm = newSiteSetting.Realm,
                    Version = newSiteSetting.Version,

                    AudienceUrl = newSiteSetting.AudienceUrl,
                    
                    StsLoginUrl = newSiteSetting.StsLoginUrl,
                    StsIssuerUrl = newSiteSetting.StsIssuerUrl,
                    CtsLoginUrl = newSiteSetting.CtsLoginUrl,

                    ReturnUrlBase = newSiteSetting.ReturnUrlBase,
                    ModerateComment = newSiteSetting.ModerateComment,
                    IsFederationEnabled = newSiteSetting.IsFederationEnabled,

                    TranslateClaimsToRoles = newSiteSetting.TranslateClaimsToRoles,
                    X509CertificateThumbprint = newSiteSetting.X509CertificateThumbprint,
                    TranslateClaimsToUserProperties = newSiteSetting.TranslateClaimsToUserProperties,

                    UpdatedOn = DateTime.UtcNow,
                    UpdatedBy = newSiteSetting.UpdatedBy
                };
                dbContext.SiteSettings.Add(sitesetting);

                //Save the changes to the DB and get the success or failure respnose.
                int saveResponse = dbContext.SaveChanges();
                if (saveResponse > 0) //Success
                {
                    newSiteSetting.Id = sitesetting.Id;
                    response = new ProcessResponse<BE.SiteSetting>()
                    {
                        Status = ResponseStatus.Success,
                        Message = Message.CREATED,
                        Object = newSiteSetting
                    };
                }
                else
                {
                    response = new ProcessResponse<BE.SiteSetting>()
                    {
                        Status = ResponseStatus.Failed,
                        Message = Message.FAILED
                    };
                }
            }
            return response;
        }

        /// <summary>
        /// Updates the SiteSetting in DataBase
        /// </summary>
        /// <returns>return response</returns>
        public ProcessResponse<BE.SiteSetting> Update(BE.SiteSetting newSiteSetting)
        {
            ProcessResponse<BE.SiteSetting> response = null;
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                //Transform the Business entity to Database Entity for Repository operations.
                var sitesetting = dbContext.SiteSettings
                                           .Where(a => a.Id == newSiteSetting.Id)
                                           .FirstOrDefault();

                if (sitesetting != null)
                {
                    //populating data entity using business entity
                    sitesetting.Name = newSiteSetting.Name;
                    sitesetting.Realm = newSiteSetting.Realm.Trim();
                    sitesetting.Version = newSiteSetting.Version;

                    sitesetting.StsLoginUrl = newSiteSetting.StsLoginUrl.Trim();
                    sitesetting.StsIssuerUrl = newSiteSetting.StsIssuerUrl.Trim();
                    sitesetting.CtsLoginUrl = newSiteSetting.CtsLoginUrl.Trim();

                    sitesetting.ReturnUrlBase = newSiteSetting.ReturnUrlBase.Trim();

                    sitesetting.AudienceUrl = newSiteSetting.AudienceUrl.Trim();
                    sitesetting.ModerateComment = newSiteSetting.ModerateComment;
                    sitesetting.IsFederationEnabled = newSiteSetting.IsFederationEnabled;

                    sitesetting.TranslateClaimsToRoles = newSiteSetting.TranslateClaimsToRoles;
                    sitesetting.X509CertificateThumbprint = newSiteSetting.X509CertificateThumbprint.Trim();
                    sitesetting.TranslateClaimsToUserProperties = newSiteSetting.TranslateClaimsToUserProperties;

                    sitesetting.UpdatedOn = DateTime.UtcNow;
                    sitesetting.UpdatedBy = newSiteSetting.UpdatedBy;

                    //Save the changes to the DB and get the success or failure respnose.
                    int count = dbContext.SaveChanges();
                    if (count > 0)//Success
                    {
                        response = new ProcessResponse<BE.SiteSetting>()
                                                        {
                                                            Status = ResponseStatus.Success,
                                                            Message = Message.UPDATED,
                                                            Object = newSiteSetting
                                                        };
                    }

                }

                else
                {
                    var processResponse = Create(newSiteSetting);
                    response = new ProcessResponse<BE.SiteSetting>() { Message = processResponse.Message, Status = processResponse.Status, Object = newSiteSetting };
                }

                return response;
            }
        }

        /// <summary>
        /// Gets the SiteSetting Detail using userId
        /// </summary>
        public BE.SiteSetting Get()
        {
            BE.SiteSetting sitesetting = null;
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                //Transform the Business entity to Database Entity for Repository operations.
                var selectedSite = dbContext.SiteSettings.First();
                if (selectedSite != null && selectedSite.Id > 0)
                {
                    sitesetting = new BE.SiteSetting()
                    {
                        Id = selectedSite.Id,
                        Name = selectedSite.Name,
                        Realm = selectedSite.Realm,

                        Version = selectedSite.Version,
                        AudienceUrl = selectedSite.AudienceUrl,
                        StsLoginUrl = selectedSite.StsLoginUrl,
                        CtsLoginUrl = selectedSite.CtsLoginUrl,

                        StsIssuerUrl = selectedSite.StsIssuerUrl,
                        ReturnUrlBase = selectedSite.ReturnUrlBase,
                        ModerateComment = selectedSite.ModerateComment,

                        IsFederationEnabled = selectedSite.IsFederationEnabled,
                        TranslateClaimsToRoles = selectedSite.TranslateClaimsToRoles,
                        X509CertificateThumbprint = selectedSite.X509CertificateThumbprint,

                        TranslateClaimsToUserProperties = selectedSite.TranslateClaimsToUserProperties,
                        UpdatedBy = selectedSite.UpdatedBy.HasValue ? selectedSite.UpdatedBy.Value : 1, //Default to 1 (Admin)....
                        UpdatedOn = selectedSite.UpdatedOn
                    };
                }
            }
            return sitesetting;
        }
    }
}
