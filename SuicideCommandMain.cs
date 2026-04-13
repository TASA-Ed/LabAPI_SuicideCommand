#nullable enable
using CommandSystem;
using LabApi.Features;
using LabApi.Features.Wrappers;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using System;
using PlayerStatsSystem;

namespace LabAPI_SuicideCommand;

public sealed class SuicideCommandMain : Plugin
{
    private static readonly SuicideCommandConfig DefaultConfig = new();

    public static SuicideCommandMain? MainSingleton { get; private set; }

    public override string Name => "Suicide Command Plugin";
    public override string Description => "Player suicide command plugin";
    public override string Author => "TASA-Ed Studio";
    public override Version Version => new(1, 1, 0, 0);

    public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);

    public SuicideCommandConfig? Config { get; private set; } = DefaultConfig;

    public override void LoadConfigs()
    {
        base.LoadConfigs();
        Config = this.LoadConfig<SuicideCommandConfig>("configs.yml") ?? DefaultConfig;
        SuicideCommand.UpdateConfig(Config);
    }

    public override void Enable()
    {
        MainSingleton = this;
    }

    public override void Disable()
    {
        SuicideCommand.UpdateConfig(null);
        Config = null;
        MainSingleton = null;
    }

    /// <summary>
    /// 注册一个允许玩家瞬间杀死自己的指令。
    /// </summary>
    [CommandHandler(typeof(ClientCommandHandler))]
    public sealed class SuicideCommand : ICommand
    {
        private static SuicideCommandConfig _config = DefaultConfig;

        public string Command => "suicide";

        public string[] Aliases => _config.CommandAlias;

        public string Description => _config.CommandDescription;

        internal static void UpdateConfig(SuicideCommandConfig? config)
        {
            _config = config ?? DefaultConfig;
        }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var player = Player.Get(sender);

            if (player == null)
            {
                // 非玩家
                response = _config.CommandResponseNotPlayer;
                return false;
            }

            if (!player.IsAlive)
            {
                // 玩家未存活
                response = _config.CommandResponseNotAlive;
                return false;
            }

            if (player.IsSCP)
                Announcer.ScpTermination(player, new UniversalDamageHandler());

            // 杀死玩家
            if (player.Kill(_config.CauseOfDeath))
            {
                // 成功
                response = _config.CommandResponseSuccess;
                return true;
            }
            // 失败
            response = "failed to commit suicide.";
            return false;
        }
    }
}

public sealed class SuicideCommandConfig
{
    public string[] CommandAlias { get; set; } = ["killme", "killself"];
    public string CommandDescription { get; set; } = "Suicide command";
    public string CommandResponseNotPlayer { get; set; } = "You're not a player.";
    public string CommandResponseNotAlive { get; set; } = "You're not alive.";
    public string CauseOfDeath { get; set; } = "He/she committed suicide~";
    public string CommandResponseSuccess { get; set; } = "Now you dead~";
}
