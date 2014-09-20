using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
namespace C2C.BusinessEntities.C2CEntities
{
   public class SiteSetting : Audit
    {
       public int Id { get; set; }
       [Required]
       [StringLength(50, ErrorMessage = "Name Max Length is 50")]
       public string Name { get; set; }
       [Required]
       public short Version { get; set; }

       public bool IsFederationEnabled { get; set; }
       public bool TranslateClaimsToUserProperties { get; set; }
       public bool TranslateClaimsToRoles { get; set; }

       [Required]
       public string StsIssuerUrl { get; set; }
       [Required]
       public string StsLoginUrl { get; set; }
       [Required]
       public string CtsLoginUrl { get; set; }

       [Required]
       public string Realm { get; set; }
       [Required]
       public string ReturnUrlBase { get; set; }
       [Required]
       public string AudienceUrl { get; set; }

       [Required]
       public string X509CertificateThumbprint { get; set; }
       public bool ModerateComment { get; set; }
    }
}
