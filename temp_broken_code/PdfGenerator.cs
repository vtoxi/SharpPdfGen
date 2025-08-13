using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SharpPdfGen.Core;
using SharpPdfGen.Implementation;

namespace SharpPdfGen
{
    /// <summary>
    /// Main entry point for PDF generation operations.
    /// This class provides static methods for creating and manipulating PDF documents.
    /// </summary>
    public static class PdfGenerator
    {
        /// <summary>
        /// Creates a new PDF document.
        /// </summary>
        /// <param name="title">The document title.</param>
        /// <param name="author">The document author.</param>
        /// <param name="subject">The document subject.</param>
        /// <param name="keywords">The document keywords.</param>
        /// <returns>A new PDF document instance.</returns>
        public static IPdfDocument CreateDocument(string? title = null, string? author = null, string? subject = null, string? keywords = null)
        {
            var document = new PdfDocument();
            
            if (!string.IsNullOrEmpty(title))
                document.Title = title;
            if (!string.IsNullOrEmpty(author))
                document.Author = author;
            if (!string.IsNullOrEmpty(subject))
                document.Subject = subject;
            if (!string.IsNullOrEmpty(keywords))
                document.Keywords = keywords;

            return document;
        }

        /// <summary>
        /// Loads a PDF document from a file asynchronously.
        /// </summary>
        /// <param name="filePath">The path to the PDF file.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The loaded PDF document.</returns>
        public static async Task<IPdfDocument> LoadDocumentAsync(string filePath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
            return await LoadDocumentAsync(fileStream, cancellationToken);
        }

        /// <summary>
        /// Loads a PDF document from a stream asynchronously.
        /// </summary>
        /// <param name="stream">The stream containing the PDF data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The loaded PDF document.</returns>
        public static async Task<IPdfDocument> LoadDocumentAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            return await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return (IPdfDocument)new PdfDocument(stream);
            }, cancellationToken);
        }

        /// <summary>
        /// Loads a PDF document from byte array asynchronously.
        /// </summary>
        /// <param name="data">The PDF data as byte array.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The loaded PDF document.</returns>
        public static async Task<IPdfDocument> LoadDocumentAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            using var stream = new MemoryStream(data);
            return await LoadDocumentAsync(stream, cancellationToken);
        }

        /// <summary>
        /// Merges multiple PDF documents into a single document asynchronously.
        /// </summary>
        /// <param name="documents">The documents to merge.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A new document containing all pages from the input documents.</returns>
        public static async Task<IPdfDocument> MergeDocumentsAsync(IEnumerable<IPdfDocument> documents, CancellationToken cancellationToken = default)
        {
            if (documents == null)
                throw new ArgumentNullException(nameof(documents));

            var mergedDocument = CreateDocument();
            
            foreach (var document in documents)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await mergedDocument.MergeAsync(document, cancellationToken);
            }

            return mergedDocument;
        }

        /// <summary>
        /// Splits a PDF document into multiple documents, one per page.
        /// </summary>
        /// <param name="document">The document to split.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A collection of single-page documents.</returns>
        public static async Task<IList<IPdfDocument>> SplitDocumentAsync(IPdfDocument document, CancellationToken cancellationToken = default)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            return await Task.Run(() =>
            {
                var result = new List<IPdfDocument>();
                
                for (int i = 0; i < document.PageCount; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    var singlePageDoc = CreateDocument(document.Title, document.Author, document.Subject, document.Keywords);
                    
                    // Note: This is a simplified implementation.
                    // In the real implementation, we would copy the actual page content.
                    var newPage = singlePageDoc.AddPage(document.Pages[i].PageSize);
                    
                    result.Add(singlePageDoc);
                }
                
                return result;
            }, cancellationToken);
        }

        /// <summary>
        /// Generates a PDF from HTML content asynchronously.
        /// </summary>
        /// <param name="htmlContent">The HTML content to convert.</param>
        /// <param name="pageSize">The page size for the PDF. If null, uses A4.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A PDF document containing the rendered HTML.</returns>
        public static async Task<IPdfDocument> FromHtmlAsync(string htmlContent, PageSize? pageSize = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(htmlContent))
                throw new ArgumentException("HTML content cannot be null or empty.", nameof(htmlContent));

            return await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return (IPdfDocument)PdfDocument.FromHtml(htmlContent, pageSize ?? PageSize.A4);
            }, cancellationToken);
        }

        /// <summary>
        /// Converts page size enum to dimensions in points.
        /// </summary>
        /// <param name="pageSize">The page size.</param>
        /// <returns>The page dimensions in points (width, height).</returns>
        public static (double Width, double Height) GetPageDimensions(PageSize pageSize)
        {
            return pageSize switch
            {
                PageSize.A4 => (595.276, 841.890),      // 210 × 297 mm
                PageSize.A3 => (841.890, 1190.551),     // 297 × 420 mm
                PageSize.A5 => (419.528, 595.276),      // 148 × 210 mm
                PageSize.Letter => (612, 792),          // 8.5 × 11 inches
                PageSize.Legal => (612, 1008),          // 8.5 × 14 inches
                PageSize.Tabloid => (792, 1224),        // 11 × 17 inches
                _ => (595.276, 841.890)                  // Default to A4
            };
        }
    }
}
