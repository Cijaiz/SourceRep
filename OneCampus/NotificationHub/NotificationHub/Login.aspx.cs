//-----------------------------------------------------------------------------
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
//
//-----------------------------------------------------------------------------

using System;
using System.Linq;
using System.Web.Security;

namespace Cognizant.Octane.OrchardSTS
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtUserName.Text))
            {
                var isValidUser = new OrchardMembershipValidator().ValidateOneTimeUser(txtUserName.Text.Trim(), txtPassword.Text);

                if (isValidUser)
                {
                    if (Request.QueryString["ReturnUrl"] != null)
                    {
                        FormsAuthentication.SetAuthCookie(txtUserName.Text, false);
                        FormsAuthentication.RedirectFromLoginPage(txtUserName.Text, false);
                    }
                    else
                    {
                        FormsAuthentication.SetAuthCookie(txtUserName.Text, false);
                    }
                }
                else
                    lblError.Text = "User Invalid";
            }
            else if (!IsPostBack)
            {
                txtUserName.Text = string.Empty;
            }
        }
    }
}
