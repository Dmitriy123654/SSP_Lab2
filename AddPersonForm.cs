using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

public class AddPersonForm : Form
{
    private TextBox _FIOTextBox;
    private TextBox _YearOfBirthTextBox;
    private TextBox _colorTextBox;
    private SqlConnection connection;

    public string FIO { get; private set; }
    public int YearOfBirth { get; private set; }
    public string Address { get; private set; }

    public AddPersonForm(SqlConnection connection)
    {
        this.connection = connection;
        FormLayout();
    }

    private void FormLayout()
    {
        // Set up form properties
        this.Text = "Add Person";
        this.Size = new System.Drawing.Size(300, 200);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        // Create labels
        Label modelLabel = new Label();
        modelLabel.Text = "ФИО:";
        modelLabel.Location = new System.Drawing.Point(10, 10);

        Label yearLabel = new Label();
        yearLabel.Text = "Год рождения:";
        yearLabel.Location = new System.Drawing.Point(10, 40);

        Label colorLabel = new Label();
        colorLabel.Text = "Адрес:";
        colorLabel.Location = new System.Drawing.Point(10, 70);

        // Create text boxes
        _FIOTextBox = new TextBox();
        _FIOTextBox.Location = new System.Drawing.Point(125, 10);
        _FIOTextBox.Width = 150;
        _FIOTextBox.MaxLength = 50;

        _YearOfBirthTextBox = new TextBox();
        _YearOfBirthTextBox.Location = new System.Drawing.Point(125, 40);
        _YearOfBirthTextBox.Width = 150;

        _colorTextBox = new TextBox();
        _colorTextBox.Location = new System.Drawing.Point(125, 70);
        _colorTextBox.Width = 150;
        _colorTextBox.MaxLength = 50;

        // Create OK and Cancel buttons
        Button okButton = new Button();
        okButton.Text = "Сохранить";
        okButton.DialogResult = DialogResult.OK;
        okButton.Location = new System.Drawing.Point(80, 120);
        okButton.Click += OkButton_Click;

        Button cancelButton = new Button();
        cancelButton.Text = "Закрыть";
        cancelButton.DialogResult = DialogResult.Cancel;
        cancelButton.Location = new System.Drawing.Point(170, 120);

        // Add controls to the form
        this.Controls.Add(modelLabel);
        this.Controls.Add(yearLabel);
        this.Controls.Add(colorLabel);
        this.Controls.Add(_FIOTextBox);
        this.Controls.Add(_YearOfBirthTextBox);
        this.Controls.Add(_colorTextBox);
        this.Controls.Add(okButton);
        this.Controls.Add(cancelButton);
    }

    private void OkButton_Click(object sender, EventArgs e)
    {
        if (!ValidateInput())
        {
            MessageBox.Show("Некорректный ввод, пожалуйста перепроверьте поля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.DialogResult = DialogResult.Cancel;
            return;
        }

        try
        {
            AddPersonToDatabase();
        }
        catch (SqlException ex)
        {
            MessageBox.Show("Ошибка соединения с базой данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.DialogResult = DialogResult.Cancel;
            return;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ошибка соединения с базой данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.DialogResult = DialogResult.Cancel;
            return;
        }

        MessageBox.Show("Новая запись успешно добавлена", "Запись добавлена", MessageBoxButtons.OK, MessageBoxIcon.Information);
        this.DialogResult = DialogResult.OK;
    }

    private bool ValidateInput()
    {
        if (_FIOTextBox.Text.Length > 50 || _colorTextBox.Text.Length > 50 || _colorTextBox.Text.Length < 1 || _FIOTextBox.Text.Length < 1)
            return false;

        int.TryParse(_YearOfBirthTextBox.Text, out int tempYear);
        if (!int.TryParse(_YearOfBirthTextBox.Text, out int year) || tempYear < 0 || tempYear > 2024 || tempYear < 1917)
            return false;

        return true;
    }

    private void AddPersonToDatabase()
    {
        // Retrieve the entered values from the modal form
        int.TryParse(_YearOfBirthTextBox.Text, out int year);
        string targetFIO = _FIOTextBox.Text;
        int targetDateOfBirth = year;
        string targetAddress = _colorTextBox.Text;

        // Call the stored procedure to store the new record
        using (SqlCommand command = new SqlCommand("InsertPerson", connection))
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@FIO", targetFIO);
            command.Parameters.AddWithValue("@YearOfBirth", targetDateOfBirth);
            command.Parameters.AddWithValue("@Address", targetAddress);
            command.ExecuteNonQuery();
        }
    }
}