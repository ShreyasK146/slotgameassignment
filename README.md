Game Overview:
Slots/Jackpot Game : Initial money is set to 50 . Bet carefully and earn more money

HOW TO RUN?
Prerequisites:
Make sure you have python installed - > https://www.python.org/downloads/
Have any web browser to run the WebGL Build

1) Open the command prompt and
2) Navigate to the index.html file under webbuild folder (Alternatively you can first go to this folder and then type cmd at the file explorer navigation bar and enter that will open up the cmd at this folder)
3) Once you are in the index.html directory on cmd type the following -> python -m http.server 8000
   and hit enter
   <img width="741" height="55" alt="image" src="https://github.com/user-attachments/assets/f2d393fe-b328-4f08-8d34-637bee5fe1e9" />
4) Go to your web browser and type http://127.0.0.1:8000 to play the game


BONUS FEATURES ADDED INCLUDE : 
1) 1 extra symbol (golden bars)
2) Sound files are added such as main background music and the clip that plays when the slot reels are scrolling for better immersion
3) High score feature is added
4) Option to continue playing even after you get all 3 same symbol


MY THOUGHT PROCESS/APPROACH :
3 Scripts are created 
1)GameManager which manages the gameplay (starting from the betting to earning/score calculation)
2)UIManager which is responsible to showcase all the UI elements and its changes during the gameplay
3)EventManager which is a singelton class used to decouple GameManager and UIManager

GameManager will signal whenever the new betting happens which starts the process to pre determine the random symbol that will be shown in all 3 slots.

Then the UI is responsible for updating the scores and showcase the scrolling animations (which is basically moving the anchor position of image to showcase the movement).

Elements/Symbols in the roll are constantly shifted to last position as the roll is out of sight which in turn helps us to showcase the looped reel.

Random Number Generator is responsible for picking up random symbols from the symbol list(Too get try RNG i have used security.cryptography namespace).

