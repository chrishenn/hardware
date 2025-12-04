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

/* Characterize the device geometry that differs between models */
namespace AtmelCrypto
{
    class Geometry
    {
        /* Number of user pages */
        public int page_count { get; set; }

        /* User page size */
        public int page_size { get; set; }

        /* AR / PR register count */
        public byte ar_pr_count { get; set; }

        /* Reserved size */
        public int reserved_size { get; set; }

        /* Device name */
        public String name { get; set; }

        /* Device ATR */
        public byte[] atr { get; set; }

        public const int ATR_LENGTH = 8;

        public Geometry(string name, byte[] atr, int page_count,
                        int page_size, byte ar_pr_count, int reserved_size)
        {
            this.name = name;
            this.atr = new byte[ATR_LENGTH];
            Buffer.BlockCopy(atr, 0, this.atr, 0, ATR_LENGTH);
            this.page_count = page_count;
            this.page_size = page_size;
            this.ar_pr_count = ar_pr_count;
            this.reserved_size = reserved_size;
        }
    }
}
