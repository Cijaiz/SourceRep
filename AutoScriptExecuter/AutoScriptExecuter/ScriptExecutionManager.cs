using System;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

namespace AutoScriptExecuter
{
    public class ScriptExecutionManager
    {
        static StringBuilder entrylog = new StringBuilder();

        string rootpath = string.Empty;
        string EntryLogpath = string.Empty;
        string EntryLogString = string.Empty;   
        string connectionString= string.Empty;
        public bool exceptionThrown = false;
        List<string> fileExecuted = new List<string>();

        ScriptExecutionDataLoader loader = new ScriptExecutionDataLoader();
        SqlConnection conn = null;
        Server server = null;

        
        childForm childFrominstance = null;
        int pointX = 30;
        int pointY = 20;

        public void LoadExecutionTypeCombobox(ComboBox cmbExecutionType)
        {
            cmbExecutionType.DataSource =loader.GetExecutionTypes();
            cmbExecutionType.DisplayMember = "Type";
            cmbExecutionType.ValueMember = "TypeId";
            cmbExecutionType.SelectedIndex = 1;
            
        }

        public void LoadConnectionStringCombobox(ComboBox cmbConnectionString)
        {
            List<string> strConnectionstrings = new List<string>();
            strConnectionstrings = loader.LoadConnectionString();
            if (strConnectionstrings.Count > 0)
            {
                cmbConnectionString.DataSource = strConnectionstrings;
                cmbConnectionString.SelectedIndex = -1;
            }
        }

        public void LoadDatabaseConnectionStringAndpathDetails(int selectedExecutionType,TextBox txtConnectionString,Label lblRootpathSelected,Label lblEntryLogPathSelected)
        {
            if(selectedExecutionType==1)
                connectionString = ConfigurationManager.ConnectionStrings["LocalConfig"].ConnectionString;            
            else if(selectedExecutionType==2)
                connectionString = ConfigurationManager.ConnectionStrings["AzureConfig"].ConnectionString;   

            loader.GetLoaderRootPath(out rootpath, selectedExecutionType);
            lblRootpathSelected.Text = rootpath;
            EntryLogpath= ConfigurationManager.AppSettings["entrylogpath"];
            lblEntryLogPathSelected.Text = EntryLogpath;
            txtConnectionString.Text = connectionString;

            loadScripts(lblRootpathSelected, entrylog);
        }

        public bool TestSQLConnection(TextBox txtConnectionString, ListBox lstboxMessage)
        {
            bool connectionsuccess = false;
            CreateMessagelog(lstboxMessage, string.Format("{0}{1}{2}", Constant.endDataLoading, Environment.NewLine, Constant.createSQLConn), string.Format("{0},{1}", Constant.endDataLoading, Constant.createSQLConn));
            connectionString = txtConnectionString.Text;
               try
                   {
                       if (exceptionThrown)
                           exceptionThrown = false;
                    conn = new SqlConnection(connectionString);
                    conn.Open();
                    server = PrepareSQLConnection(lstboxMessage, conn);
                    connectionsuccess = true;
                   }
                catch (Exception ex)
                   {
                       exceptionThrown = true; 
                       lstboxMessage.Items.Add(ex.Message);
                     //  CreateMessagelog(lstboxMessage, string.Format("{0}{1}{2}{3}", Constant.exceptionMessage1, Constant.exceptionMessage2, ex.Message, Constant.exceptionMessage3), string.Format("{0},{1},{2}", Constant.exceptionMessage1, string.Format("{0}{1}", Constant.exceptionMessage2, ex.Message), Constant.exceptionMessage3));
                       connectionsuccess = false;
                   }
            return connectionsuccess;
        }

