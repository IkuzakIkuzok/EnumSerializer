
// (c) 2026 Kazuki Kohzuki

namespace EnumSerializer.Test.UnitTest;

public sealed class ToStringTest
{
    [Fact]
    public void ToStringSimple()
    {
        var val1 = SimpleEnum.Value1.ToDefaultSerializeValue();
        var val2 = SimpleEnum.Value2.ToIgnoreCaseTest();

        Assert.Equal("val1", val1);
        Assert.Equal("VAL2", val2);
    } // public void ToStringSimple ()
} // public sealed class ToStringTest
