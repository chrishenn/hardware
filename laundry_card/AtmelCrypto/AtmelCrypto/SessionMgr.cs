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
using System.Linq;
using System.Text;
using System.IO;


using GemCard;

namespace AtmelCrypto
{
    public class SessionMgr
    {

        // Return values
        const ushort SC_OK = 0x9000;
        const ushort SC_NO_AUTH = 0x6900;
        const byte SC_PENDING = 0x9F;

        // Return values for session manager
        const int SESSION_OK = 0;
        const int SESSION_FAILED = -1;

        // Handle to the Atmel CryptoMem card
        private AtmelCryptoMem mem;

        // Handle to the iCard interface
        private CardNative iCard;

        // Handle to the UI
        private Form1 ui;

        // Flag to track if we are connected
        private Boolean connected = false;
        private Boolean authenticated = false;

        // Constructor
        public SessionMgr(Form1 formUI)
        {
            ui = formUI;
        }

        public Boolean isConnected()
        {
            return connected;
        }

        public Boolean isAuthenticated()
        {
            return authenticated;
        }

        public void Init()
        {
            /* Init card interfaces */
            iCard = new CardNative();  // interface to reader

            /* Look for smartcard reader */
            string[] readers = iCard.ListReaders();

            /* Add readers to dropdown menu */
            for (int i = 0; i < readers.Length; i++)
            {
                ui.AddSCReader(readers[i]);
            }

            /* Write info to console */
            ui.WriteConsole("~ Atmel Crypto Memory Utility");
            ui.WriteConsole("~ Hans Viksler - 2010");
        }

        public void Connect(string reader)
        {
            try
            {
                iCard.Connect(reader, SHARE.Shared, PROTOCOL.T0orT1);
                connected = true;
                ui.WriteConsole("Connected to " + reader);

                // Fetch the ATR, we should be able to get this through iCard interface
                // but I can't figure out how...
                APDUCommand cmd = AtmelCryptoMem.ApduGetAtr();
                APDUResponse apduResp;
                apduResp = iCard.Transmit(cmd);

                if (apduResp.Status != SC_OK && apduResp.Status != SC_NO_AUTH &&
                    apduResp.SW1 != SC_PENDING)
                    throw new Exception("Failed to get ATR: " + apduResp.ToString());

                // Instantiate mem card
                mem = new AtmelCryptoMem(apduResp.Data);
                UpdateAtrUI();
            }
            catch (Exception ex)
            {
                iCard.Disconnect(DISCONNECT.Unpower);
                ui.WriteConsole(ex.Message);
            }
        }

        public void UpdateAtrUI()
        {
            string atr = String.Format("{0:X02}{1:X02}{2:X02}{3:X02}{4:X02}{5:X02}{6:X02}{7:X02}",
                            mem.geometry.atr[0], mem.geometry.atr[1], mem.geometry.atr[2], mem.geometry.atr[3],
                            mem.geometry.atr[4], mem.geometry.atr[5], mem.geometry.atr[6], mem.geometry.atr[7]);
            ui.UpdateCardInfo(mem.geometry.name + " [" + atr + "]");
        }

        public void UpdateConfigUI()
        {
            ui.UpdateArPr(mem.ar, mem.pr);
            UpdateAtrUI();
            ui.UpdateDCR(String.Format("{0:X02}", mem.dcr));
            ui.UpdateFUSE(String.Format("{0:X02}", mem.fuse));
            ui.UpdateW7Attempts(String.Format("{0:X02}", mem.wr_pac[7]));
        }

        public string GetConfigString()
        {
            return mem.ConfigToString();
        }

        public int verifyPassword(int pwIndex)
        {
            return SESSION_OK;
        }

        public void Authenticate(byte[] pass)
        {
            UInt16 status;
            if (pass.Length != mem.passwd_length)
            {
                ui.WriteConsole("Password length = "+ pass.Length + " incorrect");
                return;
            }

            try
            {
                APDUResponse apduResp;
                APDUCommand cmd = mem.ApduVerifyPasswd(7, false);
                APDUParam p = new APDUParam();
                p.Data = pass;
                cmd.Update(p);
                apduResp = iCard.Transmit(cmd);
                status = apduResp.Status;
                if (apduResp.Status != SC_OK && apduResp.Status != SC_NO_AUTH &&
                    apduResp.SW1 != SC_PENDING)
                    throw new Exception("Authenticate failed: " + apduResp.ToString());
                else if (status == SC_NO_AUTH)
                    ui.WriteConsole("Access denied! Some fields will be incorrect.");
                else
                    authenticated = true;
            }
            catch (Exception ex)
            {
                ui.WriteConsole(ex.Message);
            }
        }

