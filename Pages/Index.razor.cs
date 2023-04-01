//I didn't add anything on here
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System;
using System.Xml.Linq;

namespace RaceTo21_Blazor.Pages
{
    //a part of Index class
    public partial class Index
    {
        /* 
         * get status color by PlayerStatus
         * call: none
         * called by: Index.razor (self)
         * parameter: PlayerStatus - player status
         * return: string - status color
         */
        //when to use static
        //ca1822 https://learn.microsoft.com/zh-tw/dotnet/fundamentals/code-analysis/quality-rules/ca1822
        static private string StatusColor(PlayerStatus status)
        {
            //the default color to return
            string statusColor = "#000";
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
            //return status color according above conditions
            return statusColor;
        }

        //a list to save player names, array length is 8
        //a temp list, will call by game to add players list, also for checking invalid name
        static public string[] playerNames = new string[8];
        //border to highlight error fields, borderColors is readonly but borderColors[i] is not
        //default is #ced4da
        private readonly string[] borderColors = new string[] { "#ced4da", "#ced4da", "#ced4da", "#ced4da", "#ced4da", "#ced4da", "#ced4da", "#ced4da" };
        //show or hide warning message if some fields are invalid
        private string showWarning = "hidden";
        /*
         * get the name from input field, can't use in onchange to save playerNames[i] including @bind:event=oninput
         * call: CheckAllSameNames()
         * called by: Index.razor (self)
         * parameter: ChangeEventArgs - event values, int - ID of input fields
         * return: none (void)
         */
        private void GetPlayerName(ChangeEventArgs e, int inputID)
        {
            //save player name to the temp list according to the field ID and field value
            playerNames[inputID] = e.Value.ToString();
            //checking if the name invalid according to the playerNames list
            CheckAllSameNames();
        }

        /* 
         * checking if the name invalid according to the playerNames list
         * call: none
         * called by: Index.razor.cs (self)
         * parameter: none
         * return: bool - true if there is the same name
         */
        private bool CheckAllSameNames()
        {
            //the variable to return. if didn't find the same name will return false here
            bool foundSame = false;
            //check each name in the playerNames by the setting of numberOfPlayers
            //length of playerNames is 8, so use Game.numberOfPlayers
            for (int i = 0; i < Game.numberOfPlayers; i++)
            {
                //https://learn.microsoft.com/zh-tw/dotnet/api/system.string.isnullorwhitespace?view=net-7.0
                //checking if null or only white spaces
                //if the name is valid
                if (!string.IsNullOrWhiteSpace(playerNames[i]))
                {
                    //use default color
                    borderColors[i] = "#ced4da";
                    for (int j = 0; j < Game.numberOfPlayers; j++)
                    {
                        //if the comparing name is also valid
                        if (!string.IsNullOrWhiteSpace(playerNames[i]))
                        {
                            //if found the same name
                            if (i != j && playerNames[i] == playerNames[j])
                            {
                                //highlight error fields
                                borderColors[i] = "red";
                                borderColors[j] = "red";
                                //will return ture
                                foundSame = true;
                            }
                        }
                    }
                }
            }
            //if no field is invalid
            showWarning = "hidden";
            for (int i = 0; i < Game.numberOfPlayers; i++)
            {
                //if any of the field is invalid (borderColors[i]="red"), including "red" by onther functions
                if (borderColors[i] == "red")
                {
                    //show the warning message
                    showWarning = "visible";
                    break;
                }
            }
            //return bool according if found the same name
            return foundSame;
        }

