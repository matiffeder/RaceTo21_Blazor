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
        static public int numberOfPlayers { get; set; } = 2;//players count

        //use public property to get set players list ,and set privatly
        static private List<Player> _players = new();
        static public List<Player> players { get { return _players; } private set { _players = value; } }

        static private Deck _deck = new(); //cards deck
        static public Deck deck { get { return _deck; } private set { _deck = value; } } //cards deck

        static private int _currentPlayer = 0; //current turn of players
        static public int currentPlayer { get { return _currentPlayer; } private set { _currentPlayer = value; } }
        
        //use enum to store the next task
        //save next task privatly
        static private Tasks nextTask; //to define what to do next
        //track the first player who get the "highest score"
        static private int firstHighPoints = 0;
        //track the first "player" who get the "highest score"
        static private Player? firstHighPlayer;
        //the score to end the game
        static public int gamesGoal { get; set; } = 21;
        //
        static public Player? _finalWinner;
        static public Player? finalWinner { get { return _finalWinner; } private set { _finalWinner = value; } }

        static private void GameReset()
        {
            //claim a new deck for new game since some cards have removed
            Deck newDeck = new Deck();
            //apply the new deck for the game
            deck = newDeck;
            //start from the new turn's first player
            //player might leave the game the, currentPlayer couldn't bigger than the player list
            currentPlayer = 0;
            //reset first high points
            firstHighPoints = 0;
            //clear the list to store a new ranking
            CardTable.scoresRanking.Clear();
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
         * call: GetNumberOfPlayers, GetPlayerName, AddPlayer, ShowPlayers, ShowHands, YesOrNo, DealTopCard, PointsInHand, CheckToEnd, CheckWinner, AnnounceWinner, Shuffle, ShufflePlayers
         * called by: Program
         * parameter: no
         * return: no (void)
         */
        //it seems we don't need this line since the first task in enum is GetNumberOfPlayers
        static public void DoNextTask()
        {
            Console.WriteLine("================================" + nextTask);
            //task for getting score of game goal
            if (nextTask == Tasks.SaveSetting)
            {
                deck.ShuffleDeck();
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    AddPlayer(Pages.Index.playerNames[i]);
                }
                nextTask = Tasks.PlayerTurn;
                Pages.Index.displayGetCard = "d-flex";
                Console.WriteLine("================================");
            }
            else if (nextTask == Tasks.ShowConfirming)
            {
                /*
                if (players[currentPlayer].points != 0 && players[currentPlayer].status == PlayerStatus.active) Pages.Index.displayGetCard = "d-flex";
                else Pages.Index.displayGetCard = "d-none";
                nextTask = Tasks.PlayerTurn;
                */
                Pages.Index.displayConfirming = "d-flex";
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
                    //force players to get a card at the begining (player.points == 0)
                    //should check player.points==0 before YesOrNo()
                    //ask current player if they want a card and get the result : YesOrNo(player.name + ", do you want a card?"))
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
                        //check the highest points (player.points < 21 has exluded by above conditions
                        if (player.points > firstHighPoints)
                        {
                            //save new highest points
                            firstHighPoints = player.points;
                            //save the player with the highest points
                            firstHighPlayer = player;
                        }
                    }
                }
                //show winner confirming when player did not has 21
                if (player.status == PlayerStatus.bust) nextTask = Tasks.ShowConfirming;
                //if (player.status == PlayerStatus.bust || player.status == PlayerStatus.win) nextTask = Tasks.ShowConfirming;
                else nextTask = Tasks.CheckForEnd;
            }
            //task for checking if to go to the end of the current game
            else if (nextTask == Tasks.CheckForEnd)
            {
                //Wrote a new field to check if the game is finished
                //if (!CheckActivePlayers())
                //check if current game needs to end
                if (CheckToEnd())
                {
                    Pages.Index.displayGetCard = "d-none";
                    //check the highest points of players and players' status to find the winner
                    (Player winner, _) = CheckWinner();
                    //AnnounceWinner uses arg.name to show the name
                    CardTable.AnnounceWinner(winner);
                    Pages.Index.displayConfirming = "d-flex";
                    winner.gamesScore += winner.points;
                    CardTable.ShowScores(players);
                    //Pages.Index.displayScores = "inherit";
                    Console.WriteLine("                       Goal: " + gamesGoal);
                    Console.WriteLine("================================");
                    //if gamesGoal score reached
                    if (winner.gamesScore >= gamesGoal)
                    {
                        //use $ to allowe write value in string with {}
                        //https://learn.microsoft.com/zh-tw/dotnet/csharp/language-reference/tokens/interpolated
                        Console.WriteLine($"{winner.name} reached {gamesGoal} and is the final winner!!!");
                        Console.WriteLine("");
                        //next task is to end the current game
                        finalWinner = winner;
                        nextTask = Tasks.GameOver;
                    }
                    else
                    {
                        Array.Fill(Pages.Index.joinNextRound, true);
                        nextTask = Tasks.NextRound;
                    }
                }
                else //CheckToEnd() get false, can't find the winner
                {
                    for (int i = currentPlayer + 1; i < players.Count + currentPlayer + 1; i++)
                    {
                        if (i > players.Count - 1) i -= players.Count;
                        if (players[i].status == PlayerStatus.active)
                        {
                            currentPlayer = i;
                            break;
                        }
                    }
                    //next task is to deliver cards for next player
                    nextTask = Tasks.PlayerTurn;
                }
            }
            else if (nextTask == Tasks.NextRound)
            {
                (Player winner, _) = CheckWinner();
                //restart a game with different players
                //Program.Main();

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
                if (players.Count <= 1)
                {
                    Console.WriteLine("No enough players :(");
                    Console.WriteLine("================================");
                    //it will be more logical for me if players.Count()<=1 then end the game rather than the one only left win the game
                    //end the game
                    Pages.Index.displaySetting = "inherit";
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
                        Console.WriteLine("================================");
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
                    GameReset();
                    deck.ShuffleDeck();
                    //next task is to introduce players
                    nextTask = Tasks.PlayerTurn;
                    Pages.Index.displayGetCard = "d-flex";
                    Console.WriteLine("================================");
                }
            }
            else if (nextTask == Tasks.GameOver)
            {
                GameReset();
                players = new();
                nextTask = Tasks.SaveSetting;
            }
            else //can't find nextTask or other situation
            {
                Console.WriteLine("I'm sorry, I don't know what to do now!");
                GameReset();
                players = new();
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
         * called by: DoNextTask()
         * parameter: no
         * return: Player - player data
         */
        static public (Player, int?) CheckWinner()
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].status == PlayerStatus.win)
                {
                    return (players[i], i);
                }
            }
            //define the highPoints as 0 first to check highest point later
            //int highPoints = 0;
            //show result
            CardTable.ShowHands(players);
            Console.WriteLine("================================");
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
            //claim a new radom
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
