using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using NUnit.Framework.Internal;

////TODO: localization support

////TODO: deal with composites that have parts bound in different control schemes

namespace UnityEngine.InputSystem.Samples.RebindUI
{
    /// <summary>
    /// A reusable component with a self-contained UI for rebinding a single action.
    /// </summary>
    public class RebindActionUI : MonoBehaviour
    {
        /// <summary>
        /// Reference to the action that is to be rebound.
        /// </summary>
        public InputActionReference actionReference
        {
            get => m_Action;
            set
            {
                m_Action = value;
                m_RuntimeAction = null;
                UpdateActionLabel();
                UpdateBindingDisplay();
            }
        }

                public string bindingId
        {
            get => m_BindingId;
            set
            {
                m_BindingId = value;
                UpdateBindingDisplay();
            }
        }

        public InputBinding.DisplayStringOptions displayStringOptions
        {
            get => m_DisplayStringOptions;
            set
            {
                m_DisplayStringOptions = value;
                UpdateBindingDisplay();
            }
        }

        /// <summary>
        /// Text component that receives the name of the action. Optional.
        /// </summary>
        public TextMeshProUGUI actionLabel
        {
            get => m_ActionLabel;
            set
            {
                m_ActionLabel = value;
                UpdateActionLabel();
            }
        }

        /// <summary>
        /// Text component that receives the display string of the binding. Can be <c>null</c> in which
        /// case the component entirely relies on <see cref="updateBindingUIEvent"/>.
        /// </summary>
        public TextMeshProUGUI bindingText
        {
            get => m_BindingText;
            set
            {
                m_BindingText = value;
                UpdateBindingDisplay();
            }
        }

        /// <summary>
        /// Optional text component that receives a text prompt when waiting for a control to be actuated.
        /// </summary>
        /// <seealso cref="startRebindEvent"/>
        /// <seealso cref="rebindOverlay"/>
        public TextMeshProUGUI rebindPrompt
        {
            get => m_RebindText;
            set => m_RebindText = value;
        }

        /// <summary>
        /// Optional text component that shows relevant information when waiting for a control to be actuated.
        /// </summary>
        /// <seealso cref="rebindPrompt"/>
        /// <seealso cref="rebindOverlay"/>
        public TextMeshProUGUI rebindInfo
        {
            get => m_RebindInfo;
            set => m_RebindInfo = value;
        }

        /// <summary>
        /// Optional button to manually cancel rebinding while waiting.
        /// </summary>
        public Button rebindCancelButton
        {
            get => m_RebindCancelButton;
            set => m_RebindCancelButton = value;
        }

        
        public GameObject rebindOverlay
        {
            get => m_RebindOverlay;
            set => m_RebindOverlay = value;
        }

        /// <summary>
        /// Event that is triggered every time the UI updates to reflect the current binding.
        /// This can be used to tie custom visualizations to bindings.
        /// </summary>
        public UpdateBindingUIEvent updateBindingUIEvent
        {
            get
            {
                if (m_UpdateBindingUIEvent == null)
                    m_UpdateBindingUIEvent = new UpdateBindingUIEvent();
                return m_UpdateBindingUIEvent;
            }
        }

        /// <summary>
        /// Event that is triggered when an interactive rebind is started on the action.
        /// </summary>
        public InteractiveRebindEvent startRebindEvent
        {
            get
            {
                if (m_RebindStartEvent == null)
                    m_RebindStartEvent = new InteractiveRebindEvent();
                return m_RebindStartEvent;
            }
        }

        
        public InteractiveRebindEvent stopRebindEvent
        {
            get
            {
                if (m_RebindStopEvent == null)
                    m_RebindStopEvent = new InteractiveRebindEvent();
                return m_RebindStopEvent;
            }
        }

        
        public InputActionRebindingExtensions.RebindingOperation ongoingRebind => m_RebindOperation;

       
        private InputAction m_RuntimeAction;

        private InputAction RuntimeAction
        {
            get
            {
                if (m_RuntimeAction == null && m_Action?.action != null && GameInputManager.Instance != null)
                    m_RuntimeAction = GameInputManager.Instance.Controls.asset.FindAction(m_Action.action.name);

                return m_RuntimeAction;
            }
        }
        
