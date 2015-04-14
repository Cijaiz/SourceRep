using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoScriptExecuter
{
    public partial class childForm : Form
    {
        public childForm()
        {
            InitializeComponent();
            
        }
        public childForm(ScriptExecutionManager scriptExeManager)
        {
            scriptExeManager.LoadChildFormWithScriptDetails(this);
            this.Width = 1200;
            this.Height = 500;
            this.Text = "Modify Script Details";
            this.BackColor =System.Drawing.Color.LightSteelBlue;
        }
    }
}
