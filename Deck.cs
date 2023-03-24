using System;
using System.Collections.Generic;
using System.Linq; // currently only needed if we use alternate shuffle method
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace RaceTo21_Blazor
{
    public class Deck
    {
        //replaced string with Card class to store short and long name
        //List<string> cards = new List<string>();
        //save cards privately
        private readonly List<Card> cards = new List<Card>();

        public Deck()
        {
            Console.WriteLine("Building deck...");
            //string[] suits = { "S", "H", "C", "D" };
            //since we need longname and shortname
            //we can only store longnames of the suits and get its first character for shortname
            string[] suits = { "Spades", "Hearts", "Clubs", "Diamonds" };
            //use icon to show cards in the webpage
            //create a dictionary by key is suits and value is icon
            Dictionary<string, string> cardIcon = new Dictionary<string, string>()
            {
                {"Spades", "♠"},
                {"Hearts", "♥"},
                {"Clubs", "♣"},
                {"Diamonds", "♦"},
            };

            //run for loop with card number, and add into the deck
            for (int cardNum = 1; cardNum <= 13; cardNum++)
            {
                //run loop in suits array to create cards for each number
                foreach (string cardSuit in suits)
                {
                    string shortName;
                    string longName;
                    //check card num in different condition
                    switch (cardNum)
                    {
                        //if card num = 1
                        case 1:
                            shortName = "A";
                            longName = "Ace";
                            break;
                        case 11:
                            shortName = "J";
                            longName = "Jack";
                            break;
                        case 12:
                            shortName = "Q";
                            longName = "Queen";
                            break;
                        case 13:
                            shortName = "K";
                            longName = "King";
                            break;
                        //if not above
                        default:
                            //change int to string
                            shortName = cardNum.ToString();
                            //get long name by cardNum and index of numToWords
                            string[] numToWords = { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten" };
                            //the index start from 0, so we need to - 1
                            longName = numToWords[cardNum-1];
                            break;
                    }
                    //add the card to the card list with id(inculding icon) and displayName
                    //cards.Add(new Card(shortName + cardSuit.First<char>(), longName + " of " + cardSuit));
                    cards.Add(new Card(shortName + cardIcon[cardSuit], longName + " of " + cardSuit));
                    //-----the other way to add the card image without a method or using Regex
                    /*if (shortName == "A" || shortName == "J" || shortName == "Q" || shortName == "K")
                    {
                        cardImg.Add(shortName + cardSuit.First<char>(), "card_" + cardSuit.ToLower() + "_" + shortName + ".png");
                    }
                    else
                    {
                        cardImg.Add(shortName + cardSuit.First<char>(), "card_" + cardSuit.ToLower() + "_" + shortName.PadLeft(2, '0') + ".png");
                    }*/
                }
            }
        }

        /* 
         * shuffle the card in the deck
         * call: SetCardImg()
         * called by: game
         * parameter: no
         * return: no (void)
         */
        public void ShuffleDeck()
        {
            Console.WriteLine("Shuffling Cards...");
            //create a Random as rng
            Random rng = new Random();

            //run for loop by cards' count to shuffle cards list
            for (int i=0; i<cards.Count; i++)
            {
                //replaced string with Card class
                //string tmp = cards[i];
                //create a new card by current index to exchange to the new index, save in tmp
                Card tmp = cards[i];
                //get a random num in cards.Count
                int swapindex = rng.Next(cards.Count);
                //change current card to the new index's card
                cards[i] = cards[swapindex];
                //change the new index's card to current card (tmp)
                cards[swapindex] = tmp;
            }
        }


        /* 
         * show all cards on the console
         * call: no
         * called by: game if needed
         * parameter: no
         * return: no (void)
         */
        public void ShowAllCards()
        {
            //run for loop by cards.Count to write cards in a line
            for (int i=0; i<cards.Count; i++)
            {
                //Console.Write(i+":"+cards[i]); // a list property can look like an Array!
                //Console.Write(i + ":" + cards[i].id);
                //show the long name in console
                //shows like 1:Ace of Club
                Console.Write(i + ":" + cards[i].displayName);
                if (i < cards.Count -1)
                {
                    //separate names
                    Console.Write(" ");
                }
                else
                {
                    //the last one write above names on console
                    Console.WriteLine("");
                }
            }
        }

        /* 
         * get the last card in deck
         * There are two args in the Card.Card field
         * so we need to give the two args back when we need to add the card into player's hand
         * when (nextTask == Task.PlayerTurn)
         * call: no
         * called by: game
         * parameter: no
         * return: string, string - card id, card long name
         */
        public (string, string) DealTopCard()
        {
            //string card = cards[cards.Count - 1];
            //the index start from 0, so we need to -1
            //get the last card's id
            string card = cards[cards.Count - 1].id;
            //get the last card's long name
            string cardLong = cards[cards.Count - 1].displayName;
            //remove the last card from the deck
            cards.RemoveAt(cards.Count - 1);
            return (card, cardLong);
        }
    }
}

