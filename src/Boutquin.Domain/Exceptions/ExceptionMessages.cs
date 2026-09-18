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

namespace Boutquin.Domain.Exceptions;

/// <summary>
/// Contains constants for exception messages.
/// </summary>
public static class ExceptionMessages
{
    /// <summary>
    /// The message used when a boundary guard rejects an array argument that is
    /// <see langword="null"/> or empty. Carried by <see cref="EmptyOrNullArrayException"/> so the
    /// rejection reads consistently wherever array input is validated.
    /// </summary>
    public const string EmptyOrNullArray = "Input array must not be empty or null.";

    /// <summary>
    /// The default message carried by <see cref="EmptyOrNullCollectionException"/> when a collection
    /// argument is rejected for being <see langword="null"/> or empty. Mirrors
    /// <see cref="EmptyOrNullArray"/> so collection rejection reads consistently.
    /// </summary>
    public const string EmptyOrNullCollection = "Input collection must not be empty or null.";

    /// <summary>
    /// The default message carried by <see cref="EmptyOrNullDictionaryException"/> when a dictionary
    /// argument is rejected for being <see langword="null"/> or empty. Uses the same rejection-message
    /// structure as <see cref="EmptyOrNullArray"/> and <see cref="EmptyOrNullCollection"/> so all three
    /// read consistently.
    /// </summary>
    public const string EmptyOrNullDictionary = "Input dictionary must not be empty or null.";

    /// <summary>
    /// The message used when a <i>sample</i> statistic is requested for fewer than two elements.
    /// Carried by <see cref="InsufficientDataException"/>: sample variance and standard deviation
    /// divide by <c>n - 1</c> (Bessel's correction), which is undefined for a single observation.
    /// </summary>
    public const string InsufficientDataForSampleCalculation = "Input array must have at least two elements for sample calculation.";

    // ── Default messages for the HTTP client-error (4xx) DomainException family ─────────────────
    // Each message-less constructor in the 4xx DomainException family carries the matching constant
    // below so that Exception.Message never falls back to the .NET framework default —
    // "Exception of type '<namespace>.<TypeName>' was thrown." — which embeds the internal
    // fully-qualified type name. Because CustomExceptionHandlerMiddleware forwards 4xx messages to
    // the client verbatim (server-side 5xx messages are suppressed instead), a leaked framework
    // default would disclose the internal type name in the RFC 7807 response body. These phrases are
    // deliberately generic — safe to surface to any API consumer — and double as sensible defaults
    // wherever a message-less 4xx exception is logged or inspected.

    /// <summary>
    /// The default message carried by <see cref="BadRequestException"/> (HTTP 400) when no
    /// caller-supplied message is provided. A safe, client-facing phrase that discloses no internal
    /// detail.
    /// </summary>
    public const string BadRequest = "The request was invalid.";

    /// <summary>
    /// The default message carried by <see cref="UnauthorizedException"/> (HTTP 401) when no
    /// caller-supplied message is provided. A safe, client-facing phrase that discloses no internal
    /// detail.
    /// </summary>
    public const string Unauthorized = "Authentication is required to access this resource.";

    /// <summary>
    /// The default message carried by <see cref="ForbiddenException"/> (HTTP 403) when no
    /// caller-supplied message is provided. A safe, client-facing phrase that discloses no internal
    /// detail.
    /// </summary>
    public const string Forbidden = "Access to this resource is forbidden.";

    /// <summary>
    /// The default message carried by <see cref="NotFoundException"/> (HTTP 404) when no
    /// caller-supplied message is provided. A safe, client-facing phrase that discloses no internal
    /// detail.
    /// </summary>
    public const string NotFound = "The requested resource was not found.";

    /// <summary>
    /// The default message carried by <see cref="ConflictException"/> (HTTP 409) when no
    /// caller-supplied message is provided. A safe, client-facing phrase that discloses no internal
    /// detail.
    /// </summary>
    public const string Conflict = "The request conflicts with the current state of the resource.";

    /// <summary>
    /// The default message carried by <see cref="UnsupportedMediaTypeException"/> (HTTP 415) when no
    /// caller-supplied message is provided. A safe, client-facing phrase that discloses no internal
    /// detail.
    /// </summary>
    public const string UnsupportedMediaType = "The request media type is not supported.";

    /// <summary>
    /// The default message carried by <see cref="UnprocessableEntityException"/> (HTTP 422) when no
    /// caller-supplied message is provided. A safe, client-facing phrase that discloses no internal
    /// detail.
    /// </summary>
    public const string UnprocessableEntity = "The request was well-formed but could not be processed.";

    /// <summary>
    /// The default message carried by <see cref="TooManyRequestsException"/> (HTTP 429) when no
    /// caller-supplied message is provided. A safe, client-facing phrase that discloses no internal
    /// detail.
    /// </summary>
    public const string TooManyRequests = "Too many requests have been sent in a given amount of time.";
}
