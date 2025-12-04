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

using GemCard;

namespace AtmelCrypto
{

    class AtmelCryptoMem
    {
        // Geometry of this card
        public Geometry geometry;

        // Config must be 0xF0 bytes long
        public const int CONFIG_LENGTH = 0xF0;

        // Config header
        public const int FAB_LENGTH = 2;
        public byte[] fab_code { get; set; }

        public const int MTZ_LENGTH = 2;
        public byte[] mtz { get; set; }

        public const int MANU_CODE_LENGTH = 4;
        public byte[] manu_code { get; set; }

        public const int LOT_HIST_LENGTH = 8;
        public byte[] lot_hist_code { get; private set; }


        // Access control section
        // DCR = Device configuration register
        // SME | UCR | UAT | ETA | CS3 | CS2 | CS1 | CS0
        // SME = supervisor mode enable = 0
        // UCR = unlimited checksum reads = 0
        // UAT = unlimited suthentication trials = 0
        // EAT = eight trials allowed = 0
        // CS3 - CS0 = chip select (sync mode)
        public byte dcr { get; set; }
        public const byte SUPERVISOR_ENABLE = 0x7F;
        public const byte DCR_OFFSET = 0x18;

        // Fuse byte
        // RSVD | RSVD | RSVD | RSVD | SEC | PER | CMA | FAB
        // RSVD = reserved
        // SEC = lot history lock (blown by atmel)
        // PER = config memory lcok
        // CMA = card manu code lock
        // FAB = ATR + fab code lock
        public byte fuse { get; set; }

        // OEM set id
        public const int ID_LENGTH = 7;
        public byte[] id { get; set; }

        // AR = Access register
        // PM1 | PM0 | AM1 | AM0 | ER | WLM | MDF | PGO
        // (PM1,PM0) 11=no passwd, 10 wr passwd, 0* rw passwd
        // (AM1,AM0) 11=no auth, 10=w auth, 01=normal auth, 00=dual access
        // ER = encyption required = 0
        // WLM = write lock mode = 0
        // MDF = modify forbidden = user zone read-only
        // PGO = program only = 0
        public byte[] ar { get; set; }
        public const byte AR_OFFSET = 0x20;

        // PR = Password register
        // AK1 | AK0 | POK1 | POK0 | RES | PW2 |PW1 | PW0
        // (AK1,AK0) = which auth key to use
        // (POK1,POK0) = which POK key to use (dual mode only)
        // RES = reserved
        // (PW2,PW1,PW0) = which pw set to use
        public byte[] pr { get; set; }

        public const int ISSUER_CODE_LENGTH = 16;
        public const int ISSUER_CODE_OFFSET = 0x40;
        public byte[] issuer_code { get; set; }

        // Cryptography section
        // AAC = authentication attempts counter
        public const int CRYPTOGRAM_LENGTH = 7;
        public const int SESSION_KEY_LENGTH = 8;

        // Cryptograms and session keys
        public const int CRYPTO_COUNT = 4;
        private byte[] aac;
        private byte[][] c { get; set; }
        private byte[][] s { get; set; }

        // Secret seed section
        public const int SEED_LENGTH = 8;
        public const int SEED_COUNT = 4;
        private byte[][] seed { get; set; }

        // Password section
        // PAC = password attempt counter
        public const int PASSWD_LENGTH = 3;
        public const int PASSWD_COUNT = 8;
        public int passwd_length = PASSWD_LENGTH;
        public byte[] rd_pac { get; set; }
        public byte[] wr_pac { get; set; }
        public byte[][] wr_pass { get; set; }
        public byte[][] rd_pass { get; set; }

        // Page data
        // Each cryptoMemory card has a different page geometry
        public byte[][] user_page { get; set; }


