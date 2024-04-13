using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

public class DeletePersonForm : Form
{
    private TextBox _idTextBox;
    private SqlConnection connection;

    public int Id { get; private set; }

    public DeletePersonForm(SqlConnection connection)
    {
        this.connection = connection;
        FormLayout();
    }

    private void FormLayout()
    {
        // Set up form properties
        this.Text = "Удалить запись";
        this.Size = new System.Drawing.Size(300, 150);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        // Create labels
        Label idLabel = new Label();
        idLabel.Text = "Id человека :";
        idLabel.Location = new System.Drawing.Point(10, 10);

        // Create text boxes
        _idTextBox = new TextBox();
        _idTextBox.Location = new System.Drawing.Point(125, 10);
        _idTextBox.Width = 150;

        // Create OK and Cancel buttons
        Button okButton = new Button();
        okButton.Text = "Удалить";
        okButton.DialogResult = DialogResult.OK;
        okButton.Location = new System.Drawing.Point(80, 80);
        okButton.Click += OkButton_Click;

        Button cancelButton = new Button();
        cancelButton.Text = "Закрыть";
        cancelButton.DialogResult = DialogResult.Cancel;
        cancelButton.Location = new System.Drawing.Point(170, 80);

        // Add controls to the form
        this.Controls.Add(_idTextBox);
        this.Controls.Add(idLabel);
        this.Controls.Add(okButton);
        this.Controls.Add(cancelButton);
    }

    private void OkButton_Click(object sender, EventArgs e)
    {
        if (!ValidateInput())
        {
            MessageBox.Show("Некорректный ввод, перепроверьте введённый данные.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.DialogResult = DialogResult.Cancel;
            return;
        }

        try
        {
            if (!PersonExistsInDatabase())
            {
                MessageBox.Show("Отсутствует человек с заданным ID", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                return;
            }
            RemovePeopleFromDatabase();
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
        MessageBox.Show("Человек с ID " + Id + " успешно удалён", "Человек удалён", MessageBoxButtons.OK, MessageBoxIcon.Information);
        this.DialogResult = DialogResult.OK;
    }

   

    private void RemovePeopleFromDatabase()
    {
        int.TryParse(_idTextBox.Text, out int parsedId);
        int id = parsedId;


        // Удаление связанных записей
        using (SqlCommand deleteCascadeCommand = new SqlCommand("DELETE FROM CascadeTable2 WHERE Id = @Id ", connection))
        {
            deleteCascadeCommand.Parameters.AddWithValue("@Id", id);
            deleteCascadeCommand.Parameters.AddWithValue("@FIO", id.ToString());
            deleteCascadeCommand.ExecuteNonQuery();
        }


        using (SqlCommand command = new SqlCommand("DeletePerson", connection))
        {
            int.TryParse(_idTextBox.Text, out int Id1);
            Id = parsedId;
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Id", Id);
            command.ExecuteNonQuery();
        }
    }
    private bool ValidateInput()
    {
        if (!int.TryParse(_idTextBox.Text, out int year) || year < 0)
            return false;

        return true;
    }

    public bool PersonExistsInDatabase()
    {
        using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM People WHERE id = @Id", connection))
        {
            int.TryParse(_idTextBox.Text, out int parsedId);
            Id = parsedId;
            command.Parameters.AddWithValue("@Id", Id);
            int count = (int)command.ExecuteScalar();
            return count > 0;
        }
    }

}
