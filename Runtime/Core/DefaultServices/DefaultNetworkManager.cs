using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Utils.Logs;
using Core.Utils;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Core.DefaultServices
{
    /// <summary>
    /// Default UnityWebRequest-based implementation of <see cref="INetworkManager"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This implementation:
    /// </para>
    /// <list type="bullet">
    ///   <item>Builds URLs from a base URL and endpoint map</item>
    ///   <item>Sends JSON POST requests using <see cref="UnityWebRequest"/></item>
    ///   <item>Deserializes responses using Newtonsoft.Json</item>
    ///   <item>Provides a basic, customizable <see cref="INetworkManager.ApiStatus"/> mapping</item>
    /// </list>
    /// </remarks>
    public class DefaultNetworkManager : INetworkManager
    {
        private readonly string _baseUrl;
        private readonly Dictionary<string, string> _endpointMap;
        private bool _isOnline = true;

        /// <summary>
        /// Creates a new <see cref="DefaultNetworkManager"/>.
        /// </summary>
        /// <param name="baseUrl">
        /// The base URL for your backend, e.g. <c>"https://api.mygame.com/v1"</c>.
        /// Should not include a trailing slash.
        /// </param>
        /// <param name="endpointMap">
        /// Optional mapping from logical endpoint names (e.g. "login") to relative paths
        /// (e.g. <c>"/auth/login"</c>). If <c>null</c>, the endpoint name is appended
        /// directly to <paramref name="baseUrl"/>.
        /// </param>
        public DefaultNetworkManager(string baseUrl, Dictionary<string, string> endpointMap = null)
        {
            _baseUrl = baseUrl?.TrimEnd('/');
            _endpointMap = endpointMap ?? new Dictionary<string, string>();
        }

        /// <inheritdoc />
        public string GetAPIUrl(string endPointName)
        {
            if (string.IsNullOrEmpty(endPointName))
                throw new ArgumentNullException(nameof(endPointName));

            // If caller already passed a full URL, just use it.
            if (endPointName.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                endPointName.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return endPointName;
            }

            if (string.IsNullOrEmpty(_baseUrl))
                throw new InvalidOperationException("Base URL is not configured for DefaultNetworkManager.");

            if (_endpointMap.TryGetValue(endPointName, out var path))
            {
                return $"{_baseUrl}/{path.TrimStart('/')}";
            }

            // Fallback: just append endpoint name directly.
            return $"{_baseUrl}/{endPointName.TrimStart('/')}";
        }

        /// <inheritdoc />
        public void OnConnectionStatusChanged(bool connectionStatus)
        {
            if (_isOnline == connectionStatus)
                return;

            _isOnline = connectionStatus;
            Logger.Log($"[Network] Connection status changed → {(_isOnline ? "Online" : "Offline")}");
        }

        /// <inheritdoc />
        public INetworkManager.ApiStatus GetApiStatus(JSONObject responseJSON)
        {
            if (responseJSON == null)
            {
                return INetworkManager.ApiStatus.Unknown;
            }

            // NOTE:
            // This is intentionally minimal because JSONObject’s API and your backend
            // response shape are project-specific.
            //
            // Recommended pattern:
            // - Look for a numeric "statusCode" or string "status"
            // - Or look for a "success" boolean / "errorCode" field, etc.
            //
            // Example (pseudo-code; adjust to your JSONObject implementation):
            //
            // if (responseJSON.HasField("statusCode"))
            // {
            //     int code = (int)responseJSON["statusCode"].n;
            //     return MapStatusCode(code);
            // }
            //
            // For now we assume success by default, and you can customize later.
            return INetworkManager.ApiStatus.OK;
        }

        /// <inheritdoc />
        public async Task<TResponse> SendAsync<TRequest, TResponse>(string endpoint, TRequest request)
        {
            if (!_isOnline)
            {
                Logger.LogWarning($"[Network] Attempting to send request to '{endpoint}' while offline.");
                // You might want to throw, return default, or queue for later.
            }

            string url = GetAPIUrl(endpoint);

            // Serialize request body to JSON
            string jsonBody = request != null
                ? JsonConvert.SerializeObject(request)
                : "{}";

            var bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

            using var req = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST)
            {
                uploadHandler = new UploadHandlerRaw(bodyRaw),
                downloadHandler = new DownloadHandlerBuffer()
            };

            req.SetRequestHeader("Content-Type", "application/json");

            Logger.Log($"[Network] Sending POST {url} : {jsonBody}");

            UnityWebRequestAsyncOperation op = req.SendWebRequest();

            // Await until the request is done, without blocking the main thread.
            while (!op.isDone)
            {
                await Task.Yield();
            }

#if UNITY_2020_1_OR_NEWER
            bool success = req.result == UnityWebRequest.Result.Success;
#else
            bool success = !req.isNetworkError && !req.isHttpError;
#endif

            string responseText = req.downloadHandler.text;
            Logger.Log($"[Network] Response from {url}: {responseText}");

            if (!success)
            {
                Logger.LogError($"[Network] Request to {url} failed: {req.responseCode} {req.error}");
                // You may want to map HTTP status code to ApiStatus here,
                // or throw a more specific exception type.
                throw new Exception($"Network error ({req.responseCode}): {req.error}");
            }

            // Optionally, analyze JSON with JSONObject + GetApiStatus
            JSONObject responseJsonObject = null;
            try
            {
                if (!string.IsNullOrEmpty(responseText))
                {
                    // Assumes your JSONObject has a constructor that accepts a JSON string.
                    responseJsonObject = new JSONObject(responseText);
                    var status = GetApiStatus(responseJsonObject);

                    if (status != INetworkManager.ApiStatus.OK)
                    {
                        Logger.LogWarning($"[Network] API reported non-OK status: {status}");
                        // You could throw here or wrap this status in a custom exception.
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogWarning($"[Network] Failed to parse JSONObject or evaluate ApiStatus: {e}");
            }

            // Deserialize typed response
            if (string.IsNullOrWhiteSpace(responseText))
            {
                // No content but request succeeded: return default(TResponse).
                return default;
            }

            try
            {
                var response = JsonConvert.DeserializeObject<TResponse>(responseText);
                return response;
            }
            catch (Exception e)
            {
                Logger.LogError($"[Network] Failed to deserialize response into {typeof(TResponse).Name}: {e}");
                throw;
            }
        }

        /// <summary>
        /// Optional helper to map HTTP status codes to <see cref="INetworkManager.ApiStatus"/>.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <returns>A best-effort mapping to <see cref="INetworkManager.ApiStatus"/>.</returns>
        protected INetworkManager.ApiStatus MapStatusCode(long statusCode)
        {
            switch (statusCode)
            {
                case 200:
                case 201:
                case 204:
                    return INetworkManager.ApiStatus.OK;
                case 400:
                    return INetworkManager.ApiStatus.BadRequest;
                case 401:
                    return INetworkManager.ApiStatus.Unauthorized;
                case 403:
                    return INetworkManager.ApiStatus.Forbidden;
                case 409:
                    return INetworkManager.ApiStatus.Conflict;
                case 417:
                    return INetworkManager.ApiStatus.ExpectationFailed;
                case 500:
                case 502:
                case 503:
                    return INetworkManager.ApiStatus.AuthServiceError;
                default:
                    return INetworkManager.ApiStatus.Unknown;
            }
        }
    }
}
