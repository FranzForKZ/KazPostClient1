using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PostUserActivity.Contracts
{
    /// <summary>
    /// параметр, для контроллера. в модели содержаться идентификаторы, которые нужны для отправки изображений
    /// </summary>
    public class AnalyzeImgArgs
    {
        public long IIN { get; set; }        
        public long WmfId { get; set; }        
        public string Username { get; set; }
    }
}
