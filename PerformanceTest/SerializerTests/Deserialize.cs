using BenchmarkDotNet.Attributes;
using Ceras;
using Nerdbank.MessagePack;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SerializerTests;

[MemoryDiagnoser(false)]
public class Deserialize
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

    string stjSerializedWithoutReference;
    string stjTypedSerializedWithoutReference;
    byte[] msgPackSerializedWithoutReference;
    byte[] cerasSerializedWithoutReference;

    string stjSerializedWithReference;
    string stjTypedSerializedWithReference;
    byte[] msgPackSerializedWithReference;
    byte[] cerasSerializedWithReference;

    string stjSerializedWithCycle;
    string stjTypedSerializedWithCycle;
    byte[] msgPackSerializedWithCycle;
    byte[] cerasSerializedWithCycle;

    [GlobalSetup]
    public void GlobalSetup()
    {
        data = A.Seed(10, false);
        dataCycle = A.Seed(10, true);

        jsonOptions = new JsonSerializerOptions() { ReferenceHandler = null };
        jsonOptionsTyped = new JsonSerializerOptions() { ReferenceHandler = null, TypeInfoResolver = SourceGenerationContext.Default };
        messagePackSerializer = new MessagePackSerializer() { PreserveReferences = ReferencePreservationMode.Off };
        cerasSerializer = new CerasSerializer(new() { PreserveReferences = false });
        CerasBufferPool.Pool = new CerasDefaultBufferPool();

        jsonOptionsWithReference = new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.Preserve };
        jsonOptionsTypedWithReference = new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.Preserve, TypeInfoResolver = SourceGenerationContext.Default };
        messagePackSerializerWithReference = new MessagePackSerializer() { PreserveReferences = ReferencePreservationMode.AllowCycles };
        cerasSerializerWithReference = new CerasSerializer(new() { PreserveReferences = true });



        stjSerializedWithoutReference = JsonSerializer.Serialize(data, jsonOptions);
        stjTypedSerializedWithoutReference = JsonSerializer.Serialize(data, jsonOptionsTyped);
        msgPackSerializedWithoutReference = messagePackSerializer.Serialize<List<A>, ListAWitness>(data);
        cerasSerializedWithoutReference = cerasSerializer.Serialize(data);

        stjSerializedWithReference = JsonSerializer.Serialize(data, jsonOptionsWithReference);
        stjTypedSerializedWithReference = JsonSerializer.Serialize(data, jsonOptionsTypedWithReference);
        msgPackSerializedWithReference = messagePackSerializerWithReference.Serialize<List<A>, ListAWitness>(data);
        cerasSerializedWithReference = cerasSerializerWithReference.Serialize(data);

        stjSerializedWithCycle = JsonSerializer.Serialize(dataCycle, jsonOptionsWithReference);
        stjTypedSerializedWithCycle = JsonSerializer.Serialize(dataCycle, jsonOptionsTypedWithReference);
        msgPackSerializedWithCycle = messagePackSerializerWithReference.Serialize<List<A>, ListAWitness>(dataCycle);
        cerasSerializedWithCycle = cerasSerializerWithReference.Serialize(dataCycle);
    }



    [Benchmark(Baseline = true)]
    public void StjWithoutReference()
        => JsonSerializer.Deserialize<List<A>>(stjSerializedWithoutReference, jsonOptions);
    [Benchmark]
    public void StjTypedWithoutReference()
        => JsonSerializer.Deserialize<List<A>>(stjTypedSerializedWithoutReference, jsonOptionsTyped);
    [Benchmark]
    public void MsgPackWithoutReference()
        => messagePackSerializer.Deserialize<List<A>, ListAWitness>(msgPackSerializedWithoutReference);
    [Benchmark]
    public void CerasWithoutReference()
        => cerasSerializer.Deserialize<List<A>>(cerasSerializedWithoutReference);

    [Benchmark]
    public void StjWithReference()
        => JsonSerializer.Deserialize<List<A>>(stjSerializedWithReference, jsonOptionsWithReference);
    [Benchmark]
    public void StjTypedWithReference()
        => JsonSerializer.Deserialize<List<A>>(stjTypedSerializedWithReference, jsonOptionsTypedWithReference);
    [Benchmark]
    public void MsgPackWithReference()
        => messagePackSerializerWithReference.Deserialize<List<A>, ListAWitness>(msgPackSerializedWithReference);
    [Benchmark]
    public void CerasWithReference()
        => cerasSerializerWithReference.Deserialize<List<A>>(cerasSerializedWithReference);

    [Benchmark]
    public void StjWithCycle()
        => JsonSerializer.Deserialize<List<A>>(stjSerializedWithCycle, jsonOptionsWithReference);
    [Benchmark]
    public void StjTypedWithCycle()
        => JsonSerializer.Deserialize<List<A>>(stjTypedSerializedWithCycle, jsonOptionsTypedWithReference);
    [Benchmark]
    public void MsgPackWithCycle()
        => messagePackSerializerWithReference.Deserialize<List<A>, ListAWitness>(msgPackSerializedWithCycle);
    [Benchmark]
    public void CerasWithCycle()
        => cerasSerializerWithReference.Deserialize<List<A>>(cerasSerializedWithCycle);
}