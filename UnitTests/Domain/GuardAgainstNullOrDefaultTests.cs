// Copyright (c) 2024 Pierre G. Boutquin. All rights reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License").
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//
//  See the License for the specific language governing permissions and
//  limitations under the License.
//
namespace Boutquin.UnitTests.Domain;

/// <summary>
/// Unit tests for the Guard.AgainstNullOrDefault method.
/// </summary>
public class GuardAgainstNullOrDefaultTests
{
    /// <summary>
    /// Tests that AgainstNullOrDefault throws an ArgumentNullException for null reference type.
    /// </summary>
    [Fact]
    public void AgainstNullOrDefault_NullReference_ThrowsArgumentNullException()
    {
        string value = null;
        Assert.Throws<ArgumentNullException>(() => Guard.AgainstNullOrDefault(() => value));
    }

    /// <summary>
    /// Tests that AgainstNullOrDefault does not throw for non-null reference type.
    /// </summary>
    [Fact]
    public void AgainstNullOrDefault_NonNullReference_DoesNotThrow()
    {
        string value = "test";
        Guard.AgainstNullOrDefault(() => value);
    }

    /// <summary>
    /// Tests that AgainstNullOrDefault throws an ArgumentException for default int value.
    /// </summary>
    [Fact]
    public void AgainstNullOrDefault_IntDefault_ThrowsArgumentException()
    {
        int value = default;
        Assert.Throws<ArgumentException>(() => Guard.AgainstNullOrDefault(() => value));
    }

    /// <summary>
    /// Tests that AgainstNullOrDefault does not throw for non-default int value.
    /// </summary>
    [Fact]
    public void AgainstNullOrDefault_IntNonDefault_DoesNotThrow()
    {
        int value = 1;
        Guard.AgainstNullOrDefault(() => value);
    }

    /// <summary>
    /// Tests that AgainstNullOrDefault throws an ArgumentException for default Guid value.
    /// </summary>
    [Fact]
    public void AgainstNullOrDefault_GuidDefault_ThrowsArgumentException()
    {
        Guid value = default;
        Assert.Throws<ArgumentException>(() => Guard.AgainstNullOrDefault(() => value));
    }

    /// <summary>
    /// Tests that AgainstNullOrDefault does not throw for non-default Guid value.
    /// </summary>
    [Fact]
    public void AgainstNullOrDefault_GuidNonDefault_DoesNotThrow()
    {
        Guid value = Guid.NewGuid();
        Guard.AgainstNullOrDefault(() => value);
    }

    /// <summary>
    /// Tests that AgainstNullOrDefault throws an ArgumentException for default DateTime value.
    /// </summary>
    [Fact]
    public void AgainstNullOrDefault_DateTimeDefault_ThrowsArgumentException()
    {
        DateTime value = default;
        Assert.Throws<ArgumentException>(() => Guard.AgainstNullOrDefault(() => value));
    }

    /// <summary>
    /// Tests that AgainstNullOrDefault does not throw for non-default DateTime value.
    /// </summary>
    [Fact]
    public void AgainstNullOrDefault_DateTimeNonDefault_DoesNotThrow()
    {
        DateTime value = DateTime.Now;
        Guard.AgainstNullOrDefault(() => value);
    }

    /// <summary>
    /// Tests that AgainstNullOrDefault throws an ArgumentException for default TimeSpan value.
    /// </summary>
    [Fact]
    public void AgainstNullOrDefault_TimeSpanDefault_ThrowsArgumentException()
    {
        TimeSpan value = default;
        Assert.Throws<ArgumentException>(() => Guard.AgainstNullOrDefault(() => value));
    }

    /// <summary>
    /// Tests that AgainstNullOrDefault does not throw for non-default TimeSpan value.
    /// </summary>
    [Fact]
    public void AgainstNullOrDefault_TimeSpanNonDefault_DoesNotThrow()
    {
        TimeSpan value = TimeSpan.FromMinutes(1);
        Guard.AgainstNullOrDefault(() => value);
    }

    /// <summary>
    /// Tests that AgainstNullOrDefault throws an ArgumentException for default double value.
    /// </summary>
    [Fact]
    public void AgainstNullOrDefault_DoubleDefault_ThrowsArgumentException()
    {
        double value = default;
        Assert.Throws<ArgumentException>(() => Guard.AgainstNullOrDefault(() => value));
    }

    /// <summary>
    /// Tests that AgainstNullOrDefault does not throw for non-default double value.
    /// </summary>
    [Fact]
    public void AgainstNullOrDefault_DoubleNonDefault_DoesNotThrow()
    {
        double value = 1.0;
        Guard.AgainstNullOrDefault(() => value);
    }

