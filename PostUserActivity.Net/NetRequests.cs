using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using CommonLib;
using Newtonsoft.Json;
using PostUserActivity.Contracts.Network;
using RestSharp;
using RestSharp.Serializers;
using NLog;
using System.Net.NetworkInformation;    
namespace PostUserActivity.Net
{
    public class URLDevider
    {
        public string ServerAndPort {get;private set;}
        public string urlPart { get; private set; }

        public URLDevider(string UrlToDevide)
        {
            var index = UrlToDevide.IndexOf(":", 6);
            var devideIndex = 0;
            for (int i = index; i < UrlToDevide.Length; i++)
            {
                if (char.IsLetter(UrlToDevide[i]))
                {
                    devideIndex = i - 1;
                    break;
                }


            }
            ServerAndPort = UrlToDevide.Substring(0, devideIndex);
            urlPart = UrlToDevide.Substring(devideIndex);
        }
        
    }
    public class NetRequests
    {
        static int RequestTimeOut { get; set; }

       
        static Logger logger = LogManager.GetCurrentClassLogger();
        static NetRequests()
        {
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
            try
            {
                RequestTimeOut = int.Parse(ConfigurationManager.AppSettings["RequestTimeOut"]) * 1000;
                RegistrationUrl = new URLDevider(ConfigurationManager.AppSettings["RegistrationUrl"]);
                AuthUrl = new URLDevider(ConfigurationManager.AppSettings["AuthorizationUrl"]);
                FullFrameUrl = new URLDevider(ConfigurationManager.AppSettings["FullFrameUrl"]);
                PreviewUrl = new URLDevider(ConfigurationManager.AppSettings["PreviewUrl"]);
                LogUrl = new URLDevider(ConfigurationManager.AppSettings["LogUrl"]);
            }
            catch (Exception ex)
            {
                logger.Error("Ошибка в чтении файла конфигурации приложения(параметры для отправки пакетов):" + ex.Message.ToString());                
            }
          
        }

