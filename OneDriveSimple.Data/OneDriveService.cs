using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OneDriveSimple.Request;
using OneDriveSimple.Response;

namespace OneDriveSimple
{
    // https://dev.onedrive.com/misc/
    // Documentación: https://msdn.microsoft.com/magazine/mt614268  Revista MSDN Enero 2016

    public class OneDriveService
    {
        private const string AccessTokenIsNotSet = "AccessToken is not set";
        private const string AccessTokenKey = "access_token";
        private const string Bearer = "Bearer";

        private const string RedirectUrl = "https://login.live.com/oauth20_desktop.srf";
        private const string RequestChildren = "/drive/items/{0}/children";
        private const string RequestItem = "/drive/items/{0}";
        private const string RequestLink = "/drive/items/{0}/action.createLink";
        private const string RequestLogOut = "https://login.live.com/oauth20_logout.srf?client_id={0}&redirect_uri={1}";
        private const string RequestPutFile = "/drive/items/{0}:/{1}:/content";
        private const string RequestRootFolder = "/drive/root";
        private const string RequestSpecialFolder = "/drive/special/{0}";
        private const string RequestSubfolder = "/drive/root:/{0}";
        private const string RootUrl = "https://api.onedrive.com/v1.0";

        private const string ScopeString = "wl.signin onedrive.readwrite onedrive.appfolder"; // Hard coded here
        private const string StartQuery = "?client_id={0}&scope={1}&response_type={2}&redirect_uri={3}";
        private const string StartUrl = "https://login.live.com/oauth20_authorize.srf";
        private const string TokenMode = "token";

        private readonly string _clientId;
        private Action _errorCallback;
        private Action _successCallback;

        public string AccessToken
        {
            get;
            private set;
        }

        public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken);

        public OneDriveService(string clientId)
        {
            _clientId = clientId;
        }

        public bool CheckAuthenticate(Action successCallback, Action errorCallback)
        {
            if (IsAuthenticated)
            {
                successCallback();
                return true;
            }

            _successCallback = successCallback;
            _errorCallback = errorCallback;
            return false;
        }

        public bool CheckRedirectUrl(string redirectUrl)
        {
            return redirectUrl.StartsWith(RedirectUrl);
        }

        public void ContinueGetTokens(Uri redirectUri)
        {
            if (redirectUri == null)
            {
                _errorCallback?.Invoke();
                return;
            }

            var ok = GetTokensTokenMode(redirectUri.Fragment);

            if (ok)
            {
                _successCallback?.Invoke();
            }
            else
            {
                _errorCallback?.Invoke();
            }
        }

        // /drive/special/approot
        public async Task<ItemInfoResponse> GetAppRoot()
        {
            return await GetSpecialFolder(SpecialFolder.AppRoot);
        }

        public async Task<ItemInfoResponse> GetItem(string path)
        {
            if (string.IsNullOrEmpty(AccessToken))
            {
                throw new InvalidOperationException(AccessTokenIsNotSet);
            }

            path = path.Replace("\\", "/");

            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }

            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue(Bearer, AccessToken);

            var request = string.Format(RequestSubfolder, path);

