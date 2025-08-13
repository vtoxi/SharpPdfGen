using System.Drawing;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using PdfSharp.Pdf;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SharpPdfGen;
using SharpPdfGen.Core;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace SharpPdfGen.Benchmarks;

/// <summary>
/// Benchmarks for basic PDF creation performance.
/// </summary>
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class PdfCreationBenchmarks
{
    private const int PageCount = 10;
    private const string SampleText = "This is a sample text for benchmarking PDF generation performance. " +
                                     "We are comparing different PDF libraries to see which one performs better.";

    [GlobalSetup]
    public void Setup()
    {
        // Configure QuestPDF license for benchmarking
        QuestPDF.Settings.License = LicenseType.Community;
    }

    [Benchmark(Baseline = true)]
    public async Task<byte[]> SharpPdfGen_CreateDocument()
    {
        using var document = PdfGenerator.CreateDocument("Benchmark Test");
        
        for (int i = 0; i < PageCount; i++)
        {
            var page = document.AddPage();
            await page.AddTextAsync($"Page {i + 1}", 50, 50, TextStyle.Heading);
            await page.AddTextAsync(SampleText, 50, 100);
            await page.AddTextAsync($"This is page {i + 1} of {PageCount}", 50, 150);
        }

        return await document.ToByteArrayAsync();
    }

    [Benchmark]
    public byte[] PdfSharp_CreateDocument()
    {
        using var document = new PdfDocument();
        document.Info.Title = "Benchmark Test";

        for (int i = 0; i < PageCount; i++)
        {
            var page = document.AddPage();
            var graphics = PdfSharp.Drawing.XGraphics.FromPdfPage(page);
            var font = new PdfSharp.Drawing.XFont("Arial", 12);
            var titleFont = new PdfSharp.Drawing.XFont("Arial", 16, PdfSharp.Drawing.XFontStyleEx.Bold);

            graphics.DrawString($"Page {i + 1}", titleFont, PdfSharp.Drawing.XBrushes.Black, 50, 50);
            graphics.DrawString(SampleText, font, PdfSharp.Drawing.XBrushes.Black, 
                new PdfSharp.Drawing.XRect(50, 100, 500, 200), PdfSharp.Drawing.XStringFormats.TopLeft);
            graphics.DrawString($"This is page {i + 1} of {PageCount}", font, PdfSharp.Drawing.XBrushes.Black, 50, 150);
            
            graphics.Dispose();
        }

        using var stream = new MemoryStream();
        document.Save(stream);
        return stream.ToArray();
    }

    [Benchmark]
    public byte[] QuestPDF_CreateDocument()
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(50);

                page.Content().Column(column =>
                {
                    for (int i = 0; i < PageCount; i++)
                    {
                        if (i > 0)
                            column.PageBreak();

                        column.Item().Text($"Page {i + 1}").FontSize(16).Bold();
                        column.Item().PaddingTop(20).Text(SampleText);
                        column.Item().PaddingTop(10).Text($"This is page {i + 1} of {PageCount}");
                    }
                });
            });
        });

        return document.GeneratePdf();
    }

    [Benchmark]
    public byte[] iText7_CreateDocument()
    {
        using var stream = new MemoryStream();
        using var writer = new PdfWriter(stream);
        using var pdf = new iText.Kernel.Pdf.PdfDocument(writer);
        using var document = new Document(pdf);

        for (int i = 0; i < PageCount; i++)
        {
            if (i > 0)
                document.Add(new AreaBreak());

            document.Add(new Paragraph($"Page {i + 1}")
                .SetFontSize(16)
                .SetBold());
            
            document.Add(new Paragraph(SampleText));
            document.Add(new Paragraph($"This is page {i + 1} of {PageCount}"));
        }

        document.Close();
        return stream.ToArray();
    }
}
