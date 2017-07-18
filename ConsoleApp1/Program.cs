using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Program
{


    static void Main()
    {

        //Data Source must be changed to match server
        string connectionString =
            "Data Source=BRUKER-PC\\SQLEXPRESS;database=Notteroy_UserInfo;Integrated Security=True";

        var columnNames = new List<string>();
        var columnCount = 0;

        var pkRowsUserInfoOld = new List<string>();
        var pkRowsUserinfoV2 = new List<string>();

        var rowsUserInfoOld = new List<string>();
        var rowsUserinfoV2 = new List<string>();

        List<object[]> dataListUserInfoOld = new List<object[]>();
        List<object[]> dataListUserinfoV2 = new List<object[]>();

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();

            using (SqlCommand command = new SqlCommand("SELECT * FROM UserInfoOld", con))
            {

                SqlDataReader reader = command.ExecuteReader();
                columnCount = reader.FieldCount;

                for (int i = 0; i < columnCount; i++)
                {
                    columnNames.Add(reader.GetName(i));
                }

                while (reader.Read())
                {
                    object[] row = new object[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[i] = reader[i];
                    }
                    dataListUserInfoOld.Add(row);
                }

                reader.Close();


            }

            

            using (SqlCommand command = new SqlCommand("SELECT * FROM UserinfoV2", con))
            {
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    object[] row = new object[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[i] = reader[i];
                    }
                    dataListUserinfoV2.Add(row);
                }
            }
        }


        for (int i = 0; i < dataListUserInfoOld.Count; i++)
        {
            pkRowsUserInfoOld.Add(dataListUserInfoOld[i].GetValue(0).ToString());
        }

        for (int i = 0; i < dataListUserinfoV2.Count; i++)
        {
            pkRowsUserinfoV2.Add(dataListUserinfoV2[i].GetValue(0).ToString());
        }

        var addedRows = new List<string>();
        var removedRows = new List<string>();
        var equalRows = new List<string>();

        foreach (string pk in pkRowsUserInfoOld.Intersect(pkRowsUserinfoV2))
        {
            equalRows.Add(pk);
        }

        foreach (string pk in pkRowsUserInfoOld.Except(pkRowsUserinfoV2))
        {
            removedRows.Add(pk);
        }

        foreach (string pk in pkRowsUserinfoV2.Except(pkRowsUserInfoOld))
        {
            addedRows.Add(pk);
        }

        var nameOfFile1 = "Tekstfil1.txt";
        var nameOfFile2 = "Tekstfil2.txt";

        
        WriteToFile(addedRows, nameOfFile1);
        WriteToFile(removedRows, nameOfFile2);

        var differentValueList = new Dictionary<string, string>();

        foreach (object[] objectNew in dataListUserinfoV2)
        {
            string pk = RowObjectIsShared(objectNew, equalRows);
            if (pk == null)
            {
                continue;
            }

            else
            {
                foreach (object[] objectOld in dataListUserInfoOld)
                {
                    if (objectOld.GetValue(0).ToString().Contains(pk))
                    {
                        for (int i = 0; i < columnCount; i++)
                        {
                            if (objectOld.GetValue(i).ToString().Contains(objectNew.GetValue(i).ToString())) {
                                continue;
                            } else
                            {
                                differentValueList.Add(objectOld.GetValue(i).ToString(), objectNew.GetValue(i).ToString());
                            }
                                    
                        }
                    }
                }
            }



        }
        

        using (StreamWriter file = new StreamWriter("Tekstfil3.txt"))
            foreach (var entry in differentValueList)
                file.WriteLine("[{0} was found in old table, {1} was found in new table]", entry.Key, entry.Value);


    
        

    }

    static string RowObjectIsShared(Object[] o, List<string> rows)
    {
        foreach (string pk in rows)
        {
            if (o.GetValue(0).ToString().Contains(pk))
            {
                return pk;
            }
        }
        return null;
    }

    static void WriteToFile(List<String> columnList, String nameOfFile)
    {
     
        File.WriteAllLines(nameOfFile, columnList);
    }

}





