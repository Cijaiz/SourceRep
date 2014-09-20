using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using NotificationHub.DataModel;
using C2C.Core.Helper;
using C2C.Core.Security;

namespace Cognizant.Octane.OrchardSTS
{
    public partial class Change_Password : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            C2CDataEntities context = new C2CDataEntities();

            var userName = txtUserName.Text;

            var user = context.Users.Where(c => c.UserName == userName).FirstOrDefault();

            var oldPassword = CryptoHelper.HashData(user.PasswordSalt, txtOldPassword.Text);
            var existingPassword = user.Password;

            if (oldPassword == existingPassword)
            {
                string newPassword = txtNewPassword.Text;
                user.PasswordSalt = CryptoHelper.GenerateSalt();
                user.Password = CryptoHelper.HashData(user.PasswordSalt, newPassword);
                context.SaveChanges();

                FormsAuthentication.SetAuthCookie(txtUserName.Text, false);
                FormsAuthentication.RedirectFromLoginPage(txtUserName.Text, false);
            }
            else
                Response.Write("Enter valid password..");
        }
    }
}