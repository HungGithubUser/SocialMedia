using System.Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace SocialMedia.Exporter.Tests;

public static class ListExtension
{
    public static DataSet ToDataSet<T>(this IList<T> list)
    {
        Type elementType = typeof(T);
        DataSet ds = new DataSet();
        DataTable t = new DataTable();
        ds.Tables.Add(t);

        //add a column to table for each public property on T
        foreach (var propInfo in elementType.GetProperties())
        {
            Type ColType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;

            t.Columns.Add(propInfo.Name, ColType);
        }

        //go through each property on T and add each value to the table
        foreach (T item in list)
        {
            DataRow row = t.NewRow();

            foreach (var propInfo in elementType.GetProperties())
            {
                row[propInfo.Name] = propInfo.GetValue(item, null) ?? DBNull.Value;
            }

            t.Rows.Add(row);
        }

        return ds;
    }
}
[TestClass]
public class OpenXmlTests
{
    [TestMethod]
    public void CreateSpreadsheetSuccess()
    {
        var spreadsheetDocument =
            SpreadsheetDocument.Create("filepath.xlsx",
                SpreadsheetDocumentType.Workbook);

        var workbookpart = spreadsheetDocument.AddWorkbookPart();
        workbookpart.Workbook = new Workbook();

        var worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
        var sheetData = new SheetData
        {
        };
        worksheetPart.Worksheet = new Worksheet(sheetData);

        var sheets = spreadsheetDocument.WorkbookPart?.Workbook.AppendChild(new Sheets());

        var sheet = new Sheet
        {
            Id = spreadsheetDocument.WorkbookPart?.GetIdOfPart(worksheetPart),
            SheetId = 1,
            Name = "mySheet",
        };
        sheets?.Append(sheet);

        workbookpart.Workbook.Save();

        spreadsheetDocument.Close();
    }

    [TestMethod]
    public void CreateExcelFile()
    {
        var list = new List<SomeTable>
        {
            new() { Id = 1, Name = "1" },
            new() { Id = 2, Name = "2" },
            new() { Id = 3, Name = "3" },
            new() { Id = 4, Name = "4" },
        };
        var ds = list.ToDataSet();
        ExportDataSet(ds,"Test.xlsx");
    }

    private class SomeTable
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
    
    private void ExportDataSet(DataSet ds, string destination)
    {
        using var workbook = SpreadsheetDocument.Create(destination, SpreadsheetDocumentType.Workbook);
        workbook.AddWorkbookPart();

        if (workbook.WorkbookPart == null) return;
        workbook.WorkbookPart.Workbook = new Workbook
        {
            Sheets = new Sheets()
        };

        foreach (DataTable table in ds.Tables)
        {
            var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
            var sheetData = new SheetData();
            sheetPart.Worksheet = new Worksheet(sheetData);

            var sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
            var relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

            uint sheetId = 1;
            if (sheets != null && sheets.Elements<Sheet>().Any())
            {
                sheetId =
                    sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            var sheet = new Sheet
            {
                Id = relationshipId,
                SheetId = sheetId,
                Name = table.TableName
            };
            sheets?.Append(sheet);
            var headerRow = new Row();
            var columns = new List<string>();
            foreach (DataColumn column in table.Columns)
            {
                columns.Add(column.ColumnName);
                var cell = new Cell
                {
                    DataType = CellValues.String,
                    CellValue = new CellValue(column.ColumnName)
                };
                headerRow.AppendChild(cell);
            }
            sheetData.AppendChild(headerRow);
            foreach (DataRow dsrow in table.Rows)
            {
                var newRow = new Row();
                foreach (var col in columns)
                {
                    var cell = new Cell
                    {
                        DataType = CellValues.String,
                        CellValue = new CellValue(dsrow[col].ToString() ?? string.Empty)
                    };
                    newRow.AppendChild(cell);
                }
                sheetData.AppendChild(newRow);
            }
        }
    }
}