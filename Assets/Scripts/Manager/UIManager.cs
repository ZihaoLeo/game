using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager :Singleton<UIManager>
{
    public Dictionary<string, UIBase> uiPanelDic = new Dictionary<string, UIBase>();
    private UIBase currentDefaultPanel; // The Default panel currently displayed
    public T OpenUIPanel<T>(Transform parent, string prefabName, params object[] parameters) where T : UIBase
    {
        T components = null;
        // Find what already exists UIPanel
        if (uiPanelDic.TryGetValue(prefabName, out UIBase existingPanel))
        {
            // If found, return to the existing panel
            components = (T)existingPanel;
        }
        else
        {
            GameObject panel = GameUtils.CreateObj(parent, prefabName);
            // Gets all UIBase components on UIPanel
            components = panel.GetComponent<T>();
            if (components != null)
            {
                uiPanelDic[prefabName] = components;
            }
        }

        // If no component is found, null is returned
        if (components != null)
        {
            if (components.uIPanelType == UIPanelType.Default)
            {
                // If you currently have a Default panel, hide it first
                if (currentDefaultPanel != null && currentDefaultPanel != components)
                {
                    currentDefaultPanel.Hide();
                }
                // Update the current Default panel reference
                currentDefaultPanel = components;
            }

            components.Show(parameters);
            return components;
        }
        // Return null if no matching component is found
        Debug.LogError($"No UIBase component named {prefabName} was found on UIPanel.");
        return null;
    }

    public void CloseUIPanel(string prefabName)
    {
        // Try to get the panel from the dictionary
        if (uiPanelDic.TryGetValue(prefabName, out UIBase panelToClose))
        {
            // Hidden panel
            panelToClose.Hide();

            // If the closed panel is the currentDefault panel, update currentDefaultPanel
            if (panelToClose == currentDefaultPanel)
            {
                currentDefaultPanel = null;
            }
        }
        else
        {
            // If the panel is not found, output a warning log
            Debug.LogWarning($"The name is not found {prefabName} The UI panel cannot be closed¡£");
        }
    }


    public T GetUIPanelByName<T>(string prefabName) where T : UIBase
    {
        if (uiPanelDic.TryGetValue(prefabName, out UIBase existingPanel))
        {
            return (T)existingPanel;
        }

        Debug.LogWarning($"The name is not found {prefabName} UIPanel¡£");
        return null;
    }
}
