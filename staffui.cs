﻿using System;
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
    public partial class staffui : Form
    {
        OleDbConnection conn = new OleDbConnection(Properties.Settings.Default.DbConnectionString);
        OleDbDataAdapter adapter;
        DataTable dt;

        public staffui()
        {
            InitializeComponent();
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

        private void staffui_Load(object sender, EventArgs e)
        {
            getProducts();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            getProducts();
        }

        private void btnInventory_Click(object sender, EventArgs e)
        {
            staffui Sales = new staffui();
            Sales.Show();
            this.Close();
        }

        private void btnSales_Click(object sender, EventArgs e)
        {
            staffsales Sales = new staffsales();
            Sales.Show();
            this.Close();
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            login Sales = new login();
            Sales.Show();
            this.Close();
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            stafforder Sales = new stafforder();
            Sales.Show();
            this.Close();
        }

        private void btnSales_Click_1(object sender, EventArgs e)
        {
            staffsales Sales = new staffsales();
            Sales.Show();
            this.Close();
        }

        private void btnLogOut_Click_1(object sender, EventArgs e)
        {
            login Sales = new login();
            Sales.Show();
            Auth.user = null;
            this.Close();
        }
    }
    
}
