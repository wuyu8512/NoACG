using System.Collections.Generic;
using Newtonsoft.Json;
using Wuyu.OneBot.Onebot.Models;

namespace Wuyu.OneBot.Entities.CQCodes.CQCodeModel
{
    /// <summary>
    /// 自定义合并转发节点
    /// </summary>
    public struct Node
    {
        #region 属性

        /// <summary>
        /// 发送者昵称
        /// </summary>
        [JsonProperty(PropertyName = "sender")]
        public NodeSender Sender { get; internal set; }

        /// <summary>
        /// 发送事件戳
        /// </summary>
        [JsonProperty(PropertyName = "time")]
        public long Time { get; internal set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        [JsonProperty(PropertyName = "content")]
        public List<CQCode> MessageList { get; internal set; }

        #endregion

        #region 发送者结构体

        /// <summary>
        /// 节点消息发送者
        /// </summary>
        public struct NodeSender
        {
            /// <summary>
            /// 发送者昵称
            /// </summary>
            [JsonProperty(PropertyName = "nickname")]
            public string Nick { get; internal set; }

            /// <summary>
            /// UID
            /// </summary>
            [JsonProperty(PropertyName = "user_id")]
            public long Uid { get; internal set; }
        }

        #endregion
    }
}