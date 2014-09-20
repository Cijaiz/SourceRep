using C2C.Core.Helper;
using C2C.Core.Security;
using NotificationHub.DataModel;
using System;
using System.Linq;
using System.Web.Security;

namespace Cognizant.Octane.OrchardSTS
{
    public partial class ForgetPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_forgetPassword_Click(object sender, EventArgs e)
        {
            C2CDataEntities context = new C2CDataEntities();
            var email = Txtbox_mailID.Text;
            var user = context.Users.Where(c => c.UserProfile.Email == email).FirstOrDefault();

            if (user != null)
            {
                string tempPassword = FormsAuthenticationProvider.GeneratePassword();
                user.PasswordSalt = CryptoHelper.GenerateSalt();
                user.Password = CryptoHelper.HashData(user.PasswordSalt, tempPassword);
                context.SaveChanges();
                //TODO Mailing for new Password.
                FormsAuthentication.SetAuthCookie(Txtbox_mailID.Text, false);
                FormsAuthentication.RedirectFromLoginPage(Txtbox_mailID.Text, false);
            }
            else
                Response.Write("Enter valid email Id..");
            
        }
    }
}