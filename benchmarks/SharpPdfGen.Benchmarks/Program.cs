using BenchmarkDotNet.Running;

namespace SharpPdfGen.Benchmarks;

/// <summary>
/// Main entry point for running benchmarks.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("🚀 SharpPdfGen Benchmarks");
        Console.WriteLine("========================\n");

        // Run all benchmarks
        BenchmarkRunner.Run<PdfCreationBenchmarks>();
        BenchmarkRunner.Run<TextRenderingBenchmarks>();
        BenchmarkRunner.Run<MemoryUsageBenchmarks>();
    }
}
