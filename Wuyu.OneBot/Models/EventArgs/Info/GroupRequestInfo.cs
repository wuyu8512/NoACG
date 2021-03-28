using Newtonsoft.Json;

namespace Wuyu.OneBot.Models.EventArgs.Info
{
    /// <summary>
    /// <para>群组请求信息</para>
    /// <para>通过获取群系统消息API获得</para>
    /// </summary>
    public struct GroupRequestInfo
    {
        /// <summary>
        /// 请求ID
        /// </summary>
        [JsonProperty(PropertyName = "request_id")]
        public long ID { get; internal init; }

        /// <summary>
        /// 请求源UID
        /// </summary>
        [JsonProperty(PropertyName = "invitor_uin")]
        public long UserID { get; internal init; }

        /// <summary>
        /// 请求源用户名
        /// </summary>
        [JsonProperty(PropertyName = "invitor_nick")]
        public string UserNick { get; internal init; }

        /// <summary>
        /// <para>验证消息</para>
        /// <para>当信息类型为邀请列表时此字段为空</para>
        /// </summary>
        [JsonProperty(PropertyName = "message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; internal init; }

        /// <summary>
        /// 请求源群号
        /// </summary>
        [JsonProperty(PropertyName = "group_id")]
        public long GroupId { get; internal init; }

        /// <summary>
        /// 请求源群名
        /// </summary>
        [JsonProperty(PropertyName = "group_name")]
        public string GroupName { get; internal init; }

        /// <summary>
        /// 请求是否已经被处理
        /// </summary>
        [JsonProperty(PropertyName = "checked")]
        public bool Checked { get; internal init; }

        /// <summary>
        /// <para>处理者UID</para>
        /// <para>未处理时为0</para>
        /// </summary>
        [JsonProperty(PropertyName = "actor")]
        public long ActorUserId { get; internal init; }
    }
}