        //https://stackoverflow.com/questions/59137973/how-to-set-the-focus-to-an-inputtext-element
        //use for ref tag in html to use FocusAsync to change the focus field
        private readonly ElementReference[] input = new ElementReference[10];
        /* 
         * change the focus field by key of enter, down, or up
         * call: CheckAllSameNames()
         * called by: Index.razor (self)
         * parameter: KeyboardEventArgs - key codes, int - ID of input fields
         * return: none (void)
         */
        private void InputEnter(KeyboardEventArgs e, int inputID)
        {
            //if key is enter, down, or up (used Console.WriteLine found the values)
            //Console.WriteLine("-------------------------------------" + e.Key);
            if (e.Key == "Enter" || e.Key == "Tab" || e.Key == "ArrowDown" || e.Key == "ArrowUp")
            {
                //if input field is for a player name
                if (inputID <= 7)
                {
                    //if it is null or only spaces
                    //https://learn.microsoft.com/zh-tw/dotnet/api/system.string.isnullorwhitespace?view=net-7.0
                    if (string.IsNullOrWhiteSpace(playerNames[inputID]))
                    {
                        //highlight error fields
                        borderColors[inputID] = "red";
                    }
                    else
                    {
                        //if not null or only spaces, set as default
                        borderColors[inputID] = "#ced4da";
                    }
                    //checking if the name in the field is invalid, should run after checking null to show the highlight that is not null
                    CheckAllSameNames();
                    if (e.Key != "Tab")
                    {
                        //the inputID for name field is bound with player list, so they are from 0 - 7
                        //if it is not the last field for names, go to the field below
                        if (inputID < Game.numberOfPlayers - 1 && e.Code != "ArrowUp") _ = input[inputID + 1].FocusAsync();
                        //if it is not the first field for names, go to the field above 
                        if (inputID > 0 && e.Code == "ArrowUp") _ = input[inputID - 1].FocusAsync();
                        //if it is the last field for names, go to the field of points to win
                        if (inputID == Game.numberOfPlayers - 1 && e.Code != "ArrowUp") _ = input[8].FocusAsync();
                        //if it is the first field for names, go to the field of num of players
                        if (inputID == 0 && e.Code == "ArrowUp") _ = input[9].FocusAsync();
                    }
                }
            }
            if (e.Key == "Enter")
            {
                //if it is the field of points to win, go to the field of num of players
                if (inputID == 8) _ = input[9].FocusAsync();
                //if it is the field of num of players, go to the first field for names
                if (inputID == 9) _ = input[0].FocusAsync();
            }
        }

        //the variable to hide or show setting frame
        static public string displaySetting = "inherit";
        /* 
         * the method of click start button on the setting frame to save the settings
         * call: DoNextTask - Game
         * called by: Index.razor (self)
         * parameter: none
         * return: none (void)
         */
        private void SaveSettings()
        {
            //the variable to check if to start the game
            //if all the names is valid run the next task
            bool next = true;
            //check the name fields according numberOfPlayers
            for (int i = 0; i < Game.numberOfPlayers; i++)
            {
                //if null or only white spaces
                if (string.IsNullOrWhiteSpace(playerNames[i]))
                {
                    //highlight the error field
                    borderColors[i] = "red";
                    //do the run the next task
                    next = false;
                }
                else
                {
                    //show border as default color
                    borderColors[i] = "#ced4da";
                }
            }
            //run CheckAllSameNames() to find invalid fields
            //if no same name found and not null or only white spaces
            if (!CheckAllSameNames() && next)
            {
                //close the setting frame
                displaySetting = "none";
                //go to Tasks.SaveSetting -> [PlayerTurn -> CheckForEnd] * numberOfPlayers times
                //because each player get a card in the first turn
                for (int i = 1; i <= Game.numberOfPlayers*2+1; i++)
                {
                    Game.DoNextTask();
                }
            }
        }

