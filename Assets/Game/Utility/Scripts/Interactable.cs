using UnityEngine;

public class Interactable : Monobehaviour
{
    public event EventHandler InteractedWith;

    public string promptText;

    private void Start()
    {
        InteractedWith += Trigger;
    }

    public void Trigger(object sender, EventArgs e)
    {

    }
}