using System;
using HarmonyLib;
using Il2CppInterop.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VGEditor;
using Object = UnityEngine.Object;

namespace ReplacePrefab;


[HarmonyPatch(typeof(EditorManager))]
public class QsEditorPatch
{
    [HarmonyPatch(nameof(EditorManager.Start))]
    [HarmonyPostfix]
    static void PostSetup(ref EditorManager __instance)
    {
        var dialogs = __instance.transform.parent.Find("Editor GUI/sizer/EditorDialogs");

        var content = dialogs.Find("MultiObjectDialog/data/Viewport/Content");
        var buttonPrefab = dialogs.Find("PrefabObjectEditor/data/left/Scroll View/Viewport/content/expand");

        Transform button = Object.Instantiate(buttonPrefab, content);
        button.name = "ReplaceButton";
        button.GetComponent<Button>().onClick.AddListener(new Action(() =>
        {
            string instID = ObjectEditor.Inst.MainSelectedObject.GetObjectData().prefabInstanceID;

            ObjectEditor.Inst.SelectedObjects.InOrder.ForEach(new Action<ObjectSelection>(x =>
            {
                x.GetObjectData().prefabInstanceID = instID;
            }));
            
            PrefabEditor.Inst.CollapseCurrentPrefab();
        }));
        
        MultiObjectPanelPatch.ButtonText = button.GetChild(1).GetComponent<TextMeshProUGUI>();
        MultiObjectPanelPatch.ButtonText.text = "Replace prefab with selection";

        MultiObjectPanelPatch.ReplaceButton = button.gameObject;
    }
}

[HarmonyPatch(typeof(EditorElement_MultiObjectPanel))]
public class MultiObjectPanelPatch
{
    public static GameObject ReplaceButton;
    public static TextMeshProUGUI ButtonText;

    [HarmonyPatch(nameof(EditorElement_MultiObjectPanel.OnRender))]
    [HarmonyPostfix]
    static void PostRender(ref EditorElement_MultiObjectPanel __instance)
    {

        if (ObjectEditor.Inst.MainSelectedObject.GetObjectData().prefabInstanceID != "")
        {
            //gets the prefab name of the latest selected obj
            string id = ObjectEditor.Inst.MainSelectedObject.GetObjectData().prefabID;
            id = DataManager.inst.gameData.prefabs
                .Find(new Predicate<DataManager.GameData.Prefab>(x => x.ID == id).ToIL2CPP()).Name;
            
            ButtonText.text = $"Replace '{id}' with Selection";
            ReplaceButton.SetActive(true);
            
            //ObjectEditor.Inst.MainSelectedObject.GetPrefabData() returns null :(
        }
        else
        {
            ReplaceButton.SetActive(false);
        }
    }
}

public static class PredicateExtension
{
    //this is here so I dont have to call ConvertDelegate manually every time. will likely re-use this in other mods
    public static Il2CppSystem.Predicate<T> ToIL2CPP<T>(this Predicate<T> predicate)
    {
        return DelegateSupport.ConvertDelegate<Il2CppSystem.Predicate<T>>(predicate);
    }
}