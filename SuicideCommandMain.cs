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
public sealed class SuicideCommandMain : Plugin {
    // 默认配置
    private static readonly SuicideCommandConfig DefaultConfig = new();

    /// <summary>
    /// 单例模式。
    /// </summary>
    public static SuicideCommandMain? MainSingleton { get; private set; }

    /// <inheritdoc />
    public override string Name => "Suicide Command Plugin";
    /// <inheritdoc />
    public override string Description => "Player suicide command plugin";
    /// <inheritdoc />
    public override string Author => "TASA-Ed Studio";
    /// <inheritdoc />
    public override Version Version => new(1, 1, 0, 0);

    /// <inheritdoc />
    public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);

    /// <summary>
    /// 插件配置。
    /// </summary>
    public SuicideCommandConfig? Config { get; private set; } = DefaultConfig;

    /// <inheritdoc />
    public override void LoadConfigs() {
        base.LoadConfigs();
        Config = this.LoadConfig<SuicideCommandConfig>("configs.yml") ?? DefaultConfig;
        SuicideCommand.UpdateConfig(Config);
    }

    /// <inheritdoc />
    public override void Enable() {
        MainSingleton = this;
    }

    /// <inheritdoc />
    public override void Disable() {
        SuicideCommand.UpdateConfig(null);
        Config = null;
        MainSingleton = null;
    }

    /// <summary>
    /// 注册一个允许玩家瞬间杀死自己的指令。
    /// </summary>
    [CommandHandler(typeof(ClientCommandHandler))]
    public sealed class SuicideCommand : ICommand {
        private static SuicideCommandConfig _config = DefaultConfig;

        /// <summary>
        /// 指令名称
        /// </summary>
        public string Command => "suicide";

        /// <summary>
        /// 指令别名
        /// </summary>
        public string[] Aliases => _config.CommandAlias;

        /// <summary>
        /// 指令描述
        /// </summary>
        public string Description => _config.CommandDescription;

        /// <summary>
        /// 更新命令配置。
        /// </summary>
        internal static void UpdateConfig(SuicideCommandConfig? config) {
            _config = config ?? DefaultConfig;
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response) {
            var player = Player.Get(sender);

            if (player == null) {
                // 非玩家
                response = _config.CommandResponseNotPlayer;
                return false;
            }

            if (!player.IsAlive) {
                // 玩家未存活
                response = _config.CommandResponseNotAlive;
                return false;
            }

            if (player.IsSCP)
            // 杀死玩家
            player.Kill(_config.CauseOfDeath);
            // 成功
            response = _config.CommandResponseSuccess;
            return true;
        }
    }
}

/// <summary>
/// 插件配置类。
/// </summary>
public sealed class SuicideCommandConfig {
    // 所有配置必须为可 set
    public string[] CommandAlias { get; set; } = ["killme", "killself"];
    public string CommandDescription { get; set; } = "Suicide command";
    public string CommandResponseNotPlayer { get; set; } = "You're not a player.";
    public string CommandResponseNotAlive { get; set; } = "You're not alive.";
    public string CauseOfDeath { get; set; } = "He/she committed suicide~";
    public string CommandResponseSuccess { get; set; } = "Now you dead~";
}
