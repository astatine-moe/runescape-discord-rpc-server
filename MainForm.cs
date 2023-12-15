using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace runescape_discord_rpc_server
{
    public partial class MainForm : Form
    {

        private ContextMenuStrip contextMenuStrip;
        public MainForm()
        {
            InitializeComponent();

            contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.Add("Start", null, StartAction);
            contextMenuStrip.Items.Add("Stop", null, StopAction);
            contextMenuStrip.Items.Add("Exit", null, ExitAction);

            notifyIcon1.ContextMenuStrip = contextMenuStrip;
        }



        private void StartAction(object sender, EventArgs e)
        {
            // Implement the "Start" action
            // Show your form or perform necessary actions
        }

        private void StopAction(object sender, EventArgs e)
        {
            // Implement the "Stop" action
        }

        private void ExitAction(object sender, EventArgs e)
        {
            // Implement the "Exit" action
            Application.Exit();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