        // Constructor
        // Takes ATR to reolve card type and initializes all fields
        public AtmelCryptoMem (byte[] card_atr)
        {

            // Initialize common fields
            fab_code = new byte[FAB_LENGTH];
            mtz = new byte[MTZ_LENGTH];
            manu_code = new byte[MANU_CODE_LENGTH];
            lot_hist_code = new byte[LOT_HIST_LENGTH];
            id = new byte[ID_LENGTH];
            issuer_code = new byte[ISSUER_CODE_LENGTH];

            // Initialize supported cards
            //const int CRYPTOMEM_DEVICE_COUNT = 9;
            //Geometry[] card = new Geometry[CRYPTOMEM_DEVICE_COUNT];
            Geometry[] card = {
                               new Geometry("Atmel AT88SC0104C", new byte[] { 0x3B, 0xB2, 0x11, 0x00, 0x10, 0x80, 0x00, 0x01 }, 4, 32, 4, 24),
                               new Geometry("Atmel AT88SC0204C", new byte[] { 0x3B, 0xB2, 0x11, 0x00, 0x10, 0x80, 0x00, 0x02 }, 4, 64, 4, 24),
                               new Geometry("Atmel AT88SC0404C", new byte[] { 0x3B, 0xB2, 0x11, 0x00, 0x10, 0x80, 0x00, 0x04 }, 4, 128, 4, 24),
                               new Geometry("Atmel AT88SC0808C", new byte[] { 0x3B, 0xB2, 0x11, 0x00, 0x10, 0x80, 0x00, 0x08 }, 8, 128, 8, 16),
                               new Geometry("Atmel AT88SC1616C", new byte[] { 0x3B, 0xB2, 0x11, 0x00, 0x10, 0x80, 0x00, 0x16 }, 16, 128, 16, 8),
                               new Geometry("Atmel AT88SC3216C", new byte[] { 0x3B, 0xB2, 0x11, 0x00, 0x00, 0x00, 0x00, 0x32 }, 16, 256, 16, 0),
                               new Geometry("Atmel AT88SC6416C", new byte[] { 0x3B, 0xB2, 0x11, 0x00, 0x00, 0x00, 0x00, 0x64 }, 16, 512, 16, 0),
                               new Geometry("Atmel AT88SC12816C", new byte[] { 0x3B, 0xB2, 0x11, 0x00, 0x00, 0x00, 0x01, 0x28 }, 16, 1024, 16, 0),
                               new Geometry("Atmel AT88SC25616C", new byte[] { 0x3B, 0xB2, 0x11, 0x00, 0x00, 0x00, 0x02, 0x56 }, 16, 2048, 16, 0)
                              };

            // Which card are we working on?
            CardDetect(card, card_atr);

            ar = new byte[geometry.ar_pr_count];
            pr = new byte[geometry.ar_pr_count];

            c = new byte[CRYPTO_COUNT][];
            s = new byte[CRYPTO_COUNT][];
            aac = new byte[CRYPTO_COUNT];

            for (int i = 0; i < CRYPTO_COUNT; i++)
            {
                c[i] = new byte[CRYPTOGRAM_LENGTH];
                s[i] = new byte[SESSION_KEY_LENGTH];
            }

            seed = new byte[SEED_COUNT][];
            for (int i = 0; i < SEED_COUNT; i++)
            {
                seed[i] = new byte[SEED_LENGTH];
            }

            rd_pac = new byte[PASSWD_COUNT];
            wr_pac = new byte[PASSWD_COUNT];
            rd_pass = new byte[PASSWD_COUNT][];
            wr_pass = new byte[PASSWD_COUNT][];
            for (int i = 0; i < PASSWD_COUNT; i++)
            {
                rd_pass[i] = new byte[PASSWD_LENGTH];
                wr_pass[i] = new byte[PASSWD_LENGTH];
            }

            // Allocate user pages
            user_page = new byte[geometry.page_count][];
            for (int i = 0; i < geometry.page_count; i++)
            {
                user_page[i] = new byte[geometry.page_size];
            }
        }


        public int UpdateConfig(byte[] data)
        {
            int offset = 0;

            if (data.Length != CONFIG_LENGTH)
                return -1;

            // Start parsing the config
            offset += Geometry.ATR_LENGTH;
            Buffer.BlockCopy(data, offset, fab_code, 0, FAB_LENGTH);
            offset += FAB_LENGTH;
            Buffer.BlockCopy(data, offset, mtz, 0, MTZ_LENGTH);
            offset += MTZ_LENGTH;
            Buffer.BlockCopy(data, offset, manu_code, 0, MANU_CODE_LENGTH);
            offset += MANU_CODE_LENGTH;
            Buffer.BlockCopy(data, offset, lot_hist_code, 0, LOT_HIST_LENGTH);
            offset += LOT_HIST_LENGTH;
            dcr = data[offset++];

            // Access control
            Buffer.BlockCopy(data, offset, id, 0, ID_LENGTH);
            offset += ID_LENGTH;

            // Calculate geometry based on ATR

            for (int i = 0; i < geometry.ar_pr_count; i++)
            {
                ar[i] = data[offset++];
                pr[i] = data[offset++];
            }

            offset += geometry.reserved_size;
            Buffer.BlockCopy(data, offset, issuer_code, 0, ISSUER_CODE_LENGTH);
            offset += ISSUER_CODE_LENGTH;

            // Crypto section
            for (int i = 0; i < CRYPTO_COUNT; i++)
            {
                aac[i] = data[offset++];
                Buffer.BlockCopy(data, offset, c[i], 0, CRYPTOGRAM_LENGTH);
                offset += CRYPTOGRAM_LENGTH;
                Buffer.BlockCopy(data, offset, s[i], 0, SESSION_KEY_LENGTH);
                offset += SESSION_KEY_LENGTH;
            }


            // Secrets
            for (int i = 0; i < SEED_COUNT; i++)
            {
                Buffer.BlockCopy(data, offset, seed[i], 0, SEED_LENGTH);
                offset += SEED_LENGTH;
            }

            // Passwords
            for (int i = 0; i < PASSWD_COUNT; i++)
            {
                wr_pac[i] = data[offset++];
                Buffer.BlockCopy(data, offset, wr_pass[i], 0, PASSWD_LENGTH);
                offset += PASSWD_LENGTH;
                rd_pac[i] = data[offset++];
                Buffer.BlockCopy(data, offset, rd_pass[i], 0, PASSWD_LENGTH);
                offset += PASSWD_LENGTH;
            }

            return 0;
        }

