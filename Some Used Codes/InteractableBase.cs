using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

namespace VHS
{
    public class InteractableBase : MonoBehaviour, IInteractable
    {
        #region Variables    
            [Space,Header("Interactable Settings")]

            [SerializeField] private bool holdInteract = true;
            [ShowIf("holdInteract")][SerializeField] private float holdDuration = 3f;
            
            [Space] 
            [SerializeField] private bool multipleUse = false;
            [SerializeField] private bool isInteractable = true;

            [SerializeField] private string tooltipMessage = "interact";
            public PlayerHealthh IntCont;

        public Animator enemyanimator;  
         public Animation[] anims;   
         public string[] names;

         public GameObject[] destroyThis;
         public GameObject[] enableThis;

public int SceneNumber;

        #endregion

        #region Properties    
            public float HoldDuration => holdDuration; 

            public bool HoldInteract => holdInteract;
            public bool MultipleUse => multipleUse;
            public bool IsInteractable => isInteractable;

            public string TooltipMessage => tooltipMessage;
        #endregion

        #region Methods
        public virtual void OnInteract()
            {
                Debug.Log("INTERACTED: " + gameObject.name);


            if(gameObject.tag == "banana"){
                Debug.Log("bananeeeeee");
                IntCont.inchealth();
            }

            if(gameObject.tag == "door"){
                Debug.Log("doooor");
                for(int i = 0; i<anims.Length ; i++ ){
                anims[i].Play(names[i]);}
            }

            if(gameObject.tag == "Enemy"){
                Debug.Log("killlll");
                enemyanimator.SetBool("killed", true);
                for(int i = 0; i<anims.Length ; i++ ){
                anims[i].Play(names[i]);}
            }
            if(gameObject.tag == "Exit"){
               SceneManager.LoadScene(SceneNumber);
            
            }
             if(gameObject.tag == "Change"){
               
               for(int i = 0; i<destroyThis.Length ; i++ ){
                destroyThis[i].SetActive(false);}
               
              for(int i = 0; i<enableThis.Length ; i++ ){
                enableThis[i].SetActive(true);}
            }

            }

            public virtual void Transition (){
                SceneManager.LoadScene(SceneNumber);
            }
        #endregion
    }
}
