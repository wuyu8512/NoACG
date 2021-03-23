using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sora.Enumeration.EventParamsType;
using Sora.Entities.CQCodes.CQCodeModel;
using Sora.Enumeration;
using NoAcgNew.Onebot.Models;
using Microsoft.Extensions.Logging;
using NoAcgNew.Internal;

namespace Sora.Entities.CQCodes
{
    /// <summary>
    /// CQ码类
    /// </summary>
    public sealed class CQCode
    {
        private static ILogger _log = ApplicationLogging.CreateLogger<CQCode>();

        #region 属性

        /// <summary>
        /// CQ码类型
        /// </summary>
        public CQFunction Function { get; }

        /// <summary>
        /// CQ码数据实例
        /// </summary>
        public object CQData { get; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造CQ码实例
        /// </summary>
        /// <param name="cqFunction">CQ码类型</param>
        /// <param name="dataObj"></param>
        internal CQCode(CQFunction cqFunction, object dataObj)
        {
            this.Function = cqFunction;
            this.CQData = dataObj;
        }

        #endregion

        #region CQ码构建方法

        /// <summary>
        /// 纯文本
        /// </summary>
        /// <param name="msg">文本消息</param>
        public static CQCode CQText(string msg)
        {
            return new(CQFunction.Text, new Text {Content = msg});
        }

        /// <summary>
        /// At CQ码
        /// </summary>
        /// <param name="uid">用户uid</param>
        public static CQCode CQAt(long uid)
        {
            if (uid < 10000)
            {
                _log.LogError("[CQAt]非法参数，已忽略CQ码[uid超出范围限制({Uid})]", uid);
                return CQIlleage();
            }

            return new CQCode(CQFunction.At,
                new At {Traget = uid.ToString()});
        }

        /// <summary>
        /// At全体 CQ码
        /// </summary>
        public static CQCode CQAtAll()
        {
            return new(CQFunction.At,
                new At {Traget = "all"});
        }

        /// <summary>
        /// 表情CQ码
        /// </summary>
        /// <param name="id">表情 ID</param>
        public static CQCode CQFace(int id)
        {
            //检查ID合法性
            if (id is < 0 or > 244)
            {
                _log.LogError("[CQFace]非法参数，已忽略CQ码[id超出范围限制({ID})]", id);
                return CQIlleage();
            }

            return new CQCode(CQFunction.Face,
                new Face {Id = id});
        }

        /// <summary>
        /// 语音CQ码
        /// </summary>
        /// <param name="data">文件名/绝对路径/URL/base64</param>
        /// <param name="isMagic">是否为变声</param>
        /// <param name="useCache">是否使用已缓存的文件</param>
        /// <param name="useProxy">是否通过代理下载文件</param>
        /// <param name="timeout">超时时间，默认为<see langword="null"/>(不超时)</param>
        public static CQCode CQRecord(string data, bool isMagic = false, bool useCache = true, bool useProxy = true,
            int? timeout = null)
        {
            var (dataStr, isDataStr) = ParseDataStr(data);
            if (!isDataStr)
            {
                _log.LogError("[CQRecord]非法参数({Data})，已忽略此CQ码", data);
                return CQIlleage();
            }

            return new CQCode(CQFunction.Record,
                new Record
                {
                    RecordFile = dataStr,
                    Magic = isMagic ? 1 : null,
                    Cache = useCache ? 1 : null,
                    Proxy = useProxy ? 1 : null,
                    Timeout = timeout
                });
        }

        /// <summary>
        /// 图片CQ码
        /// </summary>
        /// <param name="data">图片名/绝对路径/URL/base64</param>
        /// <param name="useCache">通过URL发送时有效,是否使用已缓存的文件</param>
        /// <param name="threadCount">通过URL发送时有效,通过网络下载图片时的线程数,默认单线程</param>
        public static CQCode CQImage(string data, bool useCache = true, int? threadCount = null)
        {
            if (string.IsNullOrEmpty(data)) throw new NullReferenceException(nameof(data));
            (string dataStr, bool isDataStr) = ParseDataStr(data);
            if (!isDataStr)
            {
                _log.LogError("[CQImage]非法参数({Data})，已忽略CQ码", data);
                return CQIlleage();
            }

            return new CQCode(CQFunction.Image,
                new Image
                {
                    ImgFile = dataStr,
                    ImgType = null,
                    UseCache = useCache ? 1 : null,
                    ThreadCount = threadCount
                });
        }

