using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;                    // Library for performing Input/ Output Operations
using System.Security.Cryptography; // Library used for encryption and decryption

// Creating the namespace for the Notepad application
namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public string pathInfo = "";
        public Form1()
        {
            InitializeComponent();
            this.Text = "Untitled";            
        }
        
        // Creating the new tool strip menu item
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            richTextBox1.Visible = true;
            this.Text = "Untitled";
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(this.Text == "Untitled")
            {
                save();
            }
            else
            {
                System.IO.File.WriteAllText(this.pathInfo, richTextBox1.Text);

            }
            
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = openFileDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string path = openFileDialog1.FileName;         // Creating the Filename context path
                    string sourceCode = File.ReadAllText(path);     // Reading all the data/text from the filename provided through context path
                    string enteredPassword = Microsoft.VisualBasic.Interaction.InputBox("Please Enter Password", "Attention Required...!!!!", ""); // Creating a dialog box to enter the password of the filename
                    richTextBox1.Visible = true;                    // Sets visibility for the text in the notepad
                    string decryptText = DecryptText(sourceCode.ToString(), enteredPassword.ToString());   //  the decrypted text or data in the notepad 
                    if(decryptText != "")
                    {
                        richTextBox1.Text = decryptText;
                        string filename = Path.GetFileNameWithoutExtension(path); // Getting the filename without the extension
                        this.Text = filename;   // Setting text and pathinfo
                        this.pathInfo = path;
                    }                    
                }
            }
            catch(Exception ex)
            {
                //Application.Exit();
            }            
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save();
        }

        public void save()
        {
            try
            {
                string enteredPassword = Microsoft.VisualBasic.Interaction.InputBox("Please Enter Password", "Attention Required...!!!!", "");
                string path = "";                
                DialogResult result = saveFileDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string encryptText = "";
                    path = saveFileDialog1.FileName;    // Creating the Filename context path
                    encryptText = EncryptText(richTextBox1.Text, enteredPassword.ToString());  //  the encrypted text or data in the notepad 
                    if (encryptText != "")
                    {
                        System.IO.File.WriteAllText(path, encryptText);
                        string filename = Path.GetFileNameWithoutExtension(path);   // Getting the filename without the extension
                        this.Text = filename;   // Setting text and pathinfo
                        this.pathInfo = path;
                    }                    
                }                
            }
            catch(Exception e)
            {
                //Application.Exit();
            }            
        }

        public byte[] Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] myNumberBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged Encryption = new RijndaelManaged())
                {
                    try
                    {
                        Encryption.KeySize = 256;   // Setting the encryption key size
                        Encryption.BlockSize = 128; // Setting the block size

                        // Setting the encryption key & Mode
                        var key = new Rfc2898DeriveBytes(passwordBytes, myNumberBytes, 1000);
                        Encryption.Key = key.GetBytes(Encryption.KeySize / 8);
                        Encryption.IV = key.GetBytes(Encryption.BlockSize / 8);

                        Encryption.Mode = CipherMode.CBC;

                        using (var cs = new CryptoStream(ms, Encryption.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                            cs.Close();
                        }
                        encryptedBytes = ms.ToArray();  // Converting the encrypted bytes to an array
                    }
                    catch(Exception e)
                    {
                        //Application.Exit();
                    }                    
                }
            }

            return encryptedBytes;
        }

        public byte[] Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] myNumberBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged Decryption = new RijndaelManaged())
                {
                    try
                    {
                        Decryption.KeySize = 256;    // Setting the decryption key size
                        Decryption.BlockSize = 128;  // Setting the blocksize

                        // deriving the encryption key & Mode
                        var key = new Rfc2898DeriveBytes(passwordBytes, myNumberBytes, 1000);
                        Decryption.Key = key.GetBytes(Decryption.KeySize / 8);
                        Decryption.IV = key.GetBytes(Decryption.BlockSize / 8);

                        Decryption.Mode = CipherMode.CBC;

                        using (var cs = new CryptoStream(ms, Decryption.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                            cs.Close();
                        }
                        decryptedBytes = ms.ToArray();
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show("Invalid Password");
                        //Application.Exit();
                    }

                }
            }
            return decryptedBytes;
        }

        public string EncryptText(string input, string password)
        {
            string result = "";
            try
            {
                // Get the bytes of the string
                byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(input);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // Hash the password with SHA256
                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                byte[] bytesEncrypted = Encrypt(bytesToBeEncrypted, passwordBytes);

                result = Convert.ToBase64String(bytesEncrypted);
            }
            catch(Exception e)
            {
                //Application.Exit();
            } 
            return result;
        }

        public string DecryptText(string input, string password)
        {
            string result = "";
            try
            {
                // Get the bytes of the string
                byte[] bytesToBeDecrypted = Convert.FromBase64String(input);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                byte[] bytesDecrypted = Decrypt(bytesToBeDecrypted, passwordBytes);

                 result = Encoding.UTF8.GetString(bytesDecrypted);                
            }
            catch(Exception e)
            {
                //Application.Exit();
            }
            return result;
        }
    }
}
