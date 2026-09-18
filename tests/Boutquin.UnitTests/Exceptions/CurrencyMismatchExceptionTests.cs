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
using Boutquin.Domain.Enumerations;

namespace Boutquin.UnitTests.Exceptions;

public sealed class CurrencyMismatchExceptionTests
{
    [Fact]
    public void CurrencyPairConstructor_SetsExpectedActualAndMessage()
    {
        var ex = new CurrencyMismatchException(Currency.USD, Currency.EUR);

        ex.Expected.Should().Be(Currency.USD);
        ex.Actual.Should().Be(Currency.EUR);
        ex.Message.Should().Contain("USD").And.Contain("EUR");
    }

    [Fact]
    public void MessageConstructor_SetsMessage_LeavesCurrenciesUnspecified()
    {
        var ex = new CurrencyMismatchException("boom");

        ex.Message.Should().Be("boom");
        ex.Expected.Should().Be(Currency.Unspecified);
        ex.Actual.Should().Be(Currency.Unspecified);
    }

    [Fact]
    public void MessageAndInnerConstructor_SetsBoth()
    {
        var inner = new InvalidOperationException("inner");

        var ex = new CurrencyMismatchException("boom", inner);

        ex.Message.Should().Be("boom");
        ex.InnerException.Should().BeSameAs(inner);
    }
}
