﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using YamlDotNet.Core.Events;

namespace PostUserActivity.Contracts.Network
{

    public class ArmDataPackage
    {
        //"_comment": "Это запрос от АРМ в back-end с парой фоток на сравнение",
        [JsonProperty("_comment")]
        public string Comment { get; set; }

        /// <summary>
        /// уникальный идентификатор рабочей станции
        /// </summary>
        //"token":"0LfQsNGH0LXQvCDQstGLINC/0L7RgtGA0LDRgtC40LvQuCDQstGA0LXQvNGPINC90LAg0LTQtdC60L7QtNC40YDQvtCy0LDQvdC40LU/",
        [JsonProperty("token")]
        public string Token { get; set; }

        //"wfmid": 13169,
        [JsonProperty("wfmId")]
        public long WFMId { get; set; }

        //"iin": 11154,
        /// <summary>
        /// ИИН клиента, передаваемый из WFM
        /// </summary>
        [JsonProperty("iin")]
        public long IIN { get; set; }

        //"username": "OperatorName",
        /// <summary>
        /// логин пользователя на рабочей станции
        /// </summary>
        [JsonProperty("username")]
        public string UserName { get; set; }

        //"timestamp": "timestamp",
        /// <summary>
        /// время операции
        /// </summary>
        [JsonConverter(typeof(CustomDateTimeConverter))]
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        //"type": "fullframe",
        /// <summary>
        /// "preview" или "fullframe" - тип посылки.
        /// </summary>
        [JsonConverter(typeof(ArmDataPackageTypeStringEnumConverter))]
        [JsonProperty("type")]
        public ArmDataPackageType Type { get; set; }

        //"camPic": "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEASABIAA==",
        /// <summary>
        /// base46 encoded изображение, полученное от библиотеки обработки изображений для вебкамеры
        /// </summary>
        [JsonProperty("camPic")]
        public string CameraPicture { get; set; }

        //"fileContent": "data:archive/zip;base64,/9j/4AAQSkZJRgABAQEASABIAA==",
        /// <summary>
        /// base46 encoded изображение, полученное от библиотеки обработки изображений для сканера
        /// </summary>
        [JsonProperty("scanPic")]
        public string ScanPicture { get; set; }
        [JsonProperty("fileContent")]
        public string FileContent { get; set; }

        /// <summary>
        /// имя, под которым файл будет сохранен  на диске 
        /// </summary>
        [JsonProperty("fileName")]
        public string FileName { get; set; }
       
        [JsonProperty("workstation")]
        public string WorkStation { get; set; }
    }
}

