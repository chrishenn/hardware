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
using System.Windows.Forms;

namespace AtmelCrypto
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Form1 ui;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ui = new Form1();
            SessionMgr sMgr = new SessionMgr(ui);
            ui.SetSessionMgr(sMgr);
            sMgr.Init();
            Application.Run(ui);
        }
    }
}
