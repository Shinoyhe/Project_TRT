using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static DialogueManager DialogueManager { get { return Instance.dialogueManager; } }
    public static PlayerInputHandler PlayerInput { get { return Instance.playerInput; } }
    public static UiInputHandler UiInput { get { return Instance.uiInput; } }
    public static Inventory Inventory { get { return Instance.inventory; } }
    public static Player Player { get { return Instance.player; } }


    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private PlayerInputHandler playerInput;
    [SerializeField] private UiInputHandler uiInput;
    [SerializeField] private Inventory inventory;
    [SerializeField] public Player player;
}
