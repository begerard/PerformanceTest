using BenchmarkDotNet.Attributes;
using Ceras;
using MemoryPack;
using Nerdbank.MessagePack;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SerializerTests;

[MemoryDiagnoser(false)]
public class Serialize
{
    private List<A> data;
    private List<A> dataCycle;

    private JsonSerializerOptions jsonOptions;
    private JsonSerializerOptions jsonOptionsTyped;
    private MessagePackSerializer messagePackSerializer;
    private CerasSerializer cerasSerializer;

    private JsonSerializerOptions jsonOptionsWithReference;
    private JsonSerializerOptions jsonOptionsTypedWithReference;
    private MessagePackSerializer messagePackSerializerWithReference;
    private CerasSerializer cerasSerializerWithReference;

    [GlobalSetup]
    public void GlobalSetup()
    {
        data = A.Seed(10, false);
        dataCycle = A.Seed(10, true);

        jsonOptions = new JsonSerializerOptions() { ReferenceHandler = null };
        jsonOptionsTyped = new JsonSerializerOptions() { ReferenceHandler = null, TypeInfoResolver = SourceGenerationContext.Default };
        messagePackSerializer = new MessagePackSerializer() { PreserveReferences =  ReferencePreservationMode.Off };
        cerasSerializer = new CerasSerializer(new() { PreserveReferences = false });
        CerasBufferPool.Pool = new CerasDefaultBufferPool();

        jsonOptionsWithReference = new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.Preserve };
        jsonOptionsTypedWithReference = new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.Preserve, TypeInfoResolver = SourceGenerationContext.Default };
        messagePackSerializerWithReference = new MessagePackSerializer() { PreserveReferences = ReferencePreservationMode.AllowCycles };
        cerasSerializerWithReference = new CerasSerializer(new() { PreserveReferences = true });
    }

    [Benchmark(Baseline = true)]
    public void StjWithoutReference()
        => JsonSerializer.Serialize(data, jsonOptions);
    [Benchmark]
    public void StjTypedWithoutReference()
        => JsonSerializer.Serialize(data, jsonOptionsTyped);
    [Benchmark]
    public void MsgPackWithoutReference()
        => messagePackSerializer.Serialize<List<A>, ListAWitness>(data);
    [Benchmark]
    public void CerasWithoutReference()
        => cerasSerializer.Serialize(data);

    [Benchmark]
    public void StjWithReference()
        => JsonSerializer.Serialize(data, jsonOptionsWithReference);
    [Benchmark]
    public void StjTypedWithReference()
        => JsonSerializer.Serialize(data, jsonOptionsTypedWithReference);
    [Benchmark]
    public void MsgPackWithReference()
        => messagePackSerializerWithReference.Serialize<List<A>, ListAWitness>(data);
    [Benchmark]
    public void MemPackWithReference()
        => MemoryPackSerializer.Serialize(data);
    [Benchmark]
    public void CerasWithReference()
        => cerasSerializerWithReference.Serialize(data);

    [Benchmark]
    public void StjWithCycle()
        => JsonSerializer.Serialize(dataCycle, jsonOptionsWithReference);
    [Benchmark]
    public void StjTypedWithCycle()
        => JsonSerializer.Serialize(dataCycle, jsonOptionsTypedWithReference);
    [Benchmark]
    public void MsgPackWithCycle()
        => messagePackSerializerWithReference.Serialize<List<A>, ListAWitness>(dataCycle);
    [Benchmark]
    public void MemPackWithCycle()
        => MemoryPackSerializer.Serialize(dataCycle);
    [Benchmark]
    public void CerasWithCycle()
        => cerasSerializerWithReference.Serialize(dataCycle);
}