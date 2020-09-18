using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles input events.
public class InputPC : InputPlatform
{
    bool axisCheck = false;

    public override void Init(InputController input)
    {
        DefinePlatform();
        this.inputController = input;
        isInitialized = true;
    }

    //Used for multiplatform input.
    public override RuntimePlatform GetPlatform()
    {
        DefinePlatform();
        return base.GetPlatform();
    }

    void DefinePlatform()
    {
#if UNITY_EDITOR
        this.platform = RuntimePlatform.WindowsEditor;
#elif UNITY_STANDALONE
        this.platform = RuntimePlatform.WindowsPlayer;
#endif
    }

    //Handles the input.
    public override void UpdateInput()
    {
        if (!isInitialized)
            return;

        for (int i = 0; i < (int)InputButtonType.MouseLeft; i++)
        {
            if (Input.GetButtonDown(((InputButtonType)i).ToString()))
                inputController.ButtonEvent((InputButtonType)i, InputEventType.Pressed);

            if (Input.GetButtonUp(((InputButtonType)i).ToString()))
                inputController.ButtonEvent((InputButtonType)i, InputEventType.Released);

            if (Input.GetButton(((InputButtonType)i).ToString()))
                inputController.ButtonEvent((InputButtonType)i, InputEventType.Continuous);
        }

        for (int i = 0; i < (int)AxisType.RightAxis; i++)
        {
            if (Input.GetAxis("LeftAxisHorizontal") != 0 || Input.GetAxis("LeftAxisVertical") != 0)
            {
                inputController.AxisEvent((AxisType)i, new Vector2(Input.GetAxis("LeftAxisHorizontal"), Input.GetAxis("LeftAxisVertical")));
                axisCheck = true;
            }
        }

        //up
        if (Input.GetKey(KeyCode.Keypad8)) inputController.AxisEvent(AxisType.RightAxis, new Vector2(0, .1f));
        //down
        if (Input.GetKey(KeyCode.Keypad2)) inputController.AxisEvent(AxisType.RightAxis, new Vector2(0, -.1f));
        //left
        if (Input.GetKey(KeyCode.Keypad4)) inputController.AxisEvent(AxisType.RightAxis, new Vector2(-.1f, 0));
        //right
        if (Input.GetKey(KeyCode.Keypad6)) inputController.AxisEvent(AxisType.RightAxis, new Vector2(.1f, 0));

        if (axisCheck && Input.GetAxis("LeftAxisHorizontal") == 0 && Input.GetAxis("LeftAxisVertical") == 0)
        {
            axisCheck = false;
            inputController.AxisEvent(AxisType.LeftAxis, Vector2.zero);
            inputController.AxisEvent(AxisType.RightAxis, Vector2.zero);
        }

        if (Input.GetMouseButtonDown(0))
            inputController.ButtonEvent(InputButtonType.MouseLeft, InputEventType.Pressed);

        if (Input.GetMouseButtonUp(0))
            inputController.ButtonEvent(InputButtonType.MouseLeft, InputEventType.Released);

        if (Input.GetMouseButton(0))
            inputController.ButtonEvent(InputButtonType.MouseLeft, InputEventType.Continuous);

        inputController.mousePos = Input.mousePosition;
    }
}
