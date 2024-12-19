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
    public partial class admin_sales : Form
    {
        OleDbConnection conn = new OleDbConnection(Properties.Settings.Default.DbConnectionString);
        OleDbDataAdapter adapter;
        DataTable dt;

        public admin_sales()
        {
            InitializeComponent();
        }

        private void getSales()
        {
            try
            {
                conn.Open();
                string query = "SELECT * FROM (([sales] " +
                               "INNER JOIN [products] ON [sales].[product_id] = [products].[id]) " +
                               "INNER JOIN [users] ON [sales].[user_id] = [users].[id]) " +
                               "WHERE [products].[name] LIKE ? OR ([users].[fname] & ' ' & [users].[lname]) LIKE ?";
                adapter = new OleDbDataAdapter(query, conn);
                adapter.SelectCommand.Parameters.AddWithValue("?", $"%{txtSearch.Text}%");
                adapter.SelectCommand.Parameters.AddWithValue("?", $"%{txtSearch.Text}%");
                dt = new DataTable();
                adapter.Fill(dt);

                dgvSales.Rows.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    DateTime parsedDateTime = DateTime.Parse(row["datetime"].ToString());
                    string date = parsedDateTime.ToString("MMM. dd, yyyy");
                    string time = parsedDateTime.ToString("hh:mm tt");
                    string staffName = row["fname"].ToString() + " " + row["lname"].ToString();

                    dgvSales.Rows.Add(
                                        row["sales.id"].ToString(),
                                        row["name"].ToString(), row["quantity"].ToString(),
                                        "P " + Convert.ToDouble(row["sales.price"].ToString()).ToString("N2"),
                                        "P " + Convert.ToDouble(row["total_amount"].ToString()).ToString("N2"),
                                        date,
                                        time,
                                        staffName
                                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        private void btnInventory_Click(object sender, EventArgs e)
        {
            adminui Sales = new adminui();
            Sales.Show();
            this.Close();
        }

        private void btnSupplier_Click(object sender, EventArgs e)
        {
            adminsupplier Sales = new adminsupplier ();
            Sales.Show();
            this.Close();
        }

        private void btnStaff_Click(object sender, EventArgs e)
        {
            useraccount Sales = new useraccount();
            Sales.Show();
            this.Close();
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            login Sales = new login();
            Auth.user = null;
            Sales.Show();
            this.Close();
        }

        private void admin_sales_Load(object sender, EventArgs e)
        {
            getSales();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            getSales();
        }
    }
}
