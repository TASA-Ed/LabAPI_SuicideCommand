#nullable enable
using CommandSystem;
using LabApi.Features;
using LabApi.Features.Wrappers;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using System;

namespace LabAPI_SuicideCommand;

/// <summary>
/// 自杀命令 插件。
/// </summary>
public class SuicideCommandMain : Plugin {
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
    public SuicideCommandConfig? Config = new();

    // 加载配置时
    public override void LoadConfigs() {
        base.LoadConfigs();
        Config = this.LoadConfig<SuicideCommandConfig>("configs.yml") ?? new SuicideCommandConfig();
        SuicideCommand.Setting = Config;
    }

    // 启用插件时
    public override void Enable() {
        MainSingleton = this;
    }

    // 禁用插件时
    public override void Disable() {
        SuicideCommand.Setting = null;
        Config = null;
        MainSingleton = null;
    }

    /// <summary>
    /// 注册一个允许玩家瞬间杀死自己的指令。
    /// </summary>
    [CommandHandler(typeof(ClientCommandHandler))]
    public class SuicideCommand : ICommand {
        private static SuicideCommandConfig? _setting;
        private static readonly SuicideCommandConfig DefaultConfig = new();
        private static readonly string[] DefaultAliases = ["killme", "killself"];

        public static SuicideCommandConfig? Setting {
            get {
                _setting ??= DefaultConfig;
                return _setting;
            }
            set => _setting = value;
        }

        // 指令名称
        public string Command => "suicide";

        // 指令别名
        public string[] Aliases => Setting?.CommandAlias ?? DefaultAliases;

        // 指令描述
        public string Description => Setting?.CommandDescription ?? "Suicide command";

        // 执行指令
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response) {
            var config = Setting;
            var player = Player.Get(sender);

            if (player == null) {
                // 非玩家
                response = config?.CommandResponseNotPlayer ?? "You're not a player.";
                return false;
            }

            if (!player.IsAlive) {
                // 玩家未存活
                response = config?.CommandResponseNotAlive ?? "You're not alive.";
                return false;
            }

            // 杀死玩家
            player.Kill(config?.CauseOfDeath ?? "He/she committed suicide~");
            // 成功
            response = config?.CommandResponseSuccess ?? "Now you dead~";
            return true;
        }
    }
}

/// <summary>
/// 插件配置类。
/// </summary>
public class SuicideCommandConfig {
    // 所有配置必须为可 set
    public string[] CommandAlias { get; set; } = ["killme", "killself"];
    public string CommandDescription { get; set; } = "Suicide command";
    public string CommandResponseNotPlayer { get; set; } = "You're not a player.";
    public string CommandResponseNotAlive { get; set; } = "You're not alive.";
    public string CauseOfDeath { get; set; } = "He/she committed suicide~";
    public string CommandResponseSuccess { get; set; } = "Now you dead~";
}

