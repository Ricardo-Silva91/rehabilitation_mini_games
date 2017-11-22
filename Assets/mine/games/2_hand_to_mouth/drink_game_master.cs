using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;



public class drink_game_master : MonoBehaviour
{

    [System.Serializable]
    public class LanguagePack
    {
        public string[] mainText;
        public string[] timeText;
        public string[] logText;
    }

    [System.Serializable]
    public class fruit
    {
        public GameObject fruitObject;
        public int fruitQuantity;
    }


    public LanguagePack[] textBlocks;
    public int langPackIndex = 0;

    public Controller controller;
    public fruit[] fruits;

    public drink_game_cup GreenApple;
    public drink_game_cup redApple;

    public point_position leftHandPalm;
    public point_position leftHandPalm_point;

    public point_position rightHandPalm;
    public point_position rightHandPalm_point;

    point_position handPalm;
    point_position handPalm_point;
    public text_master bestText;
    public text_master logText;
    public text_master timeText;

    public general_time_keeper time_keeper;

    public AudioSource bite;
    public AudioSource gameWonSound;
    public AudioSource gameOverSound;

    public proximity_color_changer proximityBall;
    public ParticleSystem fireWorks;

    public drink_game_rest_client rest_client;

    //to set
    public bool workFlag = false;
    public bool leftHand = true;
    public int waitBetweenIterations = 0;
    public float appleMargin = 0.25f;

    //to check
    public bool waiting = false;
    public int secsToWait;
    public int secsWaited;
    public float startWaitingTime;
    public int gameState = 0;
    public int CountedHands = 0;
    public bool serverIsAlive = false;
    public Vector3 goalPosition;
    public bool hasFruit = false;
    public float distanceToGreenApple = 0f;
    public float distanceToRedApple = 0f;

    public bool timeStarting = true;

    public bool sentResults = false;

    public Hand firstHand;

    void Start()
    {

        time_keeper.completeReset();

        fireWorks.Pause();
        proximityBall.transform.localScale = new Vector3(appleMargin, appleMargin, appleMargin);

        if (leftHand)
        {
            handPalm = leftHandPalm;
            handPalm_point = leftHandPalm_point;
        }
        else
        {
            handPalm = rightHandPalm;
            handPalm_point = rightHandPalm_point;
        }

        //bestText.setCurrentText("Welcome to Apple!");
        bestText.setCurrentText(textBlocks[langPackIndex].mainText[0]);

        gameState = 2;
        waitFor(2);

        controller = new Controller(); //An instance must exist
        byte[] frameData = System.IO.File.ReadAllBytes("frame.data");
        Frame reconstructedFrame = new Frame();
        reconstructedFrame.Deserialize(frameData);

    }


