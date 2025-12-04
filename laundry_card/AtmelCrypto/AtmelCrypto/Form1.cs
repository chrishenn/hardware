/*
 * Hans Viksler 2010
 *
 * This work is available under the creative commons by-nc-sa license.
 * An overview of the license can be found here: http://creativecommons.org/licenses/by-nc-sa/3.0/
 * The entire license can be found here: http://creativecommons.org/licenses/by-nc-sa/3.0/legalcode.
 *
 * The Gemcard library was written by orouit and licensed under The Code Project Open License (CPOL)
 * http://www.codeproject.com/info/cpol10.aspx
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AtmelCrypto
{
    public partial class Form1 : Form
    {
        private SessionMgr mgr;

        public Form1()
        {
            InitializeComponent();
        }

        public void AddSCReader(string s)
        {
            readerComboBox.Items.Add(s);
            readerComboBox.SelectedItem = s;
        }

        private void readerSelectBtn_Click(object sender, EventArgs e)
        {
            mgr.Connect((string)readerComboBox.SelectedItem);
        }

        public void SetSessionMgr(SessionMgr m)
        {
            mgr = m;
        }

        public void WriteConsole(string s)
        {
            consoleTB.Text += s + "\r\n";
            consoleTB.SelectionStart = consoleTB.Text.Length;
            consoleTB.ScrollToCaret();
        }

        public void UpdateArPr(byte[] ar, byte[] pr)
        {
            if (ar.Length != 4 || pr.Length != 4)
                return;

            ar0TB.Text = String.Format("{0:X02}", ar[0]);
            ar1TB.Text = String.Format("{0:X02}", ar[1]);
            ar2TB.Text = String.Format("{0:X02}", ar[2]);
            ar3TB.Text = String.Format("{0:X02}", ar[3]);

            pr0TB.Text = String.Format("{0:X02}", pr[0]);
            pr1TB.Text = String.Format("{0:X02}", pr[1]);
            pr2TB.Text = String.Format("{0:X02}", pr[2]);
            pr3TB.Text = String.Format("{0:X02}", pr[3]);
        }

        public void UpdateCardInfo(string s)
        {
            cardTB.Text = s;
        }

        public void UpdateDCR(string s)
        {
            dcrTB.Text = s;
        }

        public void UpdateFUSE(string s)
        {
            fuseTB.Text = s;
        }
        public void UpdateW7Attempts(string s)
        {
            write7AttemptTB.Text = s;
        }

        private void readConfigBtn_Click(object sender, EventArgs e)
        {
            if (mgr.isConnected())
            {
                /* Try and auth if we aren't authenticated */
                if (!mgr.isAuthenticated())
                    mgr.Authenticate(HexStringToByteArray(write7passTB.Text));

                // Read the config from session manager
                mgr.ReadConfig();
            }
            else
            {
                WriteConsole("Not connected!");
            }
        }

        private static byte[] HexStringToByteArray(string Hex)
        {
            byte[] Bytes = new byte[Hex.Length / 2];
            int[] HexValue = new int[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,
                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0A, 0x0B, 0x0C, 0x0D,
                                 0x0E, 0x0F };

            for (int x = 0, i = 0; i < Hex.Length; i += 2, x += 1)
            {
                Bytes[x] = (byte)(HexValue[Char.ToUpper(Hex[i + 0]) - '0'] << 4 |
                                  HexValue[Char.ToUpper(Hex[i + 1]) - '0']);
            }

            return Bytes;
        }

        private void macroDumpDevBtn_Click(object sender, EventArgs e)
        {
            if (mgr.isConnected())
            {
                /* Try and auth if we aren't authenticated */
                if (!mgr.isAuthenticated())
                    mgr.Authenticate(HexStringToByteArray(write7passTB.Text));

                // Read the config from session manager
                mgr.ReadConfig();
                WriteConsole(mgr.GetConfigString());
            }
            else
            {
                WriteConsole("Not connected!");
            }
        }

        private void macroBackupBtn_Click(object sender, EventArgs e)
        {
            /* Select the backup directory */
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {

                if (mgr.isConnected())
                {
                    /* Try and auth if we aren't authenticated */
                    if (!mgr.isAuthenticated())
                        mgr.Authenticate(HexStringToByteArray(write7passTB.Text));

                    if (mgr.isAuthenticated())
                    {
                        //Backup card into this directory
                        mgr.MacroBackup(folderBrowserDialog.SelectedPath);
                    }
                    else
                    {
                        WriteConsole("Authentication failed!");
                    }
                }
                else
                {
                    WriteConsole("Not connected!");
                }
            }
        }

        private void macroRestoreBtn_Click(object sender, EventArgs e)
        {
            /* Select the backup directory */
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {

                if (mgr.isConnected())
                {
                    /* Try and auth if we aren't authenticated */
                    if (!mgr.isAuthenticated())
                        mgr.Authenticate(HexStringToByteArray(write7passTB.Text));

                    if (mgr.isAuthenticated())
                    {
                        //Backup card into this directory
                        mgr.MacroRestore(folderBrowserDialog.SelectedPath);
                    }
                    else
                    {
                        WriteConsole("Authentication failed!");
                    }
                }
                else
                {
                    WriteConsole("Not connected!");
                }
            }

        }

        private void writeConfigBtn_Click(object sender, EventArgs e)
        {
            WriteConsole("Feature not implemented!");
        }

        private void readUserPageBtn_Click(object sender, EventArgs e)
        {
            WriteConsole("Feature not implemented!");
        }

        private void writeUserPage_Click(object sender, EventArgs e)
        {
            WriteConsole("Feature not implemented!");
        }
    }
}
