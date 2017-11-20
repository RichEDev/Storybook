using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary.API
{
    using System.IO;
    using System.Net;
    using System.Web.Script.Serialization;

    public class cCrudCaller<T> 
    {
        #region private variables

        private readonly int _AccountId;

        private readonly string _Uri = "http://localhost/API/Service.svc/";

        private readonly string _productMetabase = "expenses";

        private readonly JavaScriptSerializer jss;

        #endregion private variables

        public cCrudCaller(int accountId, string uri = "", string productApiMetabaseName = "expenses")
        {
            if (!string.IsNullOrEmpty(uri))
            {
                if (!uri.EndsWith("/"))
                {
                    uri += "/";
                }

                _Uri = uri;
            }

            _productMetabase = productApiMetabaseName;
            _AccountId = accountId;
            this.jss = new JavaScriptSerializer { MaxJsonLength = 2147483647 };
        }

        /// <summary>
        /// The do get command.
        /// </summary>
        /// <param name="method">
        /// The method. - i.e. GET
        /// </param>
        /// <param name="apiNoun"></param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="bigId">
        /// the id as a long</param>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <param name="extraParam">
        /// The extra Parameter.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string DoGet(string method, string apiNoun, int id = 0, long bigId = 0, string tag = null, string extraParam = null)
        {
            try
            {
                HttpWebRequest req = null;
                if (id > 0)
                {
                    req = WebRequest.Create(this.MakeUrl(apiNoun, id, tag: tag, extraParam: extraParam)) as HttpWebRequest;
                }
                else
                {
                    req = WebRequest.Create(this.MakeUrl(apiNoun, bigId: bigId, tag: tag, extraParam: extraParam)) as HttpWebRequest;
                }

                if (req != null)
                {
                    req.ServicePoint.ReceiveBufferSize = 99999999;
                    req.ServicePoint.MaxIdleTime = 3000;
                    req.KeepAlive = false;
                    req.Method = method.ToString().ToUpper();

                    req.ContentType = @"application/json; charset=utf-8";

                    using (var resp = req.GetResponse() as HttpWebResponse)
                    {
                        if (resp != null)
                        {
                            if (resp.StatusCode == HttpStatusCode.OK)
                            {
                                using (var responseStream = resp.GetResponseStream())
                                {
                                    if (responseStream != null)
                                    {
                                        using (var respStream = new StreamReader(responseStream))
                                        {
                                            return respStream.ReadToEnd();
                                        }
                                    }
                                }
                            }

                            return string.Format("{0} failed. Status Code = {1}, Status Description = {2}", "GET", resp.StatusCode, resp.StatusDescription);
                        }
                    }
                }

                return string.Empty;
            }
            catch (WebException exception)
            {
                string message = exception.Message;

                return message;
            }
        }

        /// <summary>
        /// make url from parameters.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="bigId"></param>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <param name="extraParam">
        /// The extra Paramater.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string MakeUrl(string apiNoun, int id = 0, long bigId = 0, string tag = null, string extraParam = null)
        {
            string IdToUse = string.Empty;

            if (id > 0)
            {
                IdToUse = id.ToString();
            }
            else if (bigId > 0)
            {
                IdToUse = bigId.ToString();
            }

            return this._Uri
                   +
                   string.Format(
                       "{0}{1}{2}{3}{4}{5}",
                       this._productMetabase,
                       this._AccountId == 0 ? string.Empty : "/" + this._AccountId,
                       apiNoun == string.Empty ? string.Empty : "/" + apiNoun,
                       string.IsNullOrEmpty(IdToUse) ? string.Empty : "/" + IdToUse,
                       extraParam == null ? string.Empty : "/" + extraParam,
                       tag == null ? string.Empty : "?" + tag);
        }

        /// <summary>
        /// The JSON to Class List<T></T> converter.
        /// </summary>
        /// <param name="json">
        /// The JSON.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public List<T> JsonToListT(string json)
        {
            try
            {
                return (List<T>)this.jss.Deserialize(json, typeof(List<T>));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Tempoarary copy of the TemplateMapping class - remove when ApiLibrary can be referenced directly via SM (i.e. when .NET 4.5)
    /// </summary>
    public class TemplateMapping
    {
        /// <summary>
        /// The template mapping id.
        /// </summary>
        public int templateMappingID;

        /// <summary>
        /// The template id.
        /// </summary>
        public int templateID;

        /// <summary>
        /// The field id.
        /// </summary>
        public Guid fieldID;

        /// <summary>
        /// The destination field.
        /// </summary>
        public string destinationField;

        /// <summary>
        /// The column ref.
        /// </summary>
        public int? columnRef;

        /// <summary>
        /// The import element type.
        /// </summary>
        public ImportElementType importElementType;

        /// <summary>
        /// The mandatory.
        /// </summary>
        public bool mandatory;

        /// <summary>
        /// The data type.
        /// </summary>
        public byte dataType;

        /// <summary>
        /// The lookup table.
        /// </summary>
        public Guid lookupTable;

        /// <summary>
        /// The match field.
        /// </summary>
        public Guid matchField;

        /// <summary>
        /// The override primary key.
        /// </summary>
        public bool overridePrimaryKey;

        /// <summary>
        /// The import field.
        /// </summary>
        public bool importField;

        /// <summary>
        /// The trust VPD.
        /// </summary>
        public string trustVPD;

        /// <summary>
        /// The table id.
        /// </summary>
        public Guid tableid;

        /// <summary>
        /// The import element type.
        /// </summary>
        public enum ImportElementType
        {
            /// <summary>
            /// The none.
            /// </summary>
            None = 0,

            /// <summary>
            /// The employee.
            /// </summary>
            Employee = 1,

            /// <summary>
            /// The assignment.
            /// </summary>
            Assignment = 2,

            /// <summary>
            /// The location.
            /// </summary>
            Location = 3,

            /// <summary>
            /// The organisation.
            /// </summary>
            Organisation = 4,

            /// <summary>
            /// The position.
            /// </summary>
            Position = 5,

            /// <summary>
            /// The phone.
            /// </summary>
            Phone = 6,

            /// <summary>
            /// The address.
            /// </summary>
            Address = 7,

            /// <summary>
            /// The vehicle.
            /// </summary>
            Vehicle = 8,

            /// <summary>
            /// The costing.
            /// </summary>
            Costing = 9,

            /// <summary>
            /// The person.
            /// </summary>
            Person = 10,

            /// <summary>
            /// The car.
            /// </summary>
            Car = 8
        }
    }

}