    public void SetRestParameters(JSONObject liftGameParameters)
    {
        langPackIndex = (int)liftGameParameters.GetField("language").n;
        leftHand = liftGameParameters.GetField("left_hand").b;
        if(leftHand)
        {
            handPalm = leftHandPalm;
            handPalm_point = leftHandPalm_point;
        }
        else
        {
            handPalm = rightHandPalm;
            handPalm_point = rightHandPalm_point;
        }

        fruits[0].fruitQuantity = (int) liftGameParameters.GetField("green_apple_quantity").n;
        fruits[1].fruitQuantity = (int) liftGameParameters.GetField("red_apple_quantity").n;
        appleMargin = liftGameParameters.GetField("apple_margin").n;

        waitBetweenIterations = (int) liftGameParameters.GetField("time_between_interactions").n;

        time_keeper.total_time = (int)liftGameParameters.GetField("total_time").n;
        

        serverIsAlive = true;

        proximityBall.transform.localScale = new Vector3(appleMargin, appleMargin, appleMargin);


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

    bool gameOver()
    {
        bool result = true;

        for(int i=0; i<fruits.Length; i++)
        {
            if (fruits[i].fruitQuantity >0)
            {
                result = false;
                break;
            }
        }

        return result;
    }

    public int i = -1;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (i==0)
            {
                i = 1;
            }
        }

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
                    if (HandsAreCorrect())
                    {
                        if (gameState == 2)
                        {
                            if (i == 1)
                            {
                                i = -1;
                                //bestText.setCurrentText("Goal Position Set.");
                                bestText.setCurrentText(textBlocks[langPackIndex].mainText[1]);

                                goalPosition = handPalm.transform.position;

                                proximityBall.transform.position = goalPosition;

                                gameState = 3;
                                waitFor(2);
                            }
                            else
                            {
                                i = 0;
                                //bestText.setCurrentText("Set goal position.");
                                bestText.setCurrentText(textBlocks[langPackIndex].mainText[2]);


                            }
                            
                            
                        }
                        else if(gameState == 3)
                        {

                            //timeText.setCurrentText("Apples left\nGreen: " + fruits[0].fruitQuantity + "\nRed: " + fruits[1].fruitQuantity);
                            timeText.setCurrentText(textBlocks[langPackIndex].timeText[0] + fruits[0].fruitQuantity + textBlocks[langPackIndex].timeText[1] + fruits[1].fruitQuantity + "\n" + textBlocks[langPackIndex].timeText[2] + (time_keeper.total_time - time_keeper.secsPassed));
                            if (gameOver() == false && time_keeper.timeOverTotal == false)
                            {
                                if(timeStarting == true)
                                {
                                    time_keeper.reset();
                                    timeStarting = false;
                                }

                                //bestText.setCurrentText("reach apple to catch it");
                                bestText.setCurrentText(textBlocks[langPackIndex].mainText[3]);

                                proximityBall.setOpacity(0);
                                proximityBall.updateColor(handPalm.transform.position);

                                distanceToGreenApple = Vector3.Distance(handPalm.transform.position, GreenApple.transform.position);
                                distanceToRedApple = Vector3.Distance(handPalm.transform.position, redApple.transform.position);

                                if (distanceToGreenApple < appleMargin)
                                {
                                    gameState = 4;
                                    proximityBall.setOpacity(0.5f);
                                    proximityBall.updateColor(handPalm.transform.position);
                                }
                                else if (distanceToRedApple < appleMargin)
                                {
                                    gameState = 5;
                                    proximityBall.setOpacity(0.5f);
                                    proximityBall.updateColor(handPalm.transform.position);
                                }
                            }
                            else
                            {
                                gameState = 99;
                            }
                        }
                        else if(gameState==4)
                        {
                            //bestText.setCurrentText("Eat The Apple.");
                            bestText.setCurrentText(textBlocks[langPackIndex].mainText[4]);

                            proximityBall.updateColor(handPalm.transform.position);

                            //GreenApple.moveToMe(handPalm.transform.position, handPalm.transform.rotation);
                            GreenApple.moveToMe(handPalm_point.transform.position, handPalm_point.transform.rotation);

                            if (Vector3.Distance(handPalm.transform.position, goalPosition)<appleMargin)
                            {
                                bite.PlayOneShot(bite.clip);
                                fruits[0].fruitQuantity--;
                                gameState = 3;
                                timeStarting = true;
                                if (fruits[0].fruitQuantity!=0)
                                    GreenApple.putBack();
                                else
                                {
                                    GreenApple.ThrowAway();
                                }
                            }
                        }

                        else if (gameState == 5)
                        {
                            //bestText.setCurrentText("Eat The Apple.");
                            bestText.setCurrentText(textBlocks[langPackIndex].mainText[4]);

                            proximityBall.updateColor(handPalm.transform.position);

                            //redApple.moveToMe(handPalm.transform.position, handPalm.transform.rotation);
                            redApple.moveToMe(handPalm_point.transform.position, handPalm_point.transform.rotation);
                            if (Vector3.Distance(handPalm.transform.position, goalPosition) < appleMargin)
                            {
                                bite.PlayOneShot(bite.clip);
                                fruits[1].fruitQuantity--;
                                gameState = 3;
                                timeStarting = true;
                                if (fruits[1].fruitQuantity != 0)
                                    redApple.putBack();
                                else
                                {
                                    redApple.ThrowAway();
                                }
                            }
                        }
                    }
                }

            }
            else
            {
                if (sentResults == false)
                {
                    JSONObject gameResult = new JSONObject();
                    gameResult.SetField("total_in_game_time", time_keeper.completeSecsPassed());

                    time_keeper.stop();

                    if (time_keeper.timeOverTotal == false)
                    {
                        fireWorks.Play();
                        gameWonSound.PlayOneShot(gameWonSound.clip);

                        //bestText.setCurrentText("Well Done!\nThe Exercise is complete!");
                        bestText.setCurrentText(textBlocks[langPackIndex].mainText[5]);
                        gameResult.SetField("success", true);
                    }
                    else
                    {
                        //gameOverSound.PlayOneShot(gameOverSound.clip);
                        bestText.setCurrentText(textBlocks[langPackIndex].mainText[6]);
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