        public int CardDetect(Geometry[] card, byte[] atr)
        {
            for (uint i = 0; i < card.Length; i++)
            {
                if (atr.SequenceEqual(card[i].atr))
                {
                    geometry = card[i];
                    return 0;
                }
            }
            return -1;
        }

        public byte[] ConfigToBinary()
        {
            byte[] bin = new byte[CONFIG_LENGTH];
            int offset = 0;

            // Copy in each component
            Buffer.BlockCopy(geometry.atr, 0, bin, offset, Geometry.ATR_LENGTH);
            offset += Geometry.ATR_LENGTH;
            Buffer.BlockCopy(fab_code, 0, bin, offset, FAB_LENGTH);
            offset += FAB_LENGTH;
            Buffer.BlockCopy(mtz, 0, bin, offset, MTZ_LENGTH);
            offset += MTZ_LENGTH;
            Buffer.BlockCopy(manu_code, 0, bin, offset, MANU_CODE_LENGTH);
            offset += MANU_CODE_LENGTH;
            Buffer.BlockCopy(lot_hist_code, 0, bin, offset, LOT_HIST_LENGTH);
            offset += LOT_HIST_LENGTH;
            bin[offset++] = dcr;
            Buffer.BlockCopy(id, 0, bin, offset, ID_LENGTH);
            offset += ID_LENGTH;

            // Copy AR + PR
            for (int i = 0; i < geometry.ar_pr_count; i++)
            {
                bin[offset++] = ar[i];
                bin[offset++] = pr[i];
            }

            // Copy reserved space
            for (int i = 0; i < geometry.reserved_size; i++)
                bin[offset++] = 0xFF;

            // Copy Issuer code
            Buffer.BlockCopy(issuer_code, 0, bin, offset, ISSUER_CODE_LENGTH);
            offset += ISSUER_CODE_LENGTH;

            // Copy crypto + session keys
            for (int i = 0; i < CRYPTO_COUNT; i++)
            {
                bin[offset++] = aac[i];
                Buffer.BlockCopy(c[i], 0, bin, offset, CRYPTOGRAM_LENGTH);
                offset += CRYPTOGRAM_LENGTH;
                Buffer.BlockCopy(s[i], 0, bin, offset, SESSION_KEY_LENGTH);
                offset += SESSION_KEY_LENGTH;
            }

            // Copy seeds
            for (int i = 0; i < SEED_COUNT; i++)
            {
                Buffer.BlockCopy(s[i], 0, bin, offset, SEED_LENGTH);
                offset += SEED_LENGTH;
            }

            // Copy passwords
            for (int i = 0; i < PASSWD_COUNT; i++)
            {
                bin[offset++] = wr_pac[i];
                Buffer.BlockCopy(wr_pass[i], 0, bin, offset, PASSWD_LENGTH);
                offset += PASSWD_LENGTH;
                bin[offset++] = rd_pac[i];
                Buffer.BlockCopy(rd_pass[i], 0, bin, offset, PASSWD_LENGTH);
                offset += PASSWD_LENGTH;
            }

            return bin;
        }

