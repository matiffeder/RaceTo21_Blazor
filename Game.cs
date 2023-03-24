//I didn't add anything on here
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Formats.Asn1.AsnWriter;

namespace RaceTo21_Blazor
{
    public static class Game
    {
        //store the player number from setting frame, default is 2
        static private int _numberOfPlayers = 2;
        static public int numberOfPlayers {
            get { return _numberOfPlayers; } 
            set
            {
                //the value should smaller than 9 bigger than 1
                if (value > 8) value = 8;
                if (value < 2) value = 2;
                _numberOfPlayers = value;
            }
        }

        //use public property to get players list ,and set privately
        static private List<Player> _players = new();
        static public List<Player> players { get { return _players; } private set { _players = value; } }

        static private Deck _deck = new(); //cards deck for call cards
        static public Deck deck { get { return _deck; } private set { _deck = value; } } //cards deck

        static private int _currentPlayer = 0; //current player number
        static public int currentPlayer { get { return _currentPlayer; } private set { _currentPlayer = value; } }

        //use enum to store the next task
        //it seems the default task in enum is SaveSetting (first in the list)
        //so it doesn't need to set as SaveSetting at the beginning
        static private Tasks nextTask; //to define what to do next
        //track the first player who get the "highest score"
        static private int firstHighPoints = 0;
        //track the first "player" who get the "highest score"
        static private Player? firstHighPlayer;
        //the score to end the game

        //the score to end the game
        static private int _gamesGoal = 30;
        static public int gamesGoal
        {
            get { return _gamesGoal; }
            set
            {
                //the value should not bigger than 210 and smaller than 1
                if (value > 210) value = 210;
                if (value < 1) value = 1;
                _gamesGoal = value;
            }
        }
        //track the final winner, it also used in Index.razor
        static public Player? _finalWinner;
        static public Player? finalWinner { get { return _finalWinner; } private set { _finalWinner = value; } }

        /* 
         * reset the basic variables of the game
         * call: none
         * called by: game
         * parameter: none
         * return: none (void)
         */
        static private void GameReset()
        {
            //Deck newDeck = new Deck();
            //deck = newDeck;
            //same as above
            //claim a new deck for new game since some cards have removed
            deck = new();
            //start from the new turn's first player
            //player might leave the game the, currentPlayer couldn't bigger than the player list
            currentPlayer = 0;
            //reset first high points
            firstHighPoints = 0;
            //clear the list to store a new ranking
            CardTable.scoresRanking.Clear();
            //empty finalWinner
            finalWinner = null;
        }

        /* 
         * add a player of the game to players list by name
         * why do we need a method here? why don't we just use players.Add(new Player(n)) in DoNextTask()
         * call: no
         * called by: DoNextTask()
         * parameter: string - player name
         * return: no (void)
         */
        static private void AddPlayer(string n)
        {
            //add a player of the game to players list by name
            players.Add(new Player(n));
        }

