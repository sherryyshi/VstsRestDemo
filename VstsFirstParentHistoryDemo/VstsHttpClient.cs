//-----------------------------------------------------------------------
// <copyright file="VstsHttpClient.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;

namespace VstsRestDemo
{

    /// <summary>
    ///   Used for REST calls to the VSTS service that skip the SDK.
    /// </summary>
    public sealed class VstsHttpClient : VssHttpClientBase
    {
        /// <summary>
        /// One of several required constructors.
        /// </summary>
        /// <param name="collectionUri">The uri to use as a base</param>
        /// <param name="credentials">The credentials to use</param>
        public VstsHttpClient(Uri collectionUri, VssCredentials credentials)
            : base(collectionUri, credentials)
        {
        }

        /// <summary>
        /// One of several required constructors.
        /// </summary>
        /// <param name="baseUrl">The uri to use as a base</param>
        /// <param name="credentials">The credentials to use</param>
        /// <param name="settings">The settings to use</param>
        public VstsHttpClient(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings)
            : base(baseUrl, credentials, settings)
        {
        }

        /// <summary>
        /// One of several required constructors.
        /// </summary>
        /// <param name="baseUrl">The base url to use</param>
        /// <param name="credentials">The credentials to use</param>
        /// <param name="handlers">The handlers to use</param>
        public VstsHttpClient(Uri baseUrl, VssCredentials credentials, params DelegatingHandler[] handlers)
            : base(baseUrl, credentials, handlers)
        {
        }

        /// <summary>
        /// One of several required constructors.
        /// </summary>
        /// <param name="baseUrl">The base url to use</param>
        /// <param name="pipeline">The message pipeline</param>
        /// <param name="disposeHandler">Should the handler be disposed?</param>
        public VstsHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
            : base(baseUrl, pipeline, disposeHandler)
        {
        }

        /// <summary>
        /// One of several required constructors.
        /// </summary>
        /// <param name="baseUrl">The base url to use</param>
        /// <param name="credentials">The credentials to use</param>
        /// <param name="settings">The settings to use</param>
        /// <param name="handlers">The handlers</param>
        public VstsHttpClient(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings, params DelegatingHandler[] handlers)
            : base(baseUrl, credentials, settings, handlers)
        {
        }

        /// <summary>
        /// Call a web API method using the given verb and payload.
        /// </summary>
        /// <typeparam name="T">The type of result expected</typeparam>
        /// <param name="resourceUrl">The Url for the resource</param>
        /// <param name="verb">The Http verb to use</param>
        /// <param name="payload">The payload, if specified.</param>
        /// <returns>Deserialized data of type <typeparamref name="T"/></returns>
        public T CallApi<T>(
            string resourceUrl,
            HttpMethod verb = null,
            string payload = null)
        {
            if (verb == null)
            {
                verb = HttpMethod.Get;
            }

            var message = new HttpRequestMessage(verb, VssHttpUriUtility.ConcatUri(this.BaseAddress, resourceUrl));

            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (verb == HttpMethod.Post || verb.Method == "PATCH")
            {
                message.Content = new StringContent(payload, null, "application/json");
            }

            return this.SendAsync<T>(message).SyncResult();
        }

        /// <summary>
        /// Call a web API method using the given verb and payload.
        /// </summary>
        /// <typeparam name="T">The type of result expected</typeparam>
        /// <param name="resourceUrl">The Url for the resource</param>
        /// <param name="verb">The Http verb to use</param>
        /// <param name="payload">The payload, if specified.</param>
        /// <returns>Deserialized data of type <typeparamref name="T"/></returns>
        public T CallApi<T>(
            string resourceUrl,
            HttpMethod verb = null,
            object payload = null)
        {
            return this.CallApi<T>(resourceUrl, verb, payload == null ? null : JsonConvert.SerializeObject(payload));
        }
    }
}