        public bool ResolveActionAndBinding(out InputAction action, out int bindingIndex)
        {
            bindingIndex = -1;
            action = RuntimeAction; // Modificado: usamos la accion runtime, no m_Action.action

            if (action == null)
                return false;

            bindingIndex = action.FindBindingById(m_BindingId);
            if (bindingIndex >= 0)
                return true;

            if (!string.IsNullOrEmpty(m_BindingId))
                Debug.LogError($"Cannot find binding with ID '{m_BindingId}' on '{action}'", this);
            return false;
        }

       
        public void UpdateBindingDisplay()
        {
            var displayString = string.Empty;
            var deviceLayoutName = default(string);
            var controlPath = default(string);

            // Get display string from action.
            var action = RuntimeAction; // Modificado
            if (action != null)
            {
                var bindingIndex = action.bindings.IndexOf(x => x.id.ToString() == m_BindingId);
                if (bindingIndex != -1)
                    displayString = action.GetBindingDisplayString(bindingIndex, out deviceLayoutName, out controlPath, displayStringOptions);
            }

            
            if (m_BindingText != null)
                m_BindingText.text = displayString;

            
            m_UpdateBindingUIEvent?.Invoke(this, displayString, deviceLayoutName, controlPath);
        }

        
        public void ResetToDefault()
        {
            if (!ResolveActionAndBinding(out var action, out var bindingIndex))
                return;

            ResetBinding(action, bindingIndex); //Modificado

            UpdateBindingDisplay();
        }

        private void ResetBinding(InputAction action, int bindingIndex) //Modificado
        {
            InputBinding newBinding = action.bindings[bindingIndex]; //Modificado
            string oldOverridePath = newBinding.overridePath; //Modificado

            action.RemoveBindingOverride(bindingIndex); //Modificado

            foreach (InputAction otheraction in action.actionMap.actions) //Modificado
            {
                if (otheraction == action) //Modificado
                {
                    continue;  //Modificado
                }

                for (int i = 0; i < otheraction.bindings.Count; i++)  //Modificado
                {
                    InputBinding binding = otheraction.bindings[i];  //Modificado
                    if (binding.overridePath == newBinding.path)  //Modificado
                    {
                        otheraction.ApplyBindingOverride(i, oldOverridePath);  //Modificado
                    }
                }
            }

            SaveActionBinding(); //Modificado: persistir tambien al resetear
        }

        
        public void SwapBinding(RebindActionUI other)
        {
            if (this == other)
                return; // Silently ignore any request to swap binding with itself
            if (ongoingRebind != null || other.ongoingRebind != null)
                throw new Exception("Cannot swap bindings when interactive rebinding is ongoing");
            if (!ResolveActionAndBinding(out var action, out var bindingIndex))
                throw new Exception("Failed to resolve action and binding index");
            if (!other.ResolveActionAndBinding(out var otherAction, out var otherBindingIndex))
                throw new Exception("Failed to resolve action and binding index");

            var effectivePath = action.bindings[bindingIndex].effectivePath;
            var otherEffectivePath = otherAction.bindings[otherBindingIndex].effectivePath;
            action.ApplyBindingOverride(bindingIndex, otherEffectivePath);
            otherAction.ApplyBindingOverride(otherBindingIndex, effectivePath);

            SaveActionBinding(); //Modificado
            other.SaveActionBinding(); //Modificado
        }

       
        public void StartInteractiveRebind()
        {
            if (!ResolveActionAndBinding(out var action, out var bindingIndex))
                return;

            action.Disable(); //Modificado: deshabilitamos la accion RUNTIME correcta

            if (action.bindings[bindingIndex].isComposite)
            {
                var firstPartIndex = bindingIndex + 1;
                if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
                    PerformInteractiveRebind(action, firstPartIndex, allCompositeParts: true);
            }
            else
            {
                PerformInteractiveRebind(action, bindingIndex);
            }
        }

