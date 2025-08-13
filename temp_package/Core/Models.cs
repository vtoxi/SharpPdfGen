using System;
using System.Collections.Generic;
using System.Drawing;

namespace SharpPdfGen.Core
{
    /// <summary>
    /// Represents a page size for PDF documents.
    /// </summary>
    public enum PageSize
    {
        /// <summary>A4 page size (210 × 297 mm)</summary>
        A4,
        /// <summary>A3 page size (297 × 420 mm)</summary>
        A3,
        /// <summary>A5 page size (148 × 210 mm)</summary>
        A5,
        /// <summary>Letter page size (8.5 × 11 inches)</summary>
        Letter,
        /// <summary>Legal page size (8.5 × 14 inches)</summary>
        Legal,
        /// <summary>Tabloid page size (11 × 17 inches)</summary>
        Tabloid,
        /// <summary>Custom page size</summary>
        Custom
    }

    /// <summary>
    /// Represents text alignment options.
    /// </summary>
    public enum TextAlignment
    {
        /// <summary>Left alignment</summary>
        Left,
        /// <summary>Center alignment</summary>
        Center,
        /// <summary>Right alignment</summary>
        Right,
        /// <summary>Justified alignment</summary>
        Justify
    }

    /// <summary>
    /// Represents font weight options.
    /// </summary>
    public enum FontWeight
    {
        /// <summary>Normal font weight</summary>
        Normal,
        /// <summary>Bold font weight</summary>
        Bold
    }

    /// <summary>
    /// Represents font style options.
    /// </summary>
    public enum FontStyle
    {
        /// <summary>Normal font style</summary>
        Normal,
        /// <summary>Italic font style</summary>
        Italic,
        /// <summary>Bold font style</summary>
        Bold,
        /// <summary>Bold italic font style</summary>
        BoldItalic
    }

    /// <summary>
    /// Represents text style configuration.
    /// </summary>
    public class TextStyle
    {
        /// <summary>
        /// Gets or sets the font family name.
        /// </summary>
        public string FontFamily { get; set; } = "Arial";

        /// <summary>
        /// Gets or sets the font size in points.
        /// </summary>
        public double FontSize { get; set; } = 12.0;

        /// <summary>
        /// Gets or sets the font weight.
        /// </summary>
        public FontWeight FontWeight { get; set; } = FontWeight.Normal;

        /// <summary>
        /// Gets or sets the font style.
        /// </summary>
        public FontStyle FontStyle { get; set; } = FontStyle.Normal;

        /// <summary>
        /// Gets or sets the text color.
        /// </summary>
        public Color Color { get; set; } = Color.Black;

        /// <summary>
        /// Gets or sets the text alignment.
        /// </summary>
        public TextAlignment Alignment { get; set; } = TextAlignment.Left;

        /// <summary>
        /// Gets or sets the line height multiplier.
        /// </summary>
        public double LineHeight { get; set; } = 1.2;

        /// <summary>
        /// Creates a default text style.
        /// </summary>
        public static TextStyle Default => new TextStyle();

        /// <summary>
        /// Creates a heading style.
        /// </summary>
        public static TextStyle Heading => new TextStyle 
        { 
            FontSize = 18, 
            FontWeight = FontWeight.Bold 
        };

        /// <summary>
        /// Creates a subheading style.
        /// </summary>
        public static TextStyle Subheading => new TextStyle 
        { 
            FontSize = 14, 
            FontWeight = FontWeight.Bold 
        };
    }

    /// <summary>
    /// Represents a table for PDF documents.
    /// </summary>
    public class Table
    {
        /// <summary>
        /// Gets or sets the table rows.
        /// </summary>
        public List<TableRow> Rows { get; set; } = new List<TableRow>();

        /// <summary>
        /// Gets or sets the column widths in points.
        /// </summary>
        public List<double> ColumnWidths { get; set; } = new List<double>();

        /// <summary>
        /// Gets or sets the table style.
        /// </summary>
        public TableStyle Style { get; set; } = new TableStyle();

        /// <summary>
        /// Adds a row to the table.
        /// </summary>
        /// <param name="cells">The cell contents for the row.</param>
        public void AddRow(params string[] cells)
        {
            var row = new TableRow();
            foreach (var cell in cells)
            {
                row.Cells.Add(new TableCell { Content = cell });
            }
            Rows.Add(row);
        }
    }

    /// <summary>
    /// Represents a table row.
    /// </summary>
    public class TableRow
    {
        /// <summary>
        /// Gets or sets the cells in this row.
        /// </summary>
        public List<TableCell> Cells { get; set; } = new List<TableCell>();

        /// <summary>
        /// Gets or sets the row style.
        /// </summary>
        public TableStyle? Style { get; set; }
    }

    /// <summary>
    /// Represents a table cell.
    /// </summary>
    public class TableCell
    {
        /// <summary>
        /// Gets or sets the cell content.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the cell style.
        /// </summary>
        public TableStyle? Style { get; set; }

        /// <summary>
        /// Gets or sets the column span.
        /// </summary>
        public int ColumnSpan { get; set; } = 1;

        /// <summary>
        /// Gets or sets the row span.
        /// </summary>
        public int RowSpan { get; set; } = 1;
    }

    /// <summary>
    /// Represents table styling options.
    /// </summary>
    public class TableStyle
    {
        /// <summary>
        /// Gets or sets the border width in points.
        /// </summary>
        public double BorderWidth { get; set; } = 1.0;

        /// <summary>
        /// Gets or sets the border color.
        /// </summary>
        public Color BorderColor { get; set; } = Color.Black;

        /// <summary>
        /// Gets or sets the cell padding in points.
        /// </summary>
        public double CellPadding { get; set; } = 4.0;

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        public Color? BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the text style for the table.
        /// </summary>
        public TextStyle TextStyle { get; set; } = TextStyle.Default;

        /// <summary>
        /// Gets or sets whether to show borders.
        /// </summary>
        public bool ShowBorders { get; set; } = true;
    }

    /// <summary>
    /// Represents custom page dimensions.
    /// </summary>
    public class CustomPageSize
    {
        /// <summary>
        /// Gets or sets the page width in points.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the page height in points.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Initializes a new instance of the CustomPageSize class.
        /// </summary>
        /// <param name="width">The width in points.</param>
        /// <param name="height">The height in points.</param>
        public CustomPageSize(double width, double height)
        {
            Width = width;
            Height = height;
        }
    }
}
