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

namespace Boutquin.UnitTests.Validation;

using Boutquin.Validation.Exceptions;
using FluentValidation.Results;

/// <summary>
/// Contains unit tests for the <see cref="ValidationException"/> class.
/// </summary>
public sealed class ValidationExceptionTests
{
    /// <summary>
    /// Tests that the constructor stores validation failures in the Errors property.
    /// </summary>
    [Fact]
    public void Constructor_WithFailures_StoresErrorsProperty()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new("Name", "Name is required."),
            new("Age", "Age must be positive.")
        };

        // Act
        var exception = new ValidationException(failures);

        // Assert
        exception.Errors.Should().HaveCount(2);
        exception.Errors.Should().Contain(f => f.PropertyName == "Name");
        exception.Errors.Should().Contain(f => f.PropertyName == "Age");
    }

    /// <summary>
    /// Tests that the Message contains the concatenated error messages.
    /// </summary>
    [Fact]
    public void Message_WithSingleFailure_ContainsErrorMessage()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new("Email", "Email is invalid.")
        };

        // Act
        var exception = new ValidationException(failures);

        // Assert
        exception.Message.Should().Contain("Email is invalid.");
    }

    /// <summary>
    /// Tests that multiple failures produce a multi-line message.
    /// </summary>
    [Fact]
    public void Message_WithMultipleFailures_ProducesMultiLineMessage()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new("Name", "Name is required."),
            new("Age", "Age must be positive.")
        };

        // Act
        var exception = new ValidationException(failures);

        // Assert
        exception.Message.Should().Contain("Name is required.");
        exception.Message.Should().Contain("Age must be positive.");
        exception.Message.Should().Contain(Environment.NewLine);
    }
}
