using System;
using HarmonyLib;
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
        button.GetComponent<Button>().onClick.AddListener(new Action(() =>
        {
            var selectList = ObjectEditor.Inst.SelectedObjects.InOrder;
            var selection = selectList[selectList.Count - 1].GetObjectData();
            
            string instID = selection.prefabInstanceID;

            ObjectEditor.Inst.SelectedObjects.InOrder.ForEach(new Action<ObjectSelection>(x => { x.GetObjectData().prefabInstanceID = instID;}));
         
            PrefabEditor.Inst.CollapseCurrentPrefab();
        }));
        button.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Replace prefab with selection";
    }

}

[HarmonyPatch(typeof(EditorElement_MultiObjectPanel))]
public class MultiObjectPanelPatch
{
    [HarmonyPatch(nameof(EditorElement_MultiObjectPanel.OnRender))]
    [HarmonyPostfix]
    static void PostSetup(ref EditorElement_MultiObjectPanel __instance)
    {
        var select = ObjectEditor.Inst.SelectedObjects.InOrder;

        if (select[select.Count - 1].GetObjectData().prefabInstanceID != "")
        {
            __instance.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(25).gameObject.SetActive(true); //Find(string) doesnt work, WHYYYY???
        }
        else
        {
            __instance.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(25).gameObject.SetActive(false);
        }
    }
}