    /// <summary>
    /// Tests that AgainstNullOrDefault throws an ArgumentException for default bool value.
    /// </summary>
    [Fact]
    public void AgainstNullOrDefault_BoolDefault_ThrowsArgumentException()
    {
        bool value = default;
        Assert.Throws<ArgumentException>(() => Guard.AgainstNullOrDefault(() => value));
    }

    /// <summary>
    /// Tests that AgainstNullOrDefault does not throw for non-default bool value.
    /// </summary>
    [Fact]
    public void AgainstNullOrDefault_BoolNonDefault_DoesNotThrow()
    {
        bool value = true;
        Guard.AgainstNullOrDefault(() => value);
    }

    /// <summary>
    /// Tests that AgainstNullOrDefault throws an ArgumentException for default decimal value.
    /// </summary>
    [Fact]
    public void AgainstNullOrDefault_DecimalDefault_ThrowsArgumentException()
    {
        decimal value = default;
        Assert.Throws<ArgumentException>(() => Guard.AgainstNullOrDefault(() => value));
    }

    /// <summary>
    /// Tests that AgainstNullOrDefault does not throw for non-default decimal value.
    /// </summary>
    [Fact]
    public void AgainstNullOrDefault_DecimalNonDefault_DoesNotThrow()
    {
        decimal value = 1.0m;
        Guard.AgainstNullOrDefault(() => value);
    }

    /// <summary>
    /// Tests that AgainstNullOrDefault throws an ArgumentException for default enum value.
    /// </summary>
    [Fact]
    public void AgainstNullOrDefault_EnumDefault_ThrowsArgumentException()
    {
        MyEnum value = default;
        Assert.Throws<ArgumentException>(() => Guard.AgainstNullOrDefault(() => value));
    }

    /// <summary>
    /// Tests that AgainstNullOrDefault does not throw for non-default enum value.
    /// </summary>
    [Fact]
    public void AgainstNullOrDefault_EnumNonDefault_DoesNotThrow()
    {
        MyEnum value = MyEnum.Second;
        Guard.AgainstNullOrDefault(() => value);
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
    /// Tests that AgainstNullOrDefault throws an ArgumentException for default custom struct value.
    /// </summary>
    [Fact]
    public void AgainstNullOrDefault_CustomStructDefault_ThrowsArgumentException()
    {
        MyStruct value = default;
        Assert.Throws<ArgumentException>(() => Guard.AgainstNullOrDefault(() => value));
    }

    /// <summary>
    /// Tests that AgainstNullOrDefault does not throw for non-default custom struct value.
    /// </summary>
    [Fact]
    public void AgainstNullOrDefault_CustomStructNonDefault_DoesNotThrow()
    {
        MyStruct value = new MyStruct { Value = 1 };
        Guard.AgainstNullOrDefault(() => value);
    }

    /// <summary>
    /// Custom struct for testing purposes.
    /// </summary>
    private struct MyStruct
    {
        public int Value { get; set; }
    }

    /// <summary>
    /// Tests that AgainstNullOrDefault throws an ArgumentException for default record value.
    /// </summary>
    [Fact]
    public void AgainstNullOrDefault_RecordDefault_ThrowsArgumentException()
    {
        MyRecord value = default;
        Assert.Throws<ArgumentNullException>(() => Guard.AgainstNullOrDefault(() => value));
    }

    /// <summary>
    /// Tests that AgainstNullOrDefault does not throw for non-default record value.
    /// </summary>
    [Fact]
    public void AgainstNullOrDefault_RecordNonDefault_DoesNotThrow()
    {
        MyRecord value = new MyRecord(1);
        Guard.AgainstNullOrDefault(() => value);
    }

    /// <summary>
    /// Record type for testing purposes.
    /// </summary>
    private record MyRecord(int Value);

    /// <summary>
    /// Tests that AgainstNullOrDefault throws an ArgumentException for default record struct value.
    /// </summary>
    [Fact]
    public void AgainstNullOrDefault_RecordStructDefault_ThrowsArgumentException()
    {
        MyRecordStruct value = default;
        Assert.Throws<ArgumentException>(() => Guard.AgainstNullOrDefault(() => value));
    }

    /// <summary>
    /// Tests that AgainstNullOrDefault does not throw for non-default record struct value.
    /// </summary>
    [Fact]
    public void AgainstNullOrDefault_RecordStructNonDefault_DoesNotThrow()
    {
        MyRecordStruct value = new MyRecordStruct(1);
        Guard.AgainstNullOrDefault(() => value);
    }

    /// <summary>
    /// Record struct type for testing purposes.
    /// </summary>
    private record struct MyRecordStruct(int Value);
}
