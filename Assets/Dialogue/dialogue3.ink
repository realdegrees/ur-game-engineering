-> main

=== main ===

Your companion is dead.
Are you proud of yourself?
    + [Yes!]
        -> chosen("You monster.")
    + [Of course not!]
        -> chosen("Good! You should be ashamed of yourself!")
=== chosen(choice) ===
{choice}
-> END