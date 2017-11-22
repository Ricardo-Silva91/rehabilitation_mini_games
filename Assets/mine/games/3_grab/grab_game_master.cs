using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

public class grab_game_master : MonoBehaviour {

    [System.Serializable]
    public class LanguagePack
    {
        public string[] mainText;
        public string[] timeText;
        public string[] logText;
    }

    public LanguagePack[] textBlocks;
    public int langPackIndex = 0;

    public general_grab_detector grabCalculator;

    public Controller controller;
    public point_position leftHandPalm;
    public point_position rightHandPalm;
    public ParticleSystem water;

    point_position handPalm;
    public text_master bestText;
    public text_master logText;
    public text_master timeText;

    public drink_game_cup sprayBottle;

    public general_time_keeper time_keeper;

    public ParticleSystem fireWorks;
    public ParticleSystem shinyDish;

    public point_position dishB;

    public grab_game_rest_client rest_client;
    public AudioSource gameWonSound;
    public AudioSource gameOverSound;

    //to set
    public bool workFlag = false;
    public bool leftHand = true;
    public int waitBetweenIterations = 0;
    public float margin = 0.1f;
    public int holdTime = 2;
    public int totalIterations = 3;
    public GameObject[] dishTypes;
    public GameObject currentDish;

    //to check
    public int iterations=0;
    public bool waiting = false;
    public int secsToWait;
    public int secsWaited;
    public float startWaitingTime;
    public int gameState = 0;
    public int CountedHands = 0;
    public bool serverIsAlive = false;
    public int handState = 0;
    public int wastedWater = 0;
    public bool sentResults = false;
    public float maxOpenAngle = 20;

    public bool timeStarting = true;

    public Vector3 dishPosition = new Vector3(0.006f, -0.539f, 0.254f);

    public Hand firstHand;

    // Use this for initialization
    void Start ()
    {
        fireWorks.Pause();
        shinyDish.Pause();

        if (leftHand)
        {
            handPalm = leftHandPalm;
        }
        else
        {
            handPalm = rightHandPalm;
        }

        //bestText.setCurrentText("Welcome to Dish Washer!");
        bestText.setCurrentText(textBlocks[langPackIndex].mainText[0]);

        dishPosition = dishB.transform.position;

        gameState = 1;
        waitFor(2);

        controller = new Controller(); //An instance must exist
        byte[] frameData = System.IO.File.ReadAllBytes("frame.data");
        Frame reconstructedFrame = new Frame();
        reconstructedFrame.Deserialize(frameData);
    }

