using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Threading.Tasks;
using System.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Web.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security;

namespace SEWC_ToolBox.Controllers
{
    public class AccountController : Controller
    {

        private readonly static HttpClient _httpClient = new HttpClient();

        // GET: Account
        [AllowAnonymous]
        public async Task<ActionResult> Callback()
        {
            #region
            try
            {
                var code = Request.Form["code"];
                var state = Request.Form["state"];
                var authUrl = ConfigurationManager.AppSettings["Authority"].ToString();
                var callbackUrl = ConfigurationManager.AppSettings["CallbackUrl"].ToString();
                var clientId = ConfigurationManager.AppSettings["ClientId"].ToString();
                var clientSecret = ConfigurationManager.AppSettings["ClientSecret"].ToString();


                // request the configuration
                var infoResponse = await _httpClient.GetAsync(authUrl + "/.well-known/openid-configuration");
                infoResponse.EnsureSuccessStatusCode();
                var infoStr = await infoResponse.Content.ReadAsStringAsync();
                dynamic configResponse = JsonConvert.DeserializeObject<dynamic>(infoStr);
                var tokenEndpoint = (string)configResponse.token_endpoint;
                var userEndpoint = (string)configResponse.userinfo_endpoint;


                // Build up the body for the token request
                var body = new List<KeyValuePair<string, string>>();
                body.Add(new KeyValuePair<string, string>("client_id", clientId));
                body.Add(new KeyValuePair<string, string>("client_secret", clientSecret));
                body.Add(new KeyValuePair<string, string>("code", code));
                body.Add(new KeyValuePair<string, string>("redirect_uri", callbackUrl));
                body.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));

                // Request the token
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                requestMessage.Content = new FormUrlEncodedContent(body);
                HttpResponseMessage tokenResponse = await _httpClient.SendAsync(requestMessage);
                tokenResponse.EnsureSuccessStatusCode();
                string text = await tokenResponse.Content.ReadAsStringAsync();
                // Deserializes the token response
                dynamic responseAccess = JsonConvert.DeserializeObject<dynamic>(text);
                string accessToken = (string)responseAccess.access_token;

                // Get the user
                HttpRequestMessage userRequest = new HttpRequestMessage(HttpMethod.Post, userEndpoint);
                var requestInfoBody = new List<KeyValuePair<string, string>>();
                requestInfoBody.Add(new KeyValuePair<string, string>("access_token", accessToken));
                userRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                userRequest.Content = new FormUrlEncodedContent(requestInfoBody);
                HttpResponseMessage userResponse = await _httpClient.SendAsync(userRequest);
                userResponse.EnsureSuccessStatusCode();
                text = await userResponse.Content.ReadAsStringAsync();
                dynamic userInfo = JsonConvert.DeserializeObject<dynamic>(text);

                DateTime expiration = DateTime.Now.AddDays(1);// 默认一天过期
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                    (string)userInfo.gid,
                    DateTime.Now,
                    expiration,
                    true,
                    "",
                    FormsAuthentication.FormsCookiePath
                    );

                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName,
                    FormsAuthentication.Encrypt(ticket))
                {
                    HttpOnly = true,
                    Expires = expiration
                };

                HttpContext.Response.Cookies.Remove(cookie.Name);
                HttpContext.Response.Cookies.Add(cookie);
                return Redirect("~/");
            }
            catch (Exception ex)
            {
                return Redirect("~/");
            }
            #endregion
        }
    }
}