        private static URLDevider AuthUrl;
        private static URLDevider FullFrameUrl;
        private static URLDevider PreviewUrl;
        private static URLDevider LogUrl;
        private static URLDevider RegistrationUrl;
        public void updateUrls()
        {
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
            try
            {
                RequestTimeOut = int.Parse(ConfigurationManager.AppSettings["RequestTimeOut"]) * 1000;
                RegistrationUrl = new URLDevider(ConfigurationManager.AppSettings["RegistrationUrl"]);
                AuthUrl = new URLDevider(ConfigurationManager.AppSettings["AuthorizationUrl"]);
                FullFrameUrl = new URLDevider(ConfigurationManager.AppSettings["FullFrameUrl"]);
                PreviewUrl = new URLDevider(ConfigurationManager.AppSettings["PreviewUrl"]);
                LogUrl = new URLDevider(ConfigurationManager.AppSettings["LogUrl"]);
            }
            catch (Exception ex)
            {
                logger.Error("Ошибка в чтении файла конфигурации приложения(параметры для отправки пакетов):" + ex.Message.ToString());
            }
        }
        public TResponseResult<ArmSettingsParams> SetAuthRequest(string username, string hash, string comment = "test")
        {
            TResponseResult<ArmSettingsParams> result;

            var urlPart = AuthUrl.urlPart;
            var client = new RestClient(AuthUrl.ServerAndPort);
            client.Timeout = RequestTimeOut;
            var request = GetRequest(urlPart, Method.PUT);
            
            request.AddBody(new
            {
                _comment = comment,
                token = hash.ToBase64(),
                username = username,
                workstationName = Environment.MachineName
            });

            var response = client.Execute(request);

            //  небольшой хак
            if (response.StatusCode == 0)
            {
                return new TResponseResult<ArmSettingsParams>(null,RequestResultType.RequestedHostUnavailable);
            }
            result = null;
            try
            {
                result = new TResponseResult<ArmSettingsParams>(JsonConvert.DeserializeObject<ArmSettingsParams>(response.Content), (RequestResultType)(int)response.StatusCode);
            }
            catch (Exception ex)
            {
                logger.Error("Содержимое ответа BE привело к ошибке:" + ex.Message);
                logger.Error(ex.InnerException);
            }
           
            return result;
        }

   
        public ResponseResult RegisterArm(string uuid, string username, string comment = "test")
        {

            var urlPart = RegistrationUrl.urlPart;
            var client = new RestClient(RegistrationUrl.ServerAndPort);
            client.Timeout = RequestTimeOut;
            var request = GetRequest(urlPart, Method.POST);
            logger.Info("Server: " + RegistrationUrl.ServerAndPort);
            logger.Info("Url Part: " + RegistrationUrl.urlPart);
            request.AddBody(new
            {
               // _comment = comment,
                uuid,
                username,
                workstationName = Environment.MachineName,
                clientVersion = AppInfo.Version
            });
            
            var response = client.Execute(request);

            //  небольшой хак
            if (response.StatusCode == 0)
            {
                return new ResponseResult(RequestResultType.RequestedHostUnavailable);
            }

            return new ResponseResult((RequestResultType)(int)response.StatusCode);
        }
        public static bool PingHost()
        {
            bool pingable = false;
            Ping pinger = new Ping();
            try
            {
             //   PingReply reply = pinger.Send(GetSetverPart(ArmDataPackageType.FullFrame));
               // pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            return pingable;
        }
        private string GetSetverPart(ArmDataPackageType Type)
        {
            string ServerPart = "";
            switch (Type)
            {
                case ArmDataPackageType.FullFrame:
                    {
                        ServerPart = FullFrameUrl.ServerAndPort;
                        break;
                    }

                case ArmDataPackageType.Preview:
                    {
                        ServerPart = PreviewUrl.ServerAndPort;
                        break;
                    }

                case ArmDataPackageType.Log:
                    {
                        ServerPart = LogUrl.ServerAndPort;
                        break;
                    }


            }
            return ServerPart;
        }
        private string GetUrlPart(ArmDataPackageType Type)
        {
            string UrlPart = "";
            switch (Type)
            {
                case ArmDataPackageType.FullFrame:
                    {
                        UrlPart = FullFrameUrl.urlPart;
                        break;
                    }
                    
                case ArmDataPackageType.Preview:
                    {
                        UrlPart = PreviewUrl.urlPart;
                        break;
                    }
                    
                case ArmDataPackageType.Log:
                    {
                        UrlPart = LogUrl.urlPart;
                        break;
                    }
                    
                    
            }
            return UrlPart;
        }
        private RestRequest GetRequest(string urlPart, Method method)
        {
            var request = new RestRequest(urlPart, method);
            request.AddHeader("Content-Type", "application/json");
            request.JsonSerializer = NewtonsoftJsonSerializer.Default;
            request.RequestFormat = DataFormat.Json;
            return request;
        }

        public ResponseResult SendLogPackage(LogPackage data)
        {
            var urlPart = LogUrl.urlPart;
            var client = new RestClient(LogUrl.ServerAndPort);
            client.Timeout = RequestTimeOut;
            var request = GetRequest(urlPart, Method.PUT);
                request.AddBody(data);
            //WITHOUT EXECUTION!!
            //var response = client.Execute(request);
            return new ResponseResult(RequestResultType.RequestedHostUnavailable);
            //  небольшой хак
            //if (response.StatusCode == 0)
            //{
            //    return new ResponseResult(RequestResultType.RequestedHostUnavailable);
            //}

            //return new ResponseResult((RequestResultType)(int)response.StatusCode);
        }        
        public  ResponseResult SendPackage(ArmDataPackage data)
        {

            var urlPart = GetUrlPart(data.Type);
            var client = new RestClient(GetSetverPart(data.Type));
            client.Timeout = RequestTimeOut;
            var request = GetRequest(urlPart, Method.PUT);            
            request.AddBody(data);

            var response = client.Execute(request);

            //  небольшой хак
            if (response.StatusCode == 0)
            {
                return new ResponseResult(RequestResultType.RequestedHostUnavailable);
            }

            return new ResponseResult((RequestResultType)(int)response.StatusCode);
        }        
    }


    public class NewtonsoftJsonSerializer : ISerializer
    {
        private Newtonsoft.Json.JsonSerializer serializer;

        public NewtonsoftJsonSerializer(Newtonsoft.Json.JsonSerializer serializer)
        {
            this.serializer = serializer;
        }

        public string ContentType
        {
            get { return "application/json"; } // Probably used for Serialization?
            set { }
        }

        public string DateFormat { get; set; }

        public string Namespace { get; set; }

        public string RootElement { get; set; }

        public string Serialize(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    serializer.Serialize(jsonTextWriter, obj);

                    return stringWriter.ToString();
                }
            }
        }

        public T Deserialize<T>(RestSharp.IRestResponse response)
        {
            var content = response.Content;

            using (var stringReader = new StringReader(content))
            {
                using (var jsonTextReader = new JsonTextReader(stringReader))
                {
                    return serializer.Deserialize<T>(jsonTextReader);
                }
            }
        }

        public static NewtonsoftJsonSerializer Default
        {
            get
            {
                return new NewtonsoftJsonSerializer(new Newtonsoft.Json.JsonSerializer()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                });
            }
        }
    }
}