        /// <summary>
        /// 闪照CQ码
        /// </summary>
        /// <param name="data">图片名/绝对路径/URL/base64</param>
        /// <param name="useCache">通过URL发送时有效,是否使用已缓存的文件</param>
        /// <param name="threadCount">通过URL发送时有效,通过网络下载图片时的线程数,默认单线程</param>
        public static CQCode CQFlashImage(string data, bool useCache = true, int? threadCount = null)
        {
            if (string.IsNullOrEmpty(data)) throw new NullReferenceException(nameof(data));
            (string dataStr, bool isDataStr) = ParseDataStr(data);
            if (!isDataStr)
            {
                _log.LogError("[CQImage]非法参数({Data})，已忽略CQ码", data);
                return CQIlleage();
            }

            return new CQCode(CQFunction.Image,
                new Image
                {
                    ImgFile = dataStr,
                    ImgType = "flash",
                    UseCache = useCache ? 1 : null,
                    ThreadCount = threadCount
                });
        }

        /// <summary>
        /// 秀图CQ码
        /// </summary>
        /// <param name="data">图片名/绝对路径/URL/base64</param>
        /// <param name="useCache">通过URL发送时有效,是否使用已缓存的文件</param>
        /// <param name="threadCount">通过URL发送时有效,通过网络下载图片时的线程数,默认单线程</param>
        /// <param name="id">秀图特效id，默认为40000</param>
        public static CQCode CQShowImage(string data, int id = 40000, bool useCache = true, int? threadCount = null)
        {
            if (string.IsNullOrEmpty(data)) throw new NullReferenceException(nameof(data));
            (string dataStr, bool isDataStr) = ParseDataStr(data);
            if (!isDataStr)
            {
                _log.LogError("[CQShowImage]非法参数({Data})，已忽略CQ码", data);
                return CQIlleage();
            }

            return new CQCode(CQFunction.Image,
                new Image
                {
                    ImgFile = dataStr,
                    ImgType = "show",
                    UseCache = useCache ? 1 : null,
                    Id = id,
                    ThreadCount = threadCount
                });
        }

        /// <summary>
        /// 视频CQ码
        /// </summary>
        /// <param name="data">视频名/绝对路径/URL/base64</param>
        /// <param name="useCache">是否使用已缓存的文件</param>
        /// <param name="useProxy">是否通过代理下载文件</param>
        /// <param name="timeout">超时时间，默认为<see langword="null"/>(不超时)</param>
        public static CQCode CQVideo(string data, bool useCache = true, bool useProxy = true, int? timeout = null)
        {
            var (dataStr, isDataStr) = ParseDataStr(data);
            if (!isDataStr)
            {
                _log.LogError("[CQVideo]非法参数({Data})，已忽略CQ码", data);
                return CQIlleage();
            }

            return new CQCode(CQFunction.Video,
                new Video
                {
                    VideoFile = dataStr,
                    Cache = useCache ? 1 : null,
                    Proxy = useProxy ? 1 : null,
                    Timeout = timeout
                });
        }

        /// <summary>
        /// 音乐CQ码
        /// </summary>
        /// <param name="musicType">音乐分享类型</param>
        /// <param name="musicId">音乐Id</param>
        public static CQCode CQMusic(MusicShareType musicType, long musicId)
        {
            return new(CQFunction.Music,
                new Music
                {
                    MusicType = musicType,
                    MusicId = musicId
                });
        }

        /// <summary>
        /// 自定义音乐分享CQ码
        /// </summary>
        /// <param name="url">跳转URL</param>
        /// <param name="musicUrl">音乐URL</param>
        /// <param name="title">标题</param>
        /// <param name="content">内容描述[可选]</param>
        /// <param name="coverImageUrl">分享内容图片[可选]</param>
        public static CQCode CQCustomMusic(string url, string musicUrl, string title, string content = null,
            string coverImageUrl = null)
        {
            return new(CQFunction.Music,
                new CustomMusic
                {
                    ShareType = "custom",
                    Url = url,
                    MusicUrl = musicUrl,
                    Title = title,
                    Content = content,
                    CoverImageUrl = coverImageUrl
                });
        }

        /// <summary>
        /// 链接分享
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="title">标题</param>
        /// <param name="content">可选，内容描述</param>
        /// <param name="imageUrl">可选，图片 URL</param>
        public static CQCode CQShare(string url,
            string title,
            string content = null,
            string imageUrl = null)
        {
            return new(CQFunction.Share,
                new Share
                {
                    Url = url,
                    Title = title,
                    Content = content,
                    ImageUrl = imageUrl
                });
        }

