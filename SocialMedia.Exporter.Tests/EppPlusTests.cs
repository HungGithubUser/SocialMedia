using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace SocialMedia.Exporter.Tests;

[TestClass]
public class EppPlusTests
{
    [TestMethod]
    public void ExportExcelFileSuccess()
    {
        ExcelPackage.LicenseContext = LicenseContext.Commercial;
        
        var articles = new[]
        {
                new {
                    Id = "101", Name = "C++"
                },
                new {
                    Id = "102", Name = "Python"
                },
                new {
                    Id = "103", Name = "Java Script"
                },
                new {
                    Id = "104", Name = "GO"
                },
                new {
                    Id = "105", Name = "Java"
                },
                new {
                    Id = "106", Name = "C#"
                }
            };
  
        var excel = new ExcelPackage();
  
        var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
          
        workSheet.TabColor = System.Drawing.Color.Black;
        workSheet.DefaultRowHeight = 12;
  
        workSheet.Row(1).Height = 20;
        workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        workSheet.Row(1).Style.Font.Bold = true;
      
        workSheet.Cells[1, 1].Value = "S.No";
        workSheet.Cells[1, 2].Value = "Id";
        workSheet.Cells[1, 3].Value = "Name";
        
        var recordIndex = 2;
          
        foreach (var article in articles)
        {
            workSheet.Cells[recordIndex, 1].Value = (recordIndex - 1).ToString();
            workSheet.Cells[recordIndex, 2].Value = article.Id;
            workSheet.Cells[recordIndex, 3].Value = article.Name;
            recordIndex++;
        }
        workSheet.Column(1).AutoFit();
        workSheet.Column(2).AutoFit();
        workSheet.Column(3).AutoFit();
  
        const string pStrPath = "EPPTests.xlsx";
          
        if (File.Exists(pStrPath))
            File.Delete(pStrPath);
  
        var fileStream = File.Create(pStrPath);
        fileStream.Close();
  
        File.WriteAllBytes(pStrPath, excel.GetAsByteArray());
        excel.Dispose();
    }
}