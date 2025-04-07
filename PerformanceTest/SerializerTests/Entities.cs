using MemoryPack;
using PolyType;
using System.Text.Json.Serialization;

namespace SerializerTests;

[MemoryPackable(GenerateType.CircularReference)]
public partial class A
{
    [MemoryPackOrder(0)]
    public bool MyBool { get; set; }
    [MemoryPackOrder(1)]
    public int MyInt { get; set; }
    [MemoryPackOrder(2)]
    public long? MyLong { get; set; }
    [MemoryPackOrder(3)]
    public EnumX MyEnum { get; set; }
    [MemoryPackOrder(4)]
    public TimeSpan MyTimeSpan { get; set; }
    [MemoryPackOrder(5)]
    public TimeOnly MyTime { get; set; }
    [MemoryPackOrder(6)]
    public DateOnly MyDate { get; set; }
    [MemoryPackOrder(7)]
    public string MyString { get; set; }

    [MemoryPackOrder(8)]
    public List<B> Childs { get; set; } = [];

    public static List<A> Seed(int nb, bool circular)
    {
        var result = new List<A>();
        for (int i = 0; i < nb; i++)
        {
            A a = new() { MyString = RandomString(nb) };
            result.Add(a);

            var bytesArray = new byte[nb];
            Random.Shared.NextBytes(bytesArray);
            a.MyString = Convert.ToBase64String(bytesArray);

            for (int j = 0; j < nb; j++)
            {
                B b = new() { MyString = RandomString(nb) };
                a.Childs.Add(b);

                if (circular) b.Parent = a;

                for (int k = 0; k < nb; k++)
                {
                    C c = new() { MyString = RandomString(nb) };
                    b.Childs.Add(c);

                    if (circular) c.Parent = b;
                }
            }
        }

        return result;
    }

    private static string RandomString(int lenght)
    {
        var bytesArray = new byte[lenght];
        Random.Shared.NextBytes(bytesArray);
        return Convert.ToBase64String(bytesArray);
    }
}

[MemoryPackable(GenerateType.CircularReference)]
public partial class B
{
    [MemoryPackOrder(0)]
    public bool MyBool { get; set; }
    [MemoryPackOrder(1)]
    public int MyInt { get; set; }
    [MemoryPackOrder(2)]
    public long? MyLong { get; set; }
    [MemoryPackOrder(3)]
    public EnumX MyEnum { get; set; }
    [MemoryPackOrder(4)]
    public TimeSpan MyTimeSpan { get; set; }
    [MemoryPackOrder(5)]
    public TimeOnly MyTime { get; set; }
    [MemoryPackOrder(6)]
    public DateOnly MyDate { get; set; }
    [MemoryPackOrder(7)]
    public string MyString { get; set; }

    [MemoryPackOrder(8)]
    public List<C> Childs { get; set; } = [];
    [MemoryPackOrder(9)]
    public A Parent { get; set; }
}

[MemoryPackable(GenerateType.CircularReference)]
public partial class C
{
    [MemoryPackOrder(0)]
    public bool MyBool { get; set; }
    [MemoryPackOrder(1)]
    public int MyInt { get; set; }
    [MemoryPackOrder(2)]
    public long? MyLong { get; set; }
    [MemoryPackOrder(3)]
    public EnumX MyEnum { get; set; }
    [MemoryPackOrder(4)]
    public TimeSpan MyTimeSpan { get; set; }
    [MemoryPackOrder(5)]
    public TimeOnly MyTime { get; set; }
    [MemoryPackOrder(6)]
    public DateOnly MyDate { get; set; }
    [MemoryPackOrder(7)]
    public string MyString { get; set; }

    [MemoryPackOrder(8)]
    public B Parent { get; set; }
}

public enum EnumX { None, One, Two, Three }

[GenerateShape<List<A>>]
partial class ListAWitness;

[JsonSerializable(typeof(A))]
[JsonSerializable(typeof(B))]
[JsonSerializable(typeof(C))]

[JsonSerializable(typeof(List<A>))]
[JsonSerializable(typeof(List<B>))]
[JsonSerializable(typeof(List<C>))]
[JsonSourceGenerationOptions(UseStringEnumConverter = true)]
internal partial class SourceGenerationContext : JsonSerializerContext { }