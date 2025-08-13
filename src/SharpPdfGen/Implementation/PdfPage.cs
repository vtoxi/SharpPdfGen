using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using SharpPdfGen.Core;

namespace SharpPdfGen.Implementation
{
    /// <summary>
    /// Implementation of IPdfPage using PdfSharp.
    /// </summary>
    internal class PdfPage : IPdfPage
    {
        private readonly PdfSharp.Pdf.PdfPage _page;
        private readonly XGraphics _graphics;
        private readonly PdfGraphics _pdfGraphics;
        private bool _disposed;

        /// <summary>
        /// Gets the internal PdfSharp page.
        /// </summary>
        internal PdfSharp.Pdf.PdfPage InternalPage => _page;

        /// <summary>
        /// Initializes a new instance of the PdfPage class.
        /// </summary>
        /// <param name="page">The PdfSharp page.</param>
        public PdfPage(PdfSharp.Pdf.PdfPage page)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
            _graphics = XGraphics.FromPdfPage(_page);
            _pdfGraphics = new PdfGraphics(_graphics);
        }

        /// <summary>
        /// Gets the page width in points.
        /// </summary>
        public double Width => _page.Width;

        /// <summary>
        /// Gets the page height in points.
        /// </summary>
        public double Height => _page.Height;

        /// <summary>
        /// Gets the page size.
        /// </summary>
        public Core.PageSize PageSize
        {
            get
            {
                var width = Width;
                var height = Height;

                // Match dimensions to known page sizes (with some tolerance)
                const double tolerance = 1.0;

                var a4 = PdfGenerator.GetPageDimensions(Core.PageSize.A4);
                if (Math.Abs(width - a4.Width) < tolerance && Math.Abs(height - a4.Height) < tolerance)
                    return Core.PageSize.A4;

                var a3 = PdfGenerator.GetPageDimensions(Core.PageSize.A3);
                if (Math.Abs(width - a3.Width) < tolerance && Math.Abs(height - a3.Height) < tolerance)
                    return Core.PageSize.A3;

                var a5 = PdfGenerator.GetPageDimensions(Core.PageSize.A5);
                if (Math.Abs(width - a5.Width) < tolerance && Math.Abs(height - a5.Height) < tolerance)
                    return Core.PageSize.A5;

                var letter = PdfGenerator.GetPageDimensions(Core.PageSize.Letter);
                if (Math.Abs(width - letter.Width) < tolerance && Math.Abs(height - letter.Height) < tolerance)
                    return Core.PageSize.Letter;

                var legal = PdfGenerator.GetPageDimensions(Core.PageSize.Legal);
                if (Math.Abs(width - legal.Width) < tolerance && Math.Abs(height - legal.Height) < tolerance)
                    return Core.PageSize.Legal;

                var tabloid = PdfGenerator.GetPageDimensions(Core.PageSize.Tabloid);
                if (Math.Abs(width - tabloid.Width) < tolerance && Math.Abs(height - tabloid.Height) < tolerance)
                    return Core.PageSize.Tabloid;

                return Core.PageSize.Custom;
            }
        }

        /// <summary>
        /// Gets the graphics context for drawing on this page.
        /// </summary>
        public IPdfGraphics Graphics => _pdfGraphics;

        /// <summary>
        /// Adds text to the page at the specified position.
        /// </summary>
        /// <param name="text">The text to add.</param>
        /// <param name="x">The x-coordinate in points.</param>
        /// <param name="y">The y-coordinate in points.</param>
        /// <param name="style">The text style. If null, uses default style.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task AddTextAsync(string text, double x, double y, TextStyle? style = null, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(text))
                return;

