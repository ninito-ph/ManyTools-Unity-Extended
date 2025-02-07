﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ninito.UsualSuspects.Volumes;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Ninito.UsualSuspects.Interactable
{
    /// <summary>
    ///     A simple class that provides basic interaction functionality
    /// </summary>
    public class SimpleInteractor : MonoBehaviour, IInteractor
    {
        #region Private Fields

        [SerializeField]
        [Header("Interaction Volume")]
        private ContainerVolume interactionVolume;

        [FormerlySerializedAs("interactionTooltipComponent")]
        [SerializeField]
        private TMP_Text interactionTooltipText;

        [SerializeField]
        private float periodicTooltipUpdateInterval = 1.5f;

        private Coroutine _periodicTooltipUpdateRoutine;

        #endregion

        #region Unity Callbacks

        private void Start()
        {
            interactionVolume.ColliderEnteredVolume += UpdateInteractionTooltip;
            interactionVolume.ColliderLeftVolume += UpdateInteractionTooltip;

            _periodicTooltipUpdateRoutine = StartCoroutine(PeriodicTooltipUpdate());
        }

        #endregion

        #region Public Methods

        public void OnInteract()
        {
            foreach (IInteractable interactable in GetInteractablesInVolume())
            {
                interactable?.InteractWithAs(this);
            }
        }

        #endregion

        #region Private Methods

        private IEnumerator PeriodicTooltipUpdate()
        {
            WaitForSeconds wait = new WaitForSeconds(periodicTooltipUpdateInterval);
            
            // TODO: Check if this is a memory leak - it probably is
            while (true)
            {
                UpdateInteractionTooltip();
                yield return wait;
            }
        }

        private IInteractable GetInteractableInVolume()
        {
            return GetInteractablesInVolume().FirstOrDefault();
        }
        
        private IEnumerable<IInteractable> GetInteractablesInVolume()
        {
            return interactionVolume.GetComponentsInVolume<IInteractable>();
        }

        private void UpdateInteractionTooltip()
        {
            IInteractable interactable = GetInteractableInVolume();

            interactionTooltipText.text =
                interactable == null ? String.Empty : interactable.InteractionToolTip;
        }

        #endregion

        #region IActor Implementation

        public GameObject GameObject => gameObject;

        #endregion
    }
}