            var uri = new Uri(RootUrl + request);
            var json = await client.GetStringAsync(uri);
            var result = JsonConvert.DeserializeObject<ItemInfoResponse>(json);
            return result;
        }
        public async Task<ItemInfoResponse> GetItem(string parentId, string fileName)
        {
            if (string.IsNullOrEmpty(AccessToken))
            {
                throw new InvalidOperationException(AccessTokenIsNotSet);
            }
            
            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue(Bearer, AccessToken);

            var uri = new Uri(
                RootUrl
                + string.Format("/drive/items/{0}:/{1}", parentId, fileName));

            var json = await client.GetStringAsync(uri);

            var result = JsonConvert.DeserializeObject<ItemInfoResponse>(json);
            return result;
        }

        public async Task<LinkResponseInfo> GetLink(LinkKind kind, string fileId)
        {
            // LinkKind is View or Edit

            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue(Bearer, AccessToken);

            var request = string.Format(RequestLink, fileId);
            var uri = new Uri(RootUrl + request);

            var requestJson = JsonConvert.SerializeObject(
                new RequestLinkInfo
                {
                    Type = kind.ToString().ToLower()
                });

            var content = new StringContent(
                requestJson,
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(uri, content);

            var result = JsonConvert.DeserializeObject<LinkResponseInfo>(
                await response.Content.ReadAsStringAsync());
            return result;
        }

        public async Task<ItemInfoResponse> GetRootFolder()
        {
            if (string.IsNullOrEmpty(AccessToken))
            {
                throw new InvalidOperationException(AccessTokenIsNotSet);
            }

            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue(Bearer, AccessToken);

            var uri = new Uri(
                RootUrl + RequestRootFolder);
            var json = await client.GetStringAsync(uri);
            var result = JsonConvert.DeserializeObject<ItemInfoResponse>(json);
            return result;
        }

        // /drive/special/{special}
        public async Task<ItemInfoResponse> GetSpecialFolder(SpecialFolder kind)
        {
            if (kind == SpecialFolder.None)
            {
                throw new ArgumentException("Please use a value other than None", nameof(kind));
            }

            if (string.IsNullOrEmpty(AccessToken))
            {
                throw new InvalidOperationException(AccessTokenIsNotSet);
            }

            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization 
                = new AuthenticationHeaderValue(Bearer, AccessToken);

            var request = string.Format(RequestSpecialFolder, kind);
            var uri = new Uri(RootUrl + request);
            var json = await client.GetStringAsync(uri);

            var response = JsonConvert.DeserializeObject<ItemInfoResponse>(json);
            return response;
        }

        public Uri GetStartUri()
        {
            var startQuery = string.Format(
                StartQuery,
                _clientId,
                WebUtility.UrlEncode(ScopeString),
                TokenMode,
                WebUtility.UrlEncode(RedirectUrl));

            var url = StartUrl + startQuery;
            return new Uri(url);
        }

        // https://login.live.com/oauth20_logout.srf?client_id={client_id}&redirect_uri={redirect_uri}
        public async Task Logout()
        {
            if (AccessToken == null)
            {
                return;
            }

            var client = new HttpClient();
            var request = string.Format(RequestLogOut, _clientId, RedirectUrl);
            await client.GetAsync(request);
            AccessToken = null;
        }

        public async Task<IList<ItemInfoResponse>> PopulateChildren(ItemInfoResponse info)
        {
            if (string.IsNullOrEmpty(AccessToken))
            {
                throw new InvalidOperationException(AccessTokenIsNotSet);
            }

            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue(Bearer, AccessToken);

            var request = string.Format(RequestChildren, info.Id);
            var uri = new Uri(RootUrl + request);
            var json = await client.GetStringAsync(uri);

            var response = JsonConvert.DeserializeObject<ParseChildrenResponse>(json);
            return response.Value;
        }

        public async Task<Stream> RefreshAndDownloadContent(
            ItemInfoResponse model,
            bool refreshFirst)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue(Bearer, AccessToken);

            // Refresh the item's information
            if (refreshFirst)
            {
                var request = string.Format(RequestItem, model.Id);
                var uri = new Uri(RootUrl + request);
                var json = await client.GetStringAsync(uri);
                var refreshedItem = JsonConvert.DeserializeObject<ItemInfoResponse>(json);
                model.DownloadUrl = refreshedItem.DownloadUrl;
            }

            var response = await client.GetAsync(model.DownloadUrl);
            var stream = await response.Content.ReadAsStreamAsync();
            return stream;
        }

        // /drive/items/{parent-id}:/{filename}:/content
        public async Task<ItemInfoResponse> SaveFile(string parentId, string fileName, Stream stream)
        {
            if (string.IsNullOrEmpty(AccessToken))
            {
                throw new InvalidOperationException(AccessTokenIsNotSet);
            }

            var content = new StreamContent(stream);
            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue(Bearer, AccessToken);

            var uri = new Uri(
                RootUrl
                + string.Format(RequestPutFile, parentId, fileName));

            var response = await client.PutAsync(uri, content);
            var json = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<ItemInfoResponse>(json);
            return result;
        }

        /// <summary>
        /// Use this method for Token Flow
        /// </summary>
        private bool GetTokensTokenMode(string fragment)
        {
            if (fragment.StartsWith("#"))
            {
                fragment = fragment.Substring(1);
            }

            var fragmentParts = fragment.Split('&');

            foreach (var part in fragmentParts)
            {
                var partParts = part.Split('=');

                if (partParts.Length < 2)
                {
                    return false;
                }

                switch (partParts[0])
                {
                    case AccessTokenKey:
                        AccessToken = partParts[1];
                        break;
                }

                if (!string.IsNullOrEmpty(AccessToken))
                {
                    break;
                }
            }

            return !string.IsNullOrEmpty(AccessToken);
        }
    }
}