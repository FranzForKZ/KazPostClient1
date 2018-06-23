using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PostUserActivity.Contracts.Network
{
    public class ArmSettingsParams
    {
        //"blur_detection_thres": "13169",
        [JsonProperty("blur_detection_thres")]
        public decimal BlurDetectionThres { get; set; }

        //"blur_threshhold": "133",
        [JsonProperty("blur_threshhold")]
        public decimal BlurThreshhold { get; set; }

        //  "brithness_thres_high": "0.325",
        [JsonProperty("brithness_thres_high")]
        public decimal BrithnessThresHigh { get; set; }

        //"brithness_thres_low": "0.232",
        [JsonProperty("brithness_thres_low")]
        public decimal BrithnessThresLow { get; set; }

        //"lips_threshhold": "131313",
        [JsonProperty("lips_threshhold")]
        public decimal LipsThreshhold { get; set; }

        //"pitch_threshold": "0.13",
        [JsonProperty("pitch_threshold")]
        public decimal PitchThreshold { get; set; }

        //"roll_threshold": "0.12",
        [JsonProperty("roll_threshold")]
        public decimal RollThreshold { get; set; }

        //"video_record_max_duration": "169",
        [JsonProperty("video_record_max_duration")]
        public int VideoRecordMaxDuration { get; set; }

        //"yaw_threshold": "0.11"
        [JsonProperty("yaw_threshold")]
        public decimal YawThreshold { get; set; }
        // yaw_mean=0.161
        [JsonProperty("yaw_mean")]
        public decimal YawMean { get; set; }
        //roll_mean=0.358
        [JsonProperty("roll_mean")]
        public decimal RollMean { get; set; }
        //pitch_mean=-0.92
        [JsonProperty("pitch_mean")]
        public decimal PitchMean { get; set; }
    }
}
