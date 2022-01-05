using Microsoft.VisualBasic.FileIO;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace CSVFileConvertDataBase
{
    public partial class DynamicallyCSVFileToDB : System.Web.UI.Page
    {
        protected void Upload(object sender, EventArgs e)
        {
            DataTable csvData = new DataTable();

            string csvPath = Server.MapPath("~/Files/") + Path.GetFileName(FileUpload1.PostedFile.FileName);
            FileUpload1.SaveAs(csvPath);
            using (TextFieldParser csvReader = new TextFieldParser(csvPath))
            {
                csvReader.SetDelimiters(new string[] { "," });
                csvReader.HasFieldsEnclosedInQuotes = true;
                string[] colFields = csvReader.ReadFields();
                foreach (string column in colFields)
                {
                    DataColumn dataColumn = new DataColumn(column);
                    dataColumn.AllowDBNull = true;
                    csvData.Columns.Add(dataColumn);
                }
                while (!csvReader.EndOfData)
                {
                    string[] fieldData = csvReader.ReadFields();
                    csvData.Rows.Add(fieldData);
                }
            }
            string consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(consString))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand("", conn))
                    {

                        conn.Open();
                        command.CommandText = "CREATE TABLE #TmpTable(Item_Id nvarchar(255),Item_Prc_1 nvarchar(255),Item_Prc_2 nvarchar(255),Item_Prc_3 nvarchar(255))";
                        command.ExecuteNonQuery();
                        using (SqlBulkCopy bulkcopy = new SqlBulkCopy(conn))
                        {
                            bulkcopy.BulkCopyTimeout = 660;
                            bulkcopy.DestinationTableName = "#TmpTable";
                            bulkcopy.WriteToServer(csvData);
                            bulkcopy.Close();
                        }

                        // Updating destination table, and dropping temp table
                        command.CommandTimeout = 300;
                        command.CommandText = "UPDATE DisplayItems SET Item_Prc_1 = #TmpTable.Item_Prc_1 FROM #TmpTable where DisplayItems.Item_Id = #TmpTable.Item_Id; DROP TABLE #TmpTable;";
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }
}