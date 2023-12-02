using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Timers;



namespace CountDown;
[MinimumApiVersion(55)]

public static class GetUnixTime
{
    public static int GetUnixEpoch(this DateTime dateTime)
    {
        var unixTime = dateTime.ToUniversalTime() -
                       new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        return (int)unixTime.TotalSeconds;
    }
}
public partial class CountDown : BasePlugin
{
    public override string ModuleName => "SpecialRounds";
    public override string ModuleAuthor => "DeadSwim";
    public override string ModuleDescription => "Simple Special rounds.";
    public override string ModuleVersion => "V. 1.0.0";
    private static readonly int?[] IsVIP = new int?[65];



    public float Time;
    public bool Countdown_enable;


    public override void Load(bool hotReload)
    {
        RegisterListener<Listeners.OnMapStart>(name =>
        {
            Countdown_enable = false;
            Time = 0;

        });
        RegisterListener<Listeners.OnTick>(() =>
        {
            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                var ent = NativeAPI.GetEntityFromIndex(i);
                if (ent == 0)
                    continue;

                var client = new CCSPlayerController(ent);
                if (client == null || !client.IsValid)
                    continue;
                if (Countdown_enable)
                {
                    client.PrintToCenterHtml(
                    $"<font color='gray'>----</font> <font class='fontSize-l' color='green'>COUNTDOWN</font><font color='gray'>----</font><br>" +
                    $"<font color='gray'>►</font> <font class='fontSize-m' color='green'>[{Time}]</font> <font color='gray'>◄</font><br>" +
                    $"<font color='gray'>----</font> <font class='fontSize-l' color='green'>COUNTDOWN</font><font color='gray'>----</font><br>"
                    );
                }
            }
        });
    }
    private bool IsInt(string sVal)
    {
        foreach (char c in sVal)
        {
            int iN = (int)c;
            if ((iN > 57) || (iN < 48))
                return false;
        }
        return true;
    }
    [ConsoleCommand("css_countdown", "Start countdown")]
    public void CommandStartCountDown(CCSPlayerController? player, CommandInfo info)
    {
        if (!AdminManager.PlayerHasPermissions(player, "@css/root"))
        {
            player.PrintToChat($" [{ChatColors.Lime}CountDown{ChatColors.Default}] You are not admin..");
            return;
        }
        var TimeSec = info.ArgByIndex(1);
        if (TimeSec == null || TimeSec == "" || !IsInt(TimeSec))
        {
            player.PrintToChat($" [{ChatColors.Lime}CountDown{ChatColors.Default}] You must use that {ChatColors.Lime}/countdown <TIME_SECONDS>{ChatColors.Default}.");
            return;
        }
        var time_convert = Convert.ToInt32(TimeSec);
        Time = time_convert;
        Countdown_enable = true;
        var timer = AddTimer(1.0f, () =>
        {
            if (Time == 0)
            {
                Countdown_enable = false;
                return;
            }

            Time = Time - 1.0f;
        }, TimerFlags.REPEAT);
    }
}
