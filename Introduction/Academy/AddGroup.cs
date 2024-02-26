using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using System.Configuration;


namespace Academy
{
    public partial class AddGroup : Form
    {
        private string connectionString;
        SqlConnection connection;
        SqlDataAdapter adapter;//самый важный параметр для работы в отсоединенном режиме. Вытягивает результаты запросов из базы и сохраняет их в dataset
        DataSet set;
        SqlCommandBuilder builder;
        public System.Windows.Forms.ComboBox CBDirection { get => cbDirection; }
        public System.Windows.Forms.ComboBox CBLearningForm { get => cbLearningForm; }
        public System.Windows.Forms.ComboBox CBLearningTime { get => cbLearningTime; }
        CheckBox[] cbWeek;
        public AddGroup()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            connection = new SqlConnection(connectionString);
            //((Form1)Parent).LoadDataToComboBox(cbDirection, "Directions", "direction_name", "Выберите направление обучения");
            //((Form1)Parent).LoadDataToComboBox(cbLearningForm, "LearningForms", "form_name", "Выберите форму обучения");
            //((Form1)Parent).LoadDataToComboBox(cbLearningTime, "LearningTimes", "time_name", "Выберите время обучения");
            cbWeek = new CheckBox[7]; 
        }
        void GetDataFromBase()
        {
            try
            {
                set = new DataSet();
                string cmd = $@"SELECT * FROM Directions";
                adapter = new SqlDataAdapter(cmd, connection);
                builder = new SqlCommandBuilder(adapter);
                adapter.Fill(set, "Directions");
                adapter.SelectCommand.CommandText = $@"SELECT * FROM Times";
                builder.DataAdapter = adapter;
                adapter.Fill(set, "Times");
            }
            catch (Exception e)
            {
                MessageBox.Show(this, e.Message);
            }
        }
        string GenerateGroupName()
        {
            string group_name = "";
            if(cbLearningForm.SelectedItem == "Стационар")
            {
                if(cbDirection.SelectedItem == "Разработка программного обеспечения")
                    //group_name += set.Tables["Directions"].Select("")//2 01 24
            }
            return group_name;
        }
        private void lcbWeek_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
