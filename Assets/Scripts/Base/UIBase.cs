using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIPanelType
{
    Default = 0, // The primary panel only exists one at a time
    Second = 1, // Multiple secondary panels can exist
    Tips = 2, // Prompt hierarchy panel
}

public abstract class UIBase : MonoBehaviour
{
    public UIPanelType uIPanelType = UIPanelType.Default;
    // Records the current activation status of the panel
    private bool isActive;

    // Interface for panel control
    public virtual void Show(params object[] parameters)
    {
        if (!isActive)
        {
            gameObject.SetActive(true);
            isActive = true;
            OnShow(parameters); // Append custom processing logic
        }
    }

    public virtual void Hide()
    {
        OnHide(); // Append custom processing logic
    }

    // Override the method to perform the initialization logic
    protected virtual void OnShow(params object[] parameters)
    {
        // You can override this method in a subclass for specific logical processing
    }

    // Write the method to execute the hidden logic
    protected virtual void OnHide()
    {
        // You can override this method in a subclass for specific logical processing
        if (isActive)
        {
            gameObject.SetActive(false);
            isActive = false;
        }
    }

    // Write the method to execute the hidden logic
    protected virtual void Update()
    {
        // You can override this method in a subclass for specific logical processing
    }

    protected virtual void LateUpdate()
    {
        
    }

    public bool IsActive()
    {
        return isActive;
    }
}
