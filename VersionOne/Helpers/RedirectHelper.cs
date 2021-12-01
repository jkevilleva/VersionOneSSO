using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace VersionOne.Helpers
{
    public class RedirectHelper
    {
        public string ParseEmailFromSAML(string saml)
        {
            string email = String.Empty;
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.LoadXml(saml);
                XmlElement attributeStatement = (XmlElement)(doc.GetElementsByTagName("Attribute")[0]);

                foreach (var v in attributeStatement.ChildNodes)
                {
                    var newElement = (XmlElement)v;

                    if (newElement.Name == "AttributeValue")
                    {
                        email = newElement.InnerText;
                        if (!String.IsNullOrEmpty(email) && email.IndexOf("@") > -1)
                        {
                            return email;
                        }
                    }
                }
            }catch(Exception e)
            {
                LogError("Error in XML parsing", e);
            }
            return email;
        }

        public string RedirectSSO(string rawSamlData)
        {
           
            try { 
                // the sample data sent us may be already encoded, 
                // which results in double encoding
                if (rawSamlData.Contains('%'))
                {
                    rawSamlData = HttpUtility.UrlDecode(rawSamlData);
                }

                // read the base64 encoded bytes
                byte[] samlData = Convert.FromBase64String(rawSamlData);

                // read back into a UTF string
                string samlAssertion = Encoding.UTF8.GetString(samlData);

                // parse out the email from SAML
                return ParseEmailFromSAML(samlAssertion);
            }
            catch (Exception e)
            {
                LogError("Error in XML parsing", e);
                return String.Empty;
            }
        }

        public void LogInfo(string errorMessage)
        {
            string appLogging = ConfigurationManager.AppSettings["AppLogging"];
            try { 
                if (appLogging == "true")
                {
                        LoggerUtility.LogMessage(LoggerUtility.TracingLevel.INFO, errorMessage, null);
                }
            }
            catch (Exception e)
            {
                return;
               
            }
}

        public void LogError(string errorMessage,Exception ex)
        {
            string appLogging = ConfigurationManager.AppSettings["AppLogging"];
            if (appLogging == "true")
            {
                if (ex != null)
                {
                    LoggerUtility.LogMessage(LoggerUtility.TracingLevel.ERROR, errorMessage, ex);
                }
                else
                {
                    LoggerUtility.LogMessage(LoggerUtility.TracingLevel.ERROR, errorMessage, null);
                }
            }    
        }
    }
}