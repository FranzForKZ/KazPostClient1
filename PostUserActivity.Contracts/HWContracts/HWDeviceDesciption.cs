using YamlDotNet.Serialization;

namespace PostUserActivity.Contracts.HWContracts
{
    /// <summary>
    /// описание конкретной железки (камеры или сканера)
    /// </summary>
    public sealed class HWDeviceDesciption
    {
        public HWDeviceDesciption()
        {
            Name = string.Empty;
            DeviceId = string.Empty;
        }

        public string Name { get; set; }

        public string DeviceId { get; set; }

        public DeviceType Device { get; set; }

        [YamlIgnore]
        public bool IsSet
        {
            get
            {
                if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(DeviceId) || Device == DeviceType.Unknown)
                {
                    return false;
                }
                return true;
            }
        }

        #region Overrides of Object

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ DeviceId.GetHashCode() ^ Device.GetHashCode();
        }

        
        public override bool Equals(object obj)
        {
            var other = obj as HWDeviceDesciption;

            if ((object)other == null) return false;

            return EqualsHelper(this, other);
        }

        protected static bool EqualsHelper(HWDeviceDesciption first, HWDeviceDesciption second)
        {
            return first.Device == second.Device && first.DeviceId == second.DeviceId && first.Name == second.Name;
        }

        #endregion
    }
}
