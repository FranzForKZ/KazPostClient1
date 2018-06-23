using System.Collections.Generic;

namespace PostUserActivity.Contracts.HWContracts
{
    /// <summary>
    /// интерфейс, для классов, которые занимаются поиском установленных в системе устройств, в зависимости от их типа (камеры или сканеры)
    /// </summary>
    public interface IHWSearcher
    {
        IEnumerable<HWDeviceDesciption> GetDevices();
    }
}