        public String ConfigToString()
        {
            StringBuilder str = new StringBuilder(1500);

            str.AppendLine ("=> ID SECTION");

            // Setup ATR string
            str.Append ("ATR       = ");
            for (int i = 0; i < Geometry.ATR_LENGTH; i++)
                str.AppendFormat("{0:X02} ", geometry.atr[i]);
            str.AppendLine (": [" + geometry.name + "]");

            str.Append ("FAB CODE  = ");
            for (int i = 0; i < fab_code.Length; i++)
                str.AppendFormat("{0:X02} ", fab_code[i]);
            str.AppendLine();

            str.Append ("MTZ       = ");
            for (int i = 0; i < fab_code.Length; i++)
                str.AppendFormat("{0:X02} ", fab_code[i]);
            str.AppendLine();

            str.Append ("MANU CODE = ");
            for (int i = 0; i < manu_code.Length; i++)
                str.AppendFormat("{0:X02} ", manu_code[i]);
            str.AppendLine();

            str.Append ("LOT CODE  = ");
            for (int i = 0; i < lot_hist_code.Length; i++)
                str.AppendFormat("{0:X02} ", lot_hist_code[i]);
            str.AppendLine();

            str.AppendLine("=> ACCESS CONTROL");

            str.Append("DCR  = ");
            str.AppendFormat("{0:X02}", dcr);
            str.AppendLine();

            str.Append("ID   = ");
            for (int i = 0; i < id.Length; i++)
                str.AppendFormat("{0:X02} ", id[i]);
            str.AppendLine();

            for (int i = 0; i < geometry.ar_pr_count; i++)
            {
                str.AppendFormat("AR{0}={1:X02} PR{2}={3:X02}", i, ar[i], i, pr[i]);
                str.AppendLine();
            }

            str.Append("Issuer = ");
            for (int i = 0; i < issuer_code.Length; i++)
                str.AppendFormat("{0:X02} ", issuer_code[i]);
            str.AppendLine();

            str.AppendLine("=> Cryptography");

            for (int i = 0; i < 4; i++)
            {
                str.AppendFormat("AAC{0}     = {1:X02}", i, aac[i]);
                str.AppendLine();
                str.AppendFormat("Crypto{0}  = ", i);
                for (int j = 0; j < CRYPTOGRAM_LENGTH; j++)
                    str.AppendFormat("{0:X02} ", c[i][j]);
                str.AppendLine();
                str.AppendFormat("Session{0} = ", i);
                for (int j = 0; j < CRYPTOGRAM_LENGTH; j++)
                    str.AppendFormat("{0:X02} ", s[i][j]);
                str.AppendLine();
            }

            str.AppendLine("=> Secret Seed");

            for (int i = 0; i < SEED_COUNT; i++)
            {
                str.AppendFormat("Seed{0} = ", i);
                for (int j = 0; j < SEED_LENGTH; j++)
                {
                    str.AppendFormat("{0:X02} ", seed[i][j]);
                }
                str.AppendLine();
            }

            str.AppendLine("=> Passwords");

            for (int i = 0; i < PASSWD_COUNT; i++)
            {
                str.AppendFormat("PAC{0}={1:X02} ", i, wr_pac[i]);
                str.AppendFormat("WRITE{0}=", i);
                for (int j = 0; j < PASSWD_LENGTH; j++)
                    str.AppendFormat("{0:X02}", wr_pass[i][j]);
                str.AppendFormat("  PAC{0}={1:X02} ", i, rd_pac[i]);
                str.AppendFormat("READ{0}=", i);
                for (int j = 0; j < PASSWD_LENGTH; j++)
                    str.AppendFormat("{0:X02}", rd_pass[i][j]);
                str.AppendLine();
            }
            return str.ToString();
        }


        /* APDUs of supported commands
         * ApduReadConfig - Read entire config memory
         * ApduWriteConfig - Write entire config memory
         * ApduVerifyPasswd - Verify supplied password\
         * ApduReadFuse - read fuse from memory
         * ApduSetUserZone - set user zone
         * ApduReadUserZone - Read user page
         * ApduWriteUserZone - Write user page
         **/
        public static APDUCommand ApduReadConfig()
        {
            return new APDUCommand(0x00, 0xB6, 0x00, 0x00, null, CONFIG_LENGTH);
        }
        public APDUCommand ApduWriteConfig(byte offset, byte len)
        {
            return new APDUCommand(0x00, 0xB4, 0x00, offset, null, len);
        }

        public APDUCommand ApduVerifyPasswd(byte index, Boolean isRead)
        {
            byte p1;

            if (isRead) {
                p1 = 0x10;
                p1 |= index;
            }
            else {
                p1 = index;
            }
            return new APDUCommand(0x00, 0xBA, p1, 0x00, null, 0x03);
        }
        public APDUCommand ApduReadFuse()
        {
            return new APDUCommand(0x00, 0xB6, 0x01, 0x00, null, 0x01);
        }
        public APDUCommand ApduSetUserZone(byte index)
        {
            return new APDUCommand(0x00, 0xB4, 0x0B, index, null, 0x00);
        }
        public APDUCommand ApduReadUserZone()
        {
            byte pagesize = (byte)geometry.page_size;
            return new APDUCommand(0x00, 0xB2, 0x00, 0x00, null, pagesize);
        }
        public APDUCommand ApduWriteUserZone(byte index, int offset, byte len)
        {
            byte lo_addr, hi_addr;
            lo_addr = (byte)(offset & 0xFF);
            hi_addr = (byte)((offset >> 8) & 0xFF);
            return new APDUCommand(0x00, 0xB0, hi_addr, lo_addr, null, len);
        }
        public static APDUCommand ApduGetAtr()
        {
            return new APDUCommand(0x00, 0xB6, 0x00, 0x00, null, 0x08);
        }
    }
}
