using System.Drawing;
using FluentAssertions;
using SharpPdfGen.Core;
using Xunit;

namespace SharpPdfGen.Tests
{
    /// <summary>
    /// Unit tests for PDF models and data structures.
    /// </summary>
    public class ModelsTests
    {
        [Fact]
        public void TextStyle_Default_ShouldHaveCorrectDefaults()
        {
            // Act
            var style = TextStyle.Default;

            // Assert
            style.FontFamily.Should().Be("Arial");
            style.FontSize.Should().Be(12.0);
            style.FontWeight.Should().Be(FontWeight.Normal);
            style.FontStyle.Should().Be(FontStyle.Normal);
            style.Color.Should().Be(Color.Black);
            style.Alignment.Should().Be(TextAlignment.Left);
            style.LineHeight.Should().Be(1.2);
        }

        [Fact]
        public void TextStyle_Heading_ShouldHaveCorrectProperties()
        {
            // Act
            var style = TextStyle.Heading;

            // Assert
            style.FontSize.Should().Be(18);
            style.FontWeight.Should().Be(FontWeight.Bold);
        }

        [Fact]
        public void TextStyle_Subheading_ShouldHaveCorrectProperties()
        {
            // Act
            var style = TextStyle.Subheading;

            // Assert
            style.FontSize.Should().Be(14);
            style.FontWeight.Should().Be(FontWeight.Bold);
        }

        [Fact]
        public void Table_AddRow_ShouldAddCellsToRow()
        {
            // Arrange
            var table = new Table();

            // Act
            table.AddRow("Cell 1", "Cell 2", "Cell 3");

            // Assert
            table.Rows.Should().HaveCount(1);
            table.Rows[0].Cells.Should().HaveCount(3);
            table.Rows[0].Cells[0].Content.Should().Be("Cell 1");
            table.Rows[0].Cells[1].Content.Should().Be("Cell 2");
            table.Rows[0].Cells[2].Content.Should().Be("Cell 3");
        }

        [Fact]
        public void Table_Constructor_ShouldInitializeCollections()
        {
            // Act
            var table = new Table();

            // Assert
            table.Rows.Should().NotBeNull();
            table.ColumnWidths.Should().NotBeNull();
            table.Style.Should().NotBeNull();
        }

        [Fact]
        public void TableRow_Constructor_ShouldInitializeCells()
        {
            // Act
            var row = new TableRow();

            // Assert
            row.Cells.Should().NotBeNull();
            row.Style.Should().BeNull();
        }

        [Fact]
        public void TableCell_Constructor_ShouldHaveDefaults()
        {
            // Act
            var cell = new TableCell();

            // Assert
            cell.Content.Should().Be(string.Empty);
            cell.Style.Should().BeNull();
            cell.ColumnSpan.Should().Be(1);
            cell.RowSpan.Should().Be(1);
        }

        [Fact]
        public void TableStyle_Constructor_ShouldHaveDefaults()
        {
            // Act
            var style = new TableStyle();

            // Assert
            style.BorderWidth.Should().Be(1.0);
            style.BorderColor.Should().Be(Color.Black);
            style.CellPadding.Should().Be(4.0);
            style.BackgroundColor.Should().BeNull();
            style.TextStyle.Should().NotBeNull();
            style.ShowBorders.Should().BeTrue();
        }

        [Fact]
        public void CustomPageSize_Constructor_ShouldSetDimensions()
        {
            // Arrange
            const double width = 100.0;
            const double height = 200.0;

            // Act
            var pageSize = new CustomPageSize(width, height);

            // Assert
            pageSize.Width.Should().Be(width);
            pageSize.Height.Should().Be(height);
        }

        [Theory]
        [InlineData(FontWeight.Normal, FontStyle.Normal)]
        [InlineData(FontWeight.Bold, FontStyle.Normal)]
        [InlineData(FontWeight.Normal, FontStyle.Italic)]
        [InlineData(FontWeight.Bold, FontStyle.Bold)]
        public void TextStyle_FontProperties_ShouldBeSettable(FontWeight weight, FontStyle style)
        {
            // Act
            var textStyle = new TextStyle
            {
                FontWeight = weight,
                FontStyle = style
            };

            // Assert
            textStyle.FontWeight.Should().Be(weight);
            textStyle.FontStyle.Should().Be(style);
        }

        [Theory]
        [InlineData(TextAlignment.Left)]
        [InlineData(TextAlignment.Center)]
        [InlineData(TextAlignment.Right)]
        [InlineData(TextAlignment.Justify)]
        public void TextStyle_Alignment_ShouldBeSettable(TextAlignment alignment)
        {
            // Act
            var style = new TextStyle { Alignment = alignment };

            // Assert
            style.Alignment.Should().Be(alignment);
        }

        [Fact]
        public void TextStyle_CustomProperties_ShouldBeSettable()
        {
            // Arrange
            const string fontFamily = "Times New Roman";
            const double fontSize = 16.0;
            const double lineHeight = 1.5;
            var color = Color.Blue;

            // Act
            var style = new TextStyle
            {
                FontFamily = fontFamily,
                FontSize = fontSize,
                LineHeight = lineHeight,
                Color = color
            };

            // Assert
            style.FontFamily.Should().Be(fontFamily);
            style.FontSize.Should().Be(fontSize);
            style.LineHeight.Should().Be(lineHeight);
            style.Color.Should().Be(color);
        }

        [Fact]
        public void TableCell_SpanProperties_ShouldBeSettable()
        {
            // Arrange
            const int columnSpan = 3;
            const int rowSpan = 2;

            // Act
            var cell = new TableCell
            {
                ColumnSpan = columnSpan,
                RowSpan = rowSpan
            };

            // Assert
            cell.ColumnSpan.Should().Be(columnSpan);
            cell.RowSpan.Should().Be(rowSpan);
        }

        [Fact]
        public void TableStyle_BackgroundColor_ShouldBeNullable()
        {
            // Arrange
            var style = new TableStyle();

            // Act
            style.BackgroundColor = Color.LightGray;

            // Assert
            style.BackgroundColor.Should().Be(Color.LightGray);

            // Act again
            style.BackgroundColor = null;

            // Assert
            style.BackgroundColor.Should().BeNull();
        }
    }
}
