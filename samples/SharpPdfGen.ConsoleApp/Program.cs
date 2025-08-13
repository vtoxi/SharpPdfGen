using System.Drawing;
using SharpPdfGen;
using SharpPdfGen.Core;

namespace SharpPdfGen.ConsoleApp;

/// <summary>
/// Sample console application demonstrating SharpPdfGen features.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("🚀 SharpPdfGen Sample Application");
        Console.WriteLine("==================================\n");

        try
        {
            // Create output directory
            var outputDir = "output";
            Directory.CreateDirectory(outputDir);

            // Run all demonstrations
            await DemonstrateBasicPdfCreation(outputDir);
            await DemonstrateAdvancedFormatting(outputDir);
            await DemonstrateTablesAndImages(outputDir);
            await DemonstrateHtmlToPdf(outputDir);
            await DemonstratePdfMerging(outputDir);
            await DemonstratePdfSplitting(outputDir);

            Console.WriteLine("\n✅ All demonstrations completed successfully!");
            Console.WriteLine($"📁 Check the '{outputDir}' folder for generated PDF files.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    /// <summary>
    /// Demonstrates basic PDF creation with text and headers/footers.
    /// </summary>
    static async Task DemonstrateBasicPdfCreation(string outputDir)
    {
        Console.WriteLine("📄 Creating basic PDF with text, headers, and footers...");

        using var document = PdfGenerator.CreateDocument(
            title: "Basic PDF Example",
            author: "SharpPdfGen",
            subject: "Demonstration of basic PDF creation",
            keywords: "pdf,generation,demo"
        );

        // Add first page
        var page1 = document.AddPage(PageSize.A4);
        
        // Add header
        await page1.AddHeaderAsync("SharpPdfGen Basic Example", TextStyle.Heading);
        
        // Add main content
        await page1.AddTextAsync("Welcome to SharpPdfGen!", 50, 120, TextStyle.Subheading);
        
        var bodyText = "This is a demonstration of the SharpPdfGen library. " +
                      "SharpPdfGen is a modern, high-performance PDF generation library " +
                      "for .NET with full async/await support.";
        
        await page1.AddTextAsync(bodyText, 50, 160, TextStyle.Default);
        
        // Add some styled text
        var blueStyle = new TextStyle
        {
            FontSize = 14,
            Color = Color.Blue,
            FontWeight = FontWeight.Bold
        };
        
        await page1.AddTextAsync("Key Features:", 50, 220, blueStyle);
        
        var features = new[]
        {
            "• Cross-platform compatibility",
            "• Async/await support",
            "• High performance with low memory footprint",
            "• Thread-safe operations",
            "• Clean, intuitive API"
        };
        
        var yPosition = 250.0;
        foreach (var feature in features)
        {
            await page1.AddTextAsync(feature, 70, yPosition);
            yPosition += 25;
        }
        
        // Add footer
        await page1.AddFooterAsync("Generated with SharpPdfGen | Page 1", 
            new TextStyle { FontSize = 10, Color = Color.Gray });

        // Save document
        var filePath = Path.Combine(outputDir, "basic-example.pdf");
        await document.SaveAsync(filePath);
        
        Console.WriteLine($"   ✓ Saved: {filePath}");
    }

    /// <summary>
    /// Demonstrates advanced text formatting and styles.
    /// </summary>
    static async Task DemonstrateAdvancedFormatting(string outputDir)
    {
        Console.WriteLine("🎨 Creating PDF with advanced formatting...");

        using var document = PdfGenerator.CreateDocument("Advanced Formatting Example");
        var page = document.AddPage();

        // Title
        var titleStyle = new TextStyle
        {
            FontFamily = "Arial",
            FontSize = 24,
            FontWeight = FontWeight.Bold,
            Color = Color.DarkBlue,
            Alignment = TextAlignment.Center
        };
        
        await page.AddTextAsync("Advanced Text Formatting", 50, 50, titleStyle);

        // Different font styles
        var yPos = 120.0;
        
        var styles = new[]
        {
            ("Normal Text", TextStyle.Default),
            ("Bold Text", new TextStyle { FontWeight = FontWeight.Bold }),
            ("Italic Text", new TextStyle { FontStyle = FontStyle.Italic }),
            ("Large Text", new TextStyle { FontSize = 18 }),
            ("Small Text", new TextStyle { FontSize = 8 }),
            ("Red Text", new TextStyle { Color = Color.Red }),
            ("Green Text", new TextStyle { Color = Color.Green }),
            ("Times New Roman", new TextStyle { FontFamily = "Times New Roman" })
        };

        foreach (var (text, style) in styles)
        {
            await page.AddTextAsync(text, 50, yPos, style);
            yPos += 30;
        }

        // Different alignments
        yPos += 50;
        await page.AddTextAsync("Left Aligned", 50, yPos, 
            new TextStyle { Alignment = TextAlignment.Left });
        
        await page.AddTextAsync("Center Aligned", 300, yPos + 30, 
            new TextStyle { Alignment = TextAlignment.Center });
        
        await page.AddTextAsync("Right Aligned", 550, yPos + 60, 
            new TextStyle { Alignment = TextAlignment.Right });

        var filePath = Path.Combine(outputDir, "advanced-formatting.pdf");
        await document.SaveAsync(filePath);
        
        Console.WriteLine($"   ✓ Saved: {filePath}");
    }

    /// <summary>
    /// Demonstrates tables and images.
    /// </summary>
    static async Task DemonstrateTablesAndImages(string outputDir)
    {
        Console.WriteLine("📊 Creating PDF with tables and images...");

        using var document = PdfGenerator.CreateDocument("Tables and Images Example");
        var page = document.AddPage();

        // Add title
        await page.AddHeaderAsync("Tables and Images Demo");

        // Create a table
        var table = new Table();
        
        // Set column widths
        table.ColumnWidths.AddRange(new[] { 150.0, 200.0, 150.0 });
        
        // Style the table
        table.Style = new TableStyle
        {
            BorderWidth = 1.5,
            BorderColor = Color.Black,
            CellPadding = 8,
            BackgroundColor = Color.LightGray,
            ShowBorders = true,
            TextStyle = new TextStyle { FontWeight = FontWeight.Bold }
        };

        // Add header row
        table.AddRow("Product", "Description", "Price");
        
        // Add data rows
        var products = new[]
        {
            ("SharpPdfGen Pro", "Advanced PDF generation library", "$49.99"),
            ("SharpPdfGen Enterprise", "Enterprise solution with support", "$199.99"),
            ("SharpPdfGen Cloud", "Cloud-based PDF service", "$29.99/month")
        };

        foreach (var (product, description, price) in products)
        {
            table.AddRow(product, description, price);
        }

        // Add table to page
        await page.AddTableAsync(table, 50, 120);

        // Add a simple image (placeholder)
        var yPosition = 120 + (table.Rows.Count * 25) + 50;
        
        try
        {
            // Create a simple test image
            var imageData = CreateTestImage();
            await page.AddImageAsync(imageData, 50, yPosition, 100, 60);
            await page.AddTextAsync("Sample Image", 160, yPosition + 25);
        }
        catch (Exception ex)
        {
            await page.AddTextAsync($"Image placeholder (Error: {ex.Message})", 50, yPosition);
        }

        var filePath = Path.Combine(outputDir, "tables-and-images.pdf");
        await document.SaveAsync(filePath);
        
        Console.WriteLine($"   ✓ Saved: {filePath}");
    }

    /// <summary>
    /// Demonstrates HTML to PDF conversion.
    /// </summary>
    static async Task DemonstrateHtmlToPdf(string outputDir)
    {
        Console.WriteLine("🌐 Converting HTML to PDF...");

        var html = @"
<!DOCTYPE html>
<html>
<head>
    <title>HTML to PDF Demo</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 40px; }
        h1 { color: #2c3e50; border-bottom: 2px solid #3498db; }
        h2 { color: #34495e; }
        .highlight { background-color: #f39c12; color: white; padding: 5px; }
        .info-box { border: 1px solid #bdc3c7; padding: 15px; margin: 10px 0; }
        ul { margin-left: 20px; }
    </style>
</head>
<body>
    <h1>HTML to PDF Conversion</h1>
    
    <h2>Overview</h2>
    <p>SharpPdfGen can convert <span class='highlight'>HTML content</span> directly to PDF format.</p>
    
    <div class='info-box'>
        <h3>Supported Features:</h3>
        <ul>
            <li>CSS styling</li>
            <li>Tables and lists</li>
            <li>Text formatting</li>
            <li>Colors and backgrounds</li>
        </ul>
    </div>
    
    <h2>Example Table</h2>
    <table border='1' style='border-collapse: collapse; width: 100%;'>
        <tr style='background-color: #ecf0f1;'>
            <th style='padding: 10px;'>Feature</th>
            <th style='padding: 10px;'>Support Level</th>
        </tr>
        <tr>
            <td style='padding: 8px;'>Text Formatting</td>
            <td style='padding: 8px;'>Full</td>
        </tr>
        <tr>
            <td style='padding: 8px;'>CSS Styling</td>
            <td style='padding: 8px;'>Partial</td>
        </tr>
        <tr>
            <td style='padding: 8px;'>Images</td>
            <td style='padding: 8px;'>Basic</td>
        </tr>
    </table>
    
    <p><strong>Note:</strong> This document was generated from HTML using SharpPdfGen.</p>
</body>
</html>";

        try
        {
            using var document = await PdfGenerator.FromHtmlAsync(html, PageSize.A4);
            
            var filePath = Path.Combine(outputDir, "html-to-pdf.pdf");
            await document.SaveAsync(filePath);
            
            Console.WriteLine($"   ✓ Saved: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ⚠️  HTML to PDF conversion failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Demonstrates PDF merging functionality.
    /// </summary>
    static async Task DemonstratePdfMerging(string outputDir)
    {
        Console.WriteLine("🔗 Demonstrating PDF merging...");

        // Create first document
        using var doc1 = PdfGenerator.CreateDocument("Document 1");
        var page1 = doc1.AddPage();
        await page1.AddHeaderAsync("First Document");
        await page1.AddTextAsync("This is the content of the first document.", 50, 120);

        // Create second document
        using var doc2 = PdfGenerator.CreateDocument("Document 2");
        var page2 = doc2.AddPage();
        await page2.AddHeaderAsync("Second Document");
        await page2.AddTextAsync("This is the content of the second document.", 50, 120);

        // Create third document
        using var doc3 = PdfGenerator.CreateDocument("Document 3");
        var page3 = doc3.AddPage();
        await page3.AddHeaderAsync("Third Document");
        await page3.AddTextAsync("This is the content of the third document.", 50, 120);

        // Merge documents
        var documents = new[] { doc1, doc2, doc3 };
        using var mergedDocument = await PdfGenerator.MergeDocumentsAsync(documents);
        
        // Set metadata for merged document
        mergedDocument.Title = "Merged Document";
        mergedDocument.Author = "SharpPdfGen";

        var filePath = Path.Combine(outputDir, "merged-document.pdf");
        await mergedDocument.SaveAsync(filePath);
        
        Console.WriteLine($"   ✓ Merged {documents.Length} documents into: {filePath}");
    }

    /// <summary>
    /// Demonstrates PDF splitting functionality.
    /// </summary>
    static async Task DemonstratePdfSplitting(string outputDir)
    {
        Console.WriteLine("✂️ Demonstrating PDF splitting...");

        // Create a multi-page document
        using var document = PdfGenerator.CreateDocument("Multi-page Document");
        
        for (int i = 1; i <= 3; i++)
        {
            var page = document.AddPage();
            await page.AddHeaderAsync($"Page {i} of 3");
            await page.AddTextAsync($"This is the content of page {i}.", 50, 120);
            await page.AddTextAsync($"Each page will be split into separate documents.", 50, 160);
            await page.AddFooterAsync($"Page {i}");
        }

        // Split the document
        var splitDocuments = await PdfGenerator.SplitDocumentAsync(document);
        
        // Save each split document
        for (int i = 0; i < splitDocuments.Count; i++)
        {
            var splitDoc = splitDocuments[i];
            splitDoc.Title = $"Split Document {i + 1}";
            
            var filePath = Path.Combine(outputDir, $"split-document-{i + 1}.pdf");
            await splitDoc.SaveAsync(filePath);
            
            Console.WriteLine($"   ✓ Saved split document: {filePath}");
            
            splitDoc.Dispose();
        }
    }

    /// <summary>
    /// Creates a simple test image as byte array.
    /// </summary>
    static byte[] CreateTestImage()
    {
        // This is a minimal PNG file (1x1 transparent pixel)
        return new byte[]
        {
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A,
            0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52,
            0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
            0x08, 0x06, 0x00, 0x00, 0x00, 0x1F, 0x15, 0xC4,
            0x89, 0x00, 0x00, 0x00, 0x0B, 0x49, 0x44, 0x41,
            0x54, 0x78, 0x9C, 0x63, 0x00, 0x01, 0x00, 0x00,
            0x05, 0x00, 0x01, 0x0D, 0x0A, 0x2D, 0xB4, 0x00,
            0x00, 0x00, 0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE,
            0x42, 0x60, 0x82
        };
    }
}
