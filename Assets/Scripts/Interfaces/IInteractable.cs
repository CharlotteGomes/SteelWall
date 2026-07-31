using UnityEngine;

public interface IInteractable
{
    Transform Transform { get; set; }
    public abstract void Interact();
}
