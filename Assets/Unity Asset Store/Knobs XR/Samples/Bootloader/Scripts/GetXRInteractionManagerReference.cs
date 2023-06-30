using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace ByteSize.KnobsXR.Samples
{
    public class GetXRInteractionManagerReference : MonoBehaviour
    {
        private XRBaseInteractable interactable;

        private void Awake()
        {
            interactable = GetComponent<XRBaseInteractable>();

            interactable.interactionManager = FindInteractionManager();
        }

        private XRInteractionManager FindInteractionManager()
        {
            return FindObjectOfType<XRInteractionManager>();
        }
    }
}
