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
    public partial class add_supplier : Form
    {
        OleDbConnection conn = new OleDbConnection(Properties.Settings.Default.DbConnectionString);
        OleDbCommand cmd;
        DataTable dt;

        public add_supplier()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            adminsupplier adminsupplier = new adminsupplier();
            adminsupplier.Show();
            this.Hide();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSupplierName.Text) ||
                    string.IsNullOrWhiteSpace(txtItemName.Text) ||
                    string.IsNullOrWhiteSpace(txtDeliveredBy.Text) ||
                    string.IsNullOrWhiteSpace(nudPrice.Value.ToString()) ||
                    string.IsNullOrWhiteSpace(dtpDateTime.Value.ToString()))
                {
                    MessageBox.Show("Please fill in the required fields!");
                    return;
                }
                else if (nudPrice.Value < 0)
                {
                    MessageBox.Show("Please enter a valid price!");
                    return;
                }

                conn.Open();
                string query = "INSERT INTO [suppliers] ([supplier_name], [item_name], [price], [delivered_by], [datetime]) VALUES (?, ?, ?, ?, ?)";
                cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("?", txtSupplierName.Text);
                cmd.Parameters.AddWithValue("?", txtItemName.Text);
                cmd.Parameters.AddWithValue("?", nudPrice.Value);
                cmd.Parameters.AddWithValue("?", txtDeliveredBy.Text);
                cmd.Parameters.AddWithValue("?", dtpDateTime.Value.ToString());
                cmd.ExecuteNonQuery();
                MessageBox.Show("Successfully added!");
                txtSupplierName.Clear();
                txtItemName.Clear();
                nudPrice.Value = 0;
                txtDeliveredBy.Clear();
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
    }
}
