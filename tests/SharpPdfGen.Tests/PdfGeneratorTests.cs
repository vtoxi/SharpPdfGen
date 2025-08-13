using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using SharpPdfGen;
using SharpPdfGen.Core;
using Xunit;

namespace SharpPdfGen.Tests
{
    /// <summary>
    /// Unit tests for the PdfGenerator class.
    /// </summary>
    public class PdfGeneratorTests : IDisposable
    {
        private readonly List<IPdfDocument> _documentsToDispose = new();

        /// <summary>
        /// Clean up resources after each test.
        /// </summary>
        public void Dispose()
        {
            foreach (var document in _documentsToDispose)
            {
                document?.Dispose();
            }
            _documentsToDispose.Clear();
        }

        [Fact]
        public void CreateDocument_WithoutParameters_ShouldCreateEmptyDocument()
        {
            // Act
            var document = PdfGenerator.CreateDocument();
            _documentsToDispose.Add(document);

            // Assert
            document.Should().NotBeNull();
            document.PageCount.Should().Be(0);
            document.Title.Should().BeNullOrEmpty();
            document.Author.Should().BeNullOrEmpty();
        }

        [Fact]
        public void CreateDocument_WithMetadata_ShouldSetProperties()
        {
            // Arrange
            const string title = "Test Document";
            const string author = "Test Author";
            const string subject = "Test Subject";
            const string keywords = "test,pdf,generation";

            // Act
            var document = PdfGenerator.CreateDocument(title, author, subject, keywords);
            _documentsToDispose.Add(document);

            // Assert
            document.Title.Should().Be(title);
            document.Author.Should().Be(author);
            document.Subject.Should().Be(subject);
            document.Keywords.Should().Be(keywords);
        }

        [Fact]
        public void AddPage_ShouldIncreasePageCount()
        {
            // Arrange
            var document = PdfGenerator.CreateDocument();
            _documentsToDispose.Add(document);

            // Act
            var page = document.AddPage();

            // Assert
            document.PageCount.Should().Be(1);
            page.Should().NotBeNull();
            page.PageSize.Should().Be(PageSize.A4);
        }

        [Fact]
        public void AddPage_WithCustomSize_ShouldSetCorrectDimensions()
        {
            // Arrange
            var document = PdfGenerator.CreateDocument();
            _documentsToDispose.Add(document);

            // Act
            var page = document.AddPage(PageSize.Letter);

            // Assert
            var letterDimensions = PdfGenerator.GetPageDimensions(PageSize.Letter);
            page.Width.Should().Be(letterDimensions.Width);
            page.Height.Should().Be(letterDimensions.Height);
        }

        [Fact]
        public async Task SaveAsync_ToStream_ShouldWritePdfData()
        {
            // Arrange
            var document = PdfGenerator.CreateDocument("Test");
            _documentsToDispose.Add(document);
            document.AddPage();

            using var stream = new MemoryStream();

            // Act
            await document.SaveAsync(stream);

            // Assert
            stream.Length.Should().BeGreaterThan(0);
            stream.Position.Should().Be(stream.Length); // Should be at end after writing
        }

        [Fact]
        public async Task ToByteArrayAsync_ShouldReturnPdfData()
        {
            // Arrange
            var document = PdfGenerator.CreateDocument("Test");
            _documentsToDispose.Add(document);
            document.AddPage();

            // Act
            var bytes = await document.ToByteArrayAsync();

            // Assert
            bytes.Should().NotBeNull();
            bytes.Length.Should().BeGreaterThan(0);
            
            // PDF files start with "%PDF"
            var pdfHeader = System.Text.Encoding.ASCII.GetString(bytes.Take(4).ToArray());
            pdfHeader.Should().Be("%PDF");
        }

        [Fact]
        public async Task MergeDocumentsAsync_ShouldCombinePages()
        {
            // Arrange
            var doc1 = PdfGenerator.CreateDocument("Document 1");
            var doc2 = PdfGenerator.CreateDocument("Document 2");
            _documentsToDispose.AddRange(new[] { doc1, doc2 });

            doc1.AddPage();
            doc1.AddPage();
            doc2.AddPage();

            var documents = new[] { doc1, doc2 };

            // Act
            var mergedDocument = await PdfGenerator.MergeDocumentsAsync(documents);
            _documentsToDispose.Add(mergedDocument);

            // Assert
            mergedDocument.PageCount.Should().Be(3);
        }

        [Fact]
        public async Task SplitDocumentAsync_ShouldCreateSeparateDocuments()
        {
            // Arrange
            var document = PdfGenerator.CreateDocument("Test Document");
            _documentsToDispose.Add(document);
            
            document.AddPage();
            document.AddPage();
            document.AddPage();

            // Act
            var splitDocuments = await PdfGenerator.SplitDocumentAsync(document);
            _documentsToDispose.AddRange(splitDocuments);

            // Assert
            splitDocuments.Should().HaveCount(3);
            splitDocuments.All(d => d.PageCount == 1).Should().BeTrue();
        }

        [Fact]
        public async Task FromHtmlAsync_ShouldCreateDocumentFromHtml()
        {
            // Arrange
            const string html = "<html><body><h1>Test Document</h1><p>This is a test paragraph.</p></body></html>";

            // Act
            var document = await PdfGenerator.FromHtmlAsync(html);
            _documentsToDispose.Add(document);

            // Assert
            document.Should().NotBeNull();
            document.PageCount.Should().BeGreaterThan(0);
        }

        [Theory]
        [InlineData(PageSize.A4, 595.276, 841.890)]
        [InlineData(PageSize.A3, 841.890, 1190.551)]
        [InlineData(PageSize.A5, 419.528, 595.276)]
        [InlineData(PageSize.Letter, 612, 792)]
        [InlineData(PageSize.Legal, 612, 1008)]
        [InlineData(PageSize.Tabloid, 792, 1224)]
        public void GetPageDimensions_ShouldReturnCorrectSizes(PageSize pageSize, double expectedWidth, double expectedHeight)
        {
            // Act
            var (width, height) = PdfGenerator.GetPageDimensions(pageSize);

            // Assert
            width.Should().BeApproximately(expectedWidth, 0.001);
            height.Should().BeApproximately(expectedHeight, 0.001);
        }

        [Fact]
        public async Task LoadDocumentAsync_WithNullStream_ShouldThrowException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                PdfGenerator.LoadDocumentAsync((Stream)null!));
        }

        [Fact]
        public async Task LoadDocumentAsync_WithNullData_ShouldThrowException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                PdfGenerator.LoadDocumentAsync((byte[])null!));
        }

        [Fact]
        public async Task LoadDocumentAsync_WithNonExistentFile_ShouldThrowException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => 
                PdfGenerator.LoadDocumentAsync("non-existent-file.pdf"));
        }

        [Fact]
        public async Task FromHtmlAsync_WithNullContent_ShouldThrowException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                PdfGenerator.FromHtmlAsync(null!));
        }

        [Fact]
        public async Task FromHtmlAsync_WithEmptyContent_ShouldThrowException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                PdfGenerator.FromHtmlAsync(string.Empty));
        }
    }
}
