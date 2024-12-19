using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb; // For connecting to Access databases
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO; // For handling file and memory streams

namespace bike_inventory
{
    public partial class useraccount : Form
    {
        
        OleDbConnection conn = new OleDbConnection(Properties.Settings.Default.DbConnectionString);
        OleDbCommand cmd;
        OleDbDataAdapter adapter;
        DataTable dt;

        private int userId { get; set; }

        public useraccount()
        {
            InitializeComponent();
            GetUsers();
        }

        void GetUsers()
        {
            try
            {
                conn.Open();
                string query = "SELECT * FROM [users] WHERE ([fname] & ' ' & [lname]) LIKE ? OR [username] LIKE ? OR [position] LIKE ?";
                adapter = new OleDbDataAdapter(query, conn);
                adapter.SelectCommand.Parameters.AddWithValue("?", "%" + txtSearch.Text + "%");
                adapter.SelectCommand.Parameters.AddWithValue("?", "%" + txtSearch.Text + "%");
                adapter.SelectCommand.Parameters.AddWithValue("?", "%" + txtSearch.Text + "%");
                dt = new DataTable();
                adapter.Fill(dt);
                dgvUser.Rows.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    dgvUser.Rows.Add(row["id"].ToString(), row["username"].ToString(), row["fname"].ToString(), row["lname"].ToString(), row["position"].ToString(), new string('*', row["password"].ToString().Count()));
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

        private void useraccount_Load(object sender, EventArgs e)
        {
            GetUsers();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            GetUsers();
        }

        private void dgvUser_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow currentRow = dgvUser.CurrentRow;
            if (currentRow == null)
            {
                MessageBox.Show("No row is selected.");
                return;
            }

            userId = Convert.ToInt32(currentRow.Cells["id"].Value.ToString());
            txtFname.Text = currentRow.Cells["fname"].Value.ToString();
            txtLname.Text = currentRow.Cells["lname"].Value.ToString();
            txtUsername.Text = currentRow.Cells["username"].Value.ToString();
            cmbPosition.SelectedItem = currentRow.Cells["position"].Value.ToString();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (
                    string.IsNullOrWhiteSpace(txtFname.Text) ||
                    string.IsNullOrWhiteSpace(txtLname.Text) ||
                    string.IsNullOrWhiteSpace(cmbPosition.Text) ||
                    string.IsNullOrWhiteSpace(txtUsername.Text) ||
                    string.IsNullOrWhiteSpace(txtPass.Text) ||
                    string.IsNullOrWhiteSpace(txtPassConfirm.Text)
                    )
                {
                    MessageBox.Show("Please fill in the required fields!");
                    return;
                }
                else if (txtPass.Text != txtPassConfirm.Text)
                {
                    MessageBox.Show("Password didn't match!");
                    return;
                }

                conn.Open();
                string query = "INSERT INTO [users] ([username], [fname], [lname], [position], [password]) VALUES(?, ?, ?, ?, ?)";
                cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("?", txtUsername.Text);
                cmd.Parameters.AddWithValue("?", txtFname.Text);
                cmd.Parameters.AddWithValue("?", txtLname.Text);
                cmd.Parameters.AddWithValue("?", cmbPosition.Text);
                cmd.Parameters.AddWithValue("?", txtPass.Text);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Successfully added a user!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
            GetUsers();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (
                    string.IsNullOrWhiteSpace(txtFname.Text) ||
                    string.IsNullOrWhiteSpace(txtLname.Text) ||
                    string.IsNullOrWhiteSpace(cmbPosition.Text) ||
                    string.IsNullOrWhiteSpace(txtUsername.Text) ||
                    string.IsNullOrWhiteSpace(txtPass.Text) ||
                    string.IsNullOrWhiteSpace(txtPassConfirm.Text)
                    )
                {
                    MessageBox.Show("Please fill in the required fields!");
                    return;
                }
                else if (txtPass.Text != txtPassConfirm.Text)
                {
                    MessageBox.Show("Password didn't match!");
                    return;
                }

                conn.Open();
                string query = "UPDATE [users] SET [username]=?, [fname]=?, [lname]=?, [position]=?, [password]=? WHERE [id]=?";
                cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("?", txtUsername.Text);
                cmd.Parameters.AddWithValue("?", txtFname.Text);
                cmd.Parameters.AddWithValue("?", txtLname.Text);
                cmd.Parameters.AddWithValue("?", cmbPosition.Text);
                cmd.Parameters.AddWithValue("?", txtPass.Text);
                cmd.Parameters.AddWithValue("?", userId);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Successfully updated user's data!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
            GetUsers();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtUsername.Clear();
            txtFname.Clear();
            txtLname.Clear();
            cmbPosition.SelectedValue = null;
            txtPass.Clear();
            txtPassConfirm.Clear();
            userId = 0;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (DialogResult.Yes == MessageBox.Show("Are you sure you want to delete this user?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    conn.Open();
                    string query = "DELETE FROM [users] WHERE [id]=?";
                    cmd = new OleDbCommand(query, conn);
                    cmd.Parameters.AddWithValue("?", userId);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Successfully removed the user!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (Convert.ToInt32(Auth.user["id"].ToString()) == userId)
                    {
                        Auth.user = null;
                        MessageBox.Show("Your account is removed! Automatically logging out...");
                        btnLogOut.PerformClick();
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
            GetUsers();
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            login Staff = new login();
            Staff.Show();
            Auth.user = null;
            this.Hide();
        }

        private void btnInventory_Click(object sender, EventArgs e)
        {
            adminui Staff = new adminui();
            Staff.Show();
            this.Hide();
        }

        private void btnSupplier_Click(object sender, EventArgs e)
        {
            adminsupplier Staff = new adminsupplier();
            Staff.Show();
            this.Hide();
        }

        private void btnStaff_Click(object sender, EventArgs e)
        {

        }

        private void btnSales_Click(object sender, EventArgs e)
        {
            admin_sales Staff = new admin_sales();
            Staff.Show();
            this.Hide();
        }
    }
}

    


