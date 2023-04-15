// Copyright (c) 2023 Pierre G. Boutquin. All rights reserved.
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
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using Boutquin.Domain.Helpers;

namespace Boutquin.UnitTests.Domain
{
    /// <summary>
    /// Test class for the Guard class methods.
    /// </summary>
    public sealed class GuardTests
    {
        /// <summary>
        /// Tests that the Guard.AgainstNull method throws an ArgumentNullException when the value is null.
        /// </summary>
        [Fact]
        public void AgainstNull_WhenValueIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            string? nullValue = null;

            // Act
#pragma warning disable CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.
            Action act = () => Guard.AgainstNull(nullValue, nameof(nullValue));
#pragma warning restore CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage($"Parameter '{nameof(nullValue)}' cannot be null. (Parameter '{nameof(nullValue)}')")
                .And.ParamName.Should().Be(nameof(nullValue));
        }

        /// <summary>
        /// Tests that the Guard.AgainstNull method does not throw an exception when the value is not null.
        /// </summary>
        [Fact]
        public void AgainstNull_WhenValueIsNotNull_DoesNotThrow()
        {
            // Arrange
            var nonNullValue = "Some value";

            // Act
            Action act = () => Guard.AgainstNull(nonNullValue, nameof(nonNullValue));

            // Assert
            act.Should().NotThrow();
        }

        /// <summary>
        /// Tests that the Guard.AgainstNullOrEmpty method throws an ArgumentException when the value is null.
        /// </summary>
        [Fact]
        public void AgainstNullOrEmpty_WhenValueIsNull_ThrowsArgumentException()
        {
            // Arrange
            string? nullValue = null;

            // Act
#pragma warning disable CS8603 // Possible null reference return.
            Action act = () => Guard.AgainstNullOrEmpty(() => nullValue);
#pragma warning restore CS8603 // Possible null reference return.

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage($"Parameter '{nameof(nullValue)}' cannot be null or empty. (Parameter '{nameof(nullValue)}')")
                .And.ParamName.Should().Be(nameof(nullValue));
        }

        /// <summary>
        /// Tests that the Guard.AgainstNullOrEmpty method throws an ArgumentException when the value is an empty string.
        /// </summary>
        [Fact]
        public void AgainstNullOrEmpty_WhenValueIsEmpty_ThrowsArgumentException()
        {
            // Arrange
            var emptyValue = string.Empty;

            // Act
            Action act = () => Guard.AgainstNullOrEmpty(() => emptyValue);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage($"Parameter '{nameof(emptyValue)}' cannot be null or empty. (Parameter '{nameof(emptyValue)}')")
                .And.ParamName.Should().Be(nameof(emptyValue));
        }

        /// <summary>
        /// Tests that the Guard.AgainstNullOrEmpty method does not throw an exception when the value is a non-empty string.
        /// </summary>
        [Fact]
        public void AgainstNullOrEmpty_WhenValueIsNotEmpty_DoesNotThrow()
        {
            // Arrange
            var nonEmptyValue = "Some value";

            // Act
            Action act = () => Guard.AgainstNullOrEmpty(() => nonEmptyValue);

            // Assert
            act.Should().NotThrow();
        }
    }
}
