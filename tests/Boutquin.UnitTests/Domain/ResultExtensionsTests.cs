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

#pragma warning disable CA2007 // Consider calling ConfigureAwait — suppressed per xUnit1030 guidance

using Boutquin.Domain.Abstractions;

namespace Boutquin.UnitTests.Domain;

/// <summary>
/// Tests for <see cref="ResultExtensions"/> and the <see cref="Result"/> / <see cref="Result{TValue}"/>
/// factory + conversion surface that previously had no coverage, plus the fixes for the success-carries-null
/// hole, the sync Match null guards, and the Error factory methods.
/// </summary>
public sealed class ResultExtensionsTests
{
    private static readonly Error s_error = new("Test.Error", "A test error occurred");

    // ---- Result.Success<TValue>(null) guard (review finding #2) ----

    [Fact]
    public void SuccessOfT_WithNullReference_ThrowsArgumentNullException()
    {
        var act = () => Result.Success<string>(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("value");
    }

    [Fact]
    public void SuccessOfT_WithNonNullValue_CreatesSuccess()
    {
        var result = Result.Success("ok");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("ok");
    }

    // ---- Result.Cancelled (review finding #16, previously untested) ----

    [Fact]
    public void Cancelled_WithDefaults_IsFailureWithCancelledError()
    {
        var result = Result.Cancelled();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Error.Cancellation");
        result.Error.Description.Should().Be("The request was cancelled.");
        result.Error.ErrorType.Should().Be(ErrorType.Cancelled);
    }

    [Fact]
    public void Cancelled_WithCustomCodeAndName_UsesThem()
    {
        var result = Result.Cancelled("Op.Aborted", "Operation aborted by user.");

        result.Error.Code.Should().Be("Op.Aborted");
        result.Error.Description.Should().Be("Operation aborted by user.");
        result.Error.ErrorType.Should().Be(ErrorType.Cancelled);
    }

    // ---- Implicit conversions (review finding #24, previously untested) ----

    [Fact]
    public void ImplicitConversion_ErrorToResult_ProducesFailure()
    {
        Result result = s_error;

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(s_error);
    }

    [Fact]
    public void ImplicitConversion_ErrorToResultOfT_ProducesFailure()
    {
        Result<int> result = s_error;

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(s_error);
    }

    [Fact]
    public void ImplicitConversion_NullValueToResultOfT_ProducesNullValueFailure()
    {
        Result<string> result = (string?)null;

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.NullValue);
    }

    // ---- Error factory methods (review finding #3 area, previously untested) ----

    [Theory]
    [MemberData(nameof(ErrorFactoryCases))]
    public void ErrorFactory_SetsCodeNameAndType(Func<string, string, Error> factory, ErrorType expectedType)
    {
        var error = factory("Some.Code", "Some message");

        error.Code.Should().Be("Some.Code");
        error.Description.Should().Be("Some message");
        error.ErrorType.Should().Be(expectedType);
    }

    public static TheoryData<Func<string, string, Error>, ErrorType> ErrorFactoryCases() => new()
    {
        { Error.BadRequest, ErrorType.BadRequest },
        { Error.NotFound, ErrorType.NotFound },
        { Error.Conflict, ErrorType.Conflict },
        { Error.Unauthorized, ErrorType.Unauthorized },
        { Error.Forbidden, ErrorType.Forbidden },
        { Error.RequestTimeout, ErrorType.RequestTimeout },
        { Error.InternalServerError, ErrorType.InternalServerError },
    };

    [Fact]
    public void Error_EqualityIncludesErrorType()
    {
        // Same code+name but different type must NOT be equal (record value equality over all members).
        Error.BadRequest("X", "Y").Should().NotBe(Error.NotFound("X", "Y"));
    }

    // ---- sync Match null guards (review finding #22) ----

    [Fact]
    public void Match_NonGeneric_NullOnSuccess_ThrowsArgumentNullException()
    {
        var result = Result.Success();
        Func<int> onSuccess = null!;

        var act = () => result.Match(onSuccess, _ => 0);

        act.Should().Throw<ArgumentNullException>().WithParameterName("onSuccess");
    }

    [Fact]
    public void Match_NonGeneric_NullOnFailure_ThrowsArgumentNullException()
    {
        var result = Result.Success();
        Func<Error, int> onFailure = null!;

        var act = () => result.Match(() => 1, onFailure);

        act.Should().Throw<ArgumentNullException>().WithParameterName("onFailure");
    }

    [Fact]
    public void Match_Generic_NullOnSuccess_ThrowsArgumentNullException()
    {
        var result = Result.Success(5);
        Func<int, int> onSuccess = null!;

        var act = () => result.Match(onSuccess, _ => 0);

        act.Should().Throw<ArgumentNullException>().WithParameterName("onSuccess");
    }

    [Fact]
    public void Match_NonGeneric_ExecutesCorrectBranch()
    {
        Result.Success().Match(() => "ok", _ => "fail").Should().Be("ok");
        Result.Failure(s_error).Match(() => "ok", e => e.Code).Should().Be("Test.Error");
    }

