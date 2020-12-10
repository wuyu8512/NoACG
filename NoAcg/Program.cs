﻿using Fleck;
using Microsoft.Extensions.DependencyInjection;
using NoAcg.Core;
using NoAcg.Model;
using NoAcg.Model.Monitor;
using Sora.Entities.CQCodes;
using Sora.Server;
using Sora.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tool.Common;

namespace NoAcg
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
                WriteIndented = true
            };
            options.Converters.Add(new Common.DatetimeJsonConverter());
            InvokeConfig invokeConfig;
            if (!File.Exists("InvokeConfig.json"))
            {
                invokeConfig = new InvokeConfig
                {
                    GroupConfigs = new[]
                    {
                        new InvokeItem()
                        {
                            Text = "热门图片",
                            ClassName = "NoAcg.Core.Image",
                            Method = "GetHotImg",
                            Param = new object[] {3, true, true}
                        },
                        new InvokeItem()
                        {
                            Text = "热门Cos",
                            ClassName = "NoAcg.Core.Image",
                            Method = "GetCosHot",
                        },
                        new InvokeItem()
                        {
                            Text = "随机Cos",
                            ClassName = "NoAcg.Core.Image",
                            Method = "GetRandCos"
                        },
                        new InvokeItem()
                        {
                            Text = "随机图片",
                            ClassName = "NoAcg.Core.Image",
                            Method = "GetRandom"
                        }
                    },
                    PrivateConfigs = new[]
                    {
                        new InvokeItem()
                        {
                            Text = "热门图片",
                            ClassName = "NoAcg.Core.Image",
                            Method = "GetHotImg",
                            Param = new object[] {7, true, false}
                        },
                        new InvokeItem()
                        {
                            Text = "随机奶子",
                            ClassName = "NoAcg.Core.Image",
                            Method = "GetImgByTag",
                            Param = new object[] {"breasts", 1, true, true}
                        },
                        new InvokeItem()
                        {
                            Text = @"奶子(\d)连",
                            MatchMode = "Regex",
                            ClassName = "NoAcg.Core.Image",
                            Method = "GetImgByTag",
                            Param = new object[] {"breasts", @"int:$1", true, true}
                        },
                        new InvokeItem()
                        {
                            Text = "刷新配置",
                            ClassName = "NoAcg.Core.Config",
                            Method = "RefreshInvokeConfig"
                        }
                    }
                };
                await File.WriteAllTextAsync("InvokeConfig.json", JsonSerializer.Serialize(invokeConfig, options));
                ConsoleLog.Info("NoACG", "没有找到InvokeConfig.json，已经生成默认文件");
            }
            else
            {
                invokeConfig =
                    JsonSerializer.Deserialize<InvokeConfig>(await File.ReadAllTextAsync("InvokeConfig.json"), options);
                if (invokeConfig == null)
                {
                    ConsoleLog.Warning("NoACG", "读取配置文件InvokeConfig.json发生了错误，建议删除文件重新生成");
                    return;
                }

                ConsoleLog.Info("NoACG", "已经读取配置文件InvokeConfig.json");
            }

            Config.HandleInvokeParams(invokeConfig);

            AppConfig appConfig;
            if (!File.Exists("AppConfig.json"))
            {
                appConfig = new AppConfig
                {
                    ServerConfig = new ServerConfig {Port = 9200, Location = "0.0.0.0", ApiTimeOut = 3000},
                    YandeConfig = new YandeConfig {Proxy = new WebProxy("127.0.0.1:2802"), Timeout = 10000}
                };
                await File.WriteAllTextAsync("AppConfig.json", JsonSerializer.Serialize(appConfig, options));
                ConsoleLog.Info("NoACG", "没有找到AppConfig.json，已经生成默认文件");
            }
            else
            {
                appConfig = JsonSerializer.Deserialize<AppConfig>(await File.ReadAllTextAsync("AppConfig.json"),
                    options);
                if (appConfig == null)
                {
                    ConsoleLog.Warning("NoACG", "读取配置文件AppConfig.json发生了错误，建议删除文件重新生成");
                    return;
                }

                ConsoleLog.Info("NoACG", "已经读取配置文件AppConfig.json");
            }

            ConsoleLog.SetLogLevel(LogLevel.Debug);
            Yande yande = new Yande(appConfig.YandeConfig);
            var webClient = new WebClient {Proxy = appConfig.YandeConfig.Proxy};
            Twitter twitter = new Twitter(ref webClient);

            //初始化服务器实例
            SoraWSServer server = new SoraWSServer(appConfig.ServerConfig);

            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddSingleton(yande)
                .AddSingleton(server)
                .AddSingleton(invokeConfig)
                .AddSingleton(options)
                .AddSingleton(twitter)
                .BuildServiceProvider();

            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = false; //true: 不导致退出。false: 会导致退出
                server.Dispose();
            };

            TweeterMonitorConfig tweeterMonitorConfig = new TweeterMonitorConfig
            {
                Proxy = appConfig.YandeConfig.Proxy,
                Items = new List<MonitorItem>
                {
                    new MonitorItem()
                    {
                        Mark = "wuyu_8512",
                        Private = new List<long> {3117836505},
                        Group = new List<long> {764444946, 551856311},
                    },
                    new MonitorItem()
                    {
                        Mark = "Strangestone",
                        Group = new List<long> {122675463, 551856311}
                    },
                    new MonitorItem()
                    {
                        Mark = "lnnews",
                        Group = new List<long> {551856311}
                    },
                    new MonitorItem()
                    {
                        Mark = "GA_bunko",
                        Group = new List<long> {551856311}
                    },
                    new MonitorItem()
                    {
                        Mark = "MF_bunkoJ",
                        Group = new List<long> {551856311}
                    },
                    new MonitorItem()
                    {
                        Mark = "HJbunko",
                        Group = new List<long> {551856311}
                    },
                    new MonitorItem()
                    {
                        Mark = "fantasia_bunko",
                        Group = new List<long> {551856311}
                    },
                    new MonitorItem()
                    {
                        Mark = "bunko_dengeki",
                        Group = new List<long> {551856311}
                    }
                }
            };

            bool isConnected = false;
            server.Event.OnClientConnect += async (sender, eventArgs) =>
            {
                isConnected = true;
                MonitorManage monitorManage = new MonitorManage(tweeterMonitorConfig, eventArgs.SoraApi);
                monitorManage.OpenTwitterMonitor();
                var (apiStatus, nick) = await eventArgs.SoraApi.GetLoginUserName();
            };
            //群消息接收回调
            server.Event.OnGroupMessage += async (sender, eventArgs) =>
            {
                if (invokeConfig.BlackLists?.Contains(eventArgs.SourceGroup.Id) ?? false) return;
                if (invokeConfig.BlackLists?.Contains(eventArgs.SenderInfo.UserId) ?? false) return;
                if (isConnected)
                    await HandleMessage(serviceProvider,
                        invokeConfig.UniversalConfigs.Concat(invokeConfig.GroupConfigs), eventArgs,
                        eventArgs.SourceGroup.Id);
            };
            server.Event.OnPrivateMessage += async (sender, eventArgs) =>
            {
                if (invokeConfig.BlackLists?.Contains(eventArgs.SenderInfo.UserId) ?? false) return;
                if (isConnected)
                    await HandleMessage(serviceProvider,
                        invokeConfig.UniversalConfigs.Concat(invokeConfig.PrivateConfigs), eventArgs,
                        eventArgs.SenderInfo.UserId);
            };
            //启动服务器
            await server.StartServer();
        }

        private static async Task HandleMessage(IServiceProvider serviceProvider, IEnumerable<InvokeItem> config,
            dynamic eventArgs, long source)
        {
            foreach (var item in config)
            {
                if (IsMatch(item, eventArgs.Message.RawText))
                {
                    var t = Type.GetType(item.ClassName);
                    ComObject obj = new ComObject(t, serviceProvider, eventArgs);
                    var result = obj.InvokeMethod(item.Method, GetParam(item, eventArgs.Message.RawText));
                    if (result != null)
                    {
                        ConsoleLog.Debug("NoACG", "正在回复消息：" + eventArgs.Message.RawText);
                        switch (result)
                        {
                            case Task<CQCode> task1:
                                await eventArgs.Reply(task1.Result);
                                break;

                            case Task<IEnumerable<CQCode>> task2:
                                await eventArgs.Reply(task2.Result);
                                break;

                            default:
                                await eventArgs.Reply(result);
                                break;
                        }
                    }

                    if (item.Intercept) break;
                }
            }
        }

        private static bool IsMatch(InvokeItem config, string rawText)
        {
            return config.MatchMode switch
            {
                "FullText" => rawText.Equals(config.Text, StringComparison.OrdinalIgnoreCase),
                "Regex" => Regex.IsMatch(rawText, config.Text),
                _ => false
            };
        }

        private static object[] GetParam(InvokeItem config, string rawText)
        {
            switch (config.MatchMode)
            {
                case "FullText":
                    return config.Param;

                case "Regex":
                {
                    var result = new List<object>();
                    foreach (var item in config.Param)
                    {
                        if (item is string str)
                        {
                            var temp = Regex.Replace(rawText, config.Text, str);
                            if (temp.StartsWith("int:", StringComparison.OrdinalIgnoreCase))
                                result.Add(int.Parse(temp[4..]));
                            else
                                result.Add(temp);
                        }
                        else result.Add(item);
                    }

                    return result.ToArray();
                }
                default:
                    return null;
            }
        }
    }
}