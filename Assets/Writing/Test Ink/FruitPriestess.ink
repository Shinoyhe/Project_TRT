Greetings, wanderer. How may we of the Order be of assistance to you? #NPC
* I would like to Barter. 
    -> Barter
* The Order?
    As in the Church. I pray for the safety and longevity of each individuals’ soul. #NPC
    -> Knot2
* Isn’t charity supposed to be indiscriminate?
    While this is true, we can’t offer resources to every sorry sap wandering into the commune. I do hope you understand, Feline Wanderer. #NPC
    -> Knot2

=== Barter ===
Very well, you must purge all envy. We, the Order, believe in charity once proven worthy. #NPC
NULL_LINE #Barter
-> END

=== Knot2 ===
* Soul?
    Yes, soul. We call it ‘fruit of the soul’, the organic matter that powers our cores. Every living bot has one. #NPC
    -> END
* Do you usually just wait here until someone asks for your help?
    -> WaitHere

=== WaitHere ===
Not usually, Dear Wanderer. I am often running errands for the people of the commune or listening to their problems someplace else. You have found me during a rare rest period. #NPC
Rare?
... #NPC
Do not worry. Someone must do all this work for the betterment of every soul on this Earth. #NPC
-> END

=== BarterWin ===
Please do not hesitate to reach out to the Order again. May the Harvest bless you. #NPC
-> END

=== BarterLose ===
I will pray for your bartering skills to improve. #NPC
-> END
