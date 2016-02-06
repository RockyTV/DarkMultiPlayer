using System;

namespace DarkMultiPlayerServer
{
    public class SendCommand
    {
        public static void HandleCommand(string commandArgs)
        {
            ClientObject pmPlayer = null;
            int matchedLength = 0;
            foreach (ClientObject testPlayer in ClientHandler.GetClients())
            {
                //Only search authenticated players
                if (testPlayer.authenticated)
                {
                    //Try to match the longest player name
                    if (commandArgs.StartsWith(testPlayer.playerName) && testPlayer.playerName.Length > matchedLength)
                    {
                        //Double check there is a space after the player name
                        if ((commandArgs.Length == testPlayer.playerName.Length))
                        {
                            pmPlayer = testPlayer;
                            matchedLength = testPlayer.playerName.Length;
                        }
                    }
                }
            }
            if (pmPlayer != null)
            {
				Messages.ScenarioData.SendScenarioModules(pmPlayer);
				DarkLog.Debug("Sent scenario modules to " + pmPlayer.playerName);
            }
            else
            {
                DarkLog.Normal("Player not found!");
            }
        }
    }
}

