using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;
using SharpPdfGen.Core;

namespace SharpPdfGen
{
    /// <summary>
    /// Simplified PDF generator that compiles and works with basic functionality.
    /// This is a working implementation while the full implementation is being perfected.
    /// </summary>
    public static class SimplePdfGenerator
    {
        /// <summary>
        /// Creates a simple PDF document with text.
        /// </summary>
        /// <param name="title">Document title</param>
        /// <param name="content">Text content</param>
        /// <param name="outputPath">Output file path</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task CreateSimplePdfAsync(string title, string content, string outputPath, CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                using var document = new PdfDocument();
                document.Info.Title = title ?? "SharpPdfGen Document";
                document.Info.Creator = "SharpPdfGen";
                document.Info.CreationDate = DateTime.Now;

                var page = document.AddPage();
                var graphics = XGraphics.FromPdfPage(page);
                var font = new XFont("Arial", 12, XFontStyle.Regular);

                // Add title
                if (!string.IsNullOrEmpty(title))
                {
                    var titleFont = new XFont("Arial", 18, XFontStyle.Bold);
                    graphics.DrawString(title, titleFont, XBrushes.Black, 50, 50);
                }

                // Add content
                if (!string.IsNullOrEmpty(content))
                {
                    var rect = new XRect(50, 100, page.Width - 100, page.Height - 150);
                    graphics.DrawString(content, font, XBrushes.Black, rect, XStringFormats.TopLeft);
                }

                graphics.Dispose();
                document.Save(outputPath);
            }, cancellationToken);
        }

        /// <summary>
        /// Creates a PDF with multiple pages of text.
        /// </summary>
        /// <param name="title">Document title</param>
        /// <param name="pages">Array of page content</param>
        /// <param name="outputPath">Output file path</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task CreateMultiPagePdfAsync(string title, string[] pages, string outputPath, CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                using var document = new PdfDocument();
                document.Info.Title = title ?? "SharpPdfGen Document";
                document.Info.Creator = "SharpPdfGen";
                document.Info.CreationDate = DateTime.Now;

                var font = new XFont("Arial", 12, XFontStyle.Regular);
                var titleFont = new XFont("Arial", 18, XFontStyle.Bold);

                for (int i = 0; i < pages.Length; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var page = document.AddPage();
                    var graphics = XGraphics.FromPdfPage(page);

                    // Add page number
                    graphics.DrawString($"Page {i + 1} of {pages.Length}", 
                        new XFont("Arial", 10, XFontStyle.Regular), 
                        XBrushes.Gray, 
                        page.Width - 100, page.Height - 30);

                    // Add title on first page
                    if (i == 0 && !string.IsNullOrEmpty(title))
                    {
                        graphics.DrawString(title, titleFont, XBrushes.Black, 50, 50);
                    }

                    // Add content
                    var yStart = i == 0 && !string.IsNullOrEmpty(title) ? 100 : 50;
                    var rect = new XRect(50, yStart, page.Width - 100, page.Height - yStart - 50);
                    graphics.DrawString(pages[i] ?? string.Empty, font, XBrushes.Black, rect, XStringFormats.TopLeft);

                    graphics.Dispose();
                }

                document.Save(outputPath);
            }, cancellationToken);
        }

        /// <summary>
        /// Merges multiple PDF files into one.
        /// </summary>
        /// <param name="inputFiles">Array of input PDF file paths</param>
        /// <param name="outputPath">Output file path</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task MergePdfsAsync(string[] inputFiles, string outputPath, CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                using var outputDocument = new PdfDocument();
                outputDocument.Info.Title = "Merged PDF";
                outputDocument.Info.Creator = "SharpPdfGen";

                foreach (var file in inputFiles)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (File.Exists(file))
                    {
                        using var inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);
                        for (int i = 0; i < inputDocument.PageCount; i++)
                        {
                            outputDocument.AddPage(inputDocument.Pages[i]);
                        }
                    }
                }

                outputDocument.Save(outputPath);
            }, cancellationToken);
        }
    }
}
