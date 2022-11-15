/*using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace EasyUI.Dailogs
{
        public class Dialog
        {
        public string Title = "Title";
        public string Message = "Message";
        }
public class DialogUI : MonoBehaviour
{
    GameObject canvas;
    Text titleUIText;
    Text messageUIText;
    Button closeUIButton;

   Dialog dialog = new Dialog();


   public static DialogUI Instance;
   void Awake ()
   {
    Instance = this;

    //Add close event listener
    closeUIButton.onClick.removeAllListeners();
    closeUIButton.onClick.AddListener( HIDE );

   }
    //Set Dialog Title
   public DialogUI SetTitle( string title )
   {
    dialog.Title = title;
    return Instance;
   }
    //Set Dialog Message
   public DialogUI SetMessage(string message)
   {
    dialog.Message = message;
    return Instance;
   }
    //Show dialog
   public void Show()
   {
    titleUIText.text  = dialog.Title;
    messageUIText.text = dialog.Message;
    canvas.SetActive (true);
   }
   //Hide dialog
   public void Hide()
   {
    canvas.SetActive (false);
    //Reset Duakig
    dialog = new Dialog();
   }
}

}*/