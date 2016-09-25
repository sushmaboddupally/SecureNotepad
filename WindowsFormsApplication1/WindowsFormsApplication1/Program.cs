using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the Secure Notepad Application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();                       // Enabling Visual Styles
            Application.SetCompatibleTextRenderingDefault(false);   // Setting the compatible text rendering defualts
            Application.Run(new Form1());                           // Creates an instance of the new form 
        }
    }
}
