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
/// Benchmarks for text rendering performance.
/// </summary>
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class TextRenderingBenchmarks
{
    private const int TextBlockCount = 100;
    private readonly string[] _textBlocks;

    public TextRenderingBenchmarks()
    {
        _textBlocks = Enumerable.Range(1, TextBlockCount)
            .Select(i => $"Text block {i}: Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                        $"Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.")
            .ToArray();
    }

    [GlobalSetup]
    public void Setup()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    [Benchmark(Baseline = true)]
    public async Task<byte[]> SharpPdfGen_RenderText()
    {
        using var document = PdfGenerator.CreateDocument("Text Rendering Benchmark");
        var page = document.AddPage();

        var yPosition = 50.0;
        foreach (var text in _textBlocks)
        {
            await page.AddTextAsync(text, 50, yPosition);
            yPosition += 15;

            // Add new page if needed
            if (yPosition > 750)
            {
                page = document.AddPage();
                yPosition = 50;
            }
        }

        return await document.ToByteArrayAsync();
    }

    [Benchmark]
    public byte[] PdfSharp_RenderText()
    {
        using var document = new PdfDocument();
        document.Info.Title = "Text Rendering Benchmark";

        var page = document.AddPage();
        var graphics = PdfSharp.Drawing.XGraphics.FromPdfPage(page);
        var font = new PdfSharp.Drawing.XFont("Arial", 10);

        var yPosition = 50.0;
        foreach (var text in _textBlocks)
        {
            graphics.DrawString(text, font, PdfSharp.Drawing.XBrushes.Black, 50, yPosition);
            yPosition += 15;

            // Add new page if needed
            if (yPosition > 750)
            {
                graphics.Dispose();
                page = document.AddPage();
                graphics = PdfSharp.Drawing.XGraphics.FromPdfPage(page);
                yPosition = 50;
            }
        }

        graphics.Dispose();

        using var stream = new MemoryStream();
        document.Save(stream);
        return stream.ToArray();
    }

    [Benchmark]
    public byte[] QuestPDF_RenderText()
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(50);

                page.Content().Column(column =>
                {
                    foreach (var text in _textBlocks)
                    {
                        column.Item().Text(text).FontSize(10);
                    }
                });
            });
        });

        return document.GeneratePdf();
    }

    [Benchmark]
    public byte[] iText7_RenderText()
    {
        using var stream = new MemoryStream();
        using var writer = new PdfWriter(stream);
        using var pdf = new iText.Kernel.Pdf.PdfDocument(writer);
        using var document = new Document(pdf);

        foreach (var text in _textBlocks)
        {
            document.Add(new Paragraph(text).SetFontSize(10));
        }

        document.Close();
        return stream.ToArray();
    }
}
