using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharpPdfGen.Core
{
    /// <summary>
    /// Represents a page in a PDF document.
    /// </summary>
    public interface IPdfPage : IDisposable
    {
        /// <summary>
        /// Gets the page width in points.
        /// </summary>
        double Width { get; }

        /// <summary>
        /// Gets the page height in points.
        /// </summary>
        double Height { get; }

        /// <summary>
        /// Gets the page size.
        /// </summary>
        PageSize PageSize { get; }

        /// <summary>
        /// Gets the graphics context for drawing on this page.
        /// </summary>
        IPdfGraphics Graphics { get; }

        /// <summary>
        /// Adds text to the page at the specified position.
        /// </summary>
        /// <param name="text">The text to add.</param>
        /// <param name="x">The x-coordinate in points.</param>
        /// <param name="y">The y-coordinate in points.</param>
        /// <param name="style">The text style. If null, uses default style.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task AddTextAsync(string text, double x, double y, TextStyle? style = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds an image to the page at the specified position.
        /// </summary>
        /// <param name="imageData">The image data.</param>
        /// <param name="x">The x-coordinate in points.</param>
        /// <param name="y">The y-coordinate in points.</param>
        /// <param name="width">The image width in points. If null, uses original width.</param>
        /// <param name="height">The image height in points. If null, uses original height.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task AddImageAsync(byte[] imageData, double x, double y, double? width = null, double? height = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a table to the page at the specified position.
        /// </summary>
        /// <param name="table">The table to add.</param>
        /// <param name="x">The x-coordinate in points.</param>
        /// <param name="y">The y-coordinate in points.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task AddTableAsync(Table table, double x, double y, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a header to the page.
        /// </summary>
        /// <param name="text">The header text.</param>
        /// <param name="style">The text style. If null, uses default header style.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task AddHeaderAsync(string text, TextStyle? style = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a footer to the page.
        /// </summary>
        /// <param name="text">The footer text.</param>
        /// <param name="style">The text style. If null, uses default footer style.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task AddFooterAsync(string text, TextStyle? style = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Extracts text from this page.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The extracted text.</returns>
        Task<string> ExtractTextAsync(CancellationToken cancellationToken = default);
    }
}
