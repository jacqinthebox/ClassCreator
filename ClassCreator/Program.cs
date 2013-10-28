using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassCreator
{
    class Program
    {
        private class ClassCreator
        {
            //grabs the first connectionstring in App.config or Web.Config
            //private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings[1].ConnectionString;
            private static readonly string ConnectionStringName = ConfigurationManager.ConnectionStrings[1].Name;

            private const string ConnectionString = @"Data Source=.\sqlexpress;Initial Catalog=Uitvaartwensen;Integrated Security=True";
            private const string Start = @"<div class=""control-group"">";
            //@"Data Source=.\sqlexpress;Initial Catalog=SheepTracker;Integrated Security=True";


            private static string DataType(string sqlDataType)
            {
                switch (sqlDataType)
                {
                    case "uniqueidentifier":
                        return "Guid";
                    case "varchar":
                        return "string";
                    case "bit":
                        return "bool";
                    case "text":
                        return "string";
                    case "date":
                        return "DateTime?";
                    default:
                        return "string";

                }
            }

            private IEnumerable<string> getTableNames()
            {
                var names = new List<string>();

                const string queryTableName = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES order by TABLE_NAME";

                var connection = new SqlConnection(ConnectionString);
                var cmd = connection.CreateCommand();
                cmd.CommandText = queryTableName;
                connection.Open();
                var tblreader = cmd.ExecuteReader();
                while (tblreader.Read())
                {
                    names.Add(tblreader[0].ToString());
                }
                tblreader.Close();
                connection.Close();
                return names;
            }

            public void DumpClasses()
            {
                var connection = new SqlConnection(ConnectionString);
                var cmd = connection.CreateCommand();

                var tablenames = getTableNames();


                foreach (var t in tablenames)
                {

                    string queryString =
                        "select COLUMN_NAME,DATA_TYPE from information_schema.columns where table_name = '" + t +
                        "' order by ordinal_position;";

                    cmd.CommandText = queryString;

                    Console.WriteLine("");

                    //Massify the class like this:
                    Console.WriteLine("public class " + t + " {");

                    Console.WriteLine("");

                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Console.WriteLine("public " + DataType(reader[1].ToString()) + " " + reader[0] +
                                          " { get; set;} ");
                    }
                    reader.Close();
                    connection.Close();
                    Console.WriteLine("}");

                }
            }

            public void KnockoutObservables()
            {
                /*
                string text = "A class is the most powerful data type in C#. Like structures, " +
                           "a class defines the data and behavior of the data type. ";
            System.IO.File.WriteAllText(@"C:\Users\Public\TestFolder\WriteText.txt", text);
                                */

                using (var file = new System.IO.StreamWriter(@"C:\temp\WriteLines2.txt"))
                {
                    var connection = new SqlConnection(ConnectionString);
                    var cmd = connection.CreateCommand();

                    var tablenames = getTableNames();


                    foreach (var t in tablenames)
                    {

                        string queryString =
                            "select COLUMN_NAME,DATA_TYPE from information_schema.columns where table_name = '" + t +
                            "' order by ordinal_position;";

                        cmd.CommandText = queryString;

                        Console.WriteLine("");

                        //Massify the class like this:
                        Console.WriteLine("table: " + t);

                        Console.WriteLine("");

                        connection.Open();
                        SqlDataReader reader = cmd.ExecuteReader();


                        while (reader.Read())
                        {
                            Console.WriteLine("self." + reader[0] + "=ko.observable()");
                            file.WriteLine("self." + reader[0] + "=ko.observable();");
                        }

                        reader.Close();
                        connection.Close();
                  

                        //do it again:

                        connection.Open();
                        var reader2 = cmd.ExecuteReader();


                        while (reader2.Read())
                        {
                            var veld = reader2[0];
                            file.WriteLine(Start);
                            file.WriteLine(@"<label class=""control-label"">" + veld + "</label>");
                            file.WriteLine(@"<div class=""controls"">");
                            file.WriteLine(@"<input type=""text"" id=\""" + veld + "\" placeholder=\"" + veld +
                                              "\" data-bind=\"value: " + veld + "\">");
                            file.WriteLine("</div>");
                            file.WriteLine("</div>");
                            file.WriteLine("");
                        }

                    }
                }
            }

            public void MassiveMap()
            {
                var connection = new SqlConnection(ConnectionString);
                var cmd = connection.CreateCommand();

                var tablenames = getTableNames();


                foreach (var t in tablenames)
                {

                    string queryString =
                        "select COLUMN_NAME,DATA_TYPE from information_schema.columns where table_name = '" + t +
                        "' order by ordinal_position;";

                    cmd.CommandText = queryString;

                    Console.WriteLine("");

                    //Massify the class like this:
                    Console.WriteLine("public class " + t + ":DynamicModel {");
                    Console.WriteLine(@"public " + t + "():base(\"" +
                                      ConnectionStringName + "\") {\n PrimaryKeyField =\"Id\";}");

                    Console.WriteLine("");

                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Console.WriteLine("public " + DataType(reader[1].ToString()) + " " + reader[0] +
                                          " { get; set;} ");
                    }
                    reader.Close();
                    connection.Close();
                    Console.WriteLine("}");

                }
            }
        }

        private static void Main(string[] args)
        {
            var c = new ClassCreator();
            c.DumpClasses();
            c.KnockoutObservables();
            Console.ReadKey();
        }
    }
 }

