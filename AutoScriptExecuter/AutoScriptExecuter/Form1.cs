using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutoScriptExecuter
{
    public partial class Form1 : Form
    {
        ScriptExecutionManager scriptExeManager = new ScriptExecutionManager();
        string newConnectionString = string.Empty;
        public Form1()
        {
            InitializeComponent();
            scriptExeManager.LoadExecutionTypeCombobox(cbxExecutionType);
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
             //btn_exit.Enabled = false;
             btn_start.Enabled = false;        
            
            scriptExeManager.ExecuteManager(lstboxMessage,lblRootpathSelected,lblEntryLogPathSelected);

             btn_start.Enabled = true;
          //   btn_exit.Enabled = true;
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSetRootpath_Click(object sender, EventArgs e)
        {
            lblRootpathSelected.Text = scriptExeManager.SetPath(lblRootpathSelected);
        }

        private void btnSetEntryLogpath_Click(object sender, EventArgs e)
        {
            lblEntryLogPathSelected.Text = scriptExeManager.SetPath(lblEntryLogPathSelected);
        }

        private void btnsetConnectionString_Click(object sender, EventArgs e)
        {
            if (txtConnectionString.Text !="")
            { 
            newConnectionString = txtConnectionString.Text;
            MessageBox.Show("New Database Connection String is changed successfully.");
            }
            else
            {
                MessageBox.Show("Please enter a connection string.");
            }
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            lstboxMessage.Items.Clear();

            bool success = scriptExeManager.TestSQLConnection(txtConnectionString,lstboxMessage);
            lstboxMessage.Visible = true;
            if (success)
            {
                lstboxMessage.Items.Add(Constant.testconnectionSuccess);
                btn_start.Visible = true;
                btn_exit.Visible = true;
                btnTestConnection.Enabled = false;
                scriptExeManager.WriteConnectionStringToFile(txtConnectionString.Text);
            }
            else
            { 
                lstboxMessage.Items.Add(Constant.testConnectionExceptionMessage1);
            }
        }

        private void txtConnectionString_TextChanged(object sender, EventArgs e)
        {
            btnTestConnection.Enabled = true;
            btn_start.Visible = false;
            btn_exit.Visible = false;
           // btn_exit.Enabled = false;
            if (lstboxMessage.Visible == true)
                lstboxMessage.Visible = false;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if(cbxExecutionType.SelectedIndex !=-1)
            { 
            int selectedType=(int)cbxExecutionType.SelectedValue;
            scriptExeManager.LoadDatabaseConnectionStringAndpathDetails(selectedType, txtConnectionString, lblRootpathSelected, lblEntryLogPathSelected);
            scriptExeManager.LoadConnectionStringCombobox(cbxConnectionString);
            }
        }

        private void btnLoadCStr_Click(object sender, EventArgs e)
        {
            if (cbxConnectionString.SelectedIndex >= 0)
            txtConnectionString.Text = cbxConnectionString.SelectedItem.ToString();
        }

        private void lstboxMessage_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            Font myFont = e.Font;
            Brush myBrush;
            int i = e.Index;

            //if (scriptExeManager.exceptionThrown)
            //{
            //    myBrush = Brushes.Red;
            //}
            //else
            //{
             myBrush = Brushes.Red;
            //}

            e.Graphics.DrawString(lstboxMessage.Items[i].ToString(), myFont, myBrush, e.Bounds, StringFormat.GenericDefault);
        }

        private void btnDisplay_Click(object sender, EventArgs e)
        {

            scriptExeManager.DisplayChildForm(scriptExeManager);
        }

        private void cbxConnectionString_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cbx= sender as ComboBox;
            if (cbx.SelectedIndex >= 0)
                btnLoadCStr.Enabled = true;
            else
                btnLoadCStr.Enabled = false;
        }               
    }
}