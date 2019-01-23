/scene London.Streets_EstablishingShot
/action fade-from-black
+ [wait 5]
- /scene London.HorseDrawnCarriage_Interior
/character Fogg
/character Passepartoot

TODO: Add in ambient audio of London here.

- I looked at Monsieur Fogg 
*   [ask:journey] ... and I could contain myself no longer.
    'What is the purpose of our journey, Monsieur?'
    'A wager,' he replied.
    * *     [suggest:wager] 'A wager!' I returned.
            /perform nod
            He nodded. 
            * * *  [suggest:foolish] 'But surely that is foolishness!'
            * * *  [suggest:serious] 'A most serious matter then!'
            - - -   /perform nod
                    He nodded again.
            * * *   [ask:win] 'But can we win?'
                    /perform excited
                    'That is what we will endeavour to find out,' he answered.
            * * *   [ask:wager] 'A modest wager, I trust?'
                    /perform stoic
                    'Twenty thousand pounds,' he replied, quite flatly.
            * * *   [deflect] I asked nothing further of him then, and after a final, polite cough, he offered nothing more to me. <>
    * *     [deflect] 'Ah,' I replied, uncertain what I thought.
    - -     After that, <>
*   [deflect] ... but I said nothing and <> 
- we passed the day in silence.

/scene London.Airship.EstablishingShot

+ [wait 5]

- /action fade-to-black

TODO: Moar Fogg!