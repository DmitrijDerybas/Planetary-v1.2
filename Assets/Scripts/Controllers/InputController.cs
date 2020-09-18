using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputEventType
{
    Pressed,
    Released,
    Continuous
}

public enum InputButtonType
{
    Action,
    Cancel,

    Left,
    Right,
    Up,
    Down,

    MouseLeft,

    Length
}

public enum InputDevice
{
    Keyboard,
    Mouse
}

public enum AxisType
{
    LeftAxis,
    RightAxis
}

public abstract class InputPlatform : MonoBehaviour
{
    protected RuntimePlatform platform;
    protected bool isInitialized = false;
    protected InputController inputController;
    public abstract void Init(InputController input);
    public abstract void UpdateInput();

    public virtual RuntimePlatform GetPlatform() { return this.platform; }
}

//Handles the current input platform. Fires input events.
public class InputController : Controller
{
    public delegate void OnInputDelegate(InputButtonType button, InputEventType inputEvent, int joyID = 0);
    public event OnInputDelegate OnInputEvent;

    public delegate void OnAxisDelegate(AxisType axis, Vector2 val, int joyID);
    public event OnAxisDelegate OnAxisEvent;

    public delegate void OnAxisRawDelegate(AxisType axis, Vector2 val, int joyID);
    public event OnAxisRawDelegate OnAxisRawEvent;

    public Vector3 mousePos;
    public Vector2 axis;
    public bool block = false;
    public InputPlatform currentInput;

    public InputDevice device;

    public Vector3 GetMouseWorldPos()
    {
        mousePos.z = controller.gui.camera_main.transform.position.z;
        return controller.gui.camera_main.ScreenToWorldPoint(mousePos);
    }

    public override void Init(GlobalController controller)
    {
        base.Init(controller);

        InputPlatform[] platforms = GetComponentsInChildren<InputPlatform>();

        for (int i = 0; i < platforms.Length; i++)
        {
            if (Application.platform == platforms[i].GetPlatform())
            {
                currentInput = platforms[i];
                RemoveOtherPlatforms(platforms, Application.platform);
                platforms = null;
                break;
            }
        }

        currentInput.Init(this);
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized || block)
            return;

        currentInput.UpdateInput();
    }

    //Removes unnecessary platforms.
    void RemoveOtherPlatforms(InputPlatform[] platforms, RuntimePlatform p)
    {
        for (int i = 0; i < platforms.Length; i++)
        {
            if (p != platforms[i].GetPlatform())
            {
                Destroy(platforms[i]);
            }
        }
    }

    //Buttons event (Keyboard and mouse).
    public void ButtonEvent(InputButtonType button, InputEventType inputEvent, int joyID = 0) { device = button == InputButtonType.MouseLeft ? InputDevice.Mouse : InputDevice.Keyboard; OnInputEvent?.Invoke(button, inputEvent, joyID); }
    //Axis events. For testing and multiplatform support.
    public void AxisEvent(AxisType axis, Vector2 val, int joyID = 0) {  device = InputDevice.Keyboard; OnAxisEvent?.Invoke(axis, val, joyID);  }
    public void AxisRawEvent(AxisType axis, Vector2 val, int joyID = 0) { device = InputDevice.Keyboard; OnAxisRawEvent?.Invoke(axis, val, joyID);  }
}
