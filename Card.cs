using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RaceTo21_Blazor
{
    public class Card
    {
        //use public property to get card id, set card id privately
        public string id { get; private set; }
        //use public property to get card long name, set card displayName privately
        public string displayName { get; private set; }
        //use public property to get card long name, set card displayName privately
        public string img { get; private set; }


        public Card(string name, string nameLong)
        {
            id = name;
            displayName = nameLong;
            //source: I used a lot of regex in the projects I wrote long time ago, https://www.curseforge.com/members/matif525/projects
            //https://learn.microsoft.com/zh-tw/dotnet/api/system.text.regularexpressions.regex.ismatch?view=net-7.0
            //https://learn.microsoft.com/zh-tw/dotnet/api/system.string.substring?view=net-7.0
            //https://learn.microsoft.com/zh-tw/dotnet/api/system.string.padleft?view=net-7.0
            //https://learn.microsoft.com/zh-tw/dotnet/api/system.string.indexof?view=net-7.0
            //Substring(0, 1) : remain string characters from 0 to 1
            //use regular expression to find if the string contain A, J, Q, or K
            //if not contain, check if the card num is 10, if it is do not add 0 before it 
            //if contained, do not add 0 before it
            //Q: diff between Match and IsMatch? - seems nothing different
            //string num = Regex.Match(id.Substring(0, 1), "[AJQK]").Success ? id.Substring(0, 1) : id.Substring(0, 1).PadLeft(2, '0');
            string num = Regex.IsMatch(id.Substring(0, 1), "[AJQK]") ? id.Substring(0, 1) : id.Length == 3 ? id.Substring(0, 2) : id.Substring(0, 1).PadLeft(2, '0');
            //find the index of " of " and remain the characters after " of " according to the index
            //there are four index in " of ", so we need to + 4 to cut it
            img = "card_" + displayName.Substring(displayName.IndexOf(" of ") + 4).ToLower() + "_" + num + ".png";
        }
    }
}
