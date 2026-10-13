using UnityEngine;
using UnityEngine.Formats.Alembic.Importer;
using UnityEngine.InputSystem; 

public class Tent_Kalle : MonoBehaviour
{
    [Header("Character 1 & Tent")]
    public Animator characterAnimator;
    public AlembicStreamPlayer tentAlembicPlayer;

    [Header("Character 2 (New)")]
    [Tooltip("Drag your second character's Animator component here")]
    public Animator secondCharacterAnimator;

    [Header("Settings")]
    public KeyCode triggerKey = KeyCode.E;
    
    [Tooltip("The Trigger parameter name inside BOTH character animators")]
    public string triggerParameterName = "OpenTent"; 

    [Header("Playback Options")]
    public float playbackSpeed = 1.0f; 

    private int triggerID;
    private bool isTentAnimating = false;

    void Start()
    {
        // Convert the string name into a fast numerical ID
        triggerID = Animator.StringToHash(triggerParameterName);

        if (tentAlembicPlayer != null)
        {
            tentAlembicPlayer.CurrentTime = 0f;
        }
    }

    void Update()
    {
        // Listen for the 'E' key press using the New Input System
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            PlayAllAnimations();
        }

        // Push the alembic tent frames forward
        if (isTentAnimating && tentAlembicPlayer != null)
        {
            tentAlembicPlayer.CurrentTime += Time.deltaTime * playbackSpeed;
        }
    }

    void PlayAllAnimations()
    {
        // 1. Trigger Kalle
        if (characterAnimator != null)
        {
            characterAnimator.SetTrigger(triggerID);
        }

        // 2. Trigger the second character at the exact same millisecond
        if (secondCharacterAnimator != null)
        {
            secondCharacterAnimator.SetTrigger(triggerID);
            Debug.Log("🟢 SUCCESS: Sent trigger to Second Character.");
        }

        // 3. Wake up the Alembic stream for the tent cloth
        if (tentAlembicPlayer != null)
        {
            tentAlembicPlayer.CurrentTime = 0f;
            isTentAnimating = true; 
        }
        
        Debug.Log("🚀 System Online: Both characters and the tent are animating!");
    }
}