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
        private ToolStripMenuItem startMenuItem;
        private ToolStripMenuItem stopMenuItem;
        private ToolStripSeparator toolStripSeparator;
        private ToolStripLabel statusLabel;
        private string applicationState = "Stopped";
        public MainForm()
        {
            InitializeComponent();

            contextMenuStrip = new ContextMenuStrip();

            startMenuItem = new ToolStripMenuItem($"Start", null, StartAction);
            stopMenuItem = new ToolStripMenuItem($"Stop", null, StopAction);
            statusLabel = new ToolStripLabel($"RPC Status: {applicationState}");

            // Create a horizontal line divider
            toolStripSeparator = new ToolStripSeparator();

            // Set initial visibility
            startMenuItem.Visible = true;
            stopMenuItem.Visible = false;


            contextMenuStrip.Items.Add(statusLabel);
            contextMenuStrip.Items.Add(toolStripSeparator);
            contextMenuStrip.Items.Add(startMenuItem);
            contextMenuStrip.Items.Add(stopMenuItem);
            contextMenuStrip.Items.Add("Exit", null, ExitAction);

            notifyIcon1.ContextMenuStrip = contextMenuStrip;
        }



        private void StartAction(object sender, EventArgs e)
        {
            // Implement the "Start" action
            // Update state and menu items
            applicationState = "Running";
            startMenuItem.Visible = false; // Hide "Start"
            stopMenuItem.Visible = true;   // Show "Stop"
            UpdateStatusLabel();
        }

        private void StopAction(object sender, EventArgs e)
        {
            // Implement the "Stop" action
            // Update state and menu items
            applicationState = "Stopped";
            startMenuItem.Visible = true;    // Show "Start"
            stopMenuItem.Visible = false;    // Hide "Stop"
            UpdateStatusLabel();
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
        private void UpdateStatusLabel()
        {
            // Update the status label text
            statusLabel.Text = $"RPC Status: {applicationState}";
        }
    }
}
