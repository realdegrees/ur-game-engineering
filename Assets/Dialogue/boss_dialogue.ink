... Archie?! #speaker:King #portrait:king
Is that my little boy? You've grown so much! I thought you were... #speaker:King #portrait:king
... #speaker:King #portrait:king
... wait, who is this man? #speaker:King #portrait:king
... #speaker:Archie #portrait:companion
.. "This man" is my, uh... well, he saved my life back then and took me in, he said there was nobody left... that he found me all alone in these ruins... #speaker:Archie #portrait:companion
... I guess that was a lie... #speaker:Archie #portrait:companion
... #speaker:Player #portrait:player
Why are we here? #speaker:Archie #portrait:companion
... #speaker:Player #portrait:player
You were planning to kill everybody who's left in Heartingdale, weren't you? #speaker:King #portrait:king
... #speaker:Player #portrait:player
You lied to me, didn't you? You did not find me in the ruins, you killed my entire family! And now you also want to kill my father?! I cannot accept that! #speaker:Archie #portrait:companion
... #speaker:Player #portrait:player
... Yes, that is what I have to do. It is my duty to protect the Kingdom of Nightingdale. #speaker:Player #portrait:player
No! You cannot do this! If you want to kill my father, you'll have to kill me, too! #speaker:Archie #portrait:companion

* ATTACK
    I never thought it would come to this, Spud... but here we are. #speaker:Player #portrait:player
    You monster!! #speaker:Archie #portrait:companion
    -> END
* Convince Archie to fight with you
    Stop this! This man before you has been planning to attack my kingdom for the second time now, and both times it was my duty to save Nightingdale by attacking first! #speaker:Player #portrait:player
    ... #speaker:Archie #portrait:companion
    Please, Spud, this man is a warmonger and a monster. I need your help! If I don't bring my king his head, I will be branded a traitor and lose you! #speaker:Player #portrait:player
    You did not even call me by my real name! My name is Archie, not Spud! #speaker:Archie #portrait:companion
    How was I supposed to know that?! I'm sorry! But it doesn't matter. I am your father. It was me who raised you! Now please come back to me and help me fight this monster! #speaker:Player #portrait:player
    You don't know me and I am not your son! #speaker:Archie #portrait:companion
    Spud is charging at you! What do you do?
    -> archie_attack
* Figure out what's going on
    -> figure_out

== archie_attack ==
* RUN AWAY
With a heavy heart, you leave Spud behind and run for your life...
-> END
* FIGHT THEM
I never thought it would come to this, Spud... but here we are. #speaker:Player #portrait:player
-> END

== figure_out ==
Wait! Stop! #speaker:Player #portrait:player
I don't want to fight you! I did not want to be involved in another war ever again. But, the king...
#speaker:Player #portrait:player
He threatened to harm you, Sp-... Archie, if I didn't comply... I don't know what to do anymore... #speaker:Player #portrait:player
Don't believe your king's lies! He despises my kingdom and has always been envious of my nation's prosperity! #speaker:King #portrait:king
He is the one who initiated these conflicts. He spread lies and used them to justify attacking us! #speaker:King #portrait:king
Tell me, what did your king tell you was the reason for attacking us? #speaker:King #portrait:king
He told us that you were plotting a war against us. That we had to strike pre-emptively. #speaker:Player #portrait:player
That was a lie. We were nothing but friendly toward your kingdom. And this is how you pay us back? #speaker:King #portrait:king
... #speaker:Player #portrait:player
Your attack caught us off guard. During the sudden slaughter, I was separated from Archie. I feared him lost. #speaker:King #portrait:king
But I had no choice but to retreat. You have my gratitude for looking after my son. But I'll have to ask you to stand down. Please. #speaker:King #portrait:king
-> explanations

== explanations ==
* Show empathy
I understand. I, no, my entire people have been deceived by a warmonger. I have so much innocent blood on my hands… #speaker:Player #portrait:player
You do. But, you also showed mercy when you did not have to. You have my thanks. Without you, I would have also lost my son… #speaker:King #portrait:king
I will go into hiding, and I will take my son with me. I hope you understand. You can accompany us, if you want. #speaker:King #portrait:king
-> END
* Only care about your duty
NO! I don’t care. I cannot bear to lose my son like this, but betraying my king is even worse! #speaker:Player #portrait:player
 But you do not have to lose Archie. You could stay here with us. It does not have to end this way. #speaker:King #portrait:king
 Please, don’t do this… #speaker:Archie #portrait:companion
-> archie_attack