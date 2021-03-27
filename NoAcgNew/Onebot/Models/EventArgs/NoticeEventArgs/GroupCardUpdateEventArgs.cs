using Newtonsoft.Json;

namespace NoAcgNew.Onebot.Models.EventArgs.NoticeEventArgs
{
    /// <summary>
    /// Go扩展事件
    /// 群成员名片变更事件
    /// </summary>
    public sealed class GroupCardUpdateEventArgs : BaseNoticeEventArgs
    {
        /// <summary>
        /// 群号
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        internal long GroupId { get; set; }

        /// <summary>
        /// 新名片
        /// </summary>
        [JsonProperty(PropertyName = "card_new")]
        internal string NewCard { get; set; }

        /// <summary>
        /// 旧名片
        /// </summary>
        [JsonProperty(PropertyName = "card_old")]
        internal string OldCard { get; set; }
    }
}