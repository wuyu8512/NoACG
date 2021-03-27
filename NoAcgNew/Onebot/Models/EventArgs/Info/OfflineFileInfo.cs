using Newtonsoft.Json;

namespace NoAcgNew.Onebot.Models.EventArgs.Info
{
    /// <summary>
    /// 离线文件信息
    /// </summary>
    public struct OfflineFileInfo
    {
        /// <summary>
        /// 文件名
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; internal init; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [JsonProperty(PropertyName = "size")]
        public long Size { get; internal init; }

        /// <summary>
        /// 文件链接
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public string Url { get; internal init; }
    }
}