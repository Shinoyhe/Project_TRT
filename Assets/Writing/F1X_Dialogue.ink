// Global Variables. Please put these at the top.
// Flag variable. Can be any name you want. Default false
VAR key_for_USB = false
VAR gave_date_note = false
// Inventory Card Variable (Info Card or Item Card). Default false
// Name NEEDS to have "IC_" before the name. It'd be best if you keep this consistent with the item/info card name.
// Programmers will make sure this name matches the card in game. 
VAR IC_info_card_2 = false
VAR IC_info_card_4 = false
VAR IC_grove_key = false
VAR IC_date_note = false

->Intro

=== Intro ===
Welcome to the best decision you'll make all day.
->Start

=== Start ===
Welcome to the best decision you'll make all day. #NPC
* I would like to Barter. 
    -> Barter
* {IC_info_card_2 and IC_info_card_4} Got some great news for you F1X!
    {gave_date_note:
        ->GaveDateNote
    - else:
        ->UnlockBarterOption
    }
* What is the district like?
    Well I survive 'ere don't I? Even if I don't 'ave the best reputation with the denizens, it is where I reside. #NPC
    ->Start
*What is your favorite fruit?
    I have a particular fascination with mango, also maybe whatever's growin' in you cat. How about you? #NPC
        **I also like mango.
            Good choice. #NPC
            ->Start
        **What did you say?
            I'm not sure what you're referin' to. #NPC
            ->Start
*What is your favorite thing to do?
    I like collectin' and sellin' things. Gets me by and makes the time pass nicely. #NPC
    ->Start

* Nevermind. // Exit dialogue
    -> END

// Recommended knot just to keep track of where bartering starts
// Only use #Barter in NPC scripts. Required to have tag in NPC scripts.
=== Barter ===
Didn't get this cheaply, ain't gonna sell it cheaply. #NPC
NULL_LINE #Barter // This line is necessary as barter starts the instant you move to the line with the tag
-> END

// Example of unlocking a barter option
// Given an info card or item the NPC 
=== UnlockBarterOption ===
You've got some great news fa' me? And what could that possibly be? #NPC
    **Show her that Eden likes her.
        And how am I supposed to believe that Pri? You could be settin' me up to make me look like a fool. #NPC
        *** {IC_date_note}Show her Eden's date note.
            Give me that! Dear F-Fixey? ... This is... real? #NPC
            **** Got it straight from Eden herself.
                ->GaveDateNote
        *** I'll get back to you on that.
            Ya' better! #NPC
            ->END
* ->GaveDateNote

=== GaveDateNote ===
W-well that's great and all, but how are we even supposed to talk to each other? No way the church is gonna let me in. #NPC
    * That's a good point...
        Well let me know if you find something, and could ya' make it quick? P-please? #NPC
        ->END
    * {IC_grove_key} Show her the grove key.
        Let's barter, any and everythings on the table! #NPC.
        ~key_for_USB = true // Document somewhere what trade this unlocks
        ->Start
// Required Knots for NPCs
=== BarterWin === 
{shuffle:
- Don't let curiosity kill ya'! #NPC
- I guess cats do always land on their feet, eventually... #NPC
- You got some nice parts on ya', let me know if any are for sale next time. #NPC
}
-> END

=== BarterLose ===
{shuffle:
- I didn't scare you off did I? Hehehe. #NPC
- Were ya' distracted? I can't say I'd blame you. #NPC
- Window shoppers... Won't find what I've got anywhere else! #NPC
}
-> END





