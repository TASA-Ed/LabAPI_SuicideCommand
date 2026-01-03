#nullable enable
using CommandSystem;
using LabApi.Features;
using LabApi.Features.Wrappers;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using System;
using System.Reflection;
using LabApi.Features.Console;

namespace LabAPI_SuicideCommand;

/// <summary>
/// 自杀命令 插件。
/// </summary>
public class SuicideCommandMain : Plugin
{
    /// <summary>
    /// 单例模式。
    /// </summary>
    public static SuicideCommandMain? MainSingleton { get; private set; }

    /// <summary>
    /// 插件名称。
    /// </summary>
    public override string Name => "Suicide Command Plugin";
    /// <summary>
    /// 插件描述。
    /// </summary>
    public override string Description => "Player suicide command plugin";
    /// <summary>
    /// 插件作者。
    /// </summary>
    public override string Author => "TASA-Ed Studio";
    /// <summary>
    /// 插件版本。
    /// </summary>
    public override Version Version => new(1, 0, 0, 0);

    /// <summary>
    /// 需要的 LabApi 版本。
    /// </summary>
    public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);

    /// <summary>
    /// 插件配置。
    /// </summary>

    public SuicideCommandConfig? Config;

    // 加载配置时。
    public override void LoadConfigs()
    {
        base.LoadConfigs();
        Config = this.LoadConfig<SuicideCommandConfig>("configs.yml");
        Logger.Debug("Loaded configuration.");
        // 尽量在配置可用时同步一次
        SuicideCommand.Setting = Config;
    }

    // 启用插件时。
    public override void Enable()
    {
        MainSingleton = this;
    }

    // 禁用插件时。
    public override void Disable() {
        SuicideCommand.Setting = null;
        Config = null;
        MainSingleton = null;
    }

    /// <summary>
    /// 注册一个允许玩家瞬间杀死自己的指令。
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    [CommandHandler(typeof(ClientCommandHandler))]
    public class SuicideCommand : ICommand
    {
        // 使用后备字段并提供懒加载
        private static SuicideCommandConfig? _setting;

        // 获取配置
        public static SuicideCommandConfig? Setting
        {
            get => _setting ??= GetConfigFromPlugin() ?? new SuicideCommandConfig();
            set => _setting = value;
        }

        // 指令名称
        public string Command => "suicide";

        // 指令别名
        public string[] Aliases => Setting?.CommandAlias ?? ["killme", "killself"];

        // 指令描述
        public string Description => Setting?.CommandDescription ?? "Suicide command";

        // 执行指令
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            // 在执行时确保配置已加载（兼容插件注册时配置尚未加载的情况）
            var config = Setting;

            var player = Player.Get(sender);

            if (player == null)
            {
                // 非玩家
                response = config?.CommandResponseNotPlayer ?? "You're not a player.";
                return false;
            }

            if (!player.IsAlive)
            {
                // 玩家未存活
                response = config?.CommandResponseNotAlive ?? "You're not alive.";
                return false;
            }

            var causeOfDeath = config?.CauseOfDeath ?? "He/she committed suicide~";
            // 杀死玩家
            player.Kill(causeOfDeath);

            // 成功
            response = config?.CommandResponseSuccess ?? "Now you dead~";
            return true;
        }

        // 通过反射从插件实例读取配置
        private static SuicideCommandConfig? GetConfigFromPlugin()
        {
            Logger.Debug("Attempting to retrieve config via reflection...");

            var main = MainSingleton;
            if (main == null) return null;
            Logger.Debug("Plugin main instance found.");

            // 先尝试直接查找名为 "Config" 的属性
            var type = main.GetType();
            var prop = type.GetProperty("Config", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (prop != null && typeof(SuicideCommandConfig).IsAssignableFrom(prop.PropertyType))
                return prop.GetValue(main) as SuicideCommandConfig;
            Logger.Debug("Config property not found, trying field...");

            // 再尝试字段查找
            var field = type.GetField("Config", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null && typeof(SuicideCommandConfig).IsAssignableFrom(field.FieldType))
                return field.GetValue(main) as SuicideCommandConfig;
            Logger.Debug("Config field not found.");

            return null;
        }
    }
}

/// <summary>
/// 插件配置类。
/// </summary>
public class SuicideCommandConfig
{
    // 所有配置必须为可 set 。
    public string[] CommandAlias { get; set; } = ["killme", "killself"];
    public string CommandDescription { get; set; } = "Suicide command";
    public string CommandResponseNotPlayer { get; set; } = "You're not a player.";
    public string CommandResponseNotAlive { get; set; } = "You're not alive.";
    public string CauseOfDeath { get; set; } = "He/she committed suicide~";
    public string CommandResponseSuccess { get; set; } = "Now you dead~";
}

