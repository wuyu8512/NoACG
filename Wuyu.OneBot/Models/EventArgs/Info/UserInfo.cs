namespace Wuyu.OneBot.Onebot.Models.EventArgs.Info
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public struct UserInfo
    {
        #region 属性

        /// <summary>
        /// 用户id
        /// </summary>
        public long UserId { get; internal init; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string Nick { get; internal init; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; internal init; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; internal init; }

        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; internal init; }

        /// <summary>
        /// 登陆天数
        /// </summary>
        public int LoginDays { get; internal init; }

        #endregion
    }
}