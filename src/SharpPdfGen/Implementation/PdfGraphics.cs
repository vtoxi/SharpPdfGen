using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PdfSharp.Drawing;
using SharpPdfGen.Core;

namespace SharpPdfGen.Implementation
{
    /// <summary>
    /// Implementation of IPdfGraphics using PdfSharp.
    /// </summary>
    internal class PdfGraphics : IPdfGraphics
    {
        private readonly XGraphics _graphics;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the PdfGraphics class.
        /// </summary>
        /// <param name="graphics">The XGraphics instance.</param>
        public PdfGraphics(XGraphics graphics)
        {
            _graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));
        }

        /// <summary>
        /// Draws text at the specified position.
        /// </summary>
        /// <param name="text">The text to draw.</param>
        /// <param name="x">The x-coordinate in points.</param>
        /// <param name="y">The y-coordinate in points.</param>
        /// <param name="style">The text style.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task DrawTextAsync(string text, double x, double y, TextStyle style, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(text) || style == null)
                return;

            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var font = CreateFont(style);
                var brush = new XSolidBrush(ConvertColor(style.Color));
                var format = CreateStringFormat(style.Alignment);

                _graphics.DrawString(text, font, brush, x, y, format);
            }, cancellationToken);
        }

        /// <summary>
        /// Draws an image at the specified position.
        /// </summary>
        /// <param name="imageData">The image data.</param>
        /// <param name="x">The x-coordinate in points.</param>
        /// <param name="y">The y-coordinate in points.</param>
        /// <param name="width">The image width in points.</param>
        /// <param name="height">The image height in points.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task DrawImageAsync(byte[] imageData, double x, double y, double width, double height, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (imageData == null || imageData.Length == 0)
                throw new ArgumentException("Image data cannot be null or empty.", nameof(imageData));

            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                using var stream = new MemoryStream(imageData);
                var image = XImage.FromStream(stream);
                _graphics.DrawImage(image, x, y, width, height);
            }, cancellationToken);
        }

        /// <summary>
        /// Draws a line from one point to another.
        /// </summary>
        /// <param name="x1">The start x-coordinate in points.</param>
        /// <param name="y1">The start y-coordinate in points.</param>
        /// <param name="x2">The end x-coordinate in points.</param>
        /// <param name="y2">The end y-coordinate in points.</param>
        /// <param name="color">The line color.</param>
        /// <param name="width">The line width in points.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task DrawLineAsync(double x1, double y1, double x2, double y2, Color color, double width = 1.0, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var pen = new XPen(ConvertColor(color), width);
                _graphics.DrawLine(pen, x1, y1, x2, y2);
            }, cancellationToken);
        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="x">The x-coordinate in points.</param>
        /// <param name="y">The y-coordinate in points.</param>
        /// <param name="width">The rectangle width in points.</param>
        /// <param name="height">The rectangle height in points.</param>
        /// <param name="strokeColor">The stroke color. If null, no stroke is drawn.</param>
        /// <param name="fillColor">The fill color. If null, no fill is applied.</param>
        /// <param name="strokeWidth">The stroke width in points.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task DrawRectangleAsync(double x, double y, double width, double height, Color? strokeColor = null, Color? fillColor = null, double strokeWidth = 1.0, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var rect = new XRect(x, y, width, height);

                if (fillColor.HasValue)
                {
                    var brush = new XSolidBrush(ConvertColor(fillColor.Value));
                    _graphics.DrawRectangle(brush, rect);
                }

                if (strokeColor.HasValue)
                {
                    var pen = new XPen(ConvertColor(strokeColor.Value), strokeWidth);
                    _graphics.DrawRectangle(pen, rect);
                }
            }, cancellationToken);
        }

        /// <summary>
        /// Draws an ellipse.
        /// </summary>
        /// <param name="x">The x-coordinate of the bounding rectangle in points.</param>
        /// <param name="y">The y-coordinate of the bounding rectangle in points.</param>
        /// <param name="width">The width of the bounding rectangle in points.</param>
        /// <param name="height">The height of the bounding rectangle in points.</param>
        /// <param name="strokeColor">The stroke color. If null, no stroke is drawn.</param>
        /// <param name="fillColor">The fill color. If null, no fill is applied.</param>
        /// <param name="strokeWidth">The stroke width in points.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task DrawEllipseAsync(double x, double y, double width, double height, Color? strokeColor = null, Color? fillColor = null, double strokeWidth = 1.0, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var rect = new XRect(x, y, width, height);

                if (fillColor.HasValue)
                {
                    var brush = new XSolidBrush(ConvertColor(fillColor.Value));
                    _graphics.DrawEllipse(brush, rect);
                }

                if (strokeColor.HasValue)
                {
                    var pen = new XPen(ConvertColor(strokeColor.Value), strokeWidth);
                    _graphics.DrawEllipse(pen, rect);
                }
            }, cancellationToken);
        }

        /// <summary>
        /// Measures the size of text when rendered with the specified style.
        /// </summary>
        /// <param name="text">The text to measure.</param>
        /// <param name="style">The text style.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The text size in points.</returns>
        public async Task<SizeF> MeasureTextAsync(string text, TextStyle style, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(text) || style == null)
                return SizeF.Empty;

            return await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var font = CreateFont(style);
                var size = _graphics.MeasureString(text, font);
                return new SizeF((float)size.Width, (float)size.Height);
            }, cancellationToken);
        }

        /// <summary>
        /// Creates an XFont from a TextStyle.
        /// </summary>
        /// <param name="style">The text style.</param>
        /// <returns>An XFont instance.</returns>
        private XFont CreateFont(TextStyle style)
        {
            var fontStyle = XFontStyle.Regular;

            if (style.FontWeight == FontWeight.Bold && style.FontStyle == Core.FontStyle.Italic)
                fontStyle = XFontStyle.BoldItalic;
            else if (style.FontWeight == FontWeight.Bold)
                fontStyle = XFontStyle.Bold;
            else if (style.FontStyle == Core.FontStyle.Italic)
                fontStyle = XFontStyle.Italic;

            return new XFont(style.FontFamily, style.FontSize, fontStyle);
        }

        /// <summary>
        /// Creates an XStringFormat from a TextAlignment.
        /// </summary>
        /// <param name="alignment">The text alignment.</param>
        /// <returns>An XStringFormat instance.</returns>
        private XStringFormat CreateStringFormat(TextAlignment alignment)
        {
            var format = new XStringFormat();

            format.Alignment = alignment switch
            {
                TextAlignment.Left => XStringAlignment.Near,
                TextAlignment.Center => XStringAlignment.Center,
                TextAlignment.Right => XStringAlignment.Far,
                TextAlignment.Justify => XStringAlignment.Near, // PdfSharp doesn't support justify, use left
                _ => XStringAlignment.Near
            };

            return format;
        }

        /// <summary>
        /// Converts a System.Drawing.Color to XColor.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>An XColor instance.</returns>
        private XColor ConvertColor(Color color)
        {
            return XColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// Throws if the object has been disposed.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(PdfGraphics));
        }

        /// <summary>
        /// Disposes the graphics and releases resources.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                // Note: We don't dispose _graphics here as it's owned by the page
                _disposed = true;
            }
        }
    }
}
