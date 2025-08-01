using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using IntSet.Benchmarks;

namespace IntegrityTables.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = ManualConfig.Create(DefaultConfig.Instance);
            config.AddColumn(RankColumn.Arabic);
            config.AddColumn(BaselineRatioColumn.RatioMean);
            config.AddColumn(StatisticColumn.OperationsPerSecond);
            config.AddExporter(JsonExporter.Brief);
            
            BenchmarkSwitcher
                .FromAssembly(typeof(Program).Assembly)
                .Run(args, config);
        }
    }
}