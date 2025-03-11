// Global Variables. Please put these at the top.
// Flag variable. Can be any name you want. Default false
VAR Solar_for_IC_4 = false
// Inventory Card Variable (Info Card or Item Card). Default false
// Name NEEDS to have "IC_" before the name. It'd be best if you keep this consistent with the item/info card name.
// Programmers will make sure this name matches the card in game. 
VAR IC_info_card_1 = false
VAR IC_info_card_2 = false
VAR IC_solar_panel = false

->Intro

=== Intro ===
Hello and welcome. It’s Dead Mail day. How can the City Packet Service serve you today? #NPC
->Start

=== Start ===
* I would like to Barter. 
    -> Barter
*Dead mail day?
    That is correct! No packages are delivered on Sundays, so any and all undelivered mail is pawned off by me. #NPC
    ->Dead_Mail
* What is your favorite fruit?
    Well, the one that keeps me charged of course! Let's me run around the clock and do what I do best: Deliver mail. #NPC
    ->Start
* {IC_info_card_1 and IC_info_card_2} Do you know anything about these two?
    -> UnlockBarterOption
* Nevermind. // Exit dialogue
    -> END

=== Dead_Mail ===
*How does mail go undelivered?
    Mail can go undelivered for a variety of reasons! Incorrect address, no return address, but rest assured, everything I barter off has value! #NPC
    ->Dead_Mail
*So you just stand here all day?
    All day is quite an overexaggeration! I stand here from 8-4 sharp every Sunday. #NPC
    ->Dead_Mail
*I want to talk about something else.
    ->Start

// Recommended knot just to keep track of where bartering starts
// Only use #Barter in NPC scripts. Required to have tag in NPC scripts.
=== Barter ===
Many parties want this lot. Convince me, please. #NPC
NULL_LINE #Barter // This line is necessary as barter starts the instant you move to the line with the tag
-> END

// Example of unlocking a barter option
// Given an info card or item the NPC 
=== UnlockBarterOption ===
I am familiar with both. I often see F1X on my mail routes, and on my off hours I am curious about her escapades. #NPC.
I may have already revealed too much. #NPC
* Anyway you can reveal more?
    I am in need of some goods that F1X has, but I must stay here for the rest of the day. Is that a serviceable answer? #NPC
    ** {IC_solar_panel} I have something like that.
        Then we should conduct a trade. #NPC
        ~Solar_for_IC_4 = true // Document somewhere what trade this unlocks
        ->Start
    ** I'll go see what I can fetch.
        The City Packet Service thanks you. #NPC
        ->END

// Required Knots for NPCs
=== BarterWin === 
The Packet Service hopes you enjoy your package. #NPC
-> END

=== BarterLose ===
{shuffle:
- I don't see how your barter helps the Packet Service. #NPC
- No deal ...And clean up these packing peanuts, please. #NPC
}
-> END