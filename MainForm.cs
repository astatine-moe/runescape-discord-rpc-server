using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiscordRPC;
using DiscordRPC.Message;
using System.Net;
using Newtonsoft.Json;
using System.Diagnostics;

namespace runescape_discord_rpc_server
{
    public partial class MainForm : Form
    {
        //http server
        public static HttpListener listener;
        public static string url = "http://localhost:8000/";
        private static DateTime lastRequestTime = DateTime.MinValue;
        private static readonly TimeSpan clearPresenceCooldown = TimeSpan.FromMinutes(1); 
        private static System.Windows.Forms.Timer clearPresenceTimer = new System.Windows.Forms.Timer();

        public static async Task HandleIncomingConnections()
        {
            bool runServer = true;

            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();

            while (runServer)
            {
                // Note: This is a simple implementation. In a production environment, you would want to handle exceptions and manage the server's lifecycle more carefully.
                HttpListenerContext context = await listener.GetContextAsync();

                // Process the request and set Discord RPC based on the received information

                HandleRequest(context);
            }
        }

        private static void HandleRequest(HttpListenerContext context)
        {
            // Assuming the POST data is sent as JSON, you would need to deserialize it to get the required information
            string postData;
            using (var reader = new System.IO.StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
            {
                postData = reader.ReadToEnd();
            }
            // Deserialize JSON (You may need to use a JSON library like Newtonsoft.Json)
            // Example: var rpcData = JsonConvert.DeserializeObject<RpcData>(postData);

            // Set Discord RPC based on the received information
            SetDiscordRpc(postData);

            // Send a response (you may want to customize this based on your needs)
            byte[] responseBytes = Encoding.UTF8.GetBytes("OK");
            context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
            context.Response.Close();
        }

        private static void SetDiscordRpc(string postData)
        {
            // Deserialize JSON (You may need to use a JSON library like Newtonsoft.Json)
            // Example: var rpcData = JsonConvert.DeserializeObject<RpcData>(postData);

            // Assuming you have a class RpcData with properties like Details, State, LargeImageKey, etc.
            // Example: client.SetPresence(new RichPresence()
            // {
            //     Details = rpcData.Details,
            //     State = rpcData.State,
            //     Assets = new Assets()
            //     {
            //         LargeImageKey = rpcData.LargeImageKey,
            //         LargeImageText = rpcData.LargeImageText,
            //         SmallImageKey = rpcData.SmallImageKey,
            //         SmallImageText = rpcData.SmallImageText
            //     }
            // });

            // Example without a separate RpcData class (directly using dynamic)

            string processName = "runescape";

            Process[] processes = Process.GetProcessesByName(processName);

            if (processes.Length > 0)
            {
                if (rpcReady)
                {
                    dynamic rpcData = JsonConvert.DeserializeObject(postData);
                    if (rpcData != null)
                    {
                        lastRequestTime = DateTime.Now;
                        if (rpcData.clear == true)
                        {
                            // If "clear" is set to true, clear the presence
                            client.ClearPresence();
                            Console.WriteLine("Discord RPC presence cleared.");
                        }
                        else
                        {

                            client.SetPresence(new RichPresence()
                            {
                                Details = rpcData.Details,
                                State = rpcData.State,
                                Assets = new Assets()
                                {
                                    LargeImageKey = rpcData.LargeImageKey,
                                    LargeImageText = rpcData.LargeImageText,
                                    SmallImageKey = rpcData.SmallImageKey,
                                    SmallImageText = rpcData.SmallImageText
                                }
                            });
                        }
                    }
                }
            }
        }

        //ui elements
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem startMenuItem;
        private ToolStripMenuItem stopMenuItem;
        private ToolStripSeparator toolStripSeparator;
        private ToolStripLabel statusLabel;

        private string applicationState = "Stopped";

        //discord rpc
        private static DiscordRpcClient client;
        private static bool rpcReady = false;

        public MainForm()
        {
            InitializeComponent();

            clearPresenceTimer.Interval = 10000; // Adjust the interval as needed (e.g., 10000 milliseconds = 10 seconds)
            clearPresenceTimer.Tick += OnTimerElapsed;
            clearPresenceTimer.Start();

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
            Task.Run(() => HandleIncomingConnections());


            applicationState = "Running";
            startMenuItem.Visible = false; // Hide "Start"
            stopMenuItem.Visible = true;   // Show "Stop"
            UpdateStatusLabel();
        }

        private void StopAction(object sender, EventArgs e)
        {
            // Stop the HTTP server
            if (listener != null && listener.IsListening)
            {
                listener.Stop();
                listener.Close();
            }
            client.ClearPresence();

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
            client.Dispose();
            Application.Exit();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            StartAction(null, null);
            StartRpcClient();
        }
        private void UpdateStatusLabel()
        {
            // Update the status label text
            statusLabel.Text = $"RPC Status: {applicationState}";
        }

        private static void StartRpcClient()
        {
            client = new DiscordRpcClient("1185241374711361596");


            client.OnReady += (sender, msg) =>
            {
                
                Console.WriteLine("Connected to discord with user {0}", msg.User.Username);
                rpcReady = true;

            };
            client.OnPresenceUpdate += (sender, msg) =>
            {
                //The presence has updated
                Console.WriteLine("Presence has been updated! ");
            };

            client.Initialize();

            /*client.SetPresence(new RichPresence()
            {
                Details = "Exploring Gielinor",
                State = "In Game",
                Timestamps = Timestamps.FromTimeSpan(10),
                Assets = new Assets()
                {
                    LargeImageKey = "runescape-icon",
                    LargeImageText = "RuneScape"
                }
            });*/


        }

        private static void OnTimerElapsed(object sender, EventArgs e)
        {
            if (rpcReady)
            {
                string processName = "runescape";

                Process[] processes = Process.GetProcessesByName(processName);

                if (processes.Length > 0)
                {
                    // Check if enough time has passed since the last request
                    if (DateTime.Now - lastRequestTime >= clearPresenceCooldown)
                    {
                        // If cooldown has passed, clear the presence
                        client.ClearPresence();
                        Console.WriteLine("Discord RPC presence cleared.");
                    }
                    else
                    {
                        Console.WriteLine("Cooldown period active. Skipping ClearPresence.");
                    }
                }
                else
                {
                    //RuneScape not open, clear
                    client.ClearPresence();
                }
            }
        }

    }
}
