using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuLogic : MonoBehaviour
{
    float MenuDown;
    bool GoDown;
    float MenuUp;
    bool GoUp;
    float Action;
    bool doingAction = false;

    public GameObject[] MenuLights;
    public void OnDown(InputAction.CallbackContext context)
    {
         MenuDown= context.ReadValue<float>();

    }
    public void OnUp(InputAction.CallbackContext context)
    {
        MenuUp = context.ReadValue<float>();

    }

    public void OnAction(InputAction.CallbackContext context)
    {
        Action= context.ReadValue<float>();
    }


    void PlayAction()
    {
        if(Action>0&&doingAction==false)
        {
            doingAction = true;
            int index = 0;
            foreach (GameObject Light in MenuLights)
            {
                if (Light.activeSelf)
                {
                    ResponseToAction(index);
                }
                index++;
            }
        }
        else if (Action < 1)
        {
            doingAction = false;
        }
    }

    void ResponseToAction(int index)
    {
        if(index==0)
        {
            SceneManager.LoadScene(1);
        }
        if(index==2)
        {
            Application.Quit();
        }
    }

    void MoveDown()
    {
        if(MenuDown>0&&GoDown==false)
        {
            GoDown = true;
            int index=0;
            foreach(GameObject Light in MenuLights)
            {
                if(Light.activeSelf)
                {
                    if(index!=2)
                    {
                        Light.SetActive(false);
                        MenuLights[index + 1].SetActive(true);
                        return;
                    }
                    
                }
                index++;
            }

            Debug.Log(MenuDown);
        }
        else if(MenuDown<1)
        {
            GoDown = false;
        }
    }
    void MoveUP()
    {
        Debug.Log("Up Value"+MenuUp);
        if (MenuUp > 0 && GoUp == false)
        {
            GoUp = true;
            int index = 0;
            foreach (GameObject Light in MenuLights)
            {
                if (Light.activeSelf)
                {
                    if (index != 0)
                    {
                        Light.SetActive(false);
                        MenuLights[index -1].SetActive(true);
                        return;
                    }

                }
                index++;
            }

          
        }
        else if (MenuUp < 1)
        {
            GoUp = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        MoveDown();
        MoveUP();
        PlayAction();
    }
}
