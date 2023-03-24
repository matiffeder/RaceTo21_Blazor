using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System;
using System.Xml.Linq;

namespace RaceTo21_Blazor.Pages
{
    public partial class Index
    {
        static private string StatusColor(PlayerStatus status)
        {
            string statusColor = "";
            switch (status)
            {
                case PlayerStatus.stay:
                    statusColor = "#6E6E6E";
                    break;
                case PlayerStatus.bust:
                    statusColor = "#BF2A33";
                    break;
                case PlayerStatus.win:
                    statusColor = "#F39B16";
                    break;
            }
            return statusColor;
        }

        private readonly string[] showNameField = new string[] { "flex", "flex", "none", "none", "none", "none", "none", "none" };
        //Array.Fill(showNameField, "none");
        private void GetPlayersNum(ChangeEventArgs e)
        {
            int count = int.TryParse(e.Value.ToString(), out int _) ? Int16.Parse(e.Value.ToString()) : 2;
            for (int i = 2; i < 8; i++)
            {
                if (i < count)
                    showNameField[i] = "flex";
                else
                    showNameField[i] = "none";
            }
            if (count > 8) count = 8;
            if (count < 2) count = 2;
            Game.numberOfPlayers = count;
        }

        //a list to save player names
        //save player names list privatly
        static public string[] playerNames = new string[8];
        private readonly string[] borderColors = new string[] { "#ced4da", "#ced4da", "#ced4da", "#ced4da", "#ced4da", "#ced4da", "#ced4da", "#ced4da" };
        //Array.Fill(borderColors, "#ced4da");
        private string showWarning = "hidden";
        /*
         * heck if there is the same name in the playerNames list to avoid confusing
         * call: no
         * called by: cardtable
         * parameter: string - the name will add
         * return: bool - true if there is the same name
         */
        private void GetPlayerName(ChangeEventArgs e, int inputID)
        {
            string name = e.Value.ToString();
            if (name != null && name != "")
            {
                if (CheckSameName(name))
                {
                    borderColors[inputID] = "red";
                }
                else
                {
                    borderColors[inputID] = "#ced4da";
                }
            }
            else
            {
                borderColors[inputID] = "red";
            }
            playerNames[inputID] = name;
            CheckAllSameNames();
            //add the names that users inputted to the playerNames list
            //so that we can use them after restart the game
        }

        /* 
         * check if there is the same name in the playerNames list to avoid confusing
         * call: no
         * called by: cardtable
         * parameter: string - the name will add
         * return: bool - true if there is the same name
         */
        static private bool CheckSameName(string addName)
        {
            //check each name in the playerNames
            foreach (string name in playerNames)
            {
                //if found the same name then return true for checking
                if (addName == name)
                {
                    return true;
                }
            }
            //if there is no the same name
            return false;
        }

        private bool CheckAllSameNames()
        {
            bool foundSame = false;
            int sameCount = 0;
            //check each name in the playerNames
            for (int i = 0; i < Game.numberOfPlayers; i++)
            {
                if (playerNames[i] != null && playerNames[i] != "")
                {
                    borderColors[i] = "#ced4da";
                    for (int j = 0; j < Game.numberOfPlayers; j++)
                    {
                        if (playerNames[j] != null && playerNames[j] != "" )
                        {
                            //borderColors[j] = "#ced4da";
                            if (playerNames[i] == playerNames[j])
                            {
                                sameCount += 1;
                                if (sameCount > 1)
                                {
                                    borderColors[i] = "red";
                                    borderColors[j] = "red";
                                    foundSame = true;
                                }
                            }
                        }
                    }
                }
                sameCount = 0;
            }
            for (int i = 0; i < Game.numberOfPlayers; i++)
            {
                if (borderColors[i] == "red")
                {
                    showWarning = "visible";
                    break;
                }
                showWarning = "hidden";
            }
            //if there is no the same name
            return foundSame;
        }

        private readonly ElementReference[] input = new ElementReference[8];
        private void InputEnter(KeyboardEventArgs e, int inputID)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                if (playerNames[inputID] == null || playerNames[inputID] == "")
                {
                    borderColors[inputID] = "red";
                }
                else
                {
                    borderColors[inputID] = "#ced4da";
                    //Game.players.Add(new Player(name));
                }
                CheckAllSameNames();
                if (inputID < 7) input[inputID + 1].FocusAsync();
            }
        }

        static private void GetGameGoal(ChangeEventArgs e)
        {
            int score = int.TryParse(e.Value.ToString(), out int _) ? Int16.Parse(e.Value.ToString()) : 1;
            if (score > 210) score = 210;
            if (score < 1) score = 1;
            Game.gamesGoal = score;
        }

        static public string displaySetting = "inherit";
        private void SaveSettings()
        {
            bool next = true;
            for (int i = 0; i < Game.numberOfPlayers; i++)
            {
                if (playerNames[i] == null || playerNames[i] == "")
                {
                    borderColors[i] = "red";
                    next = false;
                }
                else
                {
                    borderColors[i] = "#ced4da";
                }
            }
            if (!CheckAllSameNames() && next)
            {
                displaySetting = "none";
                for (int i = 1; i <= Game.numberOfPlayers*2+1; i++)
                {
                    Game.DoNextTask();
                }
            }
        }

        static public string displayGetCard = "d-flex";
        static public bool playerContinue = true;
        static private void PlayerGetCard(bool getCard)
        {
            if (!getCard) playerContinue = false;
            else playerContinue = true;
            Game.DoNextTask();
            Game.DoNextTask();
        }

        static public string displayConfirming = "d-none";
        private void ConfirmingClick()
        {
            displayConfirming = "d-none";
            (_, int? winnerID) = Game.CheckWinner();
            if (winnerID != null)
            {
                displayScores = "inherit";
            }
            else
            {
                Game.DoNextTask();
            }
        }

        private string displayScores = "none";
        private void ScoresClick()
        {
            displayScores = "none";
            if (Game.finalWinner != null)
            {
                displaySetting = "inherit";
                Game.DoNextTask();
            }
            else displayNextRound = "inherit";
        }

        static public string displayNextRound = "none";
        static public bool[] joinNextRound { get; set; } = new bool[] { true, true, true, true, true, true, true, true };
        //Array.Fill(joinNextRound, true);
        static private void NextRoundClick()
        {
            displayNextRound = "none";
            int playerCount = 0;
            for (int i = 0; i < Game.numberOfPlayers; i++)
            {
                if (joinNextRound[i])
                {
                    playerCount++;
                }
            }
            if (playerCount <= 1)
            {
                Game.DoNextTask();
                Game.DoNextTask();
            }
            else
            {
                for (int i = 1; i <= Game.numberOfPlayers * 2 + 1; i++)
                {
                    Game.DoNextTask();
                }
            }
        }
    }
}
