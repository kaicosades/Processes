﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms.VisualStyles;//Добавляется в референсах

namespace Academy
{
    public partial class Form1 : Form
    {
        string connectionString;
        SqlConnection connection;
        DataTable table;
        SqlDataReader reader;
        public Form1()
        {
            InitializeComponent();

            connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            connection = new SqlConnection(connectionString);
            //LoadTablesToComboBox();
            LoadGroupsToComboBox(cbGroup);
            SelectStudents();
            LoadDirectionsToComboBox();
            rbStudents.Checked = true;//устанавливаем галочку студентов по умолчанию, чтобы не было ошибки без выбора галочки
            LoadDataToComboBox(cbDirectionOnGroupTab, "Directions", "direction_name", "Выберите направление");
        }
        void LoadTablesToComboBox()
        {
            string commandLine = @"SELECT table_name FROM information_schema.tables";
            SqlCommand cmd = new SqlCommand(commandLine, connection);
            connection.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                //comboBox1.Items.Add(reader[0]);
            }
            reader.Close();
            connection.Close();
        }
        public void LoadDataToComboBox(System.Windows.Forms.ComboBox comboBox,
            string sourceTable,
            string sourceColumn,
            string invite = "Выберите значение")//?
        {
            string commandLine = $@"SELECT {sourceColumn} FROM {sourceTable}";
            SqlCommand cmd = new SqlCommand(commandLine,connection);
            //SqlCommand cmd = new SqlCommand();
            //cmd.Connection = connection;
            //cmd.Parameters.Add("@table_name", SqlDbType.Table);
            //cmd.Parameters.Add("@column_name", sourceColumn);
            //cmd.CommandText = @"SELECT @column_name FROM @table_name";
            connection.Open();
            reader = cmd.ExecuteReader();
            comboBox.Items.Add(invite);
            while (reader.Read())
            {
                comboBox.Items.Add(reader[0]);
            }
            reader.Close();
            connection.Close();
            comboBox.SelectedItem = invite;

        }