        //the variable to show or hide the frame to ask player to get a card
        //the frame will show after players.Count>1 (after click the start button)
        //it will not hide until the scores frame shows
        static public string displayGetCard = "d-flex";
        //check if the current player want to continue
        static public bool playerContinue = true;
        /* 
         * get the response of if the get a card from current player
         * call: DoNextTask - Game
         * called by: Index.razor (self)
         * parameter: bool - button yes is true / no is false
         * return: none (void)
         */
        static private void PlayerGetCard(bool getCard)
        {
            //click no to stay
            if (!getCard) playerContinue = false;
            //click yes to get card
            else playerContinue = true;
            //go to Tasks.PlayerTurn
            Game.DoNextTask();
            //go to Tasks.CheckForEnd
            Game.DoNextTask();
        }

        //the variable to show or hide the confirming frame of win or bust
        static public string displayConfirming = "d-none";
        /* 
         * the behavior after click on the button of confirming frame
         * call: DoNextTask - Game, CheckWinner - Game
         * called by: Index.razor (self)
         * parameter: none
         * return: none (void)
         */
        private void ConfirmingClick()
        {
            //hide the frame after click the button
            //the frame will show if there is a winner or current player busted (Tasks.ShowConfirming)
            displayConfirming = "d-none";
            //check if there is a winner
            (_, int? winnerID) = Game.CheckWinner();
            if (winnerID != null)
            {
                //if there is a winner show the score frame
                displayScores = "inherit";
            }
            else
            {
                //go to Tasks.CheckForEnd
                Game.DoNextTask();
            }
        }

        //the variable to show or hide the scores frame
        private string displayScores = "none";
        /* 
         * the behavior after click on the button of scores frame
         * call: DoNextTask - Game
         * called by: Index.razor (self)
         * parameter: none
         * return: none (void)
         */
        private void ScoresClick()
        {
            //hide the frame after click the button
            displayScores = "none";
            //check if there is a final winner
            if (Game.finalWinner != null)
            {
                //reset the playerNames array
                playerNames = new string[8];
                //show the setting frame
                displaySetting = "inherit";
                //go to Tasks.GameOver
                Game.DoNextTask();
            }
            //show next the round frame
            else 
            {
                //check all the fields on the frame for players joining next round
                Array.Fill(joinNextRound, true);
                //show the frame for players joining next round
                displayNextRound = "inherit";
            }
        }

        //the variable to show or hide the next round frame
        static public string displayNextRound = "none";
        //the bind variable of check box on next round frame
        //using for game to remove player who want to leave the game
        //checked is default
        //the first character is not capital because for me it is more like a field although I know the book said I need to capitalize it
        static public bool[] joinNextRound { get; private set; } = new bool[] { true, true, true, true, true, true, true, true };
        /* 
         * the behavior after click on the button of next round frame
         * call: DoNextTask - Game
         * called by: Index.razor (self)
         * parameter: none
         * return: none (void)
         */
        static private void NextRoundClick()
        {
            //hide the frame after click the button
            displayNextRound = "none";
            //if players fewer than 2
            if (CountingJoin() <= 1)
            {
                //reset the playerNames array
                playerNames = new string[8];
                //go to Tasks.NextRound to check the check boxes values on the frame
                Game.DoNextTask();
                //go to Tasks.GameOver
                Game.DoNextTask();
            }
            else
            {
                //go to Tasks.NextRound -> [PlayerTurn -> CheckForEnd] * numberOfPlayers times
                //because each player get a card in the first turn
                for (int i = 1; i <= Game.numberOfPlayers * 2 + 1; i++)
                {
                    Game.DoNextTask();
                }
            }
        }

        /* 
         * count the number of players who continue play the game
         * call: none
         * called by: Index.razor (self)
         * parameter: none
         * return: int - number of checked boxes (number of players)
         */
        static private int CountingJoin()
        {
            //store the number of checked boxes
            int playerCount = 0;
            //for loop to check the joinNextRound list according to the length of numberOfPlayers
            for (int i = 0; i < Game.numberOfPlayers; i++)
            {
                //if checked
                if (joinNextRound[i])
                {
                    //playerCount + 1
                    playerCount++;
                }
            }
            //return number of checked boxes (number of players)
            return playerCount;
        }
    }
}
