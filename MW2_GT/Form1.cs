using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using XDevkit;
using JRPC_Client;

namespace MW2_GT
{
    public partial class Form1 : Form
    {
        private IXboxConsole Xbox360;

        public Form1() => InitializeComponent();
        bool CL_InGame() => Xbox360.ReadBool(Xbox360.ReadUInt32(0x825E21F0) + 0x8);

        public void SetInGameInfo(string Gamertag, string Clantag) {
            Gamertag = Gamertag.Split('\0')[0];
            Clantag = Clantag.Split('\0')[0];

            string XUIDString = Xbox360.ReadString(0x838BA850, 0x12).Split('\0')[0];

            int NatType = Xbox360.ReadInt32(0x8253A508);

            int Rank = Xbox360.Call<int>(0x822C8688, 0);
            int Prestige = Xbox360.Call<int>(0x822C86E8, 0);

            Xbox360.CallVoid(0x82224990, 0, $"cmd userinfo \"name\\{Gamertag}\\clanAbbrev\\{Clantag}\\xuid\\{XUIDString.ToLower()}\\natType\\{NatType}\\rank\\{Rank}\\prestige\\{Prestige}\"");
        }

        public void SetGamertag(string Gamertag) {
            if (Gamertag.Length >= 16) {
                MessageBox.Show("Oi pal, gamertag is too long.");
                return;
            }

            if (CL_InGame())
                SetInGameInfo(Gamertag, Xbox360.ReadString(0x82687060, 4));

            Xbox360.WriteString(0x838BA824, Gamertag);
        }

        public void SetClantag(string Clantag) {
            if (Clantag.Length > 4) {
                MessageBox.Show("Oi pal, clantag is too long.");
                return;
            }

            if (CL_InGame()) 
                SetInGameInfo(Xbox360.ReadString(0x838BA824, 16), Clantag);

            Xbox360.WriteString(0x82687060, Clantag);
        }

        private void button1_Click(object sender, EventArgs e) {
            if (Xbox360.Connect(out Xbox360)) {
                MessageBox.Show("Connected to console.");
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            if (Xbox360 != null) {
                SetClantag(textBox1.Text);
                Xbox360.XNotify("Clantag set!");
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            if (Xbox360 != null) {
                SetGamertag(textBox2.Text);
                Xbox360.XNotify("Gamertag set!");
            }
        }
    }
}
