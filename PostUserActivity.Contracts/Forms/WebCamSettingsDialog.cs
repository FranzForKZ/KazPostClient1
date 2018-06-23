using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostUserActivity.Contracts;
using PostUserActivity.Contracts.Config;
using PostUserActivity.Contracts.Forms;
using PostUserActivity.Contracts.HWContracts;

namespace PostUserActivity.Forms
{
    public class WebCamFormProperties : FormPropertiesBase, ISettingsDialog
    {
        
        public WebCamFormProperties(IDeviceConfiguration config) : base(config, DeviceType.WebCam)
        {            
        }

        #region Implementation of ISettingsDialog
        
        public virtual string GetNotFoundMessage()
        {
            return "Веб камеры не найдены";
        }

        public virtual string GetWindowHeaderText()
        {
            return "Выбор установленных камер";
        }
   
        #endregion

        
    }
}
