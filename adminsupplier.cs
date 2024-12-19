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
    public partial class adminsupplier : Form
    {
        OleDbConnection conn = new OleDbConnection(Properties.Settings.Default.DbConnectionString);
        OleDbDataAdapter adapter;
        OleDbCommand cmd;
        DataTable dt;

        public adminsupplier()
        {
            InitializeComponent();
        }

        public void GetSuppliers()
        {
            try
            {
                conn.Open();
                string query = "SELECT * FROM [suppliers] WHERE ([supplier_name] LIKE ? OR [item_name] LIKE ? OR [price] LIKE ? OR [delivered_by] LIKE ?)";
                adapter = new OleDbDataAdapter(query, conn);
                adapter.SelectCommand.Parameters.AddWithValue("?", $"%{txtSearch.Text}%");
                adapter.SelectCommand.Parameters.AddWithValue("?", $"%{txtSearch.Text}%");
                adapter.SelectCommand.Parameters.AddWithValue("?", $"%{txtSearch.Text}%");
                adapter.SelectCommand.Parameters.AddWithValue("?", $"%{txtSearch.Text}%");
                dt = new DataTable();
                adapter.Fill(dt);
                dgvSuppliers.Rows.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    DateTime parsedDateTime = DateTime.Parse(row["datetime"].ToString());
                    string date = parsedDateTime.ToString("MMM. dd, yyyy");
                    string time = parsedDateTime.ToString("hh:mm tt");
                    dgvSuppliers.Rows.Add(row["id"].ToString(), row["supplier_name"].ToString(), row["item_name"].ToString(), row["price"].ToString(), row["delivered_by"].ToString(), date, time);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void adminsupplier_Load(object sender, EventArgs e)
        {
            GetSuppliers();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            GetSuppliers();
        }

        private void btnInventory_Click(object sender, EventArgs e)
        {
            adminui adminsupplier = new adminui();
            adminsupplier.Show();
            this.Close();
        }

        private void btnStaff_Click(object sender, EventArgs e)
        {
            useraccount adminsupplier = new useraccount();
            adminsupplier.Show();
            this.Close();
        }

        private void btnSales_Click(object sender, EventArgs e)
        {
            admin_sales adminsupplier = new admin_sales();
            adminsupplier.Show();
            this.Close();
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            login adminsupplier = new login();
            Auth.user = null;
            adminsupplier.Show();
            this.Close();
        }

        private void btnAddSupplier_Click(object sender, EventArgs e)
        {
            new add_supplier().ShowDialog();
            GetSuppliers();
        }

        private void dgvSuppliers_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            btnDelete.Visible = dgvSuppliers.SelectedRows.Count > 0;
        }

        private void dgvSuppliers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            btnDelete.Visible = dgvSuppliers.SelectedRows.Count > 0;
            if (e.ColumnIndex == dgvSuppliers.Columns["btnAction"].Index && e.RowIndex >= 0)
            {
                int supplierId = Convert.ToInt32(dgvSuppliers.Rows[e.RowIndex].Cells["supplierId"].Value.ToString());
                edit_supplier f = new edit_supplier();
                f.supplierId = supplierId;
                f.ShowDialog();
                GetSuppliers();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (DialogResult.Yes == MessageBox.Show("Are you sure you want to delete this item/s?", "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                {
                    conn.Open();
                    for (int i = dgvSuppliers.SelectedRows.Count - 1; i >= 0; i--)
                    {
                        DataGridViewRow selectedRow = dgvSuppliers.SelectedRows[i];
                        int supplierId = Convert.ToInt32(dgvSuppliers.Rows[selectedRow.Index].Cells["supplierId"].Value.ToString());

                        string query = "DELETE FROM [suppliers] WHERE [id]=" + supplierId;
                        cmd = new OleDbCommand(query, conn);
                        cmd.ExecuteNonQuery();

                        dgvSuppliers.Rows.RemoveAt(selectedRow.Index);
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
    }
    
}