        public void ExecuteManager(ListBox lstboxMessage, Label lblRootpathSelected, Label lblEntryLogPathSelected)
        {
            string filepath = string.Empty;
            string filename = string.Empty;
            try
                {
                    if (exceptionThrown)
                    exceptionThrown = false;
                    CreateMessagelog(lstboxMessage, string.Format("{0}{1}{2}{1}{3}", DateTime.Now, Environment.NewLine, Constant.startExecution, Constant.startDataLoading), string.Format("{0},{1}", DateTime.Now, Constant.startDataLoading));
                    loadScripts(lblRootpathSelected, entrylog);
                    fileExecuted.Clear();
                    foreach (ScriptLocation file in loader.ScriptFiles)
                    {
                        if (file.FILETYPE.Equals(Constant.strFileType))
                        {
                            filename = file.FILENAME;
                            if (File.Exists(file.FILEPATH))
                            {
                                Execution(lstboxMessage, server, file.FILEPATH, filename);
                                fileExecuted.Add(file.FILEPATH);
                            }
                            else
                            {
                                var filenotfound = string.Format("{0}{1}{2}{1}{3}",Constant.fileNotFound , Environment.NewLine , Constant.fileNotFound2 ,Constant.fileNotFound3 );
                                DialogResult btnpress = MessageBox.Show(string.Format("{0}{1}", filename,filenotfound) , Constant.msgBoxErrorTitle, MessageBoxButtons.YesNo);
                                if (btnpress == DialogResult.Yes)
                                {
                                    CreateMessagelog(lstboxMessage, string.Format("{0}{1}",filename , Constant.fileNotFoundMessageResultYes ));
                                    continue;
                                }
                                else if (btnpress == DialogResult.No)
                                {
                                    CreateMessagelog(lstboxMessage, string.Format("{0}{1}",filename , Constant.fileNotFoundMessageResultNo ));
                                    break;
                                }
                            }
                        }
                    }
                    CreateMessagelog(lstboxMessage, Constant.endScriptExecution);
                }
                catch (SqlException ex)
                {
                    exceptionThrown = true;
                    CreateMessagelog(lstboxMessage, string.Format("{0}{1}{2}{3}{4}", Constant.exceptionMessage1, filename, Constant.exceptionMessage2, ex.Message, Constant.exceptionMessage3), string.Format("{0},{1},{2}", string.Format("{0}{1}", Constant.exceptionMessage1, filename), string.Format("{0}{1}", Constant.exceptionMessage2, ex.Message), Constant.exceptionMessage3));
                }
                catch (Exception ex)
                {
                    exceptionThrown = true;
                    CreateMessagelog(lstboxMessage, string.Format("{0}{1}{2}{3}{4}", Constant.exceptionMessage1, filename, Constant.exceptionMessage2, ex.Message, Constant.exceptionMessage3), string.Format("{0},{1},{2}", string.Format("{0}{1}", Constant.exceptionMessage1, filename), string.Format("{0}{1}", Constant.exceptionMessage2, ex.Message), Constant.exceptionMessage3));
                }
                finally
                {
                    if(server != null)
                        server.ConnectionContext.Disconnect();
                    if (conn != null)
                        conn.Close();
                    CreateMessagelog(lstboxMessage, Constant.SQLConnClose);
                }
            RecordTrace(lstboxMessage, lblEntryLogPathSelected.Text);

            if (fileExecuted.Count >0)
            { 
                lstboxMessage.Items.Clear();
                foreach (var item in fileExecuted)
                {
                    lstboxMessage.Items.Add(item);
                }
            }
            lstboxMessage.Items.Add(string.Format("{0}{1}", Constant.listboxTotalFilesExecuted, fileExecuted.Count));
            
        }
        
        private Server PrepareSQLConnection(ListBox lstboxMessage, SqlConnection conn)
        {
            CreateMessagelog(lstboxMessage, string.Format("{0}{1}{2}{1}{3}", Constant.SQLConnSucceed, Environment.NewLine, Constant.SQLConnOpen, Constant.createServerConn), string.Format("{0},{1}", Constant.SQLConnSucceed, Constant.SQLConnOpen));
            Server server = new Server(new ServerConnection(conn));

            CreateMessagelog(lstboxMessage, string.Format("{0}{1}{2}{1}{3}", Constant.ServerConnSucceed, Environment.NewLine, Constant.startScriptExecution,Constant.ServerConnOpen), string.Format("{0},{1},{2}", Constant.ServerConnSucceed ,Constant.startScriptExecution ,Constant.ServerConnOpen));
            server.ConnectionContext.Connect();

            CreateMessagelog(lstboxMessage, string.Format("{0}{1}",Constant.strRootPath,rootpath ));
            return server;
        }

