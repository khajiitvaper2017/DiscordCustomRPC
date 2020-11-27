using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using DiscordRPC;

namespace DiscordCustomRPC
{
    public partial class Form1 : Form
    {
        private const string
            ClientID = "779000218573864980"; // Your Client ID at https://discord.com/developers/applications

        private readonly Assets assets = new Assets //your images at https://discord.com/developers/applications
        {
            LargeImageKey = "obeme",
            LargeImageText = "чекушка, порнушка, засерушка",
            SmallImageKey = "rabbit",
            SmallImageText = "крол ебать"
        };

        private readonly string savedProperty = Environment.CurrentDirectory + @"\savedProperty.txt";

        public DiscordRpcClient client;
        private string wintitle;

        public Form1()
        {
            InitializeComponent();
            if (File.Exists(savedProperty))
                textBox2.Text = File.ReadAllText(savedProperty);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private static string GetActiveWindowTitle()
        {
            const int nChars = 64;
            var Buff = new StringBuilder(nChars);
            var handle = GetForegroundWindow();

            return GetWindowText(handle, Buff, nChars) > 0 ? Buff.ToString() : null;
        }

        public void Initialization()
        {
            client = new DiscordRpcClient(ClientID);

            client.OnReady += (sender, e) => { label1.Text = $"Received Ready from user {e.User.Username}"; };

            client.OnPresenceUpdate += (sender, e) => { label2.Text = $"Received Update! {e.Presence}"; };

            client.Initialize();

            client.SetPresence(new RichPresence
            {
                Details = label3.Text,
                State = textBox2.Text,

                Assets = assets
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            wintitle = GetActiveWindowTitle();
            label1.Text = "";
            label2.Text = "";
            Initialization();
            timer1.Start();
        }

        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
            SetPresence();
        }

        public void SetPresence()
        {
            client?.SetPresence(new RichPresence
            {
                Details = label3.Text,
                State = textBox2.Text,
                Assets = assets
            });
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            label2.Text = "";
            client.Deinitialize();
            Initialization();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Hide();
            notifyIcon1.Visible = true;
        }

        private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            notifyIcon1.Visible = false;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            label3.Text = GetActiveWindowTitle();
        }

        private void Label3_TextChanged(object sender, EventArgs e)
        {
            if (wintitle == label3.Text) return;
            client.SetPresence(new RichPresence
            {
                Details = label3.Text,
                State = textBox2.Text,
                Assets = assets
            });
            wintitle = label3.Text;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            File.WriteAllText(savedProperty, textBox2.Text);
        }
    }
}