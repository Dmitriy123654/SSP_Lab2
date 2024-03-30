using System.CodeDom;
using System.Data;
using System.Data.SqlClient;
public class Form1 : Form

{
    private SqlConnection connection;
    public Form1()
    {
        FormLayout();
        try
        {
            ConnectToDatabase();
        }
        catch (System.Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }
    }

    public void FormLayout()
    {
        this.Name = "База данных";
        this.Text = "База данных";
        this.Size = new System.Drawing.Size(1000, 500);
        this.StartPosition = FormStartPosition.CenterScreen;

        Button addButton = new Button();
        addButton.Text = "Добавить";
        addButton.Location = new System.Drawing.Point(10, 10);
        addButton.Click += AddButton_Click;

        Button deleteButton = new Button();
        deleteButton.Text = "Удалить";
        deleteButton.Location = new System.Drawing.Point(120, 10);
        deleteButton.Click += DeleteButton_Click;

        Button viewAllButton = new Button();
        viewAllButton.Text = "Просмотреть все";
        viewAllButton.Location = new System.Drawing.Point(230, 10);
        viewAllButton.Click += ViewAllButton_Click;

        Button showProcedure = new Button();
        showProcedure.Text = "Процедуры";
        showProcedure.Location = new System.Drawing.Point(340, 10);
        showProcedure.Width = 80;
        showProcedure.Click += ShowProcedure_Click;


        this.Controls.Add(addButton);
        this.Controls.Add(deleteButton);
        this.Controls.Add(viewAllButton);
        this.Controls.Add(showProcedure);
    }
    private void ShowProcedure_Click(object sender, EventArgs e)
    {
        if (IsDatabaseConnected())
        {
            try
            {
                string query = "SELECT name FROM sys.procedures";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        string procedures = "";
                        while (reader.Read())
                        {
                            string procedureName = reader["name"].ToString();
                            procedures += procedureName + Environment.NewLine;
                        }
                        reader.Close();

                        MessageBox.Show(procedures, "Хранимые процедуры", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        else
        {
            MessageBox.Show("Потеряно соединение с базой данных.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }


    private void ConnectToDatabase()
    {
        string connectionString = "Server=(localdb)\\mssqllocaldb;Database=SSPLab2;Trusted_Connection=True;";

        connection = new SqlConnection(connectionString);
        try
        {
            // Открываем подключение
            connection.Open();

        }
        catch (SqlException ex)
        {
            MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ViewAllButton_Click(object sender, EventArgs e)
    {
        if (IsDatabaseConnected())
        {
            using (ViewAllPeopleForm viewAllCarsForm = new ViewAllPeopleForm(connection))
            {
                try
                {
                    viewAllCarsForm.ShowDialog();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        else
        {

            MessageBox.Show("Потеряно соединение с базой данных.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }


    private void DeleteButton_Click(object sender, EventArgs e)
    {
        if (IsDatabaseConnected())
        {
            using (DeletePersonForm deleteCarForm = new DeletePersonForm(this.connection))
            {
                deleteCarForm.ShowDialog();
                Console.WriteLine("Delete Person form closed");
            }
        }
        else
        {

            MessageBox.Show("Потеряно соединение с базой данных.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }


    private void AddButton_Click(object sender, EventArgs e)
    {
        if (IsDatabaseConnected())
        {
            using (AddPersonForm addCarForm = new AddPersonForm(this.connection))
            {
                addCarForm.ShowDialog();
                Console.WriteLine("Add Person form closed");
            }
        }
        else
        {

            MessageBox.Show("Потеряно соединение с базой данных.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && (connection != null))
        {
            connection.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {

    }

    public bool IsDatabaseConnected()
    {
        try
        {

            if (connection.State != ConnectionState.Open)
            {

                connection.Open();
                return true;
            }

            return true;
        }
        catch (Exception ex)
        {

            return false;
        }
    }
}