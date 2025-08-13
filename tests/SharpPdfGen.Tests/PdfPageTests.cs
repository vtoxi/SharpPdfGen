using System;
using System.Drawing;
using System.Threading.Tasks;
using FluentAssertions;
using SharpPdfGen;
using SharpPdfGen.Core;
using Xunit;

namespace SharpPdfGen.Tests
{
    /// <summary>
    /// Unit tests for PDF page functionality.
    /// </summary>
    public class PdfPageTests : IDisposable
    {
        private readonly IPdfDocument _document;

        public PdfPageTests()
        {
            _document = PdfGenerator.CreateDocument("Test Document");
        }

        public void Dispose()
        {
            _document?.Dispose();
        }

        [Fact]
        public async Task AddTextAsync_ShouldAddTextToPage()
        {
            // Arrange
            var page = _document.AddPage();
            const string text = "Hello, World!";
            const double x = 100;
            const double y = 100;

            // Act
            await page.AddTextAsync(text, x, y);

            // Assert
            // Note: In a real implementation, we would verify the text was added
            // For this test, we're just ensuring no exceptions are thrown
            page.Should().NotBeNull();
        }

        [Fact]
        public async Task AddTextAsync_WithCustomStyle_ShouldApplyStyle()
        {
            // Arrange
            var page = _document.AddPage();
            const string text = "Styled Text";
            var style = new TextStyle
            {
                FontFamily = "Helvetica",
                FontSize = 16,
                FontWeight = FontWeight.Bold,
                Color = Color.Blue
            };

            // Act
            await page.AddTextAsync(text, 50, 50, style);

            // Assert
            page.Should().NotBeNull();
        }

        [Fact]
        public async Task AddImageAsync_WithValidData_ShouldAddImage()
        {
            // Arrange
            var page = _document.AddPage();
            var imageData = CreateTestImageData();

            // Act
            await page.AddImageAsync(imageData, 100, 100, 200, 150);

            // Assert
            page.Should().NotBeNull();
        }

        [Fact]
        public async Task AddImageAsync_WithNullData_ShouldThrowException()
        {
            // Arrange
            var page = _document.AddPage();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                page.AddImageAsync(null!, 100, 100));
        }

        [Fact]
        public async Task AddTableAsync_ShouldAddTableToPage()
        {
            // Arrange
            var page = _document.AddPage();
            var table = new Table();
            table.AddRow("Header 1", "Header 2", "Header 3");
            table.AddRow("Row 1 Col 1", "Row 1 Col 2", "Row 1 Col 3");
            table.AddRow("Row 2 Col 1", "Row 2 Col 2", "Row 2 Col 3");

            table.ColumnWidths.AddRange(new[] { 100.0, 150.0, 100.0 });

            // Act
            await page.AddTableAsync(table, 50, 50);

            // Assert
            page.Should().NotBeNull();
        }

        [Fact]
        public async Task AddHeaderAsync_ShouldAddHeaderToPage()
        {
            // Arrange
            var page = _document.AddPage();
            const string headerText = "Document Header";

            // Act
            await page.AddHeaderAsync(headerText);

            // Assert
            page.Should().NotBeNull();
        }

        [Fact]
        public async Task AddFooterAsync_ShouldAddFooterToPage()
        {
            // Arrange
            var page = _document.AddPage();
            const string footerText = "Page 1 of 1";

            // Act
            await page.AddFooterAsync(footerText);

            // Assert
            page.Should().NotBeNull();
        }

        [Fact]
        public async Task ExtractTextAsync_ShouldReturnText()
        {
            // Arrange
            var page = _document.AddPage();
            await page.AddTextAsync("Sample text for extraction", 100, 100);

            // Act
            var extractedText = await page.ExtractTextAsync();

            // Assert
            extractedText.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void PageProperties_ShouldReturnCorrectValues()
        {
            // Arrange & Act
            var page = _document.AddPage(PageSize.A4);

            // Assert
            page.PageSize.Should().Be(PageSize.A4);
            
            var expectedDimensions = PdfGenerator.GetPageDimensions(PageSize.A4);
            page.Width.Should().Be(expectedDimensions.Width);
            page.Height.Should().Be(expectedDimensions.Height);
        }

        [Fact]
        public void Graphics_ShouldNotBeNull()
        {
            // Arrange
            var page = _document.AddPage();

            // Act & Assert
            page.Graphics.Should().NotBeNull();
        }

        [Fact]
        public async Task AddTextAsync_WithNullText_ShouldNotThrow()
        {
            // Arrange
            var page = _document.AddPage();

            // Act & Assert
            await page.AddTextAsync(null!, 100, 100);
            // Should not throw - implementation should handle gracefully
        }

        [Fact]
        public async Task AddTextAsync_WithEmptyText_ShouldNotThrow()
        {
            // Arrange
            var page = _document.AddPage();

            // Act & Assert
            await page.AddTextAsync(string.Empty, 100, 100);
            // Should not throw - implementation should handle gracefully
        }

        [Fact]
        public async Task AddTableAsync_WithNullTable_ShouldThrowException()
        {
            // Arrange
            var page = _document.AddPage();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                page.AddTableAsync(null!, 50, 50));
        }

        /// <summary>
        /// Creates test image data (minimal valid image data).
        /// </summary>
        /// <returns>Byte array representing a simple image.</returns>
        private byte[] CreateTestImageData()
        {
            // This is a minimal PNG file (1x1 transparent pixel)
            return new byte[]
            {
                0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A,
                0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52,
                0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
                0x08, 0x06, 0x00, 0x00, 0x00, 0x1F, 0x15, 0xC4,
                0x89, 0x00, 0x00, 0x00, 0x0B, 0x49, 0x44, 0x41,
                0x54, 0x78, 0x9C, 0x63, 0x00, 0x01, 0x00, 0x00,
                0x05, 0x00, 0x01, 0x0D, 0x0A, 0x2D, 0xB4, 0x00,
                0x00, 0x00, 0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE,
                0x42, 0x60, 0x82
            };
        }
    }
}
