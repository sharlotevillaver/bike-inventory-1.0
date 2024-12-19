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
    public partial class item_bought : Form
    {
        OleDbConnection conn = new OleDbConnection(Properties.Settings.Default.DbConnectionString);
        OleDbDataAdapter adapter;
        OleDbCommand cmd;
        DataTable dt;

        public int productId { get; set; }
        private double price { get; set; }
        private int stock { get; set; }
        private double total_amount { get; set; }

        public item_bought()
        {
            InitializeComponent();
        }

        private void getItem()
        {
            try
            {
                conn.Open();
                string query = "SELECT * FROM [products] WHERE [id]=" + productId;
                adapter = new OleDbDataAdapter(query, conn);
                dt = new DataTable();
                adapter.Fill(dt);
                DataRow row = dt.Rows[0];
                price = Convert.ToDouble(row["price"].ToString());
                stock = Convert.ToInt32(row["stocks"].ToString());
                lblItemName.Text = row["name"].ToString();
                lblPrice.Text = "P " + price.ToString("N2");
                lblCurrentStock.Text = stock.ToString();
                nudQuantityBought.Maximum = Convert.ToDecimal(row["stocks"].ToString());
                total_amount = price * (double)nudQuantityBought.Value;
                lblTotalAmount.Text = "P " + total_amount.ToString("N2");
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

        private void item_bought_Load(object sender, EventArgs e)
        {
            getItem();
        }

        private void btnBought_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nudQuantityBought.Value.ToString()))
                {
                    MessageBox.Show("Please fill in the required fields!");
                    return;
                }
                conn.Open();
                string query = "UPDATE [products] SET [stocks]=[stocks]-? WHERE [id]=?";
                cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("?", nudQuantityBought.Value);
                cmd.Parameters.AddWithValue("?", productId);
                cmd.ExecuteNonQuery();

                query = "INSERT INTO [sales] ([product_id], [quantity], [price], [total_amount], [user_id]) VALUES (?, ?, ?, ?, ?)";
                cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("?", productId);
                cmd.Parameters.AddWithValue("?", nudQuantityBought.Value);
                cmd.Parameters.AddWithValue("?", price);
                cmd.Parameters.AddWithValue("?", total_amount);
                cmd.Parameters.AddWithValue("?", Auth.user["id"].ToString());
                cmd.ExecuteNonQuery();
                MessageBox.Show("Success!");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString());
            }
            finally
            {
                conn.Close();
            }
            getItem();
        }

        private void nudQuantityBought_ValueChanged(object sender, EventArgs e)
        {
            total_amount = price * (double)nudQuantityBought.Value;
            lblTotalAmount.Text = "P " + total_amount.ToString("N2");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
