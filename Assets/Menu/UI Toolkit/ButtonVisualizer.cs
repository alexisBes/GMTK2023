using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonVisualizer : MonoBehaviour
{
    public VisualElement root;
    
    private void Start()
    {
        // Get a reference to your button element
        Button button = root.Q<Button>("Watter");
        Debug.Log("button - " + button);
        // Create a tooltip text element
        Label tooltipText = new Label();
        tooltipText.style.display = DisplayStyle.None;
        tooltipText.style.backgroundColor = new StyleColor(Color.black);
        tooltipText.style.color = new StyleColor(Color.white);
        tooltipText.style.paddingLeft = 5;
        tooltipText.style.paddingRight = 5;

        // Add the tooltip text element as a child to the button
        button.Add(tooltipText);

        // Subscribe to the mouse enter and exit events of the button
        button.RegisterCallback<MouseEnterEvent>((evt) =>
        {
            tooltipText.style.display = DisplayStyle.Flex;
            tooltipText.text = "Your tooltip text";
            tooltipText.style.left = evt.localMousePosition.x + 10;
            tooltipText.style.top = evt.localMousePosition.y + 10;
        });

        button.RegisterCallback<MouseLeaveEvent>((evt) =>
        {
            tooltipText.style.display = DisplayStyle.None;
        });
    }
}