        private void Execution(ListBox lstboxMessage, Server server, string filepath, string filename)
        {
            string script = File.ReadAllText(filepath);
            if (script.Length > 0)
            {
                CreateMessagelog(lstboxMessage, string.Format("{0}{1}", filepath, Constant.startFileExecution));
                server.ConnectionContext.ExecuteNonQuery(script);
                CreateMessagelog(lstboxMessage, string.Format("{0}{1}", filepath, Constant.endFileExecution));
            }
        }

        private static void CreateMessagelog(ListBox lstboxMessage, string message)
        {
            entrylog.AppendLine(message);
            lstboxMessage.Items.Add(message);
        }

        private static void CreateMessagelog(ListBox lstboxMessage, string message, string listboxMessage)
        {
            entrylog.AppendLine(message);
            string[] str = listboxMessage.Split(',');
            if (str.Length > 0)
                foreach (string st in str)
                {
                    lstboxMessage.Items.Add(st);
                }
        }

        private void RecordTrace(ListBox lstboxMessage,string newEntryLogPath)
        {
            CreateMessagelog(lstboxMessage, Constant.endExecution, Constant.msgBoxScriptExeFinalmessage);
            if (EntryLogpath !=newEntryLogPath )
            { 
                EntryLogString = string.Format("{0}\\Entrylog", newEntryLogPath);
                if (!Directory.Exists(EntryLogString))
                {
                    Directory.CreateDirectory(EntryLogString);
                    EntryLogString = string.Format("{0}\\entryLog.txt", EntryLogString);
                }
                if (!File.Exists(newEntryLogPath))
                    File.CreateText(EntryLogString);
            }
            else
            {
                EntryLogString = newEntryLogPath;
            }
            try
            {
                if (exceptionThrown)
                    exceptionThrown = false;
                System.IO.File.AppendAllText(EntryLogString, entrylog.ToString());
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                CreateMessagelog(lstboxMessage, string.Format("{0}{1}{2}{3}{4}", Constant.exceptionMessage1, EntryLogString, Constant.exceptionMessage2, ex.Message, Constant.exceptionMessage3), string.Format("{0},{1},{2}", string.Format("{0}{1}", Constant.exceptionMessage1, EntryLogString), string.Format("{0}{1}", Constant.exceptionMessage2, ex.Message), Constant.exceptionMessage3));
            }
        }

        public string SetPath(Label sender)
        {
            string existingPath = sender.Text;
            string folderpath = String.Empty;
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            DialogResult dr = fbd.ShowDialog();

            if (dr == DialogResult.OK)
                folderpath = fbd.SelectedPath;
            else
                folderpath = existingPath;
            
            return folderpath;
        }

        public string SetPath(TextBox sender)
        {
            string existingPath = sender.Text;
            string folderpath = String.Empty;
            OpenFileDialog fbd = new OpenFileDialog();
            fbd.Filter = "SQL Files (.sql)|*.sql";
            DialogResult dr = fbd.ShowDialog();

            if (dr == DialogResult.OK)
                folderpath = fbd.FileName;
            else
                folderpath = existingPath;
            return folderpath;
        }
        

        public void WriteConnectionStringToFile(string textconnectionString)
        {
            List<string> connString = new List<string>();
            connString =loader.lstConnectionString;

            List<string> lstconnString = new List<string>();
            lstconnString = loader.WriteConnectionString(textconnectionString);
                        
        }

       
        public void loadScripts(Label lblRootpathSelected, StringBuilder entrylog)
        {
            loader.InitializeLoader(lblRootpathSelected.Text, entrylog);
        }

       
        public void DisplayChildForm(ScriptExecutionManager scriptExeManager)
        {
            pointX = 30;
            pointY = 20;
            childForm form = new childForm(scriptExeManager);
            form.AutoScroll = true;
            form.Show();
        }