            var textStyle = style ?? TextStyle.Default;
            await _pdfGraphics.DrawTextAsync(text, x, y, textStyle, cancellationToken);
        }

        /// <summary>
        /// Adds an image to the page at the specified position.
        /// </summary>
        /// <param name="imageData">The image data.</param>
        /// <param name="x">The x-coordinate in points.</param>
        /// <param name="y">The y-coordinate in points.</param>
        /// <param name="width">The image width in points. If null, uses original width.</param>
        /// <param name="height">The image height in points. If null, uses original height.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task AddImageAsync(byte[] imageData, double x, double y, double? width = null, double? height = null, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (imageData == null || imageData.Length == 0)
                throw new ArgumentException("Image data cannot be null or empty.", nameof(imageData));

            // If dimensions are not specified, we would need to load the image to get its dimensions
            var imageWidth = width ?? 100; // Default width
            var imageHeight = height ?? 100; // Default height

            await _pdfGraphics.DrawImageAsync(imageData, x, y, imageWidth, imageHeight, cancellationToken);
        }

        /// <summary>
        /// Adds a table to the page at the specified position.
        /// </summary>
        /// <param name="table">The table to add.</param>
        /// <param name="x">The x-coordinate in points.</param>
        /// <param name="y">The y-coordinate in points.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task AddTableAsync(Table table, double x, double y, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (table == null)
                throw new ArgumentNullException(nameof(table));

            await Task.Run(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var currentY = y;
                var rowHeight = 20.0; // Default row height

                // Draw table headers and borders if needed
                if (table.Style.ShowBorders)
                {
                    var tableWidth = table.ColumnWidths.Count > 0 
                        ? table.ColumnWidths.Sum() 
                        : table.Rows.Count > 0 && table.Rows[0].Cells.Count > 0 
                            ? table.Rows[0].Cells.Count * 100.0 
                            : 100.0;
                    
                    var tableHeight = table.Rows.Count * rowHeight;
                    
                    await _pdfGraphics.DrawRectangleAsync(x, y, tableWidth, tableHeight, 
                        table.Style.BorderColor, table.Style.BackgroundColor, 
                        table.Style.BorderWidth, cancellationToken);
                }

                // Draw table content
                foreach (var row in table.Rows)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    var currentX = x;
                    var cellIndex = 0;

                    foreach (var cell in row.Cells)
                    {
                        var cellWidth = table.ColumnWidths.Count > cellIndex 
                            ? table.ColumnWidths[cellIndex] 
                            : 100.0; // Default cell width

                        // Draw cell content
                        var cellStyle = cell.Style?.TextStyle ?? row.Style?.TextStyle ?? table.Style.TextStyle;
                        await _pdfGraphics.DrawTextAsync(cell.Content, 
                            currentX + table.Style.CellPadding, 
                            currentY + table.Style.CellPadding, 
                            cellStyle, 
                            cancellationToken);

                        // Draw cell borders
                        if (table.Style.ShowBorders)
                        {
                            await _pdfGraphics.DrawRectangleAsync(currentX, currentY, cellWidth, rowHeight,
                                table.Style.BorderColor, null, table.Style.BorderWidth, cancellationToken);
                        }

                        currentX += cellWidth;
                        cellIndex++;
                    }

                    currentY += rowHeight;
                }
            }, cancellationToken);
        }

        /// <summary>
        /// Adds a header to the page.
        /// </summary>
        /// <param name="text">The header text.</param>
        /// <param name="style">The text style. If null, uses default header style.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task AddHeaderAsync(string text, TextStyle? style = null, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(text))
                return;

            var headerStyle = style ?? TextStyle.Heading;
            var margin = 50.0;
            
            await AddTextAsync(text, margin, margin, headerStyle, cancellationToken);
        }

        /// <summary>
        /// Adds a footer to the page.
        /// </summary>
        /// <param name="text">The footer text.</param>
        /// <param name="style">The text style. If null, uses default footer style.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task AddFooterAsync(string text, TextStyle? style = null, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(text))
                return;

            var footerStyle = style ?? TextStyle.Default;
            var margin = 50.0;
            var y = Height - margin - footerStyle.FontSize;
            
            await AddTextAsync(text, margin, y, footerStyle, cancellationToken);
        }

        /// <summary>
        /// Extracts text from this page.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The extracted text.</returns>
        public async Task<string> ExtractTextAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            return await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                // Note: Text extraction from PDF is complex and would require additional libraries.
                // For this demo, we'll return a placeholder.
                return $"Text content from page (width: {Width}, height: {Height})";
            }, cancellationToken);
        }

        /// <summary>
        /// Throws if the object has been disposed.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(PdfPage));
        }

        /// <summary>
        /// Disposes the page and releases resources.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _pdfGraphics?.Dispose();
                _graphics?.Dispose();
                _disposed = true;
            }
        }
    }
}
