// Copyright (c) 2024 Pierre G. Boutquin. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License").
//   You may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//
//   See the License for the specific language governing permissions and
//   limitations under the License.
//

namespace Boutquin.UnitTests.Domain;

/// <summary>
/// Unit tests for the Guard.AgainstDefault method.
/// </summary>
public sealed class GuardAgainstDefaultTests
{
    /// <summary>
    /// Tests that AgainstDefault throws an ArgumentException for default int value.
    /// </summary>
    [Fact]
    public void AgainstDefault_IntDefault_ThrowsArgumentException()
    {
        int value = default;
        Assert.Throws<ArgumentException>(() => Guard.AgainstDefault(() => value));
    }

    /// <summary>
    /// Tests that AgainstDefault does not throw for non-default int value.
    /// </summary>
    [Fact]
    public void AgainstDefault_IntNonDefault_DoesNotThrow()
    {
        var value = 1;
        Guard.AgainstDefault(() => value);
    }

    /// <summary>
    /// Tests that AgainstDefault throws an ArgumentException for default Guid value.
    /// </summary>
    [Fact]
    public void AgainstDefault_GuidDefault_ThrowsArgumentException()
    {
        Guid value = default;
        Assert.Throws<ArgumentException>(() => Guard.AgainstDefault(() => value));
    }

    /// <summary>
    /// Tests that AgainstDefault does not throw for non-default Guid value.
    /// </summary>
    [Fact]
    public void AgainstDefault_GuidNonDefault_DoesNotThrow()
    {
        var value = Guid.NewGuid();
        Guard.AgainstDefault(() => value);
    }

    /// <summary>
    /// Tests that AgainstDefault throws an ArgumentException for default DateTime value.
    /// </summary>
    [Fact]
    public void AgainstDefault_DateTimeDefault_ThrowsArgumentException()
    {
        DateTime value = default;
        Assert.Throws<ArgumentException>(() => Guard.AgainstDefault(() => value));
    }

    /// <summary>
    /// Tests that AgainstDefault does not throw for non-default DateTime value.
    /// </summary>
    [Fact]
    public void AgainstDefault_DateTimeNonDefault_DoesNotThrow()
    {
        var value = DateTime.Now;
        Guard.AgainstDefault(() => value);
    }

    /// <summary>
    /// Tests that AgainstDefault throws an ArgumentException for default TimeSpan value.
    /// </summary>
    [Fact]
    public void AgainstDefault_TimeSpanDefault_ThrowsArgumentException()
    {
        TimeSpan value = default;
        Assert.Throws<ArgumentException>(() => Guard.AgainstDefault(() => value));
    }

    /// <summary>
    /// Tests that AgainstDefault does not throw for non-default TimeSpan value.
    /// </summary>
    [Fact]
    public void AgainstDefault_TimeSpanNonDefault_DoesNotThrow()
    {
        var value = TimeSpan.FromMinutes(1);
        Guard.AgainstDefault(() => value);
    }

    /// <summary>
    /// Tests that AgainstDefault throws an ArgumentException for default double value.
    /// </summary>
    [Fact]
    public void AgainstDefault_DoubleDefault_ThrowsArgumentException()
    {
        double value = default;
        Assert.Throws<ArgumentException>(() => Guard.AgainstDefault(() => value));
    }

    /// <summary>
    /// Tests that AgainstDefault does not throw for non-default double value.
    /// </summary>
    [Fact]
    public void AgainstDefault_DoubleNonDefault_DoesNotThrow()
    {
        var value = 1.0;
        Guard.AgainstDefault(() => value);
    }

    /// <summary>
    /// Tests that AgainstDefault throws an ArgumentException for default bool value.
    /// </summary>
    [Fact]
    public void AgainstDefault_BoolDefault_ThrowsArgumentException()
    {
        bool value = default;
        Assert.Throws<ArgumentException>(() => Guard.AgainstDefault(() => value));
    }

    /// <summary>
    /// Tests that AgainstDefault does not throw for non-default bool value.
    /// </summary>
    [Fact]
    public void AgainstDefault_BoolNonDefault_DoesNotThrow()
    {
        var value = true;
        Guard.AgainstDefault(() => value);
    }

    /// <summary>
    /// Tests that AgainstDefault throws an ArgumentException for default decimal value.
    /// </summary>
    [Fact]
    public void AgainstDefault_DecimalDefault_ThrowsArgumentException()
    {
        decimal value = default;
        Assert.Throws<ArgumentException>(() => Guard.AgainstDefault(() => value));
    }

    /// <summary>
    /// Tests that AgainstDefault does not throw for non-default decimal value.
    /// </summary>
    [Fact]
    public void AgainstDefault_DecimalNonDefault_DoesNotThrow()
    {
        var value = 1.0m;
        Guard.AgainstDefault(() => value);
    }

    /// <summary>
    /// Tests that AgainstDefault throws an ArgumentException for default enum value.
    /// </summary>
    [Fact]
    public void AgainstDefault_EnumDefault_ThrowsArgumentException()
    {
        MyEnum value = default;
        Assert.Throws<ArgumentException>(() => Guard.AgainstDefault(() => value));
    }

    /// <summary>
    /// Tests that AgainstDefault does not throw for non-default enum value.
    /// </summary>
    [Fact]
    public void AgainstDefault_EnumNonDefault_DoesNotThrow()
    {
        var value = MyEnum.Second;
        Guard.AgainstDefault(() => value);
    }

    /// <summary>
    /// Enum for testing purposes.
    /// </summary>
    private enum MyEnum
    {
        First = 0,
        Second = 1
    }

    /// <summary>
    /// Tests that AgainstDefault throws an ArgumentException for default custom struct value.
    /// </summary>
    [Fact]
    public void AgainstDefault_CustomStructDefault_ThrowsArgumentException()
    {
        MyStruct value = default;
        Assert.Throws<ArgumentException>(() => Guard.AgainstDefault(() => value));
    }

    /// <summary>
    /// Tests that AgainstDefault does not throw for non-default custom struct value.
    /// </summary>
    [Fact]
    public void AgainstDefault_CustomStructNonDefault_DoesNotThrow()
    {
        var value = new MyStruct { Value = 1 };
        Guard.AgainstDefault(() => value);
    }

    /// <summary>
    /// Custom struct for testing purposes.
    /// </summary>
    private struct MyStruct
    {
        public int Value { get; set; }
    }

    /// <summary>
    /// Tests that AgainstDefault throws an ArgumentException for default record struct value.
    /// </summary>
    [Fact]
    public void AgainstDefault_RecordStructDefault_ThrowsArgumentException()
    {
        MyRecordStruct value = default;
        Assert.Throws<ArgumentException>(() => Guard.AgainstDefault(() => value));
    }

    /// <summary>
    /// Tests that AgainstDefault does not throw for non-default record struct value.
    /// </summary>
    [Fact]
    public void AgainstDefault_RecordStructNonDefault_DoesNotThrow()
    {
        var value = new MyRecordStruct(1);
        Guard.AgainstDefault(() => value);
    }

    /// <summary>
    /// Record struct type for testing purposes.
    /// </summary>
    private record struct MyRecordStruct(int Value);
}
