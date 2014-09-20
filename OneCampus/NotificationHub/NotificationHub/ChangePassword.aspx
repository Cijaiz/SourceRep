<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="Cognizant.Octane.OrchardSTS.Change_Password" %>

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
    <form id="form2" runat="server">
    <div>
    <p class="style3">
        &nbsp;</p>
    <asp:Label ID="Label3" runat="server" Style="font-weight: 700" Text="Change Password"></asp:Label>
    &nbsp;<br />
    <table class="style1">
        <tr>
            <td class="style2">
                <asp:Label ID="Label1" runat="server" Text="User name" ></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtUserName" runat="server" Width="149px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style2">
                <asp:Label ID="Label2" runat="server" Text="Old Password"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtOldPassword" runat="server" TextMode="Password" Width="149px">Old Password</asp:TextBox>
            </td>
        </tr>
         <tr>
            <td class="style2">
                <asp:Label ID="Label4" runat="server" Text="New Password"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" Width="149px">New Password</asp:TextBox>
            </td>
        </tr>
         <tr>
            <td class="style2">
                <asp:Label ID="Label5" runat="server" Text="Retype Password"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtRetypePassword" runat="server" TextMode="Password" Width="149px">Retype Password</asp:TextBox>
            </td>
        </tr>
    </table>
    <p>
        <asp:Button ID="btnSubmit" runat="server" Text="Submit" 
            onclick="btnSubmit_Click" />
    </p>
    </div>
    </form>
</body>
</html>
