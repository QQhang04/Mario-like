using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{ 
   public InputActionAsset actions;
   protected InputAction m_movement;
   protected virtual void Awake() => CacheActions();

   private void Start()
   {
      actions.Enable();
   }

   protected void Update()
   {
      
   }

   private void OnEnable()
   {
      actions?.Enable();
   }

   private void OnDisable()
   {
      actions?.Disable();
   }

   protected virtual void CacheActions()
   {
      m_movement = actions["Movement"];
   }
   
}