        public void SelectDataFromTable
            (System.Windows.Forms.DataGridView dataGridView,
            string commandLine
           // string tableName,
           // params string[] columns
           )
        {
            //SqlCommand cmd = new SqlCommand();
            //cmd.Connection = connection;
            //cmd.CommandText = "SELECT ";
            //for (int i = 0; i< columns.Length; i++)
            //{
            //    cmd.Parameters.Add($"{columns[i]}", columns[i]);// columns[i]
            //    cmd.CommandText += $"{columns[i]}";
            //    cmd.CommandText += i == columns.Length - 1 ? " " : ", ";
            //}
            //cmd.CommandText += $"FROM {tableName}";

            SqlCommand cmd = new SqlCommand (commandLine,connection);
            connection.Open();
            reader = cmd.ExecuteReader();
            table = new DataTable();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                table.Columns.Add(reader.GetName(i));
            }
            while (reader.Read())
            {
                DataRow row = table.NewRow();
                for (int i =0; i<reader.FieldCount; i++)
                {
                    row[i] = reader[i];
                }
                table.Rows.Add(row);
            }
            dataGridView.DataSource = table;
            reader.Close();
            connection.Close();
            
        }

        public void LoadGroupsToComboBox(System.Windows.Forms.ComboBox comboBox) //Чтобы в СБ был выбор групп
        {
            string commandLine = @"SELECT group_name FROM Groups";
            SqlCommand cmd = new SqlCommand(commandLine, connection);
            connection.Open();
            reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                comboBox.Items.Add(reader[0]);
            }
            reader.Close();
            connection.Close();
        }

        public void LoadDirectionsToComboBox()
        {
            string commandLine = @"SELECT direction_name FROM Directions";
            SqlCommand cmd = new SqlCommand(commandLine, connection);
            connection.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cbDirection.Items.Add(reader[0]);
            }
            reader.Close();
            connection.Close();
        }
        void SelectStudents(string group = "")
        {
            string commandLine = @"
            SELECT last_name, first_name, middle_name, birth_date, group_name 
            FROM Students JOIN Groups ON Students.[group]=Groups.[group_id]
            ";
            if (group.Length != 0) commandLine += $" WHERE [group_name] = '{group}'";
            SqlCommand cmd = new SqlCommand(commandLine, connection);
            connection.Open();
            reader = cmd.ExecuteReader();
            table = new DataTable();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                table.Columns.Add(reader.GetName(i)); //шапка
            }
            while (reader.Read())
            {
                DataRow row = table.NewRow();
                for(int i = 0; i<reader.FieldCount; i++)
                {
                    row[i] = reader[i];
                }
                row["birth_date"] = Convert.ToDateTime(reader["birth_date"]).ToString("yyyy.MM.dd");//Нужно чтобы не выводило часы. Во время написания кода выводит у всех 12 AM
                table.Rows.Add(row); // строчка, которая заполнет таблицу(шапку)
            }
            dgvStudents.DataSource = table;
            reader.Close();
            if (group.Length > 0)
            {
                cmd.CommandText = $@"
                SELECT COUNT (stud_id) 
                FROM Students JOIN Groups ON Students.[group] = Groups.group_id
                WHERE [group_name] = '{group}' GROUP BY group_name";
            }  
            else // Если группа не выбрана то выдает количество всех студентов
            {
                cmd.CommandText = $@"
                SELECT COUNT (stud_id) 
                FROM Students JOIN Groups ON Students.[group] = Groups.group_id
                ";
            }
            lblStudCount.Text = $"Количество студенотов: {Convert.ToInt32(cmd.ExecuteScalar()).ToString()}";
            connection.Close();
        }

        private void cbGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectStudents(cbGroup.SelectedItem.ToString().Trim()); //Trim в данной проге убирает пробелы после выбора группы
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // view code для открытия формы
            // Эта форма работает с базой, основная - собирает данные
            AddStudent add_student = new AddStudent();
            LoadGroupsToComboBox(add_student.GroupCombo);
            DialogResult result = add_student.ShowDialog(this);
            if(result == DialogResult.OK)
            {
                //Параметризованные запросы:
                SqlCommand cmd = new SqlCommand(); // Часть после  '=' - выделение памяти
                cmd.Connection = connection;
                cmd.Parameters.Add("@last_name", add_student.FullName.Split(' ')[0]);
                cmd.Parameters.Add("@first_name", add_student.FullName.Split(' ')[1]);
                if(add_student.FullName.Split(' ').Length == 3)cmd.Parameters.Add("@middle_name", add_student.FullName.Split(' ')[2]);
                cmd.Parameters.Add("@birth_date", add_student.BirthDate.ToString("yyyy-MM-dd"));
                cmd.Parameters.Add("@group", add_student.Group);
                cmd.CommandText = @"
IF NOT EXISTS (SELECT stud_id FROM Students WHERE last_name = @last_name AND first_name=@first_name AND middle_name = @middle_name)
BEGIN
INSERT INTO
Students (last_name, first_name, middle_name, birth_date, [group])
VALUES (@last_name, @first_name, @middle_name, @birth_date, (SELECT group_id FROM Groups WHERE group_name = @group))
END
";
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
                cbGroup.SelectedItem = add_student.Group;
                SelectStudents();
            }
        }

        private void cbDirection_SelectedIndexChanged(object sender, EventArgs e)//обработчик события - действие с кнопкой
        {
            dgvStudents.DataSource = null;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            if (cbDirection.SelectedItem != null) cmd.Parameters.Add("@direction", cbDirection.SelectedItem); // второй параметр запоминает выбор и передается далее в запрос


            if (rbStudents.Checked)//проверка на галочку
            {
                cmd.CommandText = @"
SELECT last_name, first_name, middle_name, birth_date, group_name, direction_name
FROM Students 
JOIN Groups ON Students.[group] = Groups.group_id
JOIN Directions ON Groups.direction=Directions.direction_id";
                if(cbDirection.SelectedItem!=null) cmd.CommandText += //Проверка нужна для того, чтобы запрос запустился еще раз при смене галочки
@" WHERE Directions.direction_name=@direction
";
            }

            if (rbGroups.Checked)//проверка на галочку
            {
                cmd.CommandText = @"
SELECT  group_name, direction_name
FROM  Groups 
JOIN Directions ON Groups.direction=Directions.direction_id";
                if (cbDirection.SelectedItem != null) cmd.CommandText += //Проверка нужна для того, чтобы запрос запустился еще раз при смене галочки
@" WHERE Directions.direction_name=@direction
";
            }

            connection.Open();
            reader = cmd.ExecuteReader();
            table = new DataTable();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                table.Columns.Add(reader.GetName(i));
            }
            while (reader.Read())
            {
                DataRow row = table.NewRow();
                for (int i = 0;i < reader.FieldCount; i++) 
                {
                    row[i] = reader[i];
                }
                table.Rows.Add(row);
            }
            dgvStudents.DataSource = table;
            reader.Close();
            int studentCount = dgvStudents.RowCount - 1;//подсчитывает количество строк. Проскольку RowCount это свойство, то () не пишется
            lblStudCount.Text = $"Количество студентов: {studentCount}";
            connection.Close() ;
            //string commandLine = @"SELECT";
        }

        private void rbGroups_CheckedChanged(object sender, EventArgs e)
        {
            if(rbGroups.Checked)
            {
                cbDirection_SelectedIndexChanged(sender, e);

            }
        }

        private void rbStudents_CheckedChanged(object sender, EventArgs e)
        {
            if(rbStudents.Checked)
            {
                cbDirection_SelectedIndexChanged(sender, e);

            }
        }

        private void cbDirectionOnGroupTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            //SelectDataFromTable(dataGridViewGroups, "Groups", "group_name", "direction");
            string commandLine =
                $@"
SELECT group_name, direction_name 
FROM Groups JOIN Directions ON direction=direction_id
";
            if (cbDirectionOnGroupTab.SelectedIndex != 0)
                commandLine += $@"WHERE direction_name = '{cbDirectionOnGroupTab.SelectedItem}'";
            SelectDataFromTable(dataGridViewGroups, commandLine);
            lblGroupsCount.Text = $"Количество групп: {dataGridViewGroups.Rows.Count - 1}";
        }

        private void btnGroupAdd_Click(object sender, EventArgs e)
        {
            AddGroup add = new AddGroup();
            LoadDataToComboBox(add.CBDirection, "Directions", "direction_name", "Выберите направление обучения");
            LoadDataToComboBox(add.CBLearningForm, "LearningForms", "form_name", "Выберите форму обучения");
            LoadDataToComboBox(add.CBLearningTime, "LearningTimes", "time_name", "Выберите время обучения");
            DialogResult result = add.ShowDialog();
            if (result == DialogResult.OK)
            {
                
            }
        }
    }
}
