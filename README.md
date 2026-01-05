# LabAPI_SuicideCommand

[![GitHub release](https://flat.badgen.net/github/release/TASA-Ed/LabAPI_SuicideCommand)](https://github.com/TASA-Ed/LabAPI_SuicideCommand/releases)
[![LabAPI Version](https://flat.badgen.net/static/LabAPI/v1.1.4)](https://github.com/northwood-studios/LabAPI)
[![License](https://flat.badgen.net/github/license/TASA-Ed/LabAPI_SuicideCommand/)](https://github.com/TASA-Ed/LabAPI_SuicideCommand/blob/master/LICENSE)

A LabAPI plugin that provides a suicide command.

一个 LabAPI 插件，提供一个自杀指令。

## Use / 使用

Download LabAPI_SuicideCommand_x64.dll from Releases and place it in `%appdata%\SCP Secret Laboratory\LabAPI\plugins\<port>`.

从 Releases 中下载 LabAPI_SuicideCommand_x64.dll 并放入 `%appdata%\SCP Secret Laboratory\LabAPI\plugins\<port>`。

All done!

大功告成！

## Config / 配置

| Configuration Item | Default value | Description |
|--------|--------|------|
| command_alias | killme, killself | Command aliases |
| command_description | Suicide command | Command description |
| command_response_not_player | You're not a player. | Response when the executor is not a player |
| command_response_not_alive | You're not alive. | Response when the player is already dead |
| cause_of_death | He/she committed suicide~ | Cause of death text displayed after the player commits suicide |
| command_response_success | Now you dead~ | Response when the command is successfully executed |

| 配置项 | 默认值 | 介绍 |
|--------|--------|------|
| command_alias | killme, killself | 命令别名列表，玩家可以使用这些命令触发自杀功能 |
| command_description | Suicide command | 命令描述，说明该命令的用途 |
| command_response_not_player | You're not a player. | 当执行者不是玩家时返回的提示消息 |
| command_response_not_alive | You're not alive. | 当玩家已经死亡时返回的提示消息 |
| cause_of_death | He/she committed suicide~ | 死亡原因显示文本，会在玩家自杀后显示 |
| command_response_success | Now you dead~ | 命令执行成功后返回的提示消息 |
