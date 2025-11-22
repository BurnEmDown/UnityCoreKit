using System.Threading.Tasks;
using Core.Utils;

namespace Core.Interfaces
{
    /// <summary>
    /// Defines the contract for all high-level network operations, including:
    /// endpoint resolution, API request dispatching, connection-state reporting,
    /// and standardized interpretation of server responses.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This interface abstracts networking logic away from gameplay code,
    /// ensuring that the rest of the project does not depend on any specific
    /// transport layer (UnityWebRequest, HttpClient, WebSockets, etc.).
    /// </para>
    ///
    /// <para>
    /// Implementations are responsible for:
    /// </para>
    /// <list type="bullet">
    ///   <item>Constructing full URLs based on endpoint identifiers</item>
    ///   <item>Sending typed asynchronous requests</item>
    ///   <item>Interpreting server responses and mapping them to <see cref="ApiStatus"/></item>
    ///   <item>Handling authorization, token refresh, offline mode, and error mapping</item>
    /// </list>
    ///
    /// <para>
    /// Because the transport is abstracted, this interface can support:
    /// </para>
    /// <list type="bullet">
    ///   <item>REST APIs</item>
    ///   <item>Firebase Cloud Functions</item>
    ///   <item>GraphQL</item>
    ///   <item>Local mock servers for testing</item>
    /// </list>
    /// </remarks>
    public interface INetworkManager
    {
        /// <summary>
        /// Resolves and returns a fully qualified URL for the given endpoint identifier.
        /// </summary>
        /// <param name="endPointName">
        /// A logical or symbolic name for a backend endpoint (e.g., "login", "get_inventory").
        /// </param>
        /// <returns>
        /// A fully qualified API URL (e.g., <c>https://api.mygame.com/v1/login</c>).
        /// </returns>
        /// <remarks>
        /// Implementations commonly use:
        /// <list type="bullet">
        ///   <item>Configuration dictionaries</item>
        ///   <item>ScriptableObject mappings</item>
        ///   <item>Remote Config values</item>
        ///   <item>Environment-based routing (dev, staging, production)</item>
        /// </list>
        /// </remarks>
        string GetAPIUrl(string endPointName);

        /// <summary>
        /// Called when the application's perceived network connectivity status changes.
        /// </summary>
        /// <param name="connectionStatus">
        /// <c>true</c> if the device is considered online; 
        /// <c>false</c> if offline mode is detected.
        /// </param>
        /// <remarks>
        /// Useful for:
        /// <list type="bullet">
        ///   <item>Displaying "no internet connection" UI</item>
        ///   <item>Queuing or retrying requests</item>
        ///   <item>Pausing features that require a live connection</item>
        /// </list>
        /// </remarks>
        void OnConnectionStatusChanged(bool connectionStatus);

        /// <summary>
        /// Interprets a backend JSON response and maps it to a standardized
        /// <see cref="ApiStatus"/> value.
        /// </summary>
        /// <param name="responseJSON">The raw server response payload.</param>
        /// <returns>
        /// A mapped <see cref="ApiStatus"/> describing the high-level result of the request.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method avoids duplicating error-handling logic across the codebase,
        /// enabling all network consumers to rely on a unified interpretation of
        /// server responses.
        /// </para>
        ///
        /// <para>
        /// Implementations often inspect fields such as:
        /// <c>status</c>, <c>errorCode</c>, <c>success</c>, <c>message</c>, etc.
        /// </para>
        /// </remarks>
        ApiStatus GetApiStatus(JSONObject responseJSON);

        /// <summary>
        /// Sends a typed API request asynchronously and returns a typed response.
        /// </summary>
        /// <typeparam name="TRequest">The request data type to serialize.</typeparam>
        /// <typeparam name="TResponse">The expected response type to deserialize.</typeparam>
        /// <param name="endpoint">The backend endpoint name or fully qualified URL.</param>
        /// <param name="request">The request payload object.</param>
        /// <returns>
        /// A <see cref="Task{TResponse}"/> completing with the deserialized response object.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method enables clean, strongly typed API calls such as:
        /// </para>
        /// <code>
        /// var result = await network.SendAsync&lt;LoginRequest, LoginResponse&gt;(
        ///     "login",
        ///     new LoginRequest { username = "bob", password = "pw123" });
        /// </code>
        ///
        /// <para>
        /// The implementation is responsible for:
        /// </para>
        /// <list type="bullet">
        ///   <item>Resolving the URL via <see cref="GetAPIUrl(string)"/></item>
        ///   <item>Serializing <typeparamref name="TRequest"/> to JSON</item>
        ///   <item>Sending the HTTP request</item>
        ///   <item>Handling connection or authorization errors</item>
        ///   <item>Deserializing <typeparamref name="TResponse"/> from JSON</item>
        /// </list>
        /// </remarks>
        Task<TResponse> SendAsync<TRequest, TResponse>(string endpoint, TRequest request);

        /// <summary>
        /// High-level representation of typical API results.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This enum is designed to represent logical result categories rather than raw
        /// HTTP status codes.  
        /// It simplifies error-handling logic throughout the game by enabling
        /// centralized mapping based on backend conventions.
        /// </para>
        ///
        /// <para>
        /// For example, an implementation may map:
        /// </para>
        /// <list type="bullet">
        ///   <item>HTTP 200 + <c>{ "success": true }</c> → <see cref="OK"/></item>
        ///   <item>HTTP 401 → <see cref="Unauthorized"/></item>
        ///   <item>HTTP 409 → <see cref="Conflict"/></item>
        ///   <item>HTTP 403 → <see cref="Forbidden"/></item>
        ///   <item>Unknown or malformed responses → <see cref="Unknown"/></item>
        /// </list>
        /// </remarks>
        public enum ApiStatus
        {
            /// <summary>Request completed successfully.</summary>
            OK,

            /// <summary>User is not authorized (e.g., invalid or expired token).</summary>
            Unauthorized,

            /// <summary>Request failed because the account or action is unverified.</summary>
            Unverified,

            /// <summary>Client sent invalid or malformed data.</summary>
            BadRequest,

            /// <summary>User lacks permission to perform this action.</summary>
            Forbidden,

            /// <summary>Server indicates a conflicting state (e.g., already exists).</summary>
            Conflict,

            /// <summary>Request failed due to unmet preconditions or expectations.</summary>
            ExpectationFailed,

            /// <summary>Authentication provider or identity service error.</summary>
            AuthServiceError,

            /// <summary>Any unexpected or unmapped result.</summary>
            Unknown
        }
    }
}