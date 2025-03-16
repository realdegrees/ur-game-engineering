using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioTrigger : EditorZone<DialogueTrigger>
{
    [Header("Volume Settings")]
    [SerializeField]
    [Range(0f, 1f)] float volume;
}