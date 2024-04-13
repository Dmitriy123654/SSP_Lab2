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
                        List<string> procedureNames = new List<string>();
                        while (reader.Read())
                        {
                            string procedureName = reader["name"].ToString();
                            procedureNames.Add(procedureName);
                        }
                        reader.Close();

                        // Отображаем диалоговое окно выбора процедуры
                        using (var procedureDialog = new Form())
                        {
                            procedureDialog.Text = "Выберите процедуру";
                            var procedureComboBox = new ComboBox();
                            procedureComboBox.DataSource = procedureNames;
                            procedureComboBox.Dock = DockStyle.Top;
                            var okButton = new Button();
                            okButton.Text = "OK";
                            okButton.DialogResult = DialogResult.OK;
                            var cancelButton = new Button();
                            cancelButton.Text = "Отмена";
                            cancelButton.DialogResult = DialogResult.Cancel;

                            // Настройка внешнего вида кнопок
                            okButton.AutoSize = true;
                            okButton.Padding = new Padding(10, 5, 10, 5); // Настройка внутренних отступов
                            cancelButton.AutoSize = true;
                            cancelButton.Padding = new Padding(10, 5, 10, 5); // Настройка внутренних отступов

                            // Размещение кнопок
                            var buttonPanel = new FlowLayoutPanel();
                            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
                            buttonPanel.Dock = DockStyle.Top;
                            buttonPanel.Controls.AddRange(new Control[] { cancelButton, okButton });

                            // Размещение компонентов на форме
                            procedureDialog.Controls.Add(procedureComboBox);
                            procedureDialog.Controls.Add(buttonPanel);

                            DialogResult result = procedureDialog.ShowDialog();
                            if (result == DialogResult.OK)
                            {
                                string selectedProcedure = procedureComboBox.SelectedItem.ToString();

                                // Выводим диалоговое окно для ввода параметров
                                using (var parameterDialog = new Form())
                                {
                                    parameterDialog.Text = "Введите параметры";
                                    var parameterLabel = new Label();
                                    parameterLabel.Text = "Параметры (разделите запятыми):";
                                    parameterLabel.Dock = DockStyle.Top;
                                    var parameterTextBox = new TextBox();
                                    parameterTextBox.Dock = DockStyle.Top;
                                    var parameterOkButton = new Button();
                                    parameterOkButton.Text = "OK";
                                    parameterOkButton.DialogResult = DialogResult.OK;
                                    var parameterCancelButton = new Button();
                                    parameterCancelButton.Text = "Отмена";
                                    parameterCancelButton.DialogResult = DialogResult.Cancel;

                                    // Настройка внешнего вида кнопок
                                    parameterOkButton.AutoSize = true;
                                    parameterOkButton.Padding = new Padding(10, 5, 10, 5); // Настройка внутренних отступов
                                    parameterCancelButton.AutoSize = true;
                                    parameterCancelButton.Padding = new Padding(10, 5, 10, 5); // Настройка внутренних отступов

                                    // Размещение кнопок
                                    var buttonPanel1 = new FlowLayoutPanel();
                                    buttonPanel1.FlowDirection = FlowDirection.RightToLeft;
                                    buttonPanel1.Dock = DockStyle.Top;
                                    buttonPanel1.Controls.AddRange(new Control[] { parameterCancelButton, parameterOkButton });

                                    // Размещение компонентов на форме
                                    parameterDialog.Controls.Add(parameterLabel);
                                    parameterDialog.Controls.Add(parameterTextBox);
                                    parameterDialog.Controls.Add(buttonPanel1);

                                    DialogResult parameterResult = parameterDialog.ShowDialog();
                                    if (parameterResult == DialogResult.OK)
                                    {
                                        string parameterString = parameterTextBox.Text;
                                        List<string> parameters = parameterString.Split(',').Select(p => p.Trim()).ToList();

                                        // Выполняем процедуру с параметрами
                                        if (selectedProcedure == "DeletePerson")
                                        {
                                            int idToDelete;
                                            if (int.TryParse(parameters[0], out idToDelete))
                                            {
                                                // Подготовка SQL запроса
                                                string sql = "SELECT COUNT(*) FROM People WHERE Id = @Id";
                                                using (SqlCommand command2 = new SqlCommand(sql, connection))
                                                {
                                                    command2.Parameters.AddWithValue("@Id", idToDelete);

                                                    // Выполнение запроса и получение результата
                                                    int count = (int)command2.ExecuteScalar();

                                                    if (count > 0)
                                                    {
                                                        ExecuteProcedure(selectedProcedure, parameters);
                                                    }
                                                    else
                                                    {
                                                        MessageBox.Show("Отсутствует пользователь с указанным Id", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                        return;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                         ExecuteProcedure(selectedProcedure, parameters);
                                    }
                                }
                            }
                        }
                    }
                }
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
        else
        {
            MessageBox.Show("Потеряно соединение с базой данных.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ExecuteProcedure(string procedureName, List<string> parameters)
    {
        try
        {
            // Проверяем корректность ввода параметров
            int expectedParameterCount = GetProcedureParameterCount(procedureName);
            if (parameters.Count != expectedParameterCount)
            {
                MessageBox.Show("Неправильное количество параметров.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Выполняем процедуру с параметрами
            string parameterString = string.Join(",", parameters);
            string query = $"EXEC {procedureName} {parameterString}";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }

            MessageBox.Show("Процедура выполнена успешно.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (SqlException ex)
        {
            MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private int GetProcedureParameterCount(string procedureName)
    {
        // Получаем количество параметров процедуры
        string query = $"SELECT COUNT(*) FROM sys.parameters WHERE object_id = OBJECT_ID('{procedureName}')";
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            return (int)command.ExecuteScalar();
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