using Newtonsoft.Json;
using Wuyu.OneBot.Attributes;
using Wuyu.OneBot.Enumeration;

namespace Wuyu.OneBot.Entities.CQCodes.CQCodeModel
{
    /// <summary>
    /// 礼物
    /// 仅支持Go
    /// </summary>
    [MsgType(CQCodeType.Gift)]
    public struct Gift
    {
        /// <summary>
        /// 接收目标
        /// </summary>
        [JsonProperty(PropertyName = "qq")]
        public long Target { get; internal set; }

        /// <summary>
        /// 礼物类型
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public int GiftType { get; internal set; }
    }
}