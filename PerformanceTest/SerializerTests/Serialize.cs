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
        data = A.Seed(10, false);
        dataCycle = A.Seed(10, true);

        jsonOptions = new JsonSerializerOptions() { ReferenceHandler = null };
        messagePackSerializer = new MessagePackSerializer() { PreserveReferences = false };
        cerasSerializer = new CerasSerializer(new() { PreserveReferences = false });
        CerasBufferPool.Pool = new CerasDefaultBufferPool();

        jsonOptionsWithReference = new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.Preserve };
        messagePackSerializerWithReference = new MessagePackSerializer() { PreserveReferences = true };
        cerasSerializerWithReference = new CerasSerializer(new() { PreserveReferences = true });
    }

    [Benchmark]
    public void StjWithoutReference()
    {
        var serialized = JsonSerializer.Serialize(data, jsonOptions);
        var deserialized = JsonSerializer.Deserialize<List<A>>(serialized, jsonOptions);
        if (deserialized.Count == 0) throw new InvalidOperationException();
    }
    [Benchmark]
    public void MsgPackWithoutReference()
    {
        var serialized = messagePackSerializer.Serialize<List<A>, ListAWitness>(data);
        var deserialized = messagePackSerializer.Deserialize<List<A>, ListAWitness>(serialized);
        if (deserialized.Count == 0) throw new InvalidOperationException();
    }
    [Benchmark]
    public void CerasWithoutReference()
    {
        var serialized = cerasSerializer.Serialize(data);
        var deserialized = cerasSerializer.Deserialize<List<A>>(serialized);
        if (deserialized.Count == 0) throw new InvalidOperationException();
    }



    [Benchmark]
    public void StjWithReference()
    {
        var serialized = JsonSerializer.Serialize(data, jsonOptionsWithReference);
        var deserialized = JsonSerializer.Deserialize<List<A>>(serialized, jsonOptionsWithReference);
        if (deserialized.Count == 0) throw new InvalidOperationException();
    }
    [Benchmark]
    public void MsgPackWithReference()
    {
        var serialized = messagePackSerializerWithReference.Serialize<List<A>, ListAWitness>(data);
        var deserialized = messagePackSerializerWithReference.Deserialize<List<A>, ListAWitness>(serialized);
        if (deserialized.Count == 0) throw new InvalidOperationException();
    }
    [Benchmark]
    public void CerasWithReference()
    {
        var serialized = cerasSerializerWithReference.Serialize(data);
        var deserialized = cerasSerializerWithReference.Deserialize<List<A>>(serialized);
        if (deserialized.Count == 0) throw new InvalidOperationException();
    }



    [Benchmark]
    public void StjWithCycle()
    {
        var serialized = JsonSerializer.Serialize(dataCycle, jsonOptionsWithReference);
        var deserialized = JsonSerializer.Deserialize<List<A>>(serialized, jsonOptionsWithReference);
        if (deserialized.Count == 0) throw new InvalidOperationException();
    }
    //[Benchmark]
    //public void MsgPackWithCycle()
    //{
    //    var serialized = messagePackSerializerWithReference.Serialize<List<A>, ListAWitness>(dataCycle);
    //    var deserialized = messagePackSerializerWithReference.Deserialize<List<A>, ListAWitness>(serialized);
    //    if (deserialized.Count == 0) throw new InvalidOperationException();
    //}
    [Benchmark]
    public void CerasWithCycle()
    {
        var serialized = cerasSerializerWithReference.Serialize(dataCycle);
        var deserialized = cerasSerializerWithReference.Deserialize<List<A>>(serialized);
        if (deserialized.Count == 0) throw new InvalidOperationException();
    }
}