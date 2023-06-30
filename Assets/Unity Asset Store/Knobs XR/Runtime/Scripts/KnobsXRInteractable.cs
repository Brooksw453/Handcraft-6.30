using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace ByteSize.KnobsXR
{
    [AddComponentMenu("Byte Size/Knobs XR/Knobs XR Interactable")]
    public class KnobsXRInteractable : XRBaseInteractable
    { 
        public Vector3 ConstraintAxis { get; private set; }
        public float MinRotationLimit => this.knobLimits.Min;
        public float MaxRotationLimit => this.knobLimits.Max;

        [SerializeField]
        [HideInInspector]
        private KnobInitialDirections initialDirections;

        [SerializeField]
        [HideInInspector]
        private KnobLimits knobLimits = new KnobLimits();

        private Quaternion rotationBeforeGrab;
        private Quaternion interactorRotationOnGrab;

        protected void Start()
        {
            this.rotationBeforeGrab = this.transform.rotation;
            this.ConstraintAxis = this.transform.forward;
            this.initialDirections = new KnobInitialDirections(this.transform);
            this.knobLimits.SetInitialRotation(this.transform.rotation);
        }

        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            base.OnSelectEntering(args);

            this.interactorRotationOnGrab = args.interactor.attachTransform.rotation;
        }

        protected override void OnSelectExiting(SelectExitEventArgs args)
        {
            base.OnSelectExiting(args);

            this.rotationBeforeGrab = this.transform.rotation;
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                ProcessUpdate(updatePhase);
            }
        }

        private void ProcessUpdate(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            if (!this.isSelected) { return; }

            PerformInstantaneousUpdate();
        }

        private void PerformInstantaneousUpdate()
        {
            Quaternion interactorDelta = this.selectingInteractor.attachTransform.rotation * Quaternion.Inverse(this.interactorRotationOnGrab);

            float interactorRotationDeltaAngle;
            Vector3 interactorRotationDeltaAxis;
            interactorDelta.ToAngleAxis(out interactorRotationDeltaAngle, out interactorRotationDeltaAxis);

            float rotationAngleAlongConstraintAxis = Vector3.Dot(interactorRotationDeltaAxis, this.ConstraintAxis) * interactorRotationDeltaAngle;

            Quaternion rotationAttempt = Quaternion.AngleAxis(rotationAngleAlongConstraintAxis, this.ConstraintAxis) * this.rotationBeforeGrab;

            if (this.knobLimits.UseLimitsAndRotationNotWithinLimits(rotationAttempt, this.ConstraintAxis)) { return ;}

            this.transform.rotation = rotationAttempt;
        }
    }
}
