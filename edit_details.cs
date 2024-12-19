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
    public partial class edit_details : Form
    {
        OleDbConnection conn = new OleDbConnection(Properties.Settings.Default.DbConnectionString);
        OleDbCommand cmd;
        OleDbDataAdapter adapter;
        DataTable dt;

        public int productId { get; set; }

        public edit_details()
        {
            InitializeComponent();
        }

        private void edit_details_Load(object sender, EventArgs e)
        {
            try
            {
                conn.Open();

                string query = "SELECT * FROM [categories]";
                adapter = new OleDbDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                cmbCategory.DataSource = dt;
                cmbCategory.DisplayMember = "name";
                cmbCategory.ValueMember = "id";

                query = "SELECT * FROM [products] WHERE [id]=" + productId;
                adapter = new OleDbDataAdapter(query, conn);
                dt = new DataTable();
                adapter.Fill(dt);

                DataRow row = dt.Rows[0];

                txtItem.Text = row["name"].ToString();
                cmbCategory.SelectedValue = row["category_id"].ToString();
                nudStocks.Value = Convert.ToInt32(row["stocks"].ToString());
                nudPrice.Value = Convert.ToDecimal(row["price"].ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message);
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

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            bool isSuccess = false;
            try
            { 
                if (string.IsNullOrWhiteSpace(txtItem.Text) || cmbCategory.SelectedValue == null || nudStocks.Value < 0 || nudPrice.Value <= 0)
                {
                    MessageBox.Show("Please fill in all required fields.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                conn.Open();

                string query = "UPDATE [products] SET [name]=?, [category_id]=?, [stocks]=?, [price]=? WHERE [id]=" + productId;

                cmd = new OleDbCommand(query, conn);

                cmd.Parameters.AddWithValue("?", txtItem.Text);
                cmd.Parameters.AddWithValue("?", cmbCategory.SelectedValue);
                cmd.Parameters.AddWithValue("?", Convert.ToInt32(nudStocks.Value));
                cmd.Parameters.AddWithValue("?", nudPrice.Value);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Successfully updated the product!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
