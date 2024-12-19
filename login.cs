using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace bike_inventory
{
    public partial class login : Form
    {
        OleDbConnection conn = new OleDbConnection(Properties.Settings.Default.DbConnectionString);
        OleDbCommand cmd;
        OleDbDataAdapter adapter;
        DataTable dt;

        private int loginAttempts = 0;

        public login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Please fill in the required fields!");
                    return;
                }
                conn.Open();
                string query = "SELECT * FROM [users] WHERE [username]=? AND [password]=?";
                adapter = new OleDbDataAdapter(query, conn);
                adapter.SelectCommand.Parameters.AddWithValue("?", txtUsername.Text);
                adapter.SelectCommand.Parameters.AddWithValue("?", txtPassword.Text);
                dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    Auth.user = row;
                    string position = row["position"].ToString();
                    if (position == "Admin")
                    {
                        this.Hide();
                        new adminui().Show();
                    }
                    else if (position == "Staff")
                    {
                        this.Hide();
                        new staffui().Show();
                    }
                    else
                    {
                        Auth.user = null;
                        MessageBox.Show("Account's Position is Invalid! Please contact the administrator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    loginAttempts++;
                    MessageBox.Show("Invalid Username or Password!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    if (loginAttempts >= 3)
                    {
                        Application.Exit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void checkPW_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = chkShowPass.Checked ? '\0' : '*';
        }
    }

}