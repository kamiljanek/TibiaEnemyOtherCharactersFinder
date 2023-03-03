// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using Seeders.Benchmark;

Console.WriteLine("Hello, World!");

BenchmarkRunner.Run<CreateCharacterCorrelationsIfNotExistBenchmark>();