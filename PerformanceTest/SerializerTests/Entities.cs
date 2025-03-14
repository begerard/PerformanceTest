using PolyType;

namespace SerializerTests;

[GenerateShape<List<A>>]
partial class ListAWitness;

public class A
{
    public bool MyBool { get; set; }
    public int MyInt { get; set; }
    public long? MyLong { get; set; }
    public EnumX MyEnum { get; set; }
    public TimeSpan MyTimeSpan { get; set; }
    public TimeOnly MyTime { get; set; }
    public DateOnly MyDate { get; set; }
    public string MyString { get; set; }

    public List<B> Childs { get; set; } = [];

    public static List<A> Seed(int nb, bool circular)
    {
        var result = new List<A>();
        for (int i = 0; i < nb; i++)
        {
            A a = new();
            a.MyString = RandomString(nb);
            result.Add(a);

            var bytesArray = new byte[nb];
            Random.Shared.NextBytes(bytesArray);
            a.MyString = Convert.ToBase64String(bytesArray);

            for (int j = 0; j < nb; j++)
            {
                B b = new();
                b.MyString = RandomString(nb);
                a.Childs.Add(b);

                if (circular) b.Parent = a;

                for (int k = 0; k < nb; k++)
                {
                    C c = new();
                    c.MyString = RandomString(nb);
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

public class B
{
    public bool MyBool { get; set; }
    public int MyInt { get; set; }
    public long? MyLong { get; set; }
    public EnumX MyEnum { get; set; }
    public TimeSpan MyTimeSpan { get; set; }
    public TimeOnly MyTime { get; set; }
    public DateOnly MyDate { get; set; }
    public string MyString { get; set; }

    public List<C> Childs { get; set; } = [];
    public A Parent { get; set; }
}

public class C
{
    public bool MyBool { get; set; }
    public int MyInt { get; set; }
    public long? MyLong { get; set; }
    public EnumX MyEnum { get; set; }
    public TimeSpan MyTimeSpan { get; set; }
    public TimeOnly MyTime { get; set; }
    public DateOnly MyDate { get; set; }
    public string MyString { get; set; }

    public B Parent { get; set; }
}

public enum EnumX { None, One, Two, Three }