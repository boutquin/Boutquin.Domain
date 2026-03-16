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

using System.Net;

namespace Boutquin.Domain.Abstractions;

/// <summary>
/// Represents the type of an error, mapped to HTTP status codes for interoperability
/// with web APIs and mediator pipelines.
/// </summary>
/// <remarks>
/// Each value corresponds to an <see cref="HttpStatusCode"/> to enable consistent
/// error-to-HTTP-response mapping across application layers.
/// The <see cref="None"/> value (0) is used for backward compatibility with <see cref="Error.None"/>.
/// </remarks>
public enum ErrorType
{
    /// <summary>No error type specified. Used for backward compatibility with <see cref="Error.None"/>.</summary>
    None = 0,

    /// <summary>Maps to <see cref="HttpStatusCode.BadRequest"/> (400).</summary>
    BadRequest = (int)HttpStatusCode.BadRequest,

    /// <summary>Maps to <see cref="HttpStatusCode.Unauthorized"/> (401).</summary>
    Unauthorized = (int)HttpStatusCode.Unauthorized,

    /// <summary>Maps to <see cref="HttpStatusCode.PaymentRequired"/> (402).</summary>
    PaymentRequired = (int)HttpStatusCode.PaymentRequired,

    /// <summary>Maps to <see cref="HttpStatusCode.Forbidden"/> (403).</summary>
    Forbidden = (int)HttpStatusCode.Forbidden,

    /// <summary>Maps to <see cref="HttpStatusCode.NotFound"/> (404).</summary>
    NotFound = (int)HttpStatusCode.NotFound,

    /// <summary>Maps to <see cref="HttpStatusCode.MethodNotAllowed"/> (405).</summary>
    MethodNotAllowed = (int)HttpStatusCode.MethodNotAllowed,

    /// <summary>Maps to <see cref="HttpStatusCode.NotAcceptable"/> (406).</summary>
    NotAcceptable = (int)HttpStatusCode.NotAcceptable,

    /// <summary>Maps to <see cref="HttpStatusCode.ProxyAuthenticationRequired"/> (407).</summary>
    ProxyAuthenticationRequired = (int)HttpStatusCode.ProxyAuthenticationRequired,

    /// <summary>Maps to <see cref="HttpStatusCode.RequestTimeout"/> (408).</summary>
    RequestTimeout = (int)HttpStatusCode.RequestTimeout,

    /// <summary>Maps to <see cref="HttpStatusCode.Conflict"/> (409).</summary>
    Conflict = (int)HttpStatusCode.Conflict,

    /// <summary>Maps to <see cref="HttpStatusCode.Gone"/> (410).</summary>
    Gone = (int)HttpStatusCode.Gone,

    /// <summary>Maps to <see cref="HttpStatusCode.LengthRequired"/> (411).</summary>
    LengthRequired = (int)HttpStatusCode.LengthRequired,

    /// <summary>Maps to <see cref="HttpStatusCode.PreconditionFailed"/> (412).</summary>
    PreconditionFailed = (int)HttpStatusCode.PreconditionFailed,

    /// <summary>Maps to <see cref="HttpStatusCode.RequestEntityTooLarge"/> (413).</summary>
    RequestEntityTooLarge = (int)HttpStatusCode.RequestEntityTooLarge,

    /// <summary>Maps to <see cref="HttpStatusCode.RequestUriTooLong"/> (414).</summary>
    RequestUriTooLong = (int)HttpStatusCode.RequestUriTooLong,

    /// <summary>Maps to <see cref="HttpStatusCode.UnsupportedMediaType"/> (415).</summary>
    UnsupportedMediaType = (int)HttpStatusCode.UnsupportedMediaType,

    /// <summary>Maps to <see cref="HttpStatusCode.RequestedRangeNotSatisfiable"/> (416).</summary>
    RequestedRangeNotSatisfiable = (int)HttpStatusCode.RequestedRangeNotSatisfiable,

    /// <summary>Maps to <see cref="HttpStatusCode.ExpectationFailed"/> (417).</summary>
    ExpectationFailed = (int)HttpStatusCode.ExpectationFailed,

    /// <summary>Maps to <see cref="HttpStatusCode.TooManyRequests"/> (429).</summary>
    TooManyRequests = (int)HttpStatusCode.TooManyRequests,

    /// <summary>Maps to <see cref="HttpStatusCode.RequestHeaderFieldsTooLarge"/> (431).</summary>
    RequestHeaderFieldsTooLarge = (int)HttpStatusCode.RequestHeaderFieldsTooLarge,

    /// <summary>Maps to <see cref="HttpStatusCode.InternalServerError"/> (500).</summary>
    InternalServerError = (int)HttpStatusCode.InternalServerError,

    /// <summary>Maps to <see cref="HttpStatusCode.NotImplemented"/> (501).</summary>
    NotImplemented = (int)HttpStatusCode.NotImplemented,

    /// <summary>Maps to <see cref="HttpStatusCode.BadGateway"/> (502).</summary>
    BadGateway = (int)HttpStatusCode.BadGateway,

    /// <summary>Maps to <see cref="HttpStatusCode.ServiceUnavailable"/> (503).</summary>
    ServiceUnavailable = (int)HttpStatusCode.ServiceUnavailable,

    /// <summary>Maps to <see cref="HttpStatusCode.GatewayTimeout"/> (504).</summary>
    GatewayTimeout = (int)HttpStatusCode.GatewayTimeout,

    /// <summary>Maps to <see cref="HttpStatusCode.HttpVersionNotSupported"/> (505).</summary>
    HttpVersionNotSupported = (int)HttpStatusCode.HttpVersionNotSupported,

    /// <summary>Maps to <see cref="HttpStatusCode.InsufficientStorage"/> (507).</summary>
    InsufficientStorage = (int)HttpStatusCode.InsufficientStorage,
}
