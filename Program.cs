using System;
using System.Windows.Forms;

public class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        try
        {
            using (Form1 form = new Form1())
            {
                if (form.IsDatabaseConnected())
                {
                    form.FormLayout();
                    Application.Run(form);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}