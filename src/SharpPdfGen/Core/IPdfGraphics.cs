using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace SharpPdfGen.Core
{
    /// <summary>
    /// Represents a graphics context for drawing on a PDF page.
    /// </summary>
    public interface IPdfGraphics : IDisposable
    {
        /// <summary>
        /// Draws text at the specified position.
        /// </summary>
        /// <param name="text">The text to draw.</param>
        /// <param name="x">The x-coordinate in points.</param>
        /// <param name="y">The y-coordinate in points.</param>
        /// <param name="style">The text style.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task DrawTextAsync(string text, double x, double y, TextStyle style, CancellationToken cancellationToken = default);

        /// <summary>
        /// Draws an image at the specified position.
        /// </summary>
        /// <param name="imageData">The image data.</param>
        /// <param name="x">The x-coordinate in points.</param>
        /// <param name="y">The y-coordinate in points.</param>
        /// <param name="width">The image width in points.</param>
        /// <param name="height">The image height in points.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task DrawImageAsync(byte[] imageData, double x, double y, double width, double height, CancellationToken cancellationToken = default);

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
        Task DrawLineAsync(double x1, double y1, double x2, double y2, Color color, double width = 1.0, CancellationToken cancellationToken = default);

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
        Task DrawRectangleAsync(double x, double y, double width, double height, Color? strokeColor = null, Color? fillColor = null, double strokeWidth = 1.0, CancellationToken cancellationToken = default);

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
        Task DrawEllipseAsync(double x, double y, double width, double height, Color? strokeColor = null, Color? fillColor = null, double strokeWidth = 1.0, CancellationToken cancellationToken = default);

        /// <summary>
        /// Measures the size of text when rendered with the specified style.
        /// </summary>
        /// <param name="text">The text to measure.</param>
        /// <param name="style">The text style.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The text size in points.</returns>
        Task<SizeF> MeasureTextAsync(string text, TextStyle style, CancellationToken cancellationToken = default);
    }
}
