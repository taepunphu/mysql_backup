using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mysql_backup
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                DoBackup();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void DoBackup()
        {
            string connstr = "server=localhost;user=root;pwd=;";

            using (MySqlConnection conn = new MySqlConnection(connstr))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    int count = 0;
                    conn.Open();
                    cmd.Connection = conn;

                    cmd.CommandText = "show databases";
                    MySqlDataReader reader = cmd.ExecuteReader();
                    DataTable dtDbList = new DataTable();
                    dtDbList.Load(reader);

                    foreach (DataRow dr in dtDbList.Rows)
                    {
                        string db = dr[0] + "";

                        switch (db)
                        {
                            case "phpmyadmin":
                            case "performance_schema":
                            case "mysql":
                            case "information_schema":
                                continue;
                        }

                        cmd.CommandText = $"use `{db}`";
                        cmd.ExecuteNonQuery();

                        string defaultBackupFolder = "D:\\backup_folder";
                        string file = Path.Combine(defaultBackupFolder, $"{db}.sql");

                        using (MySqlBackup mb = new MySqlBackup(cmd))
                        {
                            mb.ExportToFile(file);
                        }

                        count++;

                    }

                    conn.Close();
                }
            }
        }
    }
}
