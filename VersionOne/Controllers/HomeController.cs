using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using VersionOne.Helpers;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;

namespace VersionOne.Controllers
{
    public class HomeController : ApiController
    {
       
        [HttpPost]
        public HttpResponseMessage Index()
        {
            var request = HttpContext.Current.Request;
            RedirectHelper redirectHelper = new RedirectHelper();
            string redirectURI = ConfigurationManager.AppSettings["RedirectURI"];
            string redirectHeaderName = ConfigurationManager.AppSettings["RedirectHeaderName"];

            if (String.IsNullOrEmpty(redirectURI) || String.IsNullOrEmpty(redirectHeaderName))
            {
                return Error("Values for redirectURI and redirectHeaderName not found in web.config", null);
            }

            try
            {
                
                string rawSamlData = request["SAMLResponse"];

                // if SAMLResponse is empty throw an error
                if (String.IsNullOrEmpty(rawSamlData))
                {
                    return Error("SAMLResponse not found in request ", null);

                }

                // get the users email from the SAML response
                string email = redirectHelper.RedirectSSO(rawSamlData);

                if (!String.IsNullOrEmpty(email))
                {
                    var redirect = Request.CreateResponse(HttpStatusCode.MovedPermanently);

                    redirect.Headers.Location = new Uri(redirectURI);
                    redirect.Headers.Add(redirectHeaderName, email);

                    // return to the client
                    redirectHelper.LogInfo(String.Format("{0}{1}","Succesful redirect for: ",email));
                    return redirect;
                }
                else
                {
                    return Error("User Info Not Available from SAML Request", null);
                }
            }
            catch (Exception e)
            {
                // log and return error
                return Error("User Info Not Available from SAML Request", e);
            }
        }

        public HttpResponseMessage Error(string errorMessage, Exception ex)
        {

            RedirectHelper redirectHelper = new RedirectHelper();

            redirectHelper.LogError(errorMessage, ex);
            HttpResponseMessage errorResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent(errorMessage)
            };

            return errorResponse;
        }
    }
}
