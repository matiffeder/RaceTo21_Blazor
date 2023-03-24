using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceTo21_Blazor
{
    public class Card
    {
        //the way to store the image to Card class, please refer the comment in Deck.SetCardImg()
        //public string img;
        //string num = Regex.Match(cards[i].id.Substring(0, 1), "[AJQK]").Success ? cards[i].id.Substring(0, 1) : cards[i].id.Substring(0, 1).PadLeft(2, '0');

        //use public property to get card id, set card id privatly
        public string id { get; private set; }
        //use public property to get card long name, set card displayName privatly
        public string displayName { get; private set; }

        public Card(string name, string nameLong)
        {
            id = name;
            displayName = nameLong;
            ///the way to store the image to Card class, please refer the comment in Deck.SetCardImg()
            //img = "card_" + cards[i].displayName.Substring(cards[i].displayName.IndexOf(" of ") + 4).ToLower() + "_" + num
        }
    }
}