        public void ReadConfig()
        {
            ushort status = 0;
            if (connected)
            {
                try
                {
                    APDUResponse apduResp;
                    APDUCommand cmd = AtmelCryptoMem.ApduReadConfig();
                    apduResp = iCard.Transmit(cmd);
                    status = apduResp.Status;
                    if (apduResp.Status != SC_OK && apduResp.Status != SC_NO_AUTH &&
                        apduResp.SW1 != SC_PENDING)
                        throw new Exception("ReadConfig failed: " + apduResp.ToString());

                    if (status == SC_NO_AUTH)
                        ui.WriteConsole("Access denied! Some fields will be incorrect.");

                    // Parse response
                    mem.UpdateConfig(apduResp.Data);

                    cmd = mem.ApduReadFuse();
                    apduResp = iCard.Transmit(cmd);
                    status = apduResp.Status;
                    if (apduResp.Status != SC_OK && apduResp.Status != SC_NO_AUTH &&
                        apduResp.SW1 != SC_PENDING)
                        throw new Exception("ReadFuse failed: " + apduResp.ToString());

                    if (status == SC_NO_AUTH)
                        ui.WriteConsole("Access denied! Some fields will be incorrect.");

                    mem.fuse = apduResp.Data[0];

                    //Update UI
                    UpdateConfigUI();
                }
                catch (Exception ex)
                {
                    ui.WriteConsole(ex.Message);
                }
            }
        }

        public void WriteConfig(byte offset, byte len)
        {
            byte max_write_unit = 0x10;
            byte bytes_written = 0;
            byte woffset = offset;

            UInt16 status;
            if (connected)
            {
                try
                {
                    APDUResponse apduResp;
                    APDUParam param = new APDUParam();
                    byte[] config = mem.ConfigToBinary();

                    while (bytes_written < len)
                    {
                        byte write_size = Math.Min(max_write_unit, (byte)(len - bytes_written));
                        byte[] wbuf = new byte[write_size];
                        APDUCommand cmd = mem.ApduWriteConfig(woffset, write_size);
                        /* Copy chunk to write buf */
                        Buffer.BlockCopy(config, woffset, wbuf, 0, write_size);
                        param.Data = wbuf;
                        cmd.Update(param);
                        apduResp = iCard.Transmit(cmd);
                        status = apduResp.Status;
                        if (apduResp.Status != SC_OK && apduResp.Status != SC_NO_AUTH &&
                            apduResp.SW1 != SC_PENDING)
                            throw new Exception("SetUserZone failed: " + apduResp.ToString());

                        if (status == SC_NO_AUTH)
                            ui.WriteConsole("Access denied!");
                        bytes_written += write_size;
                        woffset += write_size;
                    }
                }
                catch (Exception ex)
                {
                    ui.WriteConsole(ex.Message);
                }
            }
        }

        public void ReadUserPage(byte pageno)
        {
            UInt16 status;
            if (connected)
            {
                try
                {
                    APDUResponse apduResp;
                    APDUCommand cmd = mem.ApduSetUserZone(pageno);
                    apduResp = iCard.Transmit(cmd);
                    status = apduResp.Status;
                    if (apduResp.Status != SC_OK && apduResp.Status != SC_NO_AUTH &&
                        apduResp.SW1 != SC_PENDING)
                        throw new Exception("SetUserZone failed: " + apduResp.ToString());

                    if (status == SC_NO_AUTH)
                        ui.WriteConsole("Access denied!");

                    cmd = mem.ApduReadUserZone();
                    apduResp = iCard.Transmit(cmd);
                    status = apduResp.Status;
                    if (apduResp.Status != SC_OK && apduResp.Status != SC_NO_AUTH &&
                        apduResp.SW1 != SC_PENDING)
                        throw new Exception("ReadUserZone failed: " + apduResp.ToString());

                    if (status == SC_NO_AUTH)
                        ui.WriteConsole("Access denied!");

                    // Copy to memory
                    Buffer.BlockCopy(apduResp.Data, 0, mem.user_page[pageno], 0, mem.geometry.page_size);
                }
                catch (Exception ex)
                {
                    ui.WriteConsole(ex.Message);
                }
            }
        }

        public void WriteUserPage(byte pageno, byte[] data)
        {
            UInt16 status;
            if (connected)
            {
                try
                {
                    int bytes_written = 0;
                    int max_write_unit = 0x08;
                    int offset = 0;
                    APDUParam param = new APDUParam();
                    APDUResponse apduResp;
                    APDUCommand cmd = mem.ApduSetUserZone(pageno);
                    apduResp = iCard.Transmit(cmd);
                    status = apduResp.Status;
                    if (apduResp.Status != SC_OK && apduResp.Status != SC_NO_AUTH &&
                        apduResp.SW1 != SC_PENDING)
                        throw new Exception("SetUserZone failed: " + apduResp.ToString());

                    if (status == SC_NO_AUTH)
                        ui.WriteConsole("Access denied!");

                    while (bytes_written < data.Length)
                    {
                        int write_len = Math.Min(max_write_unit, data.Length - bytes_written);
                        if (write_len > 0xFF) write_len = 0xFF;
                        byte[] w_buf = new byte[write_len];
                        cmd = mem.ApduWriteUserZone(pageno, offset, (byte)write_len);
                        Buffer.BlockCopy(data, offset, w_buf, 0, write_len);
                        param.Data = w_buf;
                        cmd.Update(param);
                        apduResp = iCard.Transmit(cmd);
                        status = apduResp.Status;
                        if (apduResp.Status != SC_OK && apduResp.Status != SC_NO_AUTH &&
                            apduResp.SW1 != SC_PENDING)
                            throw new Exception("WriteUserZone failed: " + apduResp.ToString());

                        if (status == SC_NO_AUTH)
                            ui.WriteConsole("Access denied!");
                        bytes_written += write_len;
                        offset += write_len;
                    }
                }
                catch (Exception ex)
                {
                    ui.WriteConsole(ex.Message);
                }
            }
        }

