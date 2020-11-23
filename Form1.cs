using DiscordRPC;
using DiscordRPC.Logging;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace DiscordCustomRPC
{
    public partial class Form1 : Form
    {
        private readonly string ClientID = "779000218573864980";                                  // Your Client ID at https://discord.com/developers/applications

        private Assets assets = new Assets()                                                        //your images at https://discord.com/developers/applications
        {
            LargeImageKey = "obeme",
            LargeImageText = "чекушка, порнушка, засерушка",
            SmallImageKey = "rabbit",
            SmallImageText = "крол ебать"
        };

        private readonly string savedProperty = Environment.CurrentDirectory + @"\savedProperty.txt";
        private string wintitle;

        public DiscordRpcClient client;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private string GetActiveWindowTitle()
        {
            const int nChars = 64;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        public void Initialization()
        {
            client = new DiscordRpcClient(ClientID);

            client.OnReady += (sender, e) =>
            {
                label1.Text = $"Received Ready from user {e.User.Username}";
            };

            client.OnPresenceUpdate += (sender, e) =>
            {
                label2.Text = $"Received Update! {e.Presence}";
            };

            client.Initialize();

            client.SetPresence(new RichPresence()
            {
                Details = label3.Text,
                State = textBox2.Text,

                Assets = assets
            });
        }

        public Form1()
        {
            InitializeComponent();
            if (File.Exists(savedProperty))
                textBox2.Text = File.ReadAllText(savedProperty);
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
            if (client != null)
            {
                client.SetPresence(new RichPresence()
                {
                    Details = label3.Text,
                    State = textBox2.Text,
                    Assets = assets
                });
            }
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
            if (wintitle != label3.Text)
            {
                client.SetPresence(new RichPresence()
                {
                    Details = label3.Text,
                    State = textBox2.Text,
                    Assets = assets
                });
                wintitle = label3.Text;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            File.WriteAllText(savedProperty, textBox2.Text);
        }
    }
}