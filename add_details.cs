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
    public partial class add_details : Form
    {
        OleDbConnection conn = new OleDbConnection(Properties.Settings.Default.DbConnectionString);
        OleDbDataAdapter adapter;
        OleDbCommand cmd;
        DataTable dt;

        public add_details()
        {
            InitializeComponent();
        }

        private void add_details_Load(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                string query = "SELECT * FROM [categories]";
                adapter = new OleDbDataAdapter(query, conn);
                dt = new DataTable();
                adapter.Fill(dt);
                cmbCategory.DataSource = dt;
                cmbCategory.DisplayMember = "name";
                cmbCategory.ValueMember = "id";
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtItem.Text) || cmbCategory.SelectedValue == null || nudStocks.Value < 0 || nudPrice.Value <= 0)
                {
                    MessageBox.Show("Please fill in all required fields.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                conn.Open();

                string query = "INSERT INTO [products] ([name], [category_id], [stocks], [price]) " +
                               "VALUES (?, ?, ?, ?)";

                cmd = new OleDbCommand(query, conn);

                cmd.Parameters.AddWithValue("?", txtItem.Text);
                cmd.Parameters.AddWithValue("?", cmbCategory.SelectedValue);
                cmd.Parameters.AddWithValue("?", Convert.ToInt32(nudStocks.Value));
                cmd.Parameters.AddWithValue("?", nudPrice.Value);
                cmd.ExecuteNonQuery();

                txtItem.Clear();
                cmbCategory.SelectedItem = null;
                nudPrice.Value = 0;
                nudStocks.Value = 0;

                MessageBox.Show("Successfully added the product!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