        private void PerformInteractiveRebind(InputAction action, int bindingIndex, bool allCompositeParts = false)
        {
            action.Disable(); //Modificado
            m_RebindOperation?.Cancel(); // Will null out m_RebindOperation.

            var actionWasEnabledPriorToRebind = action.enabled;

            void CleanUp()
            {
                action.Enable(); //Modificado: antes decia m_Action.action.Enable() (instancia equivocada)

                if (m_RebindCancelButton != null)
                    m_RebindCancelButton.onClick.RemoveListener(CancelRebind);

                m_RebindOperation?.Dispose();
                m_RebindOperation = null;

                // Restore action enabled state based on state prior to rebind
                if (actionWasEnabledPriorToRebind)
                    action.actionMap.Enable();

                SaveActionBinding(); //Modificado
            }


            if (actionWasEnabledPriorToRebind)
                action.actionMap.Disable();

            // Configure the rebind.
            m_RebindOperation = action.PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough("<Keyboard>/escape") //Modificado
            .OnCancel(
                    operation =>
                    {
                        m_RebindStopEvent?.Invoke(this, operation);
                        if (m_RebindOverlay != null)
                            m_RebindOverlay.SetActive(false);
                        UpdateBindingDisplay();
                        CleanUp();
                    })

                .WithActionEventNotificationsBeingSuppressed()
                .WithTimeout(m_RebindTimeout)
                .OnComplete(
                    operation =>
                    {
                        if (m_RebindOverlay != null)
                            m_RebindOverlay.SetActive(false);
                        m_RebindStopEvent?.Invoke(this, operation);

                        Debug.Log($"Rebind completo -> Accion: {action.name} | bindingIndex: {bindingIndex} | overridePath: {action.bindings[bindingIndex].overridePath}"); //Modificado

                        if (CheckDuplicateBindings(action, bindingIndex, allCompositeParts)) //Modificado
                        {
                            action.RemoveBindingOverride(bindingIndex); //Modificado
                            CleanUp();  //Modificado
                            PerformInteractiveRebind(action, bindingIndex, allCompositeParts);  //Modificado
                            return; //Modificado
                        }

                        UpdateBindingDisplay();
                        CleanUp();

                        // If there's more composite parts we should bind, initiate a rebind
                        // for the next part.
                        if (allCompositeParts)
                        {
                            var nextBindingIndex = bindingIndex + 1;
                            if (nextBindingIndex < action.bindings.Count && action.bindings[nextBindingIndex].isPartOfComposite)
                                PerformInteractiveRebind(action, nextBindingIndex, true);
                        }
                    });

            var partName = default(string);
            if (action.bindings[bindingIndex].isPartOfComposite)
                partName = $"Binding '{action.bindings[bindingIndex].name}'. ";

            m_RebindOverlay?.SetActive(true);
            if (m_RebindText != null)
            {
                var text = !string.IsNullOrEmpty(m_RebindOperation.expectedControlType)
                    ? $"{partName}Waiting for {m_RebindOperation.expectedControlType} input..."
                    : $"{partName}Waiting for input...";
                m_RebindText.text = text;
            }

            if (m_RebindCancelButton != null)
            {
                m_RebindCancelButton.onClick.AddListener(CancelRebind);
            }

            // Update rebind overlay information, if we have one.
            if (m_RebindInfo != null)
            {
                m_RebindStartTime = Time.realtimeSinceStartup;
                UpdateRebindInfo(m_RebindStartTime);
            }


            if (m_RebindOverlay == null && m_RebindText == null && m_RebindStartEvent == null && m_BindingText != null)
                m_BindingText.text = "<Waiting...>";

            // Give listeners a chance to act on the rebind starting.
            m_RebindStartEvent?.Invoke(this, m_RebindOperation);

            m_RebindOperation.Start();
        }

        private void UpdateRebindInfo(double now)
        {
            if (m_RebindOperation == null)
                return;

            var elapsed = now - m_RebindStartTime;
            var remainingTimeoutWholeSeconds = (int)Math.Floor(m_RebindOperation.timeout - elapsed);
            if (remainingTimeoutWholeSeconds == m_LastRemainingTimeoutSeconds)
                return;

            var text = (m_RebindOperation.timeout > 0.0f)
                ? $"Cancels in <b>{remainingTimeoutWholeSeconds}</b> seconds if no matching input is provided."
                : string.Empty;
            m_RebindInfo.text = text;
            m_LastRemainingTimeoutSeconds = remainingTimeoutWholeSeconds;
        }

        private void CancelRebind()
        {
            m_RebindOperation?.Cancel();
        }

        protected void Update()
        {
            if (m_RebindInfo != null)
                UpdateRebindInfo(Time.realtimeSinceStartupAsDouble);  
    
        }