        /* 
         * the method to do different next tasks
         * call: ShuffleDeck, AddPlayer, DealTopCard, PointsInHand, CheckToEnd, CardTable.CheckWinner, CardTable.AnnounceWinner, ShowScores, ShufflePlayers, GameReset
         * called by: Pages.Index
         * parameter: no
         * return: no (void)
         */
        static public void DoNextTask()
        {
            //task for saving setting
            if (nextTask == Tasks.SaveSetting)
            {
                //shuffle the deck at the first time play the game or if a new round
                //GameReset don't need to run at the first time
                //so if deck.ShuffleDeck in GameReset it will shuffle deck twice if a new game start
                deck.ShuffleDeck();
                //add players to the players list according to numberOfPlayers
                //length of playerNames might bigger than numberOfPlayers
                //because it the fields will only hide after the number change and the names remains
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    AddPlayer(Pages.Index.playerNames[i]);
                }
                //next task is for a player to get a card
                nextTask = Tasks.PlayerTurn;
                //show the get card frame if it is hide in previous round
                Pages.Index.displayGetCard = "d-flex";
                Console.WriteLine("================================");
            }
            //the task to show the confirming frame of win or bust
            else if (nextTask == Tasks.ShowConfirming)
            {
                //show the confirming frame of win or bust
                Pages.Index.displayConfirming = "d-flex";
                //next task is to check if to end the round
                nextTask = Tasks.CheckForEnd;
            }
            //task for delivering cards for a player
            else if (nextTask == Tasks.PlayerTurn)
            {
                //store current player as "player"
                Player player = players[currentPlayer];
                //if player is not stay or bust
                if (player.status == PlayerStatus.active)
                {
                    //-----the better way to fix----- if no players take cards, they all “bust”
                    //force players to get a card at the beginning (player.points == 0)
                    //should check player.points==0 before Pages.Index.playerContinue
                    //and check the result if a player want a card and 
                    if (player.points == 0 || Pages.Index.playerContinue)
                    {
                        //There are two args in the Card.Card field
                        //so we need to give the two args back when we need to add the card into player's hand
                        //string card = deck.DealTopCard();
                        //get a card and store the short and long name in shortName, longName
                        (string shortName, string longName) = deck.DealTopCard();
                        //player.cards.Add(card);
                        //add the card into player's hand with shortName, longName
                        player.cards.Add(new Card(shortName, longName));
                        //calculate the points in player's hand and save in player.points
                        player.points = PointsInHand(player);
                        //if the points of player > 21 then his status would be bust
                        if (player.points > 21)
                        {
                            player.status = PlayerStatus.bust;
                            //if a player busted, the player's score will dedcut the additional point
                            player.gamesScore += 21 - player.points;
                        }
                        //if the points of player > 21 then his status would be win
                        else if (player.points == 21)
                        {
                            player.status = PlayerStatus.win;
                        }
                    }
                    else
                    {
                        //if player didn't take a card then his status would be stay
                        player.status = PlayerStatus.stay;
                        //check the highest points (player.points < 21 has excluded by above conditions
                        if (player.points > firstHighPoints)
                        {
                            //save new highest points
                            firstHighPoints = player.points;
                            //save the player with the highest points
                            firstHighPlayer = player;
                        }
                    }
                }
                //should also show winner confirming when player did not has 21
                //so can't use following way to check
                //the winner confirming will show in Tasks.CheckForEnd
                //if (player.status == PlayerStatus.bust || player.status == PlayerStatus.win) nextTask = Tasks.ShowConfirming;
                //show the busted confirming frame
                if (player.status == PlayerStatus.bust) nextTask = Tasks.ShowConfirming;
                //next task is to check if to go to the end of the current game
                else nextTask = Tasks.CheckForEnd;
            }
            //task for checking if to go to the end of the current game
            else if (nextTask == Tasks.CheckForEnd)
            {
                //if current round needs to end
                if (CheckToEnd())
                {
                    //hide the get card frame
                    Pages.Index.displayGetCard = "d-none";
                    //check the highest points of players and players' status to find the winner
                    (Player winner, _) = CheckWinner();
                    //AnnounceWinner uses arg.name to show the name
                    CardTable.AnnounceWinner(winner);
                    //show the winner confirming frame
                    Pages.Index.displayConfirming = "d-flex";
                    //accumulate score of winner
                    winner.gamesScore += winner.points;
                    //show the scores in console
                    CardTable.ShowScores(players);
                    Console.WriteLine("                       Goal: " + gamesGoal);
                    Console.WriteLine("================================");
                    //if gamesGoal score reached
                    if (winner.gamesScore >= gamesGoal)
                    {
                        //use $ to allowe write value in string with {}
                        //https://learn.microsoft.com/zh-tw/dotnet/csharp/language-reference/tokens/interpolated
                        Console.WriteLine($"{winner.name} reached {gamesGoal} and is the final winner!!!");
                        Console.WriteLine("================================");
                        Console.WriteLine("");
                        //store the finalWinner
                        finalWinner = winner;
                        //next task is to end the current game
                        nextTask = Tasks.GameOver;
                    }
                    else //if gamesGoal score not reached
                    {
                        //go to the task for checking who want to continue in next round 
                        nextTask = Tasks.NextRound;
                    }
                }
                else //if CheckToEnd() get false, can't find the winner
                {
                    //checking who the next active player in a turn
                    //if it is not active, should not run Tasks.PlayerTurn and asking get card
                    //check from the next player to the previous player in a turn (currentPlayer + 1)
                    //the players number to check is players.count
                    for (int i = currentPlayer + 1; i < players.Count + currentPlayer + 1; i++)
                    {
                        //if i > the max id of the players list, then id in the list should be i-players.Count
                        if (i > players.Count - 1) i -= players.Count;
                        //if found the next active
                        if (players[i].status == PlayerStatus.active)
                        {
                            //store the i to the currentPlayer
                            currentPlayer = i;
                            //stop checking
                            break;
                        }
                    }
                    //next task is to deliver cards for next player
                    nextTask = Tasks.PlayerTurn;
                }
            }
            //task for checking who want to continue in next round
            else if (nextTask == Tasks.NextRound)
            {
                //check the highest points of players and players' status to find the winner
                (Player winner, _) = CheckWinner();

                //save winner's score, because we will remove and add later if he join the next game
                int winnerScore = winner.gamesScore;
                //-----remove players who don't want to join the new game
                //-----Method 2: this way will ask players question in the reverse order
                //use for loop from the end, therefore, the index of player will not change
                for (var i=players.Count-1; i>=0; i--)
                {
                    //if player choose to leave the game
                    if (!Pages.Index.joinNextRound[i])
                    {
                        //remove player from the list
                        players.Remove(players[i]);
                    }
                }
                //if only one player left or no one left or other situations make players.Count()<=1
                //it will be more logical for me if players.Count()<=1 then end the game rather than the one only left win the game
                if (players.Count <= 1)
                {
                    Console.WriteLine("No enough players :(");
                    Console.WriteLine("================================");
                    //show the setting frame
                    Pages.Index.displaySetting = "inherit";
                    //end the game
                    nextTask = Tasks.GameOver;
                }
                else //if more than one player left the new game
                {
                    //check if winner left the game
                    //the original code I use
                    //bool winnerIn = players.Contains(winner);
                    //players.Remove(winner);
                    //tried if below is possible, and it is
                    //run players.Remove(winner) and return the bool value of if it found "winner" to remove
                    bool winnerIn = players.Remove(winner);
                    //change the order for player to play the game
                    ShufflePlayers();
                    //if winner in the game
                    if (winnerIn)
                    {
                        //{0} is the first arg, the line will show like: "The previous winner, AAA,..."
                        //https://learn.microsoft.com/zh-tw/dotnet/csharp/language-reference/tokens/interpolated
                        //----------------------------todo: show players shuffled on score board
                        //----------------------------todo: show below on score board
                        Console.WriteLine("The previous winner, {0}, will be the last one in this round", winner.name);
                        //add winner to the last of players list
                        players.Add(winner);
                        //get back winner's score
                        winner.gamesScore = winnerScore;
                    }
                    //run loop to reset the players
                    foreach (var player in players)
                    {
                        //reset player's cards, clean the list
                        player.cards.Clear();
                        //reset player's status to active
                        player.status = PlayerStatus.active;
                        //reset player's points to 0
                        player.points = 0;
                    }
                    //save the new list number in case some players left
                    numberOfPlayers = players.Count;
                    //reset the game
                    GameReset();
                    //shuffle the deck
                    deck.ShuffleDeck();
                    //next task is for a player to get a card
                    nextTask = Tasks.PlayerTurn;
                    //show the get card frame if it is hide in previous round
                    Pages.Index.displayGetCard = "d-flex";
                    Console.WriteLine("================================");
                }
            }
            else if (nextTask == Tasks.GameOver)
            {
                //reset the game
                GameReset();
                players = new();
                numberOfPlayers = 2;
                //task for setting up a new game
                nextTask = Tasks.SaveSetting;
            }
            else //can't find nextTask or other situation
            {
                Console.WriteLine("I'm sorry, I don't know what to do now!");
                GameReset();
                players = new();
                numberOfPlayers = 2;
                //show the setting frame
                Pages.Index.displaySetting = "inherit";
                nextTask = Tasks.SaveSetting;
            }
        }

