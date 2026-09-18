// Copyright (c) 2024-2026 Pierre G. Boutquin. All rights reserved.
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
/// Tests for the <c>Guard.With&lt;TException&gt;</c> fixes: a passing guard must be a complete no-op
/// (it must not format the message), and a constructor exception must be surfaced with its original
/// stack trace rather than re-thrown with a reset one.
/// </summary>
public sealed class GuardWithFixTests
{
    // An exception whose constructor always throws, to exercise the TargetInvocationException path.
    private sealed class ThrowingCtorException : Exception
    {
        public ThrowingCtorException(string message) : base(message)
            => throw new InvalidOperationException("constructor failed");
    }

    // An exception with only a (string, int) constructor — no single-string ctor. Used to prove the
    // params-args guard path constructs via the matching multi-arg constructor.
    private sealed class CustomTwoArgException : Exception
    {
        public CustomTwoArgException(string message, int errorCode)
            : base(message)
            => ErrorCode = errorCode;

        public int ErrorCode { get; }
    }

    [Fact]
    public void WithArgs_TwoArgConstructor_ConstructsViaMatchingCtorAndThrows()
    {
        // D-06: WithArgs<TException>(params object[]) constructs the exception via the constructor
        // matching the supplied args — here the (string, int) ctor — and throws it. The former
        // With<TException>(params object[]) overload could never be reached: overload resolution sent
        // ("msg", 1001) to With<TException>(string, params object[]) instead, which then failed to find
        // a single-string ctor and threw InvalidOperationException.
        var act = () => Guard.Against(true).WithArgs<CustomTwoArgException>("msg", 1001);

        act.Should().Throw<CustomTwoArgException>()
            .WithMessage("msg")
            .Which.ErrorCode.Should().Be(1001);
    }

    [Fact]
    public void With_FormatOverload_ConditionFalse_DoesNotFormatMessage()
    {
        // "{1}" references a second argument that isn't supplied → string.Format would throw
        // FormatException. With the guard condition false, the method must return before formatting.
        var act = () => Guard.Against(false).With<InvalidOperationException>("value {0} {1}", "only-one");

        act.Should().NotThrow();
    }

    [Fact]
    public void With_FormatOverload_ConditionTrue_MalformedFormat_ThrowsFormatException()
    {
        var act = () => Guard.Against(true).With<InvalidOperationException>("value {0} {1}", "only-one");

        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void With_FormatOverload_ConditionTrue_ValidFormat_ThrowsWithFormattedMessage()
    {
        var act = () => Guard.Against(true).With<InvalidOperationException>("value is {0}", 42);

        act.Should().Throw<InvalidOperationException>().WithMessage("value is 42");
    }

    [Fact]
    public void With_StringOverload_ConstructorThrows_PropagatesInnerExceptionNotTargetInvocation()
    {
        var act = () => Guard.Against(true).With<ThrowingCtorException>("boom");

        // The constructor's InvalidOperationException must surface directly (not wrapped in
        // TargetInvocationException), with its original throw site preserved in the stack trace.
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("constructor failed")
            .Which.StackTrace.Should().Contain(nameof(ThrowingCtorException));
    }

    // --- AC-4.2: WithArgs / format-overload MissingMethodException -> InvalidOperationException remapping ---

    [Fact]
    public void WithArgs_NoMatchingConstructor_ThrowsInvalidOperationException()
    {
        // ExceptionWithoutStringConstructor exposes only a parameterless constructor; supplying an
        // argument leaves Activator.CreateInstance with no matching constructor to bind to, which must
        // be remapped from MissingMethodException to a clear InvalidOperationException.
        var act = () => Guard.Against(true).WithArgs<ExceptionWithoutStringConstructor>("unexpected-arg");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"The exception type '{typeof(ExceptionWithoutStringConstructor).FullName}' must have a constructor that matches the provided arguments.");
    }

    [Fact]
    public void With_FormatOverload_NoMatchingConstructor_ThrowsInvalidOperationException()
    {
        // Same remapping, exercised through the (string exceptionMessage, params object[] args)
        // format overload: after formatting succeeds, the single-string constructor lookup still fails
        // for a type that only has a parameterless constructor.
        var act = () => Guard.Against(true).With<ExceptionWithoutStringConstructor>("value is {0}", 42);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage($"The exception type '{typeof(ExceptionWithoutStringConstructor).FullName}' must have a constructor that accepts a single string parameter.");
    }
}
