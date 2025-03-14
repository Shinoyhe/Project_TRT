// Global Variables. Please put these at the top.
// Flag variable. Can be any name you want. Default false
VAR flag_1 = false
VAR flag_2 = false
VAR flag_3 = false
// Inventory Card Variable (Info Card or Item Card). Default false
// Name NEEDS to have "IC_" before the name. It'd be best if you keep this consistent with the item/info card name.
// Programmers will make sure this name matches the card in game. 
VAR IC_goal_card_0 = false
VAR IC_goal_card_1 = false
VAR IC_goal_card_2 = false
VAR IC_goal_card_3 = false

-> Start
=== Start ===
Hey! This tests the flag tracker #NPC
    + Flag 1 = {flag_1}
        ~flag_1 = !flag_1
        -> Start
    + Flag 2 = {flag_2}
        ~flag_2 = !flag_2
        -> Start
    + More
        ++ flag_3 = {flag_3}
            ~flag_3 = !flag_3
            -> Start
        ++ Goal Card 0 = {IC_goal_card_0}
            ~IC_goal_card_0 = !IC_goal_card_0
            -> Start
        ++ Goal Card 1 = {IC_goal_card_1}
            ~IC_goal_card_0 = !IC_goal_card_0
            -> Start
        ++ More
            +++ Goal Card 2 = {IC_goal_card_2}
                ~IC_goal_card_2 = !IC_goal_card_2
                -> Start
            +++ Goal Card 3 = {IC_goal_card_2}
                ~IC_goal_card_2 = !IC_goal_card_2
                -> Start
            +++ Barter Ya? #Barter
                -> END
            +++ Return -> Start
    + Leave -> END

=== BarterWin ===
You did well. #NPC
-> END

=== BarterLose ===
You lost. #NPC
-> END