        public void LoadChildFormWithScriptDetails(childForm childForm)
        {
             //pointX = 30;
             //pointY = 20;
            childFrominstance = childForm;
            Button btn = new Button();
            btn.Name = "btnEdit";
            btn.Text = "Edit Configuration";
            btn.Location = new System.Drawing.Point(pointX, pointY);
            btn.Click += btn_Click;
            btn.BackColor = System.Drawing.Color.Silver;
            btn.Font = new Font(btn.Font, FontStyle.Bold);
            btn.Width = 200;
            childForm.Controls.Add(btn);

            Button btnSave = new Button();
            btnSave.Name = "btnSave";
            btnSave.Text = "Save Configuration";
            btnSave.Location = new System.Drawing.Point(300, pointY);
            btnSave.Click += btnSave_Click;
            btnSave.BackColor = System.Drawing.Color.Silver;
            btnSave.Font = new Font(btnSave.Font, FontStyle.Bold);
            btnSave.Width = 200;
            childForm.Controls.Add(btnSave);


            Button btnAdd = new Button();
            btnAdd.Name = "btnAdd";
            btnAdd.Text = "Add a new path";
            btnAdd.Location = new System.Drawing.Point(600, pointY);
            btnAdd.Click += btnAdd_Click;
            btnAdd.BackColor = System.Drawing.Color.Silver;
            btnAdd.Font = new Font(btnAdd.Font, FontStyle.Bold);
            btnAdd.Width = 200;
            childForm.Controls.Add(btnAdd);
            
            pointY += 40;

            if (loader.ScriptFiles != null)
                if (loader.ScriptFiles.Count > 0)
                    foreach (var item in loader.ScriptFiles)
                    {
                        AddControls(item, pointX, pointY, childForm);
                        pointY += 30;
                    }
        }

        private void AddControls(ScriptLocation item, int pointX, int pointY, childForm childForm)
        {
            TextBox newTextBox = new TextBox();
            newTextBox.Name = "txt@" + item.TextName;
            newTextBox.Text = item.FILEPATH;
            newTextBox.Location = new Point(pointX, pointY);
            newTextBox.ReadOnly = true;
            newTextBox.WordWrap = true;     
            newTextBox.Width = 800;
            newTextBox.BackColor = System.Drawing.Color.Azure;
         //   newTextBox.Font = new Font(newTextBox.Font, FontStyle.Bold);

            Button btnSet = new Button();
            btnSet.Name = "btnSet@" + item.TextName;
            btnSet.Text = "Set";
            btnSet.Location = new System.Drawing.Point(850, pointY);
            btnSet.Click += btnSet_Click;
            btnSet.BackColor = System.Drawing.Color.Silver;
            btnSet.Font = new Font(btnSet.Font, FontStyle.Bold);

            Button btnDelete = new Button();
            btnDelete.Name = "btnDelete@" + item.TextName;
            btnDelete.Text = "Delete";
            btnDelete.Location = new System.Drawing.Point(950, pointY);
            btnDelete.Click += btnDelete_Click;
            btnDelete.BackColor = System.Drawing.Color.Silver;
            btnDelete.Font = new Font(btnDelete.Font, FontStyle.Bold);

            childForm.Controls.Add(newTextBox);
            childForm.Controls.Add(btnSet);
            childForm.Controls.Add(btnDelete);
        }
      

