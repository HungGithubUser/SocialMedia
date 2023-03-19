using System.Configuration;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Syncfusion.Presentation;

namespace SocialMedia.Exporter.Tests;

[TestClass]
public class SyncfusionTests
{
    private readonly string _syncFusionLicenseKey;
    
    public SyncfusionTests()
    {
        _syncFusionLicenseKey = GetSyncFusionLicenseKey() ?? throw new Exception();
    }

    [TestMethod]
    public void UserSecret_Should_BeFound()
    {
        var key = GetSyncFusionLicenseKey();
        Assert.IsFalse(string.IsNullOrEmpty(key));
    }

    [TestMethod]
    public void ExportPowerPointFileSuccess()
    {
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(_syncFusionLicenseKey);
        //Create a new instance of PowerPoint Presentation file.
        using IPresentation pptxDoc = Presentation.Create();

        //Add a new slide to file and apply background color.
        ISlide slide = pptxDoc.Slides.Add(SlideLayoutType.TitleOnly);
        //Specify the fill type and fill color for the slide background.
        slide.Background.Fill.FillType = FillType.Solid;
        slide.Background.Fill.SolidFill.Color = ColorObject.FromArgb(232, 241, 229);

        //Add title content to the slide by accessing the title placeholder of the TitleOnly layout-slide.
        IShape titleShape = slide.Shapes[0] as IShape;
        titleShape.TextBody.AddParagraph("Company History").HorizontalAlignment = HorizontalAlignmentType.Center;

        //Add description content to the slide by adding a new TextBox.
        IShape descriptionShape = slide.AddTextBox(53.22, 141.73, 874.19, 77.70);
        descriptionShape.TextBody.Text =
            "IMN Solutions PVT LTD is the software company, established in 1987, by George Milton. The company has been listed as the trusted partner for many high-profile organizations since 1988 and got awards for quality products from reputed organizations.";

        //Add bullet points to the slide.
        IShape bulletPointsShape = slide.AddTextBox(53.22, 270, 437.90, 116.32);
        //Add a paragraph for a bullet point.
        IParagraph firstPara = bulletPointsShape.TextBody.AddParagraph(
            "The company acquired the MCY corporation for 20 billion dollars and became the top revenue maker for the year 2015.");
        //Format how the bullets should be displayed.
        firstPara.ListFormat.Type = ListType.Bulleted;
        firstPara.LeftIndent = 35;
        firstPara.FirstLineIndent = -35;
        // Add another paragraph for the next bullet point.
        IParagraph secondPara =
            bulletPointsShape.TextBody.AddParagraph(
                "The company is participating in top open source projects in automation industry.");
        //Format how the bullets should be displayed.
        secondPara.ListFormat.Type = ListType.Bulleted;
        secondPara.LeftIndent = 35;
        secondPara.FirstLineIndent = -35;

        //Get a picture as stream.
        // using FileStream pictureStream = new(Path.GetFullPath("Data/Image.jpg"), FileMode.Open, FileAccess.Read);
        //Add the picture to a slide by specifying its size and position.
        // slide.Shapes.AddPicture(pictureStream, 499.79, 238.59, 364.54, 192.16);

        //Add an auto-shape to the slide.
        IShape stampShape = slide.Shapes.AddShape(AutoShapeType.Explosion1, 48.93, 430.71, 104.13, 80.54);
        //Format the auto-shape color by setting the fill type and text.
        stampShape.Fill.FillType = FillType.None;
        stampShape.TextBody.AddParagraph("IMN").HorizontalAlignment = HorizontalAlignmentType.Center;

        //Save the PowerPoint Presentation as stream.
        MemoryStream pptxStream = new();
        pptxDoc.Save(pptxStream);
        pptxStream.Position = 0;

        //Download Powerpoint document in the browser.
        using FileStream file = new FileStream("powerpoint.pptx", FileMode.Create, System.IO.FileAccess.Write);
        byte[] bytes = new byte[pptxStream.Length];
        pptxStream.Read(bytes, 0, (int)pptxStream.Length);
        file.Write(bytes, 0, bytes.Length);
        pptxStream.Close();
    }
    
    private static string? GetSyncFusionLicenseKey()
    {
        var builder = new ConfigurationBuilder().AddUserSecrets(Assembly.GetExecutingAssembly());
        var root = builder.Build();
        var key = root.GetSection("SyncFusion:LicenseKey").Value;
        return key;
    }
}