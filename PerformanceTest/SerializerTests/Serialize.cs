using BenchmarkDotNet.Attributes;
using Ceras;
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
    private MessagePackSerializer messagePackSerializer;
    private CerasSerializer cerasSerializer;

    private JsonSerializerOptions jsonOptionsWithReference;
    private MessagePackSerializer messagePackSerializerWithReference;
    private CerasSerializer cerasSerializerWithReference;

    [GlobalSetup]
    public void GlobalSetup()
    {
        data = A.Seed(25, false);
        dataCycle = A.Seed(25, true);

        jsonOptions = new JsonSerializerOptions() { ReferenceHandler = null };
        messagePackSerializer = new MessagePackSerializer() { PreserveReferences =  ReferencePreservationMode.Off };
        cerasSerializer = new CerasSerializer(new() { PreserveReferences = false });
        CerasBufferPool.Pool = new CerasDefaultBufferPool();

        jsonOptionsWithReference = new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.Preserve };
        messagePackSerializerWithReference = new MessagePackSerializer() { PreserveReferences = ReferencePreservationMode.AllowCycles };
        cerasSerializerWithReference = new CerasSerializer(new() { PreserveReferences = true });
    }

    [Benchmark]
    public void StjWithoutReference()
        => JsonSerializer.Serialize(data, jsonOptions);
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
    public void MsgPackWithReference()
        => messagePackSerializerWithReference.Serialize<List<A>, ListAWitness>(data);
    [Benchmark]
    public void CerasWithReference()
        => cerasSerializerWithReference.Serialize(data);

    [Benchmark]
    public void StjWithCycle()
        => JsonSerializer.Serialize(dataCycle, jsonOptionsWithReference);
    [Benchmark]
    public void MsgPackWithCycle()
        => messagePackSerializerWithReference.Serialize<List<A>, ListAWitness>(dataCycle);
    [Benchmark]
    public void CerasWithCycle()
        => cerasSerializerWithReference.Serialize(dataCycle);
}