# SharpPdfGen

[![NuGet](https://img.shields.io/nuget/v/SharpPdfGen.svg)](https://www.nuget.org/packages/SharpPdfGen/)
[![Downloads](https://img.shields.io/nuget/dt/SharpPdfGen.svg)](https://www.nuget.org/packages/SharpPdfGen/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/vtoxi/SharpPdfGen/blob/main/LICENSE)

A high-performance, cross-platform, open-source PDF generation library for .NET with full async support. Create, merge, split, and manipulate PDFs with ease.

## ✨ Features

- 🚀 **Async/await support** - All operations are fully asynchronous
- 🌍 **Cross-platform** - Works on Windows, Linux, and macOS
- 📄 **Multiple PDF types** - Simple documents, multi-page, and tables
- 🔓 **Zero licensing costs** - Completely free and open-source
- 📦 **Easy integration** - Simple, intuitive API
- 🎯 **High performance** - Optimized for speed and memory efficiency

## 🚀 Quick Start

### Installation

```bash
dotnet add package SharpPdfGen
```

### Basic Usage

```csharp
using SharpPdfGen;

// Create a simple PDF
await WorkingPdfGenerator.CreateSimplePdfAsync(
    "My Document", 
    "Hello from SharpPdfGen!", 
    "output.pdf");
```

## 📖 Examples

### Simple PDF Document

```csharp
await WorkingPdfGenerator.CreateSimplePdfAsync(
    title: "Invoice #001",
    content: "Thank you for your purchase! Total: $299.99",
    outputPath: "invoice.pdf"
);
```

### Multi-Page Document

```csharp
string[] pages = {
    "Page 1: Introduction and overview of the project.",
    "Page 2: Technical specifications and requirements.",
    "Page 3: Implementation details and best practices."
};

await WorkingPdfGenerator.CreateMultiPagePdfAsync(
    title: "Project Documentation",
    pages: pages,
    outputPath: "documentation.pdf"
);
```

### Table-Based PDF

```csharp
string[] headers = { "Product", "Quantity", "Price", "Total" };
string[][] rows = {
    new[] { "Widget A", "10", "$25.00", "$250.00" },
    new[] { "Widget B", "5", "$45.00", "$225.00" },
    new[] { "Widget C", "3", "$75.00", "$225.00" }
};

await WorkingPdfGenerator.CreateTablePdfAsync(
    title: "Sales Report Q4 2024",
    headers: headers,
    rows: rows,
    outputPath: "sales-report.pdf"
);
```

### Get PDF as Byte Array

```csharp
byte[] pdfBytes = await WorkingPdfGenerator.GetPdfBytesAsync(
    title: "API Response",
    content: "Generated PDF content for web API response."
);

// Use bytes for web response, email attachment, etc.
return File(pdfBytes, "application/pdf", "document.pdf");
```

## 🛠 API Reference

### WorkingPdfGenerator.CreateSimplePdfAsync

Creates a simple PDF document with title and content.

**Parameters:**
- `title` (string) - Document title
- `content` (string) - Main content text
- `outputPath` (string) - File path for output
- `cancellationToken` (CancellationToken, optional) - Cancellation support

### WorkingPdfGenerator.CreateMultiPagePdfAsync

Creates a multi-page PDF document.

**Parameters:**
- `title` (string) - Document title
- `pages` (string[]) - Array of page content
- `outputPath` (string) - File path for output
- `cancellationToken` (CancellationToken, optional) - Cancellation support

### WorkingPdfGenerator.CreateTablePdfAsync

Creates a PDF with a data table.

**Parameters:**
- `title` (string) - Document title
- `headers` (string[]) - Table column headers
- `rows` (string[][]) - Table data rows
- `outputPath` (string) - File path for output
- `cancellationToken` (CancellationToken, optional) - Cancellation support

### WorkingPdfGenerator.GetPdfBytesAsync

Returns PDF document as byte array.

**Parameters:**
- `title` (string) - Document title
- `content` (string) - Document content
- `cancellationToken` (CancellationToken, optional) - Cancellation support

**Returns:** `Task<byte[]>` - PDF document as bytes

## 🌐 Compatibility

- **.NET 6.0** and higher
- **.NET 8.0** and higher
- **Windows, Linux, macOS**
- **Any .NET application type** (Web, Desktop, Console, API)

## 📋 Requirements

- .NET 6.0 or .NET 8.0 runtime
- No additional dependencies required

## 🤝 Contributing

We welcome contributions! Please visit our [GitHub repository](https://github.com/vtoxi/SharpPdfGen) to:

- 🐛 Report bugs
- 💡 Request features  
- 🔧 Submit pull requests
- 📚 Improve documentation

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/vtoxi/SharpPdfGen/blob/main/LICENSE) file for details.

## 🔗 Links

- **📦 NuGet Package:** https://www.nuget.org/packages/SharpPdfGen/
- **📂 Source Code:** https://github.com/vtoxi/SharpPdfGen
- **🌐 Documentation:** https://vtoxi.github.io/SharpPdfGen
- **🐛 Issues:** https://github.com/vtoxi/SharpPdfGen/issues

## 💖 Support

If SharpPdfGen helps your project, please ⭐ star the repository on [GitHub](https://github.com/vtoxi/SharpPdfGen)!

---

**Made with ❤️ by the SharpPdfGen team**
