﻿using Newtonsoft.Json;
using Wuyu.OneBot.Entities.CQCodes;

namespace Wuyu.OneBot.Models.QuickOperation.MsgQuickOperation
{
    public class GroupMsgQuickOperation : BaseMsgQuickOperation
    {
        [JsonProperty(PropertyName = "at_sender")]
        public bool AtSender { get; set; } = true;

        [JsonProperty(PropertyName = "delete")]
        public bool Delete { get; set; }

        [JsonProperty(PropertyName = "kick")] public bool Kick { get; set; }

        [JsonProperty(PropertyName = "ban")] public bool Ban { get; set; }

        [JsonProperty(PropertyName = "ban_duration")]
        public long BanDuration { get; set; }

        public static implicit operator GroupMsgQuickOperation(int code) => new() {Code = code};

        public static implicit operator GroupMsgQuickOperation(CQCode code) => new() {Reply = new[] {code}};

        public GroupMsgQuickOperation(BaseMsgQuickOperation baseMsg)
        {
            Reply = baseMsg.Reply;
            Code = baseMsg.Code;
            AutoEscape = baseMsg.AutoEscape;
        }

        public GroupMsgQuickOperation()
        {
        }
    }
}