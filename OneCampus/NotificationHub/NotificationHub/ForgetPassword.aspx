<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForgetPassword.aspx.cs" Inherits="Cognizant.Octane.OrchardSTS.ForgetPassword" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
        .style1
        {
            width: 100%;
        }
        .style2
        {
            width: 125px;
        }
        .style3
        {
            font-size: large;
            font-weight: bold;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <p class="style3">
        &nbsp;</p>
    <asp:Label ID="Label1" runat="server" Font-Bold="True" Text="Forget Password"></asp:Label>
    &nbsp;<br />

     <table class="style1">
        <tr>
            <td class="style2">
               <asp:Label ID="Lbl_mailID" runat="server" Text="Enter Your Mail ID"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="Txtbox_mailID" runat="server" Width="202px"></asp:TextBox>
            </td>
        </tr>
       
    </table>
    <p>
        <asp:Button ID="btn_fgtpwd" runat="server" onclick="btn_forgetPassword_Click" 
                        Text="Submit" />
    </p>
    </div>
    </form>
     <p>
        Note : An e-mail has been triggered with new password 
               for the Mail Id your going enter above..</p>
</body>
</html>
