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
    public partial class adminui : Form
    {
        OleDbConnection conn = new OleDbConnection(Properties.Settings.Default.DbConnectionString);
        OleDbCommand cmd;
        OleDbDataAdapter adapter;
        DataTable dt;

        public adminui()
        {
            InitializeComponent();
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            login adminui= new login();
            Auth.user = null;
            adminui.Show();
            this.Hide();
        }

        private void btnStaff_Click(object sender, EventArgs e)
        {
            useraccount adminui = new useraccount();
            adminui.Show();
            this.Hide();
        }

        public void getProducts()
        {
            try
            {
                conn.Open();
                string query = "SELECT * FROM [products] " + 
                    "INNER JOIN [categories] ON [products].[category_id] = [categories].[id] " +
                    "WHERE [products].[name] LIKE ? OR [categories].[name] LIKE ?";

                adapter = new OleDbDataAdapter(query, conn);
                adapter.SelectCommand.Parameters.AddWithValue("?", "%" + txtSearch.Text + "%");
                adapter.SelectCommand.Parameters.AddWithValue("?", "%" + txtSearch.Text + "%");

                dt = new DataTable();
                adapter.Fill(dt);

                dgvProducts.Rows.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    dgvProducts.Rows.Add(row["products.id"], row["products.name"], row["categories.name"], row["stocks"], row["price"]);
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

        private void adminui_Load(object sender, EventArgs e)
        {
            getProducts();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            getProducts();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            new add_details().ShowDialog();
            getProducts();
        }

        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            btnDelete.Visible = dgvProducts.SelectedRows.Count > 0;
            if (e.ColumnIndex == dgvProducts.Columns["btnAction"].Index && e.RowIndex >= 0)
            {   
                int productId = Convert.ToInt32(dgvProducts.Rows[e.RowIndex].Cells["productId"].Value.ToString());
                edit_details f = new edit_details();
                f.productId = productId;
                f.ShowDialog();
                getProducts();
            }
        }

        private void dgvProducts_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            btnDelete.Visible = dgvProducts.SelectedRows.Count > 0;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (DialogResult.Yes == MessageBox.Show("Are you sure you want to delete this item/s?", "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                {
                    conn.Open();
                    for (int i = dgvProducts.SelectedRows.Count - 1; i >= 0; i--)
                    {
                        DataGridViewRow selectedRow = dgvProducts.SelectedRows[i];
                        int productId = Convert.ToInt32(dgvProducts.Rows[selectedRow.Index].Cells["productId"].Value.ToString());

                        string query = "DELETE FROM [products] WHERE [id]=" + productId;
                        cmd = new OleDbCommand(query, conn);
                        cmd.ExecuteNonQuery();

                        dgvProducts.Rows.RemoveAt(selectedRow.Index);
                    }
                    MessageBox.Show("Removed Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void btnSupplier_Click(object sender, EventArgs e)
        {
            adminsupplier adminui = new adminsupplier();
            adminui.Show();
            this.Hide();
        }

        private void btnSales_Click(object sender, EventArgs e)
        {
            admin_sales adminui = new admin_sales();
            adminui.Show();
            this.Hide();
        }
    }
}