        /// <summary>
        /// 回复
        /// </summary>
        /// <param name="id">消息id</param>
        public static CQCode CQReply(int id)
        {
            return new(CQFunction.Reply,
                new Reply
                {
                    Traget = id
                });
        }

        #region GoCQ扩展码

        /// <summary>
        /// 群成员戳一戳
        /// 只支持Go-CQHttp
        /// </summary>
        /// <param name="uid">ID</param>
        public static CQCode CQPoke(long uid)
        {
            if (uid < 10000)
            {
                _log.LogError("[CQPoke]非法参数，已忽略CQ码[uid超出范围限制({Uid})]", uid);
                return CQIlleage();
            }

            return new CQCode(CQFunction.Poke,
                new Poke
                {
                    Uid = uid
                });
        }

        /// <summary>
        /// 接收红包
        /// </summary>
        /// <param name="title">祝福语/口令</param>
        public static CQCode CQRedbag(string title)
        {
            if (string.IsNullOrEmpty(title)) throw new NullReferenceException(nameof(title));
            return new CQCode(CQFunction.RedBag,
                new Redbag
                {
                    Title = title
                });
        }

        /// <summary>
        /// 发送免费礼物
        /// </summary>
        /// <param name="giftId">礼物id</param>
        /// <param name="target">目标uid</param>
        public static CQCode CQGift(int giftId, long target)
        {
            if (giftId is < 0 or > 8 || target < 10000) throw new ArgumentOutOfRangeException(nameof(giftId));
            return new CQCode(CQFunction.Gift,
                new Gift
                {
                    Target = target,
                    GiftType = giftId
                });
        }

        /// <summary>
        /// XML 特殊消息
        /// </summary>
        /// <param name="content">xml文本</param>
        public static CQCode CQXml(string content)
        {
            if (string.IsNullOrEmpty(content)) throw new NullReferenceException(nameof(content));
            return new CQCode(CQFunction.Xml,
                new Code
                {
                    Content = content,
                    Resid = null
                });
        }

        /// <summary>
        /// JSON 特殊消息
        /// </summary>
        /// <param name="content">JSON 文本</param>
        /// <param name="richText">富文本内容</param>
        public static CQCode CQJson(string content, bool richText = false)
        {
            if (string.IsNullOrEmpty(content)) throw new NullReferenceException(nameof(content));
            return new CQCode(CQFunction.Json,
                new Code
                {
                    Content = content,
                    Resid = richText ? (int?) 1 : null
                });
        }

        /// <summary>
        /// JSON 特殊消息
        /// </summary>
        /// <param name="content">JObject实例</param>
        public static CQCode CQJson(JObject content)
        {
            if (content == null) throw new NullReferenceException(nameof(content));
            return new CQCode(CQFunction.Json,
                new Code
                {
                    Content = JsonConvert.SerializeObject(content, Formatting.None)
                });
        }

        /// <summary>
        /// 装逼大图
        /// </summary>
        /// <param name="imageFile">图片名/绝对路径/URL/base64</param>
        /// <param name="source">来源名称</param>
        /// <param name="iconUrl">来源图标 URL</param>
        /// <param name="minWidth">最小 Width</param>
        /// <param name="minHeight">最小 Height</param>
        /// <param name="maxWidth">最大 Width</param>
        /// <param name="maxHeight">最大 Height</param>
        public static CQCode CQCardImage(string imageFile,
            string source = null,
            string iconUrl = null,
            long minWidth = 400,
            long minHeight = 400,
            long maxWidth = 400,
            long maxHeight = 400)
        {
            if (string.IsNullOrEmpty(imageFile)) throw new NullReferenceException(nameof(imageFile));
            (string dataStr, bool isDataStr) = ParseDataStr(imageFile);
            if (!isDataStr)
            {
                _log.LogError("[CQCardImage]非法参数({ImageFile})，已忽略CQ码", imageFile);
                return CQIlleage();
            }

            return new CQCode(CQFunction.CardImage,
                new CardImage
                {
                    ImageFile = dataStr,
                    Source = source,
                    Icon = iconUrl,
                    MinWidth = minWidth,
                    MinHeight = minHeight,
                    MaxWidth = maxWidth,
                    MaxHeight = maxHeight
                });
        }

