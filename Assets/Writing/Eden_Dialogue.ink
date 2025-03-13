// Global Variables. Please put these at the top.
// Flag variable. Can be any name you want. Default false
VAR solar_for_grove = false
VAR date_note = false
// Inventory Card Variable (Info Card or Item Card). Default false
// Name NEEDS to have "IC_" before the name. It'd be best if you keep this consistent with the item/info card name.
// Programmers will make sure this name matches the card in game. 
VAR IC_info_card_2 = false
VAR IC_info_card_4 = false
VAR IC_solar_panel = false
VAR IC_date_note = false

->Intro

=== Intro ===
Greetings, wanderer. How may we of the Order be of assistance to you? #NPC
->Start

=== Start ===
* I would like to Barter. 
    -> Barter
*The Order?
    As in the Church. I pray for the safety and longevity of each individuals’ soul. #NPC
    ->The_Order
* {(IC_info_card_2 and not solar_for_grove) or (IC_info_card_4 and not date_note)} I've got something else to talk about
    ** {IC_info_card_2 and not solar_for_grove} I've heard some rumors.
        -> UnlockBarterOption
    ** {IC_info_card_4 and not date_note} I have news you should hear.
        Is it about Fixey I-I MEAN... I mean F1X? #NPC
        ***It seems like they might feel the same.
            Th-they do!? Oh wanderer please tell me it is so. #NPC
            I do not wish to be a bother, but do you think you can run another errand for me? #NPC
            ~date_note = true
            ~IC_date_note = true
            ****I'm not one to get in the way of love, sure.
                Blessed wanderer, I thank you. Please give this note to F1X so we may meet. I cannot give it to her myself otherwise the church would lambast me. #NPC
                ***** I'd be happy to.
                    I'll be praying for your safe travel. #NPC
                    ->END
* Nevermind // Exit dialogue
    -> END

=== The_Order ===
*Isn’t charity supposed to be indiscriminate?
    While this is true, we can’t offer resources to every sorry sap wandering into the commune. I do hope you understand, Feline Wanderer. #NPC
    ->The_Order
*Soul?
    Yes, soul. We call it ‘fruit of the soul’, the organic matter that powers our cores. Every living bot has one. #NPC
    ->The_Order
*Do you usually just wait here?
    Not usually, Dear Wanderer. I am often running errands for the people of the commune or listening to their problems someplace else. You have found me during a rare rest period. 
    **Rare?
        ...#NPC
        Do not worry. Someone must do all this work for the betterment of every soul on this Earth. #NPC
        ->The_Order
*I want to talk about something else.
    ->Start

// Recommended knot just to keep track of where bartering starts
// Only use #Barter in NPC scripts. Required to have tag in NPC scripts.
=== Barter ===
Purge all envy. We, the Order, believe in charity once proven worthy. #NPC
NULL_LINE #Barter // This line is necessary as barter starts the instant you move to the line with the tag
-> END

// Example of unlocking a barter option
// Given an info card or item the NPC 
=== UnlockBarterOption ===
Rumors are often unsubstantiated, please do not waste either of our time on such things. #NPC.
    *Do you like F1X?
        I-She... There may be some substantiation to this rumor... #NPC
        I simply think that she is misunderstood, and I'd like to get to know her better. #NPC
            ** {IC_solar_panel} I've got something of hers.
                Are you interested in perhaps bartering for it? #NPC
                ~solar_for_grove = true
                ->Start
            **I'll see what I can do.
                Blessings to you. #NPC
                ->END

// Required Knots for NPCs
=== BarterWin === 
{shuffle:
- May fresh fruit be in your tidings. #NPC
- Please do not hesitate to reach out to the Order again. May the Harvest bless you. #NPC
- Patience is a virtue. Enjoy the spoils of this trade. #NPC
}
-> END

=== BarterLose ===
I will pray for your bartering skills to improve. #NPC
-> END