        /* 
         * get player's points in hand by player
         * call: no
         * called by: DoNextTask()
         * parameter: Player - player data
         * return: int - points in hand
         */
        static private int PointsInHand(Player player)
        {
            //set point as 0 at start
            int points = 0;
            //replaced string with Card class
            //foreach (string card in player.cards)
            //check all cards in player's hand
            foreach (Card card in player.cards)
            {
                //string faceValue = card.Remove(card.Length - 1);
                //get the first character (card num) by remove the last character of the card id (eg 5C - 2 character)
                string faceValue = card.id.Remove(card.id.Length - 1);
                //check card num in different condition
                switch (faceValue)
                {
                    //if card num is K, J, Q
                    case "K":
                    case "Q":
                    case "J":
                        //+10 points
                        points = points + 10;
                        break;
                    case "A":
                        points = points + 1;
                        break;
                    default:
                        //change number string to int and add the points
                        points = points + int.Parse(faceValue);
                        break;
                }
            }
            return points;
        }

        /* 
         * added more conditions to check if to end the game
         * check if the game need to go to the end
         * call: no
         * called by: DoNextTask()
         * parameter: no
         * return: bool - falst to continue, true to end
         */
        static private bool CheckToEnd()
        {
            //set busted players count as 0
            int busted = 0;
            //set inactive players count as 0
            int inactive = 0;
            //check each player in players list by foreach
            foreach (var player in players)
            {
                //if someone win, then end the game
                if (player.status == PlayerStatus.win)
                {
                    //true to end the current game
                    return true;
                }
                //count the busted player
                if (player.status == PlayerStatus.bust)
                {
                    //busted=busted+1;
                    busted++;
                }
                //count the number of stay or the number of bust
                //the status win has returned, so it will not count win in
                if (player.status != PlayerStatus.active)
                {
                    inactive++;
                }
            }
            //if all stay or bust, then end the game
            if (numberOfPlayers==inactive && firstHighPlayer!=null)
            {
                //busted will not be firstHighPlayer because firstHighPlayer is the player who stay
                firstHighPlayer.status = PlayerStatus.win;
                return true;
            }
            //if all but one player “busts”, then end the game
            if (numberOfPlayers - 1 == busted)
            {
                //check each player in players list by foreach
                foreach (var player in players)
                {
                    //check the remain one and change the status of the player to win
                    if (player.status == PlayerStatus.active)
                    {
                        player.status = PlayerStatus.win;
                        return true;
                    }
                }
            }
            //if not above condition, then continue the game
            return false;
        }

