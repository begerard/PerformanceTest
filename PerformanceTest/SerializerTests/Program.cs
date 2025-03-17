using BenchmarkDotNet.Running;
using SerializerTests;

//BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

var a = new Deserialize();
a.GlobalSetup();