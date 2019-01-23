/scene London.HorseDrawnCarriage
TODO: Add ambient street traffic here.
/character Passepartout
/character Fogg
- I looked at Monsieur Fogg 
*   [ask:journey] ... and I could contain myself no longer.
    'What is the purpose of our journey, Monsieur?'
    /perform scratch_chin
    'A wager,' he replied.
    * *     [yes:wager] 'A wager!' I returned.
            He nodded. 
            /perform nod
            * * *  [foolish] 'But surely that is foolishness!'
            * * *  [serious] 'A most serious matter then!'
            - - -   He nodded again.
                    /perform nod
            * * *   [ask:win] 'But can we win?'
                    /perform narrowed_eyes
                    'That is what we will endeavour to find out,' he answered.
            * * *   [ask:wager] 'A modest wager, I trust?'
                    'Twenty thousand pounds,' he replied, quite flatly.
                    /perform stoic
            * * *   [deflect] I asked nothing further of him then., and after a final, polite cough, he offered nothing more to me. <>
    * *     [deflect] 'Ah,' I replied, uncertain what I thought.
    - -     After that, <>
*   [deflect] ... but I said nothing and <>
- we passed the day in silence.
/scene London.Airship.EstablishingShot
/wait 5
/action fade_to_black
TODO: Moar Fogg!
- -> END