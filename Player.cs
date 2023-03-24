﻿using System;
using System.Collections.Generic;

namespace RaceTo21_Blazor
{
	public class Player
	{
        //replaced string with Card class to store short and long name
        //public List<string> cards = new List<string>();
        //save player cards privatly
        private List<Card> playerCards = new List<Card>();
        //save player status privatly
        private PlayerStatus playerStatus = PlayerStatus.active;
        //save player points privatly
        private int playerPoints = 0;
        //save player score privatly, default is 0
        private int playerScore = 0;

        //use public property to get player's name, sat player name privatly
        public string name { get; private set; }
        //use public property to get player's cards, privatly
        public List<Card> cards { get { return playerCards; } private set { playerCards = value; } }
        //use public property to get and set player's status
        public PlayerStatus status { get { return playerStatus; } set { playerStatus = value; } }
        //use public property to get and set player's points
        public int points { get { return playerPoints; } set { playerPoints = value; } }
        //use public property to get and set player's game score
        public int gamesScore { get { return playerScore; } set { playerScore = value; } }


        public Player(string name)
		{
			this.name = name;
        }
        /* 
         * output introction with player's and player's #
         * because card table use a player list, we have index
         * call: n/a
         * called by: CardTable
         * parameter: int - player's #
         * return: no
         */
        public void Introduce(int playerNum)
		{
			Console.WriteLine("Hello, my name is " + name + " and I am player #" + playerNum);
		}
	}
}

