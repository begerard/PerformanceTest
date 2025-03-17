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
    private MessagePackSerializer messagePackSerializer;
    private CerasSerializer cerasSerializer;

    private JsonSerializerOptions jsonOptionsWithReference;
    private MessagePackSerializer messagePackSerializerWithReference;
    private CerasSerializer cerasSerializerWithReference;

    string stjSerializedWithoutReference;
    byte[] msgPackSerializedWithoutReference;
    byte[] cerasSerializedWithoutReference;

    string stjSerializedWithReference;
    byte[] msgPackSerializedWithReference;
    byte[] cerasSerializedWithReference;

    string stjSerializedWithCycle;
    byte[] msgPackSerializedWithCycle;
    byte[] cerasSerializedWithCycle;

    [GlobalSetup]
    public void GlobalSetup()
    {
        data = A.Seed(25, false);
        dataCycle = A.Seed(25, true);

        jsonOptions = new JsonSerializerOptions() { ReferenceHandler = null };
        messagePackSerializer = new MessagePackSerializer() { PreserveReferences = ReferencePreservationMode.Off };
        cerasSerializer = new CerasSerializer(new() { PreserveReferences = false });
        CerasBufferPool.Pool = new CerasDefaultBufferPool();

        jsonOptionsWithReference = new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.Preserve };
        messagePackSerializerWithReference = new MessagePackSerializer() { PreserveReferences = ReferencePreservationMode.AllowCycles };
        cerasSerializerWithReference = new CerasSerializer(new() { PreserveReferences = true });



        stjSerializedWithoutReference = JsonSerializer.Serialize(data, jsonOptions);
        msgPackSerializedWithoutReference = messagePackSerializer.Serialize<List<A>, ListAWitness>(data);
        cerasSerializedWithoutReference = cerasSerializer.Serialize(data);

        stjSerializedWithReference = JsonSerializer.Serialize(data, jsonOptionsWithReference);
        msgPackSerializedWithReference = messagePackSerializerWithReference.Serialize<List<A>, ListAWitness>(data);
        cerasSerializedWithReference = cerasSerializerWithReference.Serialize(data);

        stjSerializedWithCycle = JsonSerializer.Serialize(dataCycle, jsonOptionsWithReference);
        msgPackSerializedWithCycle = messagePackSerializerWithReference.Serialize<List<A>, ListAWitness>(dataCycle);
        cerasSerializedWithCycle = cerasSerializerWithReference.Serialize(dataCycle);
    }

    [Benchmark]
    public void StjWithoutReference()
        => JsonSerializer.Deserialize<List<A>>(stjSerializedWithoutReference, jsonOptions);
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
    public void MsgPackWithReference()
        => messagePackSerializerWithReference.Deserialize<List<A>, ListAWitness>(msgPackSerializedWithReference);
    [Benchmark]
    public void CerasWithReference()
        => cerasSerializerWithReference.Deserialize<List<A>>(cerasSerializedWithReference);

    [Benchmark]
    public void StjWithCycle()
        => JsonSerializer.Deserialize<List<A>>(stjSerializedWithCycle, jsonOptionsWithReference);
    [Benchmark]
    public void MsgPackWithCycle()
        => messagePackSerializerWithReference.Deserialize<List<A>, ListAWitness>(msgPackSerializedWithCycle);
    [Benchmark]
    public void CerasWithCycle()
        => cerasSerializerWithReference.Deserialize<List<A>>(cerasSerializedWithCycle);
}