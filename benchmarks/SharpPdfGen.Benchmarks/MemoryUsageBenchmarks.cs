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
/// Benchmarks for memory usage during PDF generation.
/// </summary>
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class MemoryUsageBenchmarks
{
    private const int LargePageCount = 50;
    private const int TextBlocksPerPage = 50;
    private readonly string _sampleText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                                         "Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. " +
                                         "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris.";

    [GlobalSetup]
    public void Setup()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    [Benchmark(Baseline = true)]
    public async Task<int> SharpPdfGen_LargeDocument()
    {
        using var document = PdfGenerator.CreateDocument("Large Document Benchmark");
        
        for (int pageNum = 0; pageNum < LargePageCount; pageNum++)
        {
            var page = document.AddPage();
            await page.AddHeaderAsync($"Page {pageNum + 1} of {LargePageCount}");

            var yPosition = 80.0;
            for (int textBlock = 0; textBlock < TextBlocksPerPage; textBlock++)
            {
                await page.AddTextAsync($"Block {textBlock + 1}: {_sampleText}", 50, yPosition);
                yPosition += 12;
            }

            await page.AddFooterAsync($"Footer for page {pageNum + 1}");
        }

        var bytes = await document.ToByteArrayAsync();
        return bytes.Length;
    }

    [Benchmark]
    public int PdfSharp_LargeDocument()
    {
        using var document = new PdfDocument();
        document.Info.Title = "Large Document Benchmark";

        for (int pageNum = 0; pageNum < LargePageCount; pageNum++)
        {
            var page = document.AddPage();
            var graphics = PdfSharp.Drawing.XGraphics.FromPdfPage(page);
            var font = new PdfSharp.Drawing.XFont("Arial", 10);
            var headerFont = new PdfSharp.Drawing.XFont("Arial", 12, PdfSharp.Drawing.XFontStyleEx.Bold);

            // Header
            graphics.DrawString($"Page {pageNum + 1} of {LargePageCount}", headerFont, 
                PdfSharp.Drawing.XBrushes.Black, 50, 50);

            // Content
            var yPosition = 80.0;
            for (int textBlock = 0; textBlock < TextBlocksPerPage; textBlock++)
            {
                graphics.DrawString($"Block {textBlock + 1}: {_sampleText}", font, 
                    PdfSharp.Drawing.XBrushes.Black, 50, yPosition);
                yPosition += 12;
            }

            // Footer
            graphics.DrawString($"Footer for page {pageNum + 1}", font, 
                PdfSharp.Drawing.XBrushes.Black, 50, page.Height - 50);

            graphics.Dispose();
        }

        using var stream = new MemoryStream();
        document.Save(stream);
        return (int)stream.Length;
    }

    [Benchmark]
    public int QuestPDF_LargeDocument()
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(50);

                page.Header().Text("Large Document Benchmark").FontSize(12).Bold();

                page.Content().Column(column =>
                {
                    for (int pageNum = 0; pageNum < LargePageCount; pageNum++)
                    {
                        if (pageNum > 0)
                            column.PageBreak();

                        column.Item().Text($"Page {pageNum + 1} of {LargePageCount}").FontSize(12).Bold();

                        for (int textBlock = 0; textBlock < TextBlocksPerPage; textBlock++)
                        {
                            column.Item().Text($"Block {textBlock + 1}: {_sampleText}").FontSize(10);
                        }
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                    x.Span(" of ");
                    x.TotalPages();
                });
            });
        });

        var bytes = document.GeneratePdf();
        return bytes.Length;
    }

    [Benchmark]
    public int iText7_LargeDocument()
    {
        using var stream = new MemoryStream();
        using var writer = new PdfWriter(stream);
        using var pdf = new iText.Kernel.Pdf.PdfDocument(writer);
        using var document = new Document(pdf);

        for (int pageNum = 0; pageNum < LargePageCount; pageNum++)
        {
            if (pageNum > 0)
                document.Add(new AreaBreak());

            document.Add(new Paragraph($"Page {pageNum + 1} of {LargePageCount}")
                .SetFontSize(12)
                .SetBold());

            for (int textBlock = 0; textBlock < TextBlocksPerPage; textBlock++)
            {
                document.Add(new Paragraph($"Block {textBlock + 1}: {_sampleText}")
                    .SetFontSize(10));
            }
        }

        document.Close();
        return (int)stream.Length;
    }

    [Benchmark]
    public async Task<int> SharpPdfGen_MultipleSmallDocuments()
    {
        var totalSize = 0;
        const int documentCount = 20;

        for (int i = 0; i < documentCount; i++)
        {
            using var document = PdfGenerator.CreateDocument($"Document {i + 1}");
            var page = document.AddPage();
            
            await page.AddTextAsync($"This is document {i + 1} of {documentCount}", 50, 100);
            await page.AddTextAsync(_sampleText, 50, 130);

            var bytes = await document.ToByteArrayAsync();
            totalSize += bytes.Length;
        }

        return totalSize;
    }

    [Benchmark]
    public int PdfSharp_MultipleSmallDocuments()
    {
        var totalSize = 0;
        const int documentCount = 20;

        for (int i = 0; i < documentCount; i++)
        {
            using var document = new PdfDocument();
            document.Info.Title = $"Document {i + 1}";

            var page = document.AddPage();
            var graphics = PdfSharp.Drawing.XGraphics.FromPdfPage(page);
            var font = new PdfSharp.Drawing.XFont("Arial", 10);

            graphics.DrawString($"This is document {i + 1} of {documentCount}", font, 
                PdfSharp.Drawing.XBrushes.Black, 50, 100);
            graphics.DrawString(_sampleText, font, PdfSharp.Drawing.XBrushes.Black, 50, 130);

            graphics.Dispose();

            using var stream = new MemoryStream();
            document.Save(stream);
            totalSize += (int)stream.Length;
        }

        return totalSize;
    }
}
