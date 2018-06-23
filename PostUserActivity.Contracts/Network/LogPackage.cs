using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace PostUserActivity.Contracts.Network
{
  public  class LogPackage
    {
        //"token":"0LfQsNGH0LXQvCDQstGLINC/0L7RgtGA0LDRgtC40LvQuCDQstGA0LXQvNGPINC90LAg0LTQtdC60L7QtNC40YDQvtCy0LDQvdC40LU/",
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("wfmId")]
        public long WFMId { get; set; }

        /// <summary>
        /// ИИН клиента, передаваемый из WFM
        /// </summary>
        [JsonProperty("iin")]
        public long IIN { get; set; }

        /// <summary>
        /// логин пользователя на рабочей станции
        /// </summary>
        [JsonProperty("username")]
        public string UserName { get; set; }

        [JsonProperty("workstation")]
        public string WorkStation { get; set; }

        

        [JsonConverter(typeof(CustomDateTimeConverter))]
        [JsonProperty("timestamp")]
        public DateTime TimeStamp { get; set; }

        //  "log" - тип посылки.
        /// </summary>
        [JsonConverter(typeof(ArmDataPackageTypeStringEnumConverter))]
        [JsonProperty("type")]
        public ArmDataPackageType Type { get; set; }

        //"scanPic": "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEASABIAAD/=="
        /// <summary>
        /// base64 encoded zip архив с логами
        /// </summary>
        [JsonProperty("fileContent")]
        public string FileContent { get; set; }

        /// <summary>
        /// имя, под которым файл будет сохранен  на диске 
        /// </summary>
        [JsonProperty("fileName")]
        public string FileName { get; set; }        



        
    }
}
