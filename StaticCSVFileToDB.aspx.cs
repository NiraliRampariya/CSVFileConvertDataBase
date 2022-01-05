using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace CSVFileConvertDataBase
{
    public partial class StaticCSVFileToDB : System.Web.UI.Page
    {
        protected void Upload(object sender, EventArgs e)
        {
            string csvPath = Server.MapPath("~/Files/") + Path.GetFileName(FileUpload1.PostedFile.FileName);
            FileUpload1.SaveAs(csvPath);

            DataTable dataTable = new DataTable();
            dataTable.Columns.AddRange(new DataColumn[5] { new DataColumn("ID", typeof(string)),
            new DataColumn("Name", typeof(string)),
            new DataColumn("Address",typeof(string)),
            new DataColumn("Gender",typeof(string)),
            new DataColumn("ContactNo",typeof(string)) });

            string csvData = File.ReadAllText(csvPath);
            foreach (string row in csvData.Split('\n'))
            {
                if (!string.IsNullOrEmpty(row))
                {
                    dataTable.Rows.Add();
                    int i = 0;
                    foreach (string cell in row.Split(','))
                    {
                        dataTable.Rows[dataTable.Rows.Count - 1][i] = cell;
                        i++;
                    }
                }
            }

            string consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(consString))
            {
                //Add data csv file to database 
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                {
                    //Set the database table name
                    sqlBulkCopy.DestinationTableName = "PersonInformation";
                    con.Open();
                    sqlBulkCopy.WriteToServer(dataTable);
                    con.Close();
                }
            }
        }
    }
    
}