        protected void OnEnable()
{
    if (s_RebindActionUIs == null)
        s_RebindActionUIs = new List<RebindActionUI>();
    s_RebindActionUIs.Add(this);
    if (s_RebindActionUIs.Count == 1)
        InputSystem.onActionChange += OnActionChange;

    if (GameInputManager.Instance != null)
    {
        UpdateBindingDisplay();
    }
    else
    {
        StartCoroutine(WaitForInputManagerThenRefresh());
    }
}

private System.Collections.IEnumerator WaitForInputManagerThenRefresh()
{
    // Espera hasta que el singleton exista (frame a frame)
    while (GameInputManager.Instance == null)
        yield return null;

    m_RuntimeAction = null; // por si quedó cacheado en null
    UpdateBindingDisplay();
}

        protected void OnDisable()
        {
            m_RebindOperation?.Dispose();
            m_RebindOperation = null;

            s_RebindActionUIs.Remove(this);
            if (s_RebindActionUIs.Count == 0)
            {
                s_RebindActionUIs = null;
                InputSystem.onActionChange -= OnActionChange;
            }
            UpdateBindingDisplay();
        }

        
        private static void OnActionChange(object obj, InputActionChange change)
        {
            if (change != InputActionChange.BoundControlsChanged)
                return;

            var action = obj as InputAction;
            var actionMap = action?.actionMap ?? obj as InputActionMap;
            var actionAsset = actionMap?.asset ?? obj as InputActionAsset;

            for (var i = 0; i < s_RebindActionUIs.Count; ++i)
            {
                var component = s_RebindActionUIs[i];
                var referencedAction = component.RuntimeAction;
                if (referencedAction == null)
                    continue;

                if (referencedAction == action ||
                    referencedAction.actionMap == actionMap ||
                    referencedAction.actionMap?.asset == actionAsset)
                    component.UpdateBindingDisplay();
            }
        }

        [Tooltip("Reference to action that is to be rebound from the UI.")]
        [SerializeField]
        private InputActionReference m_Action;

        [SerializeField]
        private string m_BindingId;

        [SerializeField]
        private InputBinding.DisplayStringOptions m_DisplayStringOptions;

        [Tooltip("Text label that will receive the name of the action. Optional. Set to None to have the "
            + "rebind UI not show a label for the action.")]
        [SerializeField]
        private TextMeshProUGUI m_ActionLabel;

        [Tooltip("Text label that will receive the current, formatted binding string.")]
        [SerializeField]
        private TextMeshProUGUI m_BindingText;

        [Tooltip("Optional UI that will be shown while a rebind is in progress.")]
        [SerializeField]
        private GameObject m_RebindOverlay;

        [Tooltip("Optional text label that will be updated with prompt for user input.")]
        [SerializeField]
        private TextMeshProUGUI m_RebindText;

        [Tooltip("Optional text label that will be updated with relevant information during rebinding.")]
        [SerializeField]
        private TextMeshProUGUI m_RebindInfo;

        [Tooltip("Optional cancellation UI button for rebinding overlay.")]
        [SerializeField]
        private Button m_RebindCancelButton;

        [Tooltip("Optional rebinding timeout in seconds. If zero, no timeout will be used.")]
        [SerializeField]
        private float m_RebindTimeout;

        [Tooltip("Event that is triggered when the way the binding is display should be updated. This allows displaying "
            + "bindings in custom ways, e.g. using images instead of text.")]
        [SerializeField]
        private UpdateBindingUIEvent m_UpdateBindingUIEvent;

        [Tooltip("Event that is triggered when an interactive rebind is being initiated. This can be used, for example, "
            + "to implement custom UI behavior while a rebind is in progress. It can also be used to further "
            + "customize the rebind.")]
        [SerializeField]
        private InteractiveRebindEvent m_RebindStartEvent;

        [Tooltip("Event that is triggered when an interactive rebind is complete or has been aborted.")]
        [SerializeField]
        private InteractiveRebindEvent m_RebindStopEvent;

        private InputActionRebindingExtensions.RebindingOperation m_RebindOperation;

        private static List<RebindActionUI> s_RebindActionUIs;

