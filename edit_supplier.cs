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
    public partial class edit_supplier : Form
    {
        OleDbConnection conn = new OleDbConnection(Properties.Settings.Default.DbConnectionString);
        OleDbDataAdapter adapter;
        OleDbCommand cmd;
        DataTable dt;

        public int supplierId { get; set; }

        public edit_supplier()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void edit_supplier_Load(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                string query = "SELECT * FROM [suppliers] WHERE [id]=" +  supplierId;
                adapter = new OleDbDataAdapter(query, conn);
                dt = new DataTable();
                adapter.Fill(dt);
                DataRow row = dt.Rows[0];
                txtSupplierName.Text = row["supplier_name"].ToString();
                txtItemName.Text = row["item_name"].ToString();
                nudPrice.Value = Convert.ToDecimal(row["price"].ToString());
                txtDeliveredBy.Text = row["delivered_by"].ToString();
                dtpDateTime.Value = Convert.ToDateTime(row["datetime"].ToString());
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

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            bool isSuccess = false;
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
                string query = "UPDATE [suppliers] SET [supplier_name]=?, [item_name]=?, [price]=?, [delivered_by]=?, [datetime]=? WHERE [id]=" + supplierId;
                cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("?", txtSupplierName.Text);
                cmd.Parameters.AddWithValue("?", txtItemName.Text);
                cmd.Parameters.AddWithValue("?", nudPrice.Value);
                cmd.Parameters.AddWithValue("?", txtDeliveredBy.Text);
                cmd.Parameters.AddWithValue("?", dtpDateTime.Value.ToString());
                cmd.ExecuteNonQuery();
                MessageBox.Show("Successfully updated!");
                isSuccess = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                conn.Close();
                if (isSuccess)
                {
                    this.Close();
                }
            }
        }
    }
}
