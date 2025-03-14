// NOTE: The Shady Merchant is the one who's gonna say this. Rewrite the upbeat tone completely plz.
VAR bartered_once = false

-> Start

=== Start ===
Hey there! #NPC
Never seen this kinda tech around here. You must be new huh? The name's F1X. #NPC
* My name is Pri-v8.
    Well, nice to meet ya' Pri. Alright if I call ya' Pri? Bahhh you love it. What's a fine hunk o' metal like yourself doing around 'ere? #NPC
    ** Business.
        Business ey'? Then I presume you know how to barter? #NPC
        *** Couldn't hurt to learn.
            Great! #NPC
            -> Tutorial
        *** Of course I do. 
            -> NoRefusal
    ** Here to do some research on a rumor.
        Oh? Something around these parts catch your pointy lil' ears? To get anything around here you're gonna have to know how to barter. I can give you a few pointers if ya' want. #NPC
        *** Couldn't hurt to learn.
            Great! #NPC
            -> Tutorial
        *** Of course I know how to barter.
            ->NoRefusal

=== NoRefusal ===
C'mon, I'm trying to do you a favor here! #NPC
I'm offering you the opportunity of a lifetime, ya' get to learn from the best! #NPC
+ Okay...
    That's the spirit! #NPC
    {bartered_once:
        Need how bartering works again? #NPC
        ++ Nah, I'm good. 
            Now you're learnin'! #NPC
            -> Barter
        ++ Think you could give me a refresher?
            I can talk about barterin' all day. #NPC
            -> Tutorial
    - else:
        -> Tutorial
    }
+ Nah, I'm good.
    -> NoRefusal

=== Tutorial ===
Getting someone to barter with you is as simple as making 'em willing enough. #NPC
Making someone willing just means ya' gotta know how to respond to their tone cards with ya' own! #NPC
If you tell 'em what they wanna hear, they'll be more willing. Say somethin' with the wrong tone and they'll be less likey to barter with ya'. #NPC
Some tone responses won't affect their willingness, but instead affects the next tone cards they play. #NPC
People 'round 'ere aren't gonna show you their hand as soon as you talk to 'em. It's up to you to find out what clicks with people. No two people are alike neitha'. #NPC
Seems like you've got somethin' to write in too. Speaking from experience, it's a good idea to write down any info ya' learn from a barter. 'Specially proper tone card responses. #NPC
Most importantly, don't waste no one's time. We've got people ta' barter with, and people get <i>real</i> unwilling with slow barterers. #NPC

Clear on all that newbie? #NPC
+ Crystal.
    -> Barter
+ Remind me again would you?
    Can talk 'bout it all day. #NPC
    -> Tutorial

=== Barter ===
'Tastic. Now, everyone's got their eye on somethin'. You've just gotta be the one with it, and know that they want it. #NPC
I'll be easy on ya this time and letchya know that I've been eyin' that key card you've got in that there satchel. #NPC
*This dusty thing? Doubt it's good for anything anyways.
    Sounds perfect... #NPC
    NULL_LINE #Barter
    -> END

=== BarterWin ===
Impressive, for a rookie. #NPC
Well a deal's a deal! 'Preciate the keycard, hope the solar panel serves ya' well. <i>Gotta see if this baby works you know where.</i>  #NPC
Thanks again kiddo, hope to make plenty of deals with ya' soon. #NPC
-> END // Dissapears in a puff of smoke 

=== BarterLose ===
~bartered_once = true
Aww, that's too bad. C'mon try again! You'll do better next time! #NPC
+ Sure! 
    Great! Do you need to hear how bartering works again? #NPC
    ++ Nah, I'm good 
        Okay #NPC
        -> Barter
    ++ I need a refresher
        No problem! #NPC
        -> Tutorial
+ Nah, I'm good. 
    -> NoRefusal