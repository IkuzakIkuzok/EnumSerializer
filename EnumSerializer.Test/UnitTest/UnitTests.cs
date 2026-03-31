
// (c) 2026 Kazuki Kohzuki

namespace EnumSerializer.Test.UnitTest;

public sealed class UnitTests
{
    [Fact]
    public void TestSimple()
    {
        var mustBeSuccess = "val1".TryParseSimpleEnumFromDefaultSerializeValue(out var val1);

        Assert.True(mustBeSuccess);
        Assert.Equal(SimpleEnum.Value1, val1);

        var mustBeFail = "VAL1".TryParseSimpleEnumFromDefaultSerializeValue(out var _);
        Assert.False(mustBeFail);
    } // public void TestSimple ()

    public void TestIgnoreCase()
    {
        var mustBeSuccess1 = "VAL1".TryParseSimpleEnumFromIgnoreCaseTest(out var val1);
        Assert.True(mustBeSuccess1);
        Assert.Equal(SimpleEnum.Value1, val1);

        var mustBeSuccess2 = "val2".TryParseSimpleEnumFromIgnoreCaseTest(out var val2);
        Assert.True(mustBeSuccess2);
        Assert.Equal(SimpleEnum.Value2, val2);
    } // public void TestIgnoreCase ()
} // public sealed class UnitTests
