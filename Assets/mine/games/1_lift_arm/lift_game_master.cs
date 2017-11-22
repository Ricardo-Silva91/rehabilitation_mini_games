using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;


public class lift_game_master : MonoBehaviour
{
    
    [System.Serializable]
    public class LanguagePack
    {
        public string[] mainText;
        public string[] timeText;
        public string[] logText;
    }

    public LanguagePack[] textBlocks;

    public Controller controller;
    public GameObject leapHand_L;
    public GameObject leapHand_R;
    public lift_game_dumbbell dumbbell;
    public text_master bestText;
    public text_master logText;
    public text_master timeText;
    public ParticleSystem fireWorks;

    public general_arrow_direction directionArrow;

    public general_time_keeper time_keeper;
    public lift_game_rest_client rest_client;
    public AudioSource gameWonSound;
    public AudioSource gameOverSound;


    //to set
    public bool workFlag = false;
    public bool leftHand = true;
    public int waitBetweenIterations = 0;
    public int langPackIndex = 0;


    //to check
    public bool waiting = false;
    public int secsToWait;
    public int secsWaited;
    public float startWaitingTime;
    public int gameState = 0;
    public int CountedHands = 0;
    public bool serverIsAlive = false;
    public bool sentResults = false;

    void Start()
    {

        fireWorks.Pause();
        //directionArrow.flipUp();

        //bestText.setCurrentText("Welcome to Lift!");
        bestText.setCurrentText(textBlocks[langPackIndex].mainText[0]);

        time_keeper.completeReset();

        gameState = 1;
        waitFor(3);

        controller = new Controller(); //An instance must exist
        byte[] frameData = System.IO.File.ReadAllBytes("frame.data");
        
        Frame reconstructedFrame = new Frame();
        reconstructedFrame.Deserialize(frameData);


    }

    public void SetRestParameters(JSONObject liftGameParameters)
    {
        langPackIndex = (int) liftGameParameters.GetField("language").n;
        leftHand = liftGameParameters.GetField("left_hand").b;
        waitBetweenIterations = (int) liftGameParameters.GetField("time_between_interactions").n;
        dumbbell.maxDistance = liftGameParameters.GetField("distance").n;
        dumbbell.totalIterations = (int) liftGameParameters.GetField("total_interactions").n;
        dumbbell.timeToHold = (int) liftGameParameters.GetField("time_to_hold").n;
        time_keeper.total_time = (int)liftGameParameters.GetField("total_time").n;

        time_keeper.workFlag = true;
        serverIsAlive = true;

    }

    void waitFor(int seconds)
    {
        secsToWait = seconds;
        waiting = true;
        startWaitingTime = Time.time;
    }

    bool HandsAreCorrect()
    {
        bool result = true;

        Frame frame = controller.Frame();
        CountedHands = frame.Hands.Count;
        if (frame.Hands.Count == 1)
        {
            List<Hand> hands = frame.Hands;
            Hand firstHand = hands[0];

            if ((leftHand == false && firstHand.IsLeft) || (leftHand == true && firstHand.IsLeft == false))
            {
                //logText.setCurrentText("ERROR: wrong hand detected");
                logText.setCurrentText(textBlocks[langPackIndex].logText[0]);

                result = false;
            }
        }
        else
        {
            result = false;
            //logText.setCurrentText("ERROR: wrong number of hands detected");
            logText.setCurrentText(textBlocks[langPackIndex].logText[1]);
        }

        if (result == true)
        {
            logText.setCurrentText("");
        }
        return result;
    }


    // Use this for initialization


    // Update is called once per frame
    void Update()
    {
        if (workFlag == true)
        {
            if (gameState != 99)
            {

                if (waiting)
                {
                    float currentTime = Time.time;
                    secsWaited = (int)(currentTime - startWaitingTime);
                    if (secsWaited >= secsToWait)
                    {
                        waiting = false;
                    }
                }
                else
                {
                    if(time_keeper.timeOverTotal == true)
                    {
                        gameState = 99;
                    }
                    else if (gameState == 1)
                    {
                        //bestText.setCurrentText("lift the dumbbell.");
                        bestText.setCurrentText(textBlocks[langPackIndex].mainText[1]);

                        directionArrow.flipUp();

                        gameState = 2;
                        dumbbell.worFlag = true;

                        time_keeper.reset();
                        //waitFor(4);
                    }
                    else if (gameState == 3)
                    {
                        //bestText.setCurrentText("let the dumbbell go down.");
                        bestText.setCurrentText(textBlocks[langPackIndex].mainText[2]);


                        dumbbell.worFlag = true;
                        if (dumbbell.backTo_startPos == false)
                        {
                            gameState = 1;
                        }
                    }
                    else
                    {
                        if (HandsAreCorrect())
                        {

                            if (gameState == 2)
                            {
                                //timeText.setCurrentText("time to hold: " + dumbbell.secsWaited + "/" + dumbbell.timeToHold);
                                timeText.setCurrentText(textBlocks[langPackIndex].timeText[0] + dumbbell.secsWaited + "/" + dumbbell.timeToHold + "\n" + textBlocks[langPackIndex].timeText[1] + (time_keeper.total_time - time_keeper.secsPassed));
                                
                                if (dumbbell.backTo_startPos == true)
                                {

                                    directionArrow.flipDown();

                                    //bestText.setCurrentText("Well done!\nIterations: " + dumbbell.iterations + "/" + dumbbell.totalIterations);
                                    bestText.setCurrentText(textBlocks[langPackIndex].mainText[3] + dumbbell.iterations + "/" + dumbbell.totalIterations);

                                    if (dumbbell.exerciseDone == true)
                                    {
                                        gameState = 99;
                                    }
                                    else
                                    {
                                        gameState = 3;
                                    }

                                    time_keeper.stop();
                                    waitFor(waitBetweenIterations);
                                }
                            }

                        }
                        else
                        {
                            if (gameState != 1)
                                dumbbell.resetState();
                        }

                    }
                }



            }
            else
            {
                if(sentResults==false)
                {
                    JSONObject gameResult = new JSONObject();
                    gameResult.SetField("total_in_game_time", time_keeper.completeSecsPassed());
                    gameResult.SetField("max_height_achieved", dumbbell.variantMaxDistance);
                    dumbbell.worFlag = false;
                    directionArrow.flipDown();

                    if (dumbbell.exerciseDone == true)
                    {
                        fireWorks.Play();
                        gameWonSound.PlayOneShot(gameWonSound.clip);
                        //bestText.setCurrentText("Well Done!\nThe Exercise is complete!");
                        bestText.setCurrentText(textBlocks[langPackIndex].mainText[4]);
                        gameResult.SetField("success", true);
                    }
                    else
                    {
                        //gameOverSound.PlayOneShot(gameOverSound.clip);
                        bestText.setCurrentText(textBlocks[langPackIndex].mainText[5]);
                        gameResult.SetField("success", false);
                    }

                    //send results to server
                    if (serverIsAlive == true)
                    {
                        rest_client.sendToServer(gameResult);
                        sentResults = true;
                        waitFor(2);
                    }
                }
            }
        }

    }
}
