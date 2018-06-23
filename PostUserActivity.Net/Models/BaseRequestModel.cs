using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PostUserActivity.Net
{
    /// <summary>
    /// При каждом вызове из АРМ Оператора в адрес Backend Системы в качестве параметров передается GUID и hash
    /// </summary>
    public abstract class BaseRequestModel
    {
        public string GUID { get; set; }
        public string Hash { get; set; }
    }
}
