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

namespace Boutquin.UnitTests.AspNetCore;

using Boutquin.AspNetCore;
using Microsoft.AspNetCore.Builder;

/// <summary>
/// Contains unit tests for the <see cref="CustomExceptionHandlerMiddlewareExtensions"/> class.
/// </summary>
public sealed class CustomExceptionHandlerMiddlewareExtensionsTests
{
    [Fact]
    public void UseCustomExceptionHandler_ReturnsApplicationBuilder()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();

        // Act
        var result = app.UseCustomExceptionHandler();

        // Assert
        result.Should().NotBeNull();
    }
}