        public void WriteFile(string filename, byte[] data)
        {
            ui.WriteConsole("Writing file " + filename);
            using (FileStream stream = new FileStream(filename, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(data);
                    writer.Close();
                }
            }
        }

        public byte[] ReadFile(string filename, int len)
        {
            byte[] buf = new byte[len];
            ui.WriteConsole("Reading file " + filename);
            using (FileStream stream = new FileStream(filename, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    buf = reader.ReadBytes(len);
                    reader.Close();
                }
            }
            return buf;
        }

        /* Macro handlers */
        public void MacroBackup (string root)
        {
            byte[] ar_backup = new byte[mem.geometry.ar_pr_count];

            ui.WriteConsole("Backing up " + mem.geometry.name + " to " + root);

            /* Backup the config */
            ReadConfig();
            WriteFile(root + "\\config.bin", mem.ConfigToBinary());

            ui.WriteConsole("Backing up permissions...");
            Buffer.BlockCopy(mem.ar, 0, ar_backup, 0, mem.geometry.ar_pr_count);

            ui.WriteConsole("Changing permissions to (no passwd, no auth)");
            for (int i = 0; i < mem.geometry.ar_pr_count; i++)
                mem.ar[i] = 0xFF;

            WriteConfig(AtmelCryptoMem.AR_OFFSET, (byte)(mem.geometry.ar_pr_count * 2));

            /* Backup user pages */
            for (byte i = 0; i < mem.geometry.page_count; i++)
            {
                // Read page into RAM
                ReadUserPage(i);

                // Write to disk
                WriteFile(root + "\\page" + i + ".bin", mem.user_page[i]);
            }

            ui.WriteConsole("Restoring permissions...");
            Buffer.BlockCopy(ar_backup, 0, mem.ar, 0, mem.geometry.ar_pr_count);
            WriteConfig(AtmelCryptoMem.AR_OFFSET, (byte)(mem.geometry.ar_pr_count * 2));
            ui.WriteConsole("Backup Complete!");
        }

        /* Macro handlers */
        public void MacroRestore(string root)
        {
            byte[] ar_backup = new byte[mem.geometry.ar_pr_count];
            byte[] config = new byte[AtmelCryptoMem.CONFIG_LENGTH];
            byte[] page = new byte[mem.geometry.page_size];
            byte backup_dcr;

            ui.WriteConsole("Restoring from " + root + " to " + mem.geometry.name);

            /* Restore the config */
            ui.WriteConsole("Restoring config...");
            config = ReadFile(root + "\\config.bin", config.Length);
            mem.UpdateConfig(config);
            Buffer.BlockCopy(mem.ar, 0, ar_backup, 0, mem.geometry.ar_pr_count);
            backup_dcr = mem.dcr;
            mem.dcr &= AtmelCryptoMem.SUPERVISOR_ENABLE;
            WriteConfig(AtmelCryptoMem.DCR_OFFSET, 1);

            /* Change permissions */
            ui.WriteConsole("Changing permissions to (no passwd, no auth)");
            for (int i = 0; i < mem.geometry.ar_pr_count; i++)
                mem.ar[i] = 0xFF;
            WriteConfig(AtmelCryptoMem.AR_OFFSET, (byte)(mem.geometry.ar_pr_count * 2));

            /* Restore user pages */
            ui.WriteConsole("Restoring user pages...");
            for (byte i = 0; i < mem.geometry.page_count; i++)
            {
                // Write to disk
                page = ReadFile(root + "\\page" + i + ".bin", page.Length);

                // Write user page out
                WriteUserPage(i, page);
            }

            /* Restore permissions */
            ui.WriteConsole("Restoring permissions...");
            Buffer.BlockCopy(ar_backup, 0, mem.ar, 0, mem.geometry.ar_pr_count);
            WriteConfig(AtmelCryptoMem.AR_OFFSET, (byte)(mem.geometry.ar_pr_count * 2));

            ui.WriteConsole("Restoring DCR");
            mem.dcr = backup_dcr;
            WriteConfig(AtmelCryptoMem.DCR_OFFSET, 1);
            ui.WriteConsole("Restore Complete!");
        }

    }
}
