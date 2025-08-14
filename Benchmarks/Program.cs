using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using IntSet.Benchmarks;

namespace IntegrityTables.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = ManualConfig.Create(ManualConfig.CreateEmpty());
            config.AddColumn(TargetMethodColumn.Method);
            config.AddColumn(StatisticColumn.Mean);
            config.AddColumnProvider(DefaultColumnProviders.Params);
            // config.AddColumn(RankColumn.Arabic);
            // config.AddColumn(BaselineRatioColumn.RatioMean);
            
            config.AddColumn(StatisticColumn.OperationsPerSecond);
            config.AddExporter(JsonExporter.Brief);
            //config.WithOrderer(new DefaultOrderer(SummaryOrderPolicy.Method, MethodOrderPolicy.Alphabetical));
            // config.WithOptions(ConfigOptions.DisableOptimizationsValidator);
            // BenchmarkRunner.Run<IntMapBenchmarks>(config);
            config.WithOptions(ConfigOptions.LogBuildOutput);
            BenchmarkSwitcher
                .FromAssembly(typeof(Program).Assembly)
                .Run(args, config);
        }
    }
}