    [Fact]
    public void Match_Generic_ExecutesCorrectBranch()
    {
        Result.Success(42).Match(v => v * 2, _ => -1).Should().Be(84);
        ((Result<int>)s_error).Match(v => v, e => -99).Should().Be(-99);
    }

    // ---- Map ----

    [Fact]
    public void Map_OnSuccess_TransformsValue()
        => Result.Success(10).Map(v => v + 1).Value.Should().Be(11);

    [Fact]
    public void Map_OnFailure_PropagatesError()
    {
        var result = Result.Failure<int>(s_error).Map(v => v + 1);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(s_error);
    }

    [Fact]
    public void Map_NullFunc_ThrowsArgumentNullException()
    {
        var act = () => Result.Success(1).Map<int, int>(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("func");
    }

    [Fact]
    public void Map_NullProjection_ReturnsNullValueFailure()
    {
        // D-04: a projection returning null must yield a FAILED result carrying Error.NullValue,
        // not throw ArgumentNullException. Map routes the projection through Result.Create so the
        // monad stays total.
        var result = Result.Success("x").Map(_ => (string?)null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.NullValue);
    }

    // ---- Bind ----

    [Fact]
    public void Bind_OnSuccess_ChainsResult()
        => Result.Success(3).Bind(v => Result.Success(v * 3)).Value.Should().Be(9);

    [Fact]
    public void Bind_OnFailure_PropagatesError()
    {
        var result = Result.Failure<int>(s_error).Bind(v => Result.Success(v));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(s_error);
    }

    // ---- AC-4.2: ResultExtensions null-argument guards ----

    [Fact]
    public void Bind_NullResult_ThrowsArgumentNullException()
    {
        Result<int> result = null!;

        var act = () => result.Bind(v => Result.Success(v));

        act.Should().Throw<ArgumentNullException>().WithParameterName("result");
    }

    [Fact]
    public void Bind_NullFunc_ThrowsArgumentNullException()
    {
        var act = () => Result.Success(1).Bind<int, int>(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("func");
    }

    [Fact]
    public void Tap_NullResult_ThrowsArgumentNullException()
    {
        Result<int> result = null!;

        var act = () => result.Tap(_ => { });

        act.Should().Throw<ArgumentNullException>().WithParameterName("result");
    }

    [Fact]
    public void Tap_NullAction_ThrowsArgumentNullException()
    {
        var act = () => Result.Success(1).Tap(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("action");
    }

    [Fact]
    public void OnFailure_NullResult_ThrowsArgumentNullException()
    {
        Result<int> result = null!;

        var act = () => result.OnFailure(_ => { });

        act.Should().Throw<ArgumentNullException>().WithParameterName("result");
    }

    [Fact]
    public void OnFailure_NullAction_ThrowsArgumentNullException()
    {
        var act = () => Result.Success(1).OnFailure(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("action");
    }

    [Fact]
    public async Task BindAsync_TaskSource_NullResultTask_ThrowsArgumentNullException()
    {
        Task<Result<int>> resultTask = null!;

        Func<Task> act = async () => await resultTask.BindAsync(v => Task.FromResult(Result.Success(v)));

        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("resultTask");
    }

    [Fact]
    public async Task BindAsync_TaskSource_NullFunc_ThrowsArgumentNullException()
    {
        Func<Task> act = async () => await Task.FromResult(Result.Success(1)).BindAsync<int, int>(null!);

        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("func");
    }

    [Fact]
    public async Task BindAsync_ValueTaskSource_NullFunc_ThrowsArgumentNullException()
    {
        // resultTask is a ValueTask (a struct) and so can never be null — only func is guarded.
        Func<Task> act = async () => await new ValueTask<Result<int>>(Result.Success(1)).BindAsync<int, int>(null!);

        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("func");
    }

    [Fact]
    public async Task TapAsync_ValueTaskSource_NullAction_ThrowsArgumentNullException()
    {
        Func<Task> act = async () => await new ValueTask<Result<int>>(Result.Success(1)).TapAsync(null!);

        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("action");
    }

    [Fact]
    public async Task TapAsync_TaskSource_NullResultTask_ThrowsArgumentNullException()
    {
        Task<Result<int>> resultTask = null!;

        Func<Task> act = async () => await resultTask.TapAsync(_ => Task.CompletedTask);

        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("resultTask");
    }

    [Fact]
    public async Task TapAsync_TaskSource_NullAction_ThrowsArgumentNullException()
    {
        Func<Task> act = async () => await Task.FromResult(Result.Success(1)).TapAsync(null!);

        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("action");
    }

    [Fact]
    public async Task MatchAsync_Generic_NullOnSuccess_ThrowsArgumentNullException()
    {
        Func<Task> act = async () => await new ValueTask<Result<int>>(Result.Success(1))
            .MatchAsync(null!, _ => Task.FromResult(0));

        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("onSuccess");
    }

    [Fact]
    public async Task MatchAsync_Generic_NullOnFailure_ThrowsArgumentNullException()
    {
        Func<Task> act = async () => await new ValueTask<Result<int>>(Result.Success(1))
            .MatchAsync(v => Task.FromResult(v), null!);

        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("onFailure");
    }

    // ---- Tap / OnFailure ----

    [Fact]
    public void Tap_OnSuccess_RunsActionAndReturnsSame()
    {
        var seen = 0;
        var result = Result.Success(7);

        var returned = result.Tap(v => seen = v);

        seen.Should().Be(7);
        returned.Should().BeSameAs(result);
    }

    [Fact]
    public void Tap_OnFailure_DoesNotRunAction()
    {
        var ran = false;

        Result.Failure<int>(s_error).Tap(_ => ran = true);

        ran.Should().BeFalse();
    }

    [Fact]
    public void OnFailure_OnFailure_RunsActionWithError()
    {
        Error? captured = null;

        Result.Failure<int>(s_error).OnFailure(e => captured = e);

        captured.Should().Be(s_error);
    }

    [Fact]
    public void OnFailure_OnSuccess_DoesNotRunAction()
    {
        var ran = false;

        Result.Success(1).OnFailure(_ => ran = true);

        ran.Should().BeFalse();
    }

    // ---- async variants ----

    [Fact]
    public async Task MatchAsync_NonGeneric_ExecutesCorrectBranch()
    {
        var success = await new ValueTask<Result>(Result.Success())
            .MatchAsync(() => Task.FromResult("ok"), _ => Task.FromResult("fail"));
        var failure = await new ValueTask<Result>(Result.Failure(s_error))
            .MatchAsync(() => Task.FromResult("ok"), e => Task.FromResult(e.Code));

        success.Should().Be("ok");
        failure.Should().Be("Test.Error");
    }

    [Fact]
    public async Task MatchAsync_Generic_ExecutesCorrectBranch()
    {
        var value = await new ValueTask<Result<int>>(Result.Success(21))
            .MatchAsync(v => Task.FromResult(v * 2), _ => Task.FromResult(-1));

        value.Should().Be(42);
    }

    [Fact]
    public async Task MatchAsync_NullOnSuccess_ThrowsArgumentNullException()
    {
        Func<Task> act = async () => await new ValueTask<Result>(Result.Success())
            .MatchAsync(null!, _ => Task.FromResult(0));

        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("onSuccess");
    }

    [Fact]
    public async Task BindAsync_OnSuccess_ChainsResult()
    {
        var result = await Task.FromResult(Result.Success(4))
            .BindAsync(v => Task.FromResult(Result.Success(v + 6)));

        result.Value.Should().Be(10);
    }

    [Fact]
    public async Task TapAsync_OnSuccess_RunsActionAndReturnsSame()
    {
        var seen = 0;

        var returned = await new ValueTask<Result<int>>(Result.Success(8))
            .TapAsync(v => { seen = v; return Task.CompletedTask; });

        seen.Should().Be(8);
        returned.Value.Should().Be(8);
    }

    [Fact]
    public async Task BindAsync_OnValueTaskSuccess_ChainsWithoutAsTask()
    {
        var bound = await new ValueTask<Result<int>>(Result.Success(5))
            .BindAsync(v => Task.FromResult(Result.Success(v + 1)));

        bound.IsSuccess.Should().BeTrue();
        bound.Value.Should().Be(6);
    }

    [Fact]
    public async Task BindAsync_OnValueTaskFailure_PropagatesError()
    {
        var bound = await new ValueTask<Result<int>>(Result.Failure<int>(s_error))
            .BindAsync(v => Task.FromResult(Result.Success(v + 1)));

        bound.IsFailure.Should().BeTrue();
        bound.Error.Should().Be(s_error);
    }

    // ---- AC-3.8 / DE-7: Task<Result>/Task<Result<T>>-source MatchAsync/TapAsync overloads ----

    [Fact]
    public async Task MatchAsync_NonGeneric_TaskSource_ExecutesCorrectBranch()
    {
        var success = await Task.FromResult(Result.Success())
            .MatchAsync(() => Task.FromResult("ok"), _ => Task.FromResult("fail"));
        var failure = await Task.FromResult(Result.Failure(s_error))
            .MatchAsync(() => Task.FromResult("ok"), e => Task.FromResult(e.Code));

        success.Should().Be("ok");
        failure.Should().Be("Test.Error");
    }

    [Fact]
    public async Task MatchAsync_Generic_TaskSource_ExecutesCorrectBranch()
    {
        var value = await Task.FromResult(Result.Success(21))
            .MatchAsync(v => Task.FromResult(v * 2), _ => Task.FromResult(-1));

        value.Should().Be(42);
    }

    [Fact]
    public async Task TapAsync_TaskSource_OnSuccess_RunsActionAndReturnsSame()
    {
        var seen = 0;

        var returned = await Task.FromResult(Result.Success(8))
            .TapAsync(v => { seen = v; return Task.CompletedTask; });

        seen.Should().Be(8);
        returned.Value.Should().Be(8);
    }

    [Fact]
    public async Task TapAsync_TaskSource_OnFailure_DoesNotRunAction()
    {
        var ran = false;

        await Task.FromResult(Result.Failure<int>(s_error))
            .TapAsync(_ => { ran = true; return Task.CompletedTask; });

        ran.Should().BeFalse();
    }
}
