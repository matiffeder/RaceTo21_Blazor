
# [RaceTo21](https://matiffeder.github.io/RaceTo21_Blazor/)
### A game made by Blazor WebAssembly - [Link](https://matiffeder.github.io/RaceTo21_Blazor/)
---
#### Features:  
* Ask players how many scores they want to win the “whole” game.  
→ The winner in a round will get the score according to the point in hand.  
→ Deducting the score if the player is busted.  
→ After one player reaches the goal score, the player will be the final winner.  

* If no one reaches 21 in a round and players are all stay or bust, the player who reaches the highest score in this round first and stays would win the round. Not the player whose turn is earlier in a turn win the round. 

* [Additional] Show the score of players in order after finding a winner.  
→ Then, ask each player if they want to join the next round.  
→ If only one player is left, that one wins.  
→ [Additional] If no one remain in the game, the game goes to the end.  
→ If more than one player, the game continues, and the player list shuffles.  
→ The previous winner will be the last one to get a card in the next round. 

#### Other Additional Improvements:
* Check if the player inputs the name that has already been taken.
* Force people to get a card in the first round (Different from the logic in the homework).
