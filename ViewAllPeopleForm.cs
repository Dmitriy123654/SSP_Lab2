using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

public class ViewAllPeopleForm : Form
{
    private DataGridView dataGridView;

    public ViewAllPeopleForm(SqlConnection connection)
    {
        FormLayout();
        try
        {
            PopulateData(connection);
        }
        catch (System.Exception ex)
        {
            MessageBox.Show("Ошибка: " + ex.Message);
            this.DialogResult = DialogResult.Cancel;
            return;
        }

    }

    private bool CheckDatabaseConnection(SqlConnection connection)
    {
        if (connection != null && connection.State == ConnectionState.Open)
        {
            return true;
        }
        return false;
    }

    private void FormLayout()
    {
        // Set up form properties
        this.Name = "Показать всех людей";
        this.Text = "Показать всех людей";
        this.Size = new System.Drawing.Size(500, 400);
        this.StartPosition = FormStartPosition.CenterScreen;

        // Create DataGridView control
        dataGridView = new DataGridView();
        dataGridView.Dock = DockStyle.Fill;

        // Add DataGridView to the form
        this.Controls.Add(dataGridView);
    }

    private void PopulateData(SqlConnection connection)
    {
        // Retrieve data from the "People" table
        using (SqlCommand command = new SqlCommand("SELECT * FROM People", connection))
        {
            if (!CheckDatabaseConnection(connection))
            {
                MessageBox.Show("Потеряно соединение с базой данных", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                return;
            }
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Bind the data to the DataGridView
                    dataGridView.DataSource = dataTable;
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Потеряно соединение с базой данных", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Потеряно соединение с базой данных", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}