        /* 
         * check the winner by players' points and status
         * call: CardTable.ShowHands - show current table
         * called by: DoNextTask(), Pages.Index
         * parameter: no
         * return: Player - player data, int player ID
         */
        static public (Player, int?) CheckWinner()
        {
            //for loop to check players list
            for (int i = 0; i < players.Count; i++)
            {
                //check status
                if (players[i].status == PlayerStatus.win)
                {
                    //return the player and the ID
                    return (players[i], i);
                }
            }
            //define the highPoints as 0 first to check highest point later
            //int highPoints = 0;
            //show result
            CardTable.ShowHands(players);
            Console.WriteLine("================================");
            //if now one is win
            return (null, null);
        }

        /* 
         * shuffle players in list
         * able to merge with Deck.Shuffle by using args, if there is a way that args accept List<Player> List<Cards>
         *    private void ShufflePlayers(List<Player> players)
         * call: no
         * called by: DoNextTask()
         * parameter: no
         * return: no (void)
         */
        static private void ShufflePlayers()
        {
            Console.WriteLine("Shuffling Players...");
            //claim a new random
            Random newOrder = new Random();

            //run for loop by players' count to shuffle players in list
            for (int i = 0; i < players.Count; i++)
            {
                //create a new player by current index to exchange to the new index, save in tmp
                Player tmp = players[i];
                //get a random num in players.Count
                int swapindex = newOrder.Next(players.Count);
                //change the data of the current player by new index
                players[i] = players[swapindex];
                //the new index use the current index of player data
                players[swapindex] = tmp;
            }
        }
    }
}