        private void AddControls(String Name, int pointX, int pointY, childForm childForm)
        {
            TextBox newTextBox = new TextBox();
            newTextBox.Name = "txt@" + Name;
            newTextBox.Text = string.Empty;
            newTextBox.Location = new Point(pointX, pointY);
            newTextBox.Width = 800;
            newTextBox.WordWrap = true;
            newTextBox.BackColor = System.Drawing.Color.Azure;
           // newTextBox.Font = new Font(newTextBox.Font, FontStyle.Bold);

            Button btnSet = new Button();
            btnSet.Name = "btnSet@" + Name;
            btnSet.Text = "Set";
            btnSet.Location = new System.Drawing.Point(850, pointY);
            btnSet.Click += btnSet_Click;
            btnSet.BackColor = System.Drawing.Color.Silver;
            btnSet.Font = new Font(btnSet.Font, FontStyle.Bold);

            Button btnDelete = new Button();
            btnDelete.Name = "btnDelete@" + Name;
            btnDelete.Text = "Delete";
            btnDelete.Location = new System.Drawing.Point(950, pointY);
            btnDelete.Click += btnDelete_Click;
            btnDelete.BackColor = System.Drawing.Color.Silver;
            btnDelete.Font = new Font(btnDelete.Font, FontStyle.Bold);

            childForm.Controls.Add(newTextBox);
            childForm.Controls.Add(btnSet);
            childForm.Controls.Add(btnDelete);
        }
        static int CountOfNewItem = 0;

        void btnAdd_Click(object sender, EventArgs e)
        {
           CountOfNewItem = CountOfNewItem+1;
           AddControls("NewControl_" + CountOfNewItem, pointX, pointY, childFrominstance);
           pointY += 25;
        }

        void btnSet_Click(object sender, EventArgs e)
        {
            Button objBtn = sender as Button;
            string name = objBtn.Name;
            string[] textName = name.Split('@');
            string TextBoxName = "txt@" + textName[1];

            Control.ControlCollection coll = childFrominstance.Controls;
            foreach (Control c in coll)
            {
                if (c != null)
                    if (c.GetType() == typeof(TextBox))
                    {
                        TextBox txt = c as TextBox;
                        if(txt.ReadOnly == false)
                        { 
                            if (txt.Name == TextBoxName )
                            {
                                txt.Text = SetPath(txt);
                                break;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please Click Edit button to modify", "Info");
                            break;
                        }

                    }
            }
        }

        void btnSave_Click(object sender, EventArgs e)
        {
            List<string> NewScriptDetails = new List<string>();
            Control.ControlCollection coll = childFrominstance.Controls;
            foreach (Control c in coll)
            {
                if (c != null)
                    if (c.GetType() == typeof(TextBox))
                    {
                        TextBox txt = c as TextBox;
                        if(txt.Text !=string.Empty)
                        NewScriptDetails.Add(txt.Text);
                    }
            }

         string result= loader.WriteScriptDetails(NewScriptDetails,rootpath);
        }

        void btn_Click(object sender, EventArgs e)
        {
            Control.ControlCollection coll = childFrominstance.Controls;
            foreach (Control c in coll)
            {
                if (c != null)
                    if (c.GetType() == typeof(TextBox))
                    { 
                        TextBox txt = c as TextBox;
                        txt.ReadOnly = false;
                    }
            }
        }

        void btnDelete_Click(object sender, EventArgs e)
        {
            Button btnDelete = sender as Button;
            string name = btnDelete.Name;
            string[] textName = name.Split('@');
            string btnSetName = "btnSet@" + textName[1];
            string TextBoxName = "txt@" + textName[1];

            Control.ControlCollection coll = childFrominstance.Controls;
            TextBox txtToDelete = null;
            Button BtnSetToDelete = null;
            Button BtnDeleteToDelete = null;
            foreach (Control c in coll)
            {
                if (c != null)
                    if (c.GetType() == typeof(TextBox))
                    {
                        TextBox txt = c as TextBox;
                        if (txt.Name == TextBoxName)
                        {
                            txtToDelete = txt;
                        }
                    }
                    else if (c.GetType() == typeof(Button))
                    {
                        Button btn = c as Button;
                        if (btn.Name == btnSetName)
                        {
                            BtnSetToDelete = btn;
                        }
                        else if (btn.Name == name)
                        {
                            BtnDeleteToDelete = btn;
                        }
                    }
            }
            childFrominstance.Controls.Remove(txtToDelete);
            childFrominstance.Controls.Remove(BtnSetToDelete);
            childFrominstance.Controls.Remove(BtnDeleteToDelete);
        }
    }
}