        /// <summary>
        /// 语音转文字（TTS）CQ码
        /// </summary>
        /// <param name="messageStr">要转换的文本信息</param>
        /// <returns></returns>
        public static CQCode CQTTS(string messageStr)
        {
            if (string.IsNullOrEmpty(messageStr)) throw new NullReferenceException(nameof(messageStr));
            return new CQCode(CQFunction.TTS,
                new
                {
                    text = messageStr
                });
        }

        #endregion

        /// <summary>
        /// 空CQ码
        /// <para>当存在非法参数时CQ码将被本函数重置</para>
        /// </summary>
        private static CQCode CQIlleage() =>
            new(CQFunction.Text, new Text {Content = null});

        #endregion

        #region 辅助函数

        /// <summary>
        /// 获取CQ码数据格式类型
        /// 用于将object转换为可读结构体
        /// </summary>
        /// <param name="cqCode"></param>
        /// <returns>
        /// 数据结构体类型
        /// </returns>
        public static Type GetCqCodeDataType(CQCode cqCode)
        {
            return cqCode.CQData.GetType();
        }

        #endregion

        #region 获取CQ码内容(仅用于序列化)

        internal MessageElement ToOnebotMessage() => new()
        {
            MsgType = this.Function,
            RawData = JObject.FromObject(this.CQData)
        };

        #endregion

        #region 正则匹配字段

        private static readonly List<string> FileRegex = new()
        {
            @"^(/[^/ ]*)+/?([a-zA-Z0-9]+\.[a-zA-Z0-9]+)$", //绝对路径-linux/osx
            @"^(?:[a-zA-Z]:\/)(?:[^\/|<>?*:""]*\/)*[^\/|<>?*:""]*$", //绝对路径-win
            @"^base64:\/\/[\/]?([\da-zA-Z]+[\/+]+)*[\da-zA-Z]+([+=]{1,2}|[\/])?$", //base64
            @"^(http|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?$", //网络图片链接
            @"^[\w,\s-]+\.[a-zA-Z0-9]+$" //文件名
        };

        #endregion

        #region 程序集方法

        /// <summary>
        /// 处理传入数据
        /// </summary>
        /// <param name="dataStr">数据字符串</param>
        /// <returns>
        /// <para><see langword="retStr"/>处理后数据字符串</para>
        /// <para><see langword="isMatch"/>是否为合法数据字符串</para>
        /// </returns>
        internal static (string retStr, bool isMatch) ParseDataStr(string dataStr)
        {
            if (string.IsNullOrEmpty(dataStr)) return (null, false);
            var isMatch = false;
            dataStr = dataStr.Replace('\\', '/');
            //当字符串太长时跳过正则检查
            if (dataStr.Length > 1000) return (dataStr, true);
            for (var i = 0; i < 5; i++)
            {
                isMatch |= Regex.IsMatch(dataStr, FileRegex[i]);
                if (isMatch)
                {
                    switch (i)
                    {
                        case 0: //linux/osx
                            if (Environment.OSVersion.Platform != PlatformID.Unix &&
                                Environment.OSVersion.Platform != PlatformID.MacOSX &&
                                !File.Exists(dataStr))
                                return (dataStr, false);
                            else
                                return ($"file:///{dataStr}", true);
                        case 1: //win
                            if (Environment.OSVersion.Platform == PlatformID.Win32NT && File.Exists(dataStr))
                                return ($"file:///{dataStr}", true);
                            else
                                return (dataStr, false);
                        default:
                            return (dataStr, true);
                    }
                }
            }

            return (dataStr, false);
        }

        #endregion

        #region 运算符重载

        /// <summary>
        /// 等于重载
        /// </summary>
        public static bool operator ==(CQCode cqCodeL, CQCode cqCodeR)
        {
            if (cqCodeL is null && cqCodeR is null) return true;

            return cqCodeL is not null && cqCodeR is not null &&
                   cqCodeL.Function == cqCodeR.Function &&
                   JToken.DeepEquals(JToken.FromObject(cqCodeL.CQData), JToken.FromObject(cqCodeR.CQData));
        }

        /// <summary>
        /// 不等于重载
        /// </summary>
        public static bool operator !=(CQCode cqCodeL, CQCode cqCodeR)
        {
            return !(cqCodeL == cqCodeR);
        }

        #endregion

        #region 常用重载

        /// <summary>
        /// 比较重载
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is CQCode cqCode)
            {
                return this == cqCode;
            }

            return false;
        }

        /// <summary>
        /// GetHashCode
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(Function, CQData);
        }

        #endregion
    }
}