        private double m_RebindStartTime = -1;
        private int m_LastRemainingTimeoutSeconds;

       
#if UNITY_EDITOR
        protected void OnValidate()
        {
            UpdateActionLabel();
            UpdateBindingDisplay();
        }

#endif

        private void UpdateActionLabel()
        {
            if (m_ActionLabel != null)
            {
                var action = m_Action?.action;
                m_ActionLabel.text = action != null ? action.name : string.Empty;
            }
        }

        private void Start()
        {
            LoadActionBinding(); //Modificado
            UpdateBindingDisplay();
        }

        private void SaveActionBinding()
        {
            var action = RuntimeAction; //Modificado
            if (action == null || GameInputManager.Instance == null)
                return;

            var map = action.actionMap;
            var currentBindings = map.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString(map.name, currentBindings); // una key por Action Map, no por binding
        }

        private void LoadActionBinding()
        {
            var action = RuntimeAction; //Modificado
            if (action == null || GameInputManager.Instance == null)
                return;

            var map = action.actionMap;
            var savedBindings = PlayerPrefs.GetString(map.name);
            if (!string.IsNullOrEmpty(savedBindings))
                map.LoadBindingOverridesFromJson(savedBindings);
        }

        [Serializable]
        public class UpdateBindingUIEvent : UnityEvent<RebindActionUI, string, string, string>
        {
        }

        [Serializable]
        public class InteractiveRebindEvent : UnityEvent<RebindActionUI, InputActionRebindingExtensions.RebindingOperation>
        {
        }

private bool CheckDuplicateBindings(InputAction action, int bindingIndex, bool AllCompositeParts = false)
{
    InputBinding newBinding = action.bindings[bindingIndex];
    // Modificado: usamos effectivePath (override si existe, sino el default) en vez de
    // overridePath a secas. Antes, si la otra accion nunca habia sido reasignada, su
    // overridePath estaba vacio y el chequeo la saltaba entera -> por eso "a veces andaba
    // y a veces no": solo detectaba choques contra teclas que el jugador YA habia
    // reasignado antes, nunca contra una tecla que seguia siendo la default de fabrica.
    var newEffectivePath = newBinding.effectivePath;

    var asset = action.actionMap.asset;
    IEnumerable<InputActionMap> mapsToCheck = asset != null ? asset.actionMaps : new[] { action.actionMap };

    foreach (var map in mapsToCheck)
    {
        foreach (InputBinding binding in map.bindings)
        {
            // Ignoramos el binding que se esta reasignando (comparado por id, no por accion entera,
            // para no saltear otras partes de un composite que si podrian chocar entre si)
            if (binding.id == newBinding.id)
                continue;

            // Ignoramos composites/separadores sin path propio
            if (binding.isComposite || string.IsNullOrEmpty(binding.effectivePath))
                continue;

            if (binding.effectivePath == newEffectivePath)
            {
                Debug.LogError("Duplicate binding found: " + newEffectivePath);
                return true;
            }
        }
    }

    if (AllCompositeParts)
    {
        for (int f = 1; f < bindingIndex; f++)
        {
            if (string.IsNullOrEmpty(action.bindings[f].effectivePath))
                continue;

            if (action.bindings[f].effectivePath == newEffectivePath)
            {
                Debug.Log("Duplicate binding found: " + newEffectivePath);
                return true;
            }
        }
    }

    return false;
}

public void ResetAllBindingsToDefault()
{
    if (GameInputManager.Instance == null || GameInputManager.Instance.Controls == null)
    {
        Debug.LogWarning("No se pudo resetear: GameInputManager.Instance o Controls es null.");
        return;
    }

    var asset = GameInputManager.Instance.Controls.asset;

    foreach (var map in asset.actionMaps)
    {
        map.RemoveAllBindingOverrides(); // saca los overrides en memoria (vuelve al binding original)
        PlayerPrefs.DeleteKey(map.name); // borra lo guardado en disco para ese mapa
    }

    PlayerPrefs.Save();

    // Refresca todas las filas de rebind de la escena para que muestren el default actualizado
    foreach (var ui in FindObjectsByType<RebindActionUI>(FindObjectsSortMode.None))
    {
        ui.m_RuntimeAction = null; // invalida el cache por si acaso
        ui.UpdateBindingDisplay();
    }

    Debug.Log("Todos los bindings fueron restablecidos a su valor original.");
}
    }
}