namespace ApiRpc.Classes.ApiCrud
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web.Configuration;
    using System.Web.Script.Serialization;

    using ApiLibrary.ApiObjects.Base;
    using ApiLibrary.DataObjects.Base;

    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The API CRUD communications class.
    /// </summary>
    /// <typeparam name="T">
    /// a DataClassBase class to be used for this implementation
    /// </typeparam>
    public class ApiCrudBase<T> : IDataAccess<T>
        where T : DataClassBase, new()
    {
        /// <summary>
        /// The URI.
        /// </summary>
        private readonly string uri = "http://localhost/API/Service.svc/";

        /// <summary>
        /// The Serialiser for java script.
        /// </summary>
        private readonly JavaScriptSerializer jss;

        /// <summary>
        /// The noun.
        /// </summary>
        private readonly string noun;

        /// <summary>
        /// The meta base.
        /// </summary>
        private readonly string metaBase;

        /// <summary>
        /// The account id.
        /// </summary>
        private readonly int accountId;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly Log logger;

        /// <summary>
        /// Initialises a new instance of the <see cref="ApiCrudBase{T}"/> class. 
        /// </summary>
        /// <param name="baseUrl">
        /// The base URL.
        /// </param>
        /// <param name="metabase">
        /// the METABASE
        /// </param>
        /// <param name="accountid">
        /// Account Id
        /// </param>
        public ApiCrudBase(string baseUrl, string metabase, int accountid)
        {
            if (!baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }

            this.uri = baseUrl;

            this.jss = new JavaScriptSerializer { MaxJsonLength = 2147483647 };
            this.metaBase = metabase;
            this.accountId = accountid;
            if (accountid != 0)
            {
                var currentType = new T();
                var typeNamePart = currentType.GetType().ToString().Split('.');
                this.noun = typeNamePart[typeNamePart.GetUpperBound(0)];    
            }
            else
            {
                this.noun = string.Empty;
            }
            this.logger = new Log();
        }

        /// <summary>
        /// The HTTP type.
        /// </summary>
        public enum HttpType
        {
            /// <summary>
            /// The get.
            /// </summary>
            Get = 0,

            /// <summary>
            /// The post.
            /// </summary>
            Post = 1,

            /// <summary>
            /// The put.
            /// </summary>
            Put = 2,

            /// <summary>
            /// The delete.
            /// </summary>
            Delete = 3
        }

        /// <summary>
        /// Create an entity record
        /// </summary>
        /// <param name="entities">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public List<T> Create(List<T> entities)
        {
            string result = this.DoPutPost(HttpType.Post, entities);
            return this.JsonToListT(result);
        }

        /// <summary>
        /// Read an entity.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T Read(int entityId)
        {
            var result = this.DoGet(HttpType.Get, entityId);
            return this.JsonToT(result, typeof(T));
        }

        /// <summary>
        /// Read an entity.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T Read(long entityId)
        {
            var result = this.DoGet(HttpType.Get, bigId: entityId);
            return this.JsonToT(result, typeof(T));
        }

        /// <summary>
        /// Read all entities.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<T> ReadAll()
        {
            var result = this.DoGet(HttpType.Get);
            var result2 = this.JsonToListT(result);
            return result2;
        }

        /// <summary>
        /// The read by person id.
        /// </summary>
        /// <param name="esrId">
        /// The employee id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<T> ReadByEsrId(long esrId)
        {
            var result = this.DoGet(HttpType.Get, bigId: 0, tag: string.Format("esrid={0}", esrId));
            return this.JsonToListT(result);
        }

        /// <summary>
        /// The read special.
        /// </summary>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public List<T> ReadSpecial(string reference)
        {
            var result = this.DoGet(HttpType.Get, 0, extraParam: reference);
            return this.JsonToListT(result);
        }

        /// <summary>
        /// Update entities.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public List<T> Update(List<T> entity)
        {
            string result = this.DoPutPost(HttpType.Put, entity);
            return this.JsonToListT(result);
        }

        /// <summary>
        /// Delete entity.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T Delete(int entityId)
        {
            var result = this.DoGet(HttpType.Delete, entityId);
            return this.JsonToT(result, typeof(T));
        }

        /// <summary>
        /// Delete entity.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T Delete(long entityId)
        {
            var result = this.DoGet(HttpType.Delete, bigId: entityId);
            return this.JsonToT(result, typeof(T));
        }

        /// <summary>
        /// The delete by entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T Delete(T entity)
        {
            var result = this.DoGet(HttpType.Delete, bigId: entity.KeyValue);
            return this.JsonToT(result, typeof(T));
        }

        #region private methods
       
        /// <summary>
        /// do put or post HTTP.
        /// </summary>
        /// <param name="method">
        /// The method PUT or POST.
        /// </param>
        /// <param name="entity">
        /// The entity to put or post.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string DoPutPost(HttpType method, List<T> entity)
        {
            try
            {
                var req = WebRequest.Create(this.MakeUrl()) as HttpWebRequest;
                
                if (req != null)
                {
                    req.KeepAlive = false;
                    req.Method = method.ToString();

                    req.ContentType = @"application/json; charset=utf-8";
                    byte[] uploadPackage = this.ObjectToJson(entity);

                    using (Stream stream = req.GetRequestStream())
                    {
                        stream.Write(uploadPackage, 0, uploadPackage.Length);
                    }

                    using (var resp = req.GetResponse() as HttpWebResponse)
                    {
                        if (resp != null)
                        {
                            if (resp.StatusCode == HttpStatusCode.OK)
                            {
                                using (Stream responseStream = resp.GetResponseStream())
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

                            return string.Format("{0} failed. Status Code = {1}, Status Description = {2}", method, resp.StatusCode, resp.StatusDescription);
                        }
                    }
                }
                

                return string.Empty;
            }
            catch (WebException exception)
            {
                string message = exception.Message;

                this.logger.Write(
                    this.metaBase,
                    0,
                    this.accountId,
                    LogRecord.LogItemTypes.SelFileServiceErrored,
                    LogRecord.TransferTypes.None,
                    0,
                    string.Empty,
                    LogRecord.LogReasonType.None,
                    message,
                    "ApiCrudBase.DoPutPost");

                return message;
            }
        }

        /// <summary>
        /// The do get command.
        /// </summary>
        /// <param name="method">
        /// The method.
        /// </param>
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
        private string DoGet(HttpType method, int id = 0, long bigId = 0, string tag = null, string extraParam = null)
        {
            try
            {
                HttpWebRequest req = null;
                if (id > 0)
                {
                    req = WebRequest.Create(this.MakeUrl(id: id, tag: tag, extraParam: extraParam)) as HttpWebRequest;
                }
                else 
                {
                    req = WebRequest.Create(this.MakeUrl(bigId: bigId, tag: tag, extraParam: extraParam)) as HttpWebRequest;
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

                this.logger.Write(
                    this.metaBase,
                    0,
                    this.accountId,
                    LogRecord.LogItemTypes.SelFileServiceErrored,
                    LogRecord.TransferTypes.None,
                    0,
                    string.Empty,
                    LogRecord.LogReasonType.None,
                    message,
                    "ApiCrudBase.DoGet");
                return message;
            }
        }
 
        /// <summary>
        /// The object to JSON converter.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ObjectToJson(object entity)
        {
            string result;
            try
            {
                result = this.jss.Serialize(entity);
            }
            catch (Exception message)
            {
                result = string.Empty;
                this.logger.WriteDebug(
                    this.metaBase,
                    0,
                    this.accountId,
                    LogRecord.LogItemTypes.EsrServiceErrored,
                    LogRecord.TransferTypes.EsrOutbound,
                    0,
                    "",
                    LogRecord.LogReasonType.Error,
                    message.Message,
                    string.Format("ApiCrudBase:{0}", new T().ClassName()));
            }

            return result;
        }

        /// <summary>
        /// The object to JSON converter.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>byte[]</cref>
        ///     </see>
        ///     .
        /// </returns>
        private byte[] ObjectToJson(List<T> entity)
        {
            return Encoding.Default.GetBytes(this.ObjectToJson((object)entity));
        }

        /// <summary>
        /// The JSON to Class T converter.
        /// </summary>
        /// <param name="json">
        /// The JSON.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private T JsonToT(string json, Type targetType)
        {
            try
            {
                return (T)this.jss.Deserialize(json, targetType);
            }
            catch (Exception)
            {
                return null;
            }
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
        private List<T> JsonToListT(string json)
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
        private string MakeUrl(int id = 0, long bigId = 0, string tag = null, string extraParam = null)
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

            return this.uri
                   +
                   string.Format(
                       "{0}{1}{2}{3}{4}{5}",
                       this.metaBase,
                       this.accountId == 0 ? string.Empty : "/" + this.accountId,
                       this.noun == string.Empty ? string.Empty : "/" + this.noun,
                       string.IsNullOrEmpty(IdToUse) ? string.Empty : "/" + IdToUse,
                       extraParam == null ? string.Empty : "/" + extraParam,
                       tag == null ? string.Empty : "?" + tag);
        }
        #endregion
    }
}