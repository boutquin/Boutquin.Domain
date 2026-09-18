// Copyright (c) 2024-2026 Pierre G. Boutquin. All rights reserved.
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
using System.Reflection;

namespace Boutquin.UnitTests.PackageVerification;

public sealed class StreamComparisonTests
{
    [Theory]
    [InlineData(1, 8192)]
    [InlineData(4096, 7)]
    [InlineData(8192, 8192)]
    public void StreamsEqual_IdenticalContentWithDifferentReadSizes_ReturnsTrue(int leftChunk, int rightChunk)
    {
        var bytes = Enumerable.Range(0, 20000).Select(i => (byte)(i % 251)).ToArray();
        using var left = new ChunkedStream(bytes, leftChunk);
        using var right = new ChunkedStream(bytes, rightChunk);

        Compare(left, right).Should().BeTrue();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void StreamsEqual_DifferentContentOrLength_ReturnsFalse(bool differentLength)
    {
        var original = Enumerable.Range(0, 20000).Select(i => (byte)(i % 251)).ToArray();
        var changed = differentLength ? original[..^1] : original.ToArray();
        if (!differentLength)
        {
            changed[^1] ^= 1;
        }

        using var left = new ChunkedStream(original, 37);
        using var right = new ChunkedStream(changed, 4096);

        Compare(left, right).Should().BeFalse();
    }

    [Fact]
    public void StreamsEqual_EmptyStreams_ReturnsTrue()
    {
        using var left = new MemoryStream();
        using var right = new MemoryStream();

        Compare(left, right).Should().BeTrue();
    }

    private static bool Compare(Stream left, Stream right)
    {
        var type = Assembly.Load("Boutquin.PackageVerification").GetType("Boutquin.PackageVerification.Program", throwOnError: true)!;
        var method = type.GetMethod("StreamsEqual", BindingFlags.Static | BindingFlags.NonPublic)!;
        return (bool)method.Invoke(null, [left, right])!;
    }

    private sealed class ChunkedStream(byte[] bytes, int chunkSize) : MemoryStream(bytes, writable: false)
    {
        public override int Read(Span<byte> buffer) => base.Read(buffer[..Math.Min(buffer.Length, chunkSize)]);
    }
}
