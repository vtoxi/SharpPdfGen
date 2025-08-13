using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using SharpPdfGen.Core;

namespace SharpPdfGen.Implementation
{
    /// <summary>
    /// Implementation of IPdfDocument using PdfSharp.
    /// </summary>
    internal class PdfDocument : IPdfDocument
    {
        private readonly PdfDocument _document;
        private readonly List<PdfPage> _pages;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the PdfDocument class.
        /// </summary>
        public PdfDocument()
        {
            _document = new PdfDocument();
            _pages = new List<PdfPage>();
            SetupDocument();
        }

        /// <summary>
        /// Initializes a new instance of the PdfDocument class from a stream.
        /// </summary>
        /// <param name="stream">The stream containing PDF data.</param>
        public PdfDocument(Stream stream)
        {
            _document = PdfReader.Open(stream, PdfDocumentOpenMode.Modify);
            _pages = _document.Pages.Cast<PdfPage>().ToList();
            ExtractMetadata();
        }

        /// <summary>
        /// Gets the number of pages in the document.
        /// </summary>
        public int PageCount => _document.PageCount;

        /// <summary>
        /// Gets or sets the document title.
        /// </summary>
        public string? Title
        {
            get => _document.Info.Title;
            set => _document.Info.Title = value ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the document author.
        /// </summary>
        public string? Author
        {
            get => _document.Info.Author;
            set => _document.Info.Author = value ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the document subject.
        /// </summary>
        public string? Subject
        {
            get => _document.Info.Subject;
            set => _document.Info.Subject = value ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the document keywords.
        /// </summary>
        public string? Keywords
        {
            get => _document.Info.Keywords;
            set => _document.Info.Keywords = value ?? string.Empty;
        }

        /// <summary>
        /// Gets the pages in the document.
        /// </summary>
        public IReadOnlyList<IPdfPage> Pages => _pages.Select(p => new PdfPage(p)).ToList().AsReadOnly();

        /// <summary>
        /// Adds a new page to the document.
        /// </summary>
        /// <param name="pageSize">The page size. If null, uses A4.</param>
        /// <returns>The newly created page.</returns>
        public IPdfPage AddPage(Core.PageSize? pageSize = null)
        {
            ThrowIfDisposed();

            var page = _document.AddPage();
            var size = pageSize ?? Core.PageSize.A4;
            var dimensions = PdfGenerator.GetPageDimensions(size);
            
            page.Width = dimensions.Width;
            page.Height = dimensions.Height;
            
            _pages.Add(page);
            return new PdfPage(page);
        }

        /// <summary>
        /// Removes a page from the document.
        /// </summary>
        /// <param name="page">The page to remove.</param>
        public void RemovePage(IPdfPage page)
        {
            ThrowIfDisposed();

            if (page is PdfPage pdfPage)
            {
                var index = _pages.IndexOf(pdfPage.InternalPage);
                if (index >= 0)
                {
                    RemovePageAt(index);
                }
            }
        }

        /// <summary>
        /// Removes a page at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the page to remove.</param>
        public void RemovePageAt(int index)
        {
            ThrowIfDisposed();

            if (index < 0 || index >= _pages.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            _document.Pages.RemoveAt(index);
            _pages.RemoveAt(index);
        }

        /// <summary>
        /// Saves the document to a stream.
        /// </summary>
        /// <param name="stream">The stream to save to.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task SaveAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                _document.Save(stream);
            }, cancellationToken);
        }

        /// <summary>
        /// Saves the document to a file.
        /// </summary>
        /// <param name="filePath">The file path to save to.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task SaveAsync(string filePath, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                _document.Save(filePath);
            }, cancellationToken);
        }

        /// <summary>
        /// Gets the document as a byte array.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The PDF document as a byte array.</returns>
        public async Task<byte[]> ToByteArrayAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            return await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                using var stream = new MemoryStream();
                _document.Save(stream);
                return stream.ToArray();
            }, cancellationToken);
        }

        /// <summary>
        /// Merges another PDF document into this document.
        /// </summary>
        /// <param name="otherDocument">The document to merge.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task MergeAsync(IPdfDocument otherDocument, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (otherDocument == null)
                throw new ArgumentNullException(nameof(otherDocument));

            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (otherDocument is PdfDocument pdfDoc)
                {
                    foreach (var page in pdfDoc._document.Pages.Cast<PdfSharp.Pdf.PdfPage>())
                    {
                        var importedPage = _document.AddPage(page);
                        _pages.Add(importedPage);
                    }
                }
            }, cancellationToken);
        }

        /// <summary>
        /// Extracts text from all pages in the document.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The extracted text.</returns>
        public async Task<string> ExtractTextAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            return await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                // Note: Text extraction is complex and would require additional libraries
                // or custom implementation. For this demo, we'll return a placeholder.
                return "Text extraction functionality - would be implemented with additional libraries";
            }, cancellationToken);
        }

        /// <summary>
        /// Creates a PDF document from HTML content.
        /// </summary>
        /// <param name="html">The HTML content.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>A new PDF document.</returns>
        public static PdfDocument FromHtml(string html, Core.PageSize pageSize)
        {
            var document = new PdfDocument();
            var pdfDoc = PdfGenerator.GeneratePdf(html, PdfSharp.PageSize.A4);
            
            // Copy pages from generated document
            foreach (var page in pdfDoc.Pages.Cast<PdfSharp.Pdf.PdfPage>())
            {
                var importedPage = document._document.AddPage(page);
                document._pages.Add(importedPage);
            }
            
            pdfDoc.Dispose();
            return document;
        }

        /// <summary>
        /// Sets up the document with default properties.
        /// </summary>
        private void SetupDocument()
        {
            _document.Info.Title = "SharpPdfGen Document";
            _document.Info.Creator = "SharpPdfGen";
            _document.Info.CreationDate = DateTime.Now;
        }

        /// <summary>
        /// Extracts metadata from an existing document.
        /// </summary>
        private void ExtractMetadata()
        {
            // Metadata is already available through the Info property
        }

        /// <summary>
        /// Throws if the object has been disposed.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(PdfDocument));
        }

        /// <summary>
        /// Disposes the document and releases resources.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _document?.Dispose();
                _disposed = true;
            }
        }
    }
}
