using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
//using static System.Net.Mime.MediaTypeNames;
//using System.Runtime.Remoting.Contexts;




namespace Introduction
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connection_string = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Library;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection connection = new SqlConnection(connection_string); // Создаем соединение с сервером. При создании оно не активно.
            
            connection.Open(); //Открываем соединение, поскольку автоматически оно не открывается.

            //Вставка:
            //string insert_string = @"INSERT INTO Authors (first_name, last_name) VALUES ('Steven', 'Hawking')";
            string insert_string = @"INSERT INTO Books (author, title, price, pages) VALUES (4, 'The Steven Hawkings Universe', 160, 850)";
            SqlCommand cmd = new SqlCommand(insert_string, connection);
            //cmd.ExecuteNonQuery(); // INSERT, UPDATE, DELETE этот метод позваляет отправлять данные запросы

            //Выборка:
            string select_string = @"SELECT Authors.last_name + ' ' + Authors.first_name , Books.title FROM  Books, Authors WHERE Books.author = Authors.id";
            cmd.CommandText = select_string;

            SqlDataReader rdr = cmd.ExecuteReader();//контейнер, который содержит результаты запросов. У него есть итератор
            while(rdr.Read())//Метод, который переставляет итератор на след позицию
            {
                Console.WriteLine($"{rdr[0]} \t {rdr[1]}");
            }
            rdr.Close();

            Console.WriteLine($"\n Для закрытия выбрать:\n 1. Добавить книгу\n 2. Выход");
            int choice = Convert.ToInt32(Console.ReadLine());
            switch(choice)
            {
                case 1:
                    {
                        Console.WriteLine("\nВведите имя автора книги: ");
                        string first_name = Console.ReadLine();
                        Console.WriteLine("\nВведите фамилию автора книги: ");
                        string last_name = Console.ReadLine();
                        SqlCommand author_cmd = new SqlCommand($"INSERT INTO Authors (first_name, last_name) VALUES ('{first_name}','{last_name}')", connection);
                        author_cmd.ExecuteNonQuery();
                        author_cmd.CommandText = ($"SELECT Authors.id FROM Authors WHERE Authors.first_name = '{first_name}' AND Authors.last_name = '{last_name}'");
                        SqlDataReader author_rdr = author_cmd.ExecuteReader();
                        author_rdr.Read();
                      
                        

                        Console.WriteLine("\nВведите название книги: ");
                        string book = Console.ReadLine();
                        Console.WriteLine("\nВведите стоимость книги: ");
                        string price = Console.ReadLine();
                        Console.WriteLine("\nВведите кол-во страниц книги: ");
                        string pages = Console.ReadLine();
                        SqlCommand book_cmd = new SqlCommand($"INSERT INTO Books (author,title,price,pages) VALUES ({author_rdr[0]},'{book}',{price},{pages})", connection);
                        author_rdr.Close();
                        book_cmd.ExecuteNonQuery();
                    }
                    break;

                case 2: break;


            }




            connection.Close(); //Соединения обязательно нужно закрывать
        }
    }
}
