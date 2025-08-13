using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SharpPdfGen.Core
{
    /// <summary>
    /// Represents a PDF document that can be created, modified, and saved.
    /// </summary>
    public interface IPdfDocument : IDisposable
    {
        /// <summary>
        /// Gets the number of pages in the document.
        /// </summary>
        int PageCount { get; }

        /// <summary>
        /// Gets or sets the document title.
        /// </summary>
        string? Title { get; set; }

        /// <summary>
        /// Gets or sets the document author.
        /// </summary>
        string? Author { get; set; }

        /// <summary>
        /// Gets or sets the document subject.
        /// </summary>
        string? Subject { get; set; }

        /// <summary>
        /// Gets or sets the document keywords.
        /// </summary>
        string? Keywords { get; set; }

        /// <summary>
        /// Gets the pages in the document.
        /// </summary>
        IReadOnlyList<IPdfPage> Pages { get; }

        /// <summary>
        /// Adds a new page to the document.
        /// </summary>
        /// <param name="pageSize">The page size. If null, uses A4.</param>
        /// <returns>The newly created page.</returns>
        IPdfPage AddPage(PageSize? pageSize = null);

        /// <summary>
        /// Removes a page from the document.
        /// </summary>
        /// <param name="page">The page to remove.</param>
        void RemovePage(IPdfPage page);

        /// <summary>
        /// Removes a page at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the page to remove.</param>
        void RemovePageAt(int index);

        /// <summary>
        /// Saves the document to a stream.
        /// </summary>
        /// <param name="stream">The stream to save to.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task SaveAsync(Stream stream, CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves the document to a file.
        /// </summary>
        /// <param name="filePath">The file path to save to.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task SaveAsync(string filePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the document as a byte array.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The PDF document as a byte array.</returns>
        Task<byte[]> ToByteArrayAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Merges another PDF document into this document.
        /// </summary>
        /// <param name="otherDocument">The document to merge.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task MergeAsync(IPdfDocument otherDocument, CancellationToken cancellationToken = default);

        /// <summary>
        /// Extracts text from all pages in the document.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The extracted text.</returns>
        Task<string> ExtractTextAsync(CancellationToken cancellationToken = default);
    }
}