    public void SetRestParameters(JSONObject liftGameParameters)
    {
        langPackIndex = (int)liftGameParameters.GetField("language").n;
        Debug.Log(liftGameParameters.GetField("left_hand"));
        leftHand =liftGameParameters.GetField("left_hand").b;
        Debug.Log(leftHand);
        
        if (leftHand)
        {
            handPalm = leftHandPalm;
        }
        else
        {
            handPalm = rightHandPalm;
        }

        grabCalculator.leftHand = leftHand;
        grabCalculator.ready = true;

        margin = liftGameParameters.GetField("grab_margin").n;

        grabCalculator.margin = margin;

        totalIterations = (int)liftGameParameters.GetField("total_interactions").n;
        holdTime = (int) liftGameParameters.GetField("time_to_hold").n;

        waitBetweenIterations = (int) liftGameParameters.GetField("time_between_interactions").n;

        time_keeper.total_time = (int)liftGameParameters.GetField("total_time").n;

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
            firstHand = hands[0];

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


    void  updateHandState()
    {
        if (firstHand.GrabAngle < maxOpenAngle)
        {
            maxOpenAngle = firstHand.GrabAngle;
        }
        //fully open
        if (firstHand.GrabAngle <= 0 + margin)
        {
            handState = 2;
            
        }
        // fully closed
        else if (firstHand.GrabAngle > 3.14 - margin)
        {
            handState = 1;
        }
        // in between
        else
        {
            handState = 0;
        }
    }

    bool gameOver()
    {
        bool result = false;

        if(iterations == totalIterations)
        {
            result = true;
        }

        return result;
    }

    int getRandomDish()
    {
        int result = 0;

        result = Random.Range(0, dishTypes.Length-1);

        return result;
    }

    // Update is called once per frame
    void Update () {
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
                    
                    if (gameState == 1)
                    {
                        //bestText.setCurrentText("Open Hand to turn fossett on.");
                        bestText.setCurrentText(textBlocks[langPackIndex].mainText[1]);

                        //timeText.setCurrentText("dishes washed: " + iterations + "/" + totalIterations + "\nUsed Water: " + wastedWater);
                        timeText.setCurrentText(textBlocks[langPackIndex].timeText[0] + iterations + "/" + totalIterations + "\n" + textBlocks[langPackIndex].timeText[1] + wastedWater + "\n" + textBlocks[langPackIndex].timeText[2]);

                        wastedWater = 0;
                        gameState = 2;
                        waitFor(2);
                    }

                    if (HandsAreCorrect())
                    {

                        updateHandState();
                        if (handState == 2)
                        {
                            water.Play();
                            wastedWater++;

                        }
                        else
                        {
                            water.Stop();
                        }

                        if (gameState == 2)
                        {

                            if(time_keeper.timeOverTotal == true)
                            {
                                gameState = 99;
                                return;
                            }
                            if(timeStarting == true)
                            {
                                timeStarting = false;
                                time_keeper.reset();
                            }
                            

                            //bestText.setCurrentText("Wash the dish.");
                            bestText.setCurrentText(textBlocks[langPackIndex].mainText[2]);

                            //timeText.setCurrentText("dishes washed: " + iterations + "/" + totalIterations + "\nUsed Water: " + wastedWater);
                            timeText.setCurrentText(textBlocks[langPackIndex].timeText[0] + iterations + "/" + totalIterations + "\n" + textBlocks[langPackIndex].timeText[1] + wastedWater + "\n" + textBlocks[langPackIndex].timeText[2] + (time_keeper.total_time - time_keeper.secsPassed));

                            //put dirty dish on sink
                            if (currentDish==null)
                                currentDish =(GameObject) GameObject.Instantiate(dishTypes[getRandomDish()], dishPosition, new Quaternion(0,0,0,0));
                            currentDish.GetComponent<Renderer>().material.color = new Color(0,0,0);

                            secsWaited = 0;

                            if(handState==2)
                            {
                                startWaitingTime = Time.time;
                                gameState = 3;
                            }
                        }
                        if(gameState==3)
                        {
                            if(handState==2)
                            {
                                float currentTime = Time.time;
                                secsWaited = (int)(currentTime - startWaitingTime);

                                //currentDish.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white);
                                timeText.setCurrentText(textBlocks[langPackIndex].timeText[0] + iterations + "/" + totalIterations + "\n" + textBlocks[langPackIndex].timeText[1] + wastedWater + "\n" + textBlocks[langPackIndex].timeText[2] + (time_keeper.total_time - time_keeper.secsPassed));

                                currentDish.GetComponent<Renderer>().material.color = new Color(((currentTime - startWaitingTime) / holdTime), ((currentTime - startWaitingTime) / holdTime), ((currentTime - startWaitingTime) / holdTime));

                                if (secsWaited>=holdTime)
                                {
                                    //bestText.setCurrentText("The dish is clean.\nDon't waste water");
                                    bestText.setCurrentText(textBlocks[langPackIndex].mainText[3]);

                                    shinyDish.Play();

                                    gameState = 4;
                                    time_keeper.stop();

                                }
                            }
                            else
                            {
                                //Destroy(currentDish);   
                                gameState = 2;
                            }
                        }
                        if(gameState==4)
                        {
                            if (handState != 2)
                            {

                                shinyDish.Pause();
                                shinyDish.Clear();

                                secsWaited = 0;
                                iterations++;
                                //timeText.setCurrentText("dishes washed: " + iterations + "/" + totalIterations + "\nUsed Water: " + wastedWater);
                                timeText.setCurrentText(textBlocks[langPackIndex].timeText[0] + iterations + "/" + totalIterations + "\n" + textBlocks[langPackIndex].timeText[1] + wastedWater + "\n" + textBlocks[langPackIndex].timeText[2] + (time_keeper.total_time - time_keeper.secsPassed));

                                if (gameOver() == false)
                                {
                                    //bestText.setCurrentText("More Dishes coming.");
                                    bestText.setCurrentText(textBlocks[langPackIndex].mainText[4]);
                                    gameState = 2;
                                    timeStarting = true;
                                }
                                else
                                {
                                    gameState = 99;
                                }
                                Destroy(currentDish);
                                waitFor(waitBetweenIterations);                                
                            }
                            else
                            {
                                timeText.setCurrentText(textBlocks[langPackIndex].timeText[0] + iterations + "/" + totalIterations + "\n" + textBlocks[langPackIndex].timeText[1] + wastedWater + "\n" + textBlocks[langPackIndex].timeText[2] + (time_keeper.total_time - time_keeper.secsPassed));
                            }
                        }
                    }
                    else
                    {
                        secsWaited = 0;
                        water.Stop();
                    }
                }
            }
            else
            {
                if (sentResults == false)
                {
                    JSONObject gameResult = new JSONObject();
                    gameResult.SetField("total_in_game_time", time_keeper.completeSecsPassed());
                    gameResult.SetField("max_open_angle", maxOpenAngle);

                    if (time_keeper.timeOverTotal == false)
                    {

                        fireWorks.Play();
                        gameWonSound.PlayOneShot(gameWonSound.clip);

                        //bestText.setCurrentText("Well Done!\nThe Exercise is complete!");
                        bestText.setCurrentText(textBlocks[langPackIndex].mainText[5]);

                        //timeText.setCurrentText("dishes washed: " + iterations + "/" + totalIterations + "\nUsed Water: " + wastedWater);
                        timeText.setCurrentText(textBlocks[langPackIndex].timeText[0] + iterations + "/" + totalIterations + "\n" + textBlocks[langPackIndex].timeText[1] + wastedWater + "\n" + textBlocks[langPackIndex].timeText[2] + (time_keeper.total_time - time_keeper.secsPassed));

                        gameResult.SetField("success", true);

                    }
                    else
                    {
                        //gameOverSound.PlayOneShot(gameOverSound.clip);
                        //bestText.setCurrentText("Well Done!\nThe Exercise is complete!");
                        bestText.setCurrentText(textBlocks[langPackIndex].mainText[6]);

                        //timeText.setCurrentText("dishes washed: " + iterations + "/" + totalIterations + "\nUsed Water: " + wastedWater);
                        timeText.setCurrentText(textBlocks[langPackIndex].timeText[0] + iterations + "/" + totalIterations + "\n" + textBlocks[langPackIndex].timeText[1] + wastedWater + "\n" + textBlocks[langPackIndex].timeText[2] + (time_keeper.total_time - time_keeper.secsPassed));

                        gameResult.SetField("success", false);
                    }

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
