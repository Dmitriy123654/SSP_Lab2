using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

public class AddCarForm : Form
{
    private TextBox _modelTextBox;
    private TextBox _yearTextBox;
    private TextBox _colorTextBox;
    private SqlConnection connection;

    public string Model { get; private set; }
    public int Year { get; private set; }
    public string Color { get; private set; }

    public AddCarForm(SqlConnection connection)
    {
        this.connection = connection;
        FormLayout();
    }

    private void FormLayout()
    {
        // Set up form properties
        this.Text = "Add Car";
        this.Size = new System.Drawing.Size(300, 200);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        // Create labels
        Label modelLabel = new Label();
        modelLabel.Text = "Model:";
        modelLabel.Location = new System.Drawing.Point(10, 10);

        Label yearLabel = new Label();
        yearLabel.Text = "Year:";
        yearLabel.Location = new System.Drawing.Point(10, 40);

        Label colorLabel = new Label();
        colorLabel.Text = "Color:";
        colorLabel.Location = new System.Drawing.Point(10, 70);

        // Create text boxes
        _modelTextBox = new TextBox();
        _modelTextBox.Location = new System.Drawing.Point(125, 10);
        _modelTextBox.Width = 150;
        _modelTextBox.MaxLength = 50;

        _yearTextBox = new TextBox();
        _yearTextBox.Location = new System.Drawing.Point(125, 40);
        _yearTextBox.Width = 150;

        _colorTextBox = new TextBox();
        _colorTextBox.Location = new System.Drawing.Point(125, 70);
        _colorTextBox.Width = 150;
        _colorTextBox.MaxLength = 50;

        // Create OK and Cancel buttons
        Button okButton = new Button();
        okButton.Text = "Save";
        okButton.DialogResult = DialogResult.OK;
        okButton.Location = new System.Drawing.Point(80, 120);
        okButton.Click += OkButton_Click;

        Button cancelButton = new Button();
        cancelButton.Text = "Cancel";
        cancelButton.DialogResult = DialogResult.Cancel;
        cancelButton.Location = new System.Drawing.Point(170, 120);

        // Add controls to the form
        this.Controls.Add(modelLabel);
        this.Controls.Add(yearLabel);
        this.Controls.Add(colorLabel);
        this.Controls.Add(_modelTextBox);
        this.Controls.Add(_yearTextBox);
        this.Controls.Add(_colorTextBox);
        this.Controls.Add(okButton);
        this.Controls.Add(cancelButton);
    }

    private void OkButton_Click(object sender, EventArgs e)
    {
        if (!ValidateInput())
        {
            MessageBox.Show("Invalid input. Please check the fields and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.DialogResult = DialogResult.Cancel;
            return;
        }

        try
        {
            AddCarToDatabase();
        }
        catch (SqlException ex)
        {
            MessageBox.Show("Database connection error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.DialogResult = DialogResult.Cancel;
            return;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Database connection error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.DialogResult = DialogResult.Cancel;
            return;
        }

        MessageBox.Show("New car successfully added to database.", "Car added", MessageBoxButtons.OK, MessageBoxIcon.Information);
        this.DialogResult = DialogResult.OK;
    }

    private bool ValidateInput()
    {
        if (_modelTextBox.Text.Length > 50 || _colorTextBox.Text.Length > 50 || _colorTextBox.Text.Length < 1 || _modelTextBox.Text.Length < 1)
            return false;

        int.TryParse(_yearTextBox.Text, out int tempYear);
        if (!int.TryParse(_yearTextBox.Text, out int year) || tempYear < 0 || tempYear > 2024 || tempYear < 1917)
            return false;

        return true;
    }

    private void AddCarToDatabase()
    {
        // Retrieve the entered values from the modal form
        int.TryParse(_yearTextBox.Text, out int year);
        string targetModel = _modelTextBox.Text;
        int targetYear = year;
        string targetColor = _colorTextBox.Text;

        // Call the stored procedure to store the new record
        using (SqlCommand command = new SqlCommand("InsertCar", connection))
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Model", targetModel);
            command.Parameters.AddWithValue("@Year", targetYear);
            command.Parameters.AddWithValue("@Color", targetColor);
            command.ExecuteNonQuery();
        }
    }
}