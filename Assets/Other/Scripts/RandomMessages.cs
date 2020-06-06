using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Picks a random message when something happens
public static class RandomMessages
{
    //Data
    private static string[] player_death_throwable_general = new string[0]
    {

    };
    private static string[] player_death_throwable_snowball = new string[3]
    {
        "{0} lost their nose to {1}'s shot.",
        "{1} thoroughly memed {0}.",
        "{0}: I am dead!   {1}: Correct!"
    };
    private static string[] player_death_suicide = new string[3]
    {
        "{0} thought this was idiotic and went to hang themselves in style.",
        "What comes up, comes down on {0}.",
        "{0} took the death sentence"
    };
    private static string[] player_death_hypothermia = new string[2]
    {
        "{0} met a chilly end.",
        "{0} froze to oblivion."
    };
    private static string[] player_leftgame = new string[3]
    {
        "{0} threw in the towel.",
        "{0} went to go play minecraft.",
        "{0} realized they left the oven on."
    };
    private static string[] player_joingame = new string[5]
    {
        "{0} dropped in. Welcome to the warzone.",
        "{0} joined. You're in Snow Man's Land.",
        "{0} popped into the fort. You'll be assigned to a station soon.",
        "Guess who came to the playground? {0}!",
        "{0} joined... EVERYONE TO YOUR BATTLE STATIONS!"
    };
    private static string[] player_death_machinery_general = new string[1]
    {
        "{1} was killed by THE AGE OF AUTOMATION."
    };
    private static string GetRandomFormattedMessage(string[] randomMessages, string[] replacable) 
    {
        //Pick a random death message
        string unformatted = randomMessages[UnityEngine.Random.Range(0, randomMessages.Length)];
        //Format it correctly
        return string.Format(unformatted, replacable);
    }
    #region Deaths
    public static string Player_Death_Throwable_Snowball(string damagedPlayerName, string snowballOwner)
    {
        //Get a random message that is formatted correctly
        return GetRandomFormattedMessage(player_death_throwable_snowball, new string[2] { damagedPlayerName, snowballOwner });
    }
    public static string Player_Death_Hypothermia(string damagedPlayerName)
    {
        //Get a random message that is formatted correctly
        return GetRandomFormattedMessage(player_death_hypothermia, new string[1] { damagedPlayerName });
    }
    public static string Player_Death_Suicide(string damagedPlayerName)
    {
        //Get a random message that is formatted correctly
        return GetRandomFormattedMessage(player_death_suicide, new string[1] { damagedPlayerName });
    }
    #endregion
    public static string Player_Joingame(string playerName)
    {
        //Get a random message that is formatted correctly
        return GetRandomFormattedMessage(player_joingame, new string[1] { playerName });
    }
    public static string Player_Leftgame(string playerName)
    {
        //Get a random message that is formatted correctly
        return GetRandomFormattedMessage(player_leftgame, new string[1] { playerName });
    }
}