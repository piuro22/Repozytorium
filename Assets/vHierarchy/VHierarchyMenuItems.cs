#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static VHierarchy.Libs.VUtils;
using static VHierarchy.Libs.VGUI;


namespace VHierarchy
{
    class VHierarchyMenuItems
    {

        const string menuDir = "Tools/vHierarchy/";


        public static bool setActiveEnabled { get => EditorPrefs.GetBool("vHierarchy-setActiveEnabled", true); set => EditorPrefs.SetBool("vHierarchy-setActiveEnabled", value); }
        public static bool focusEnabled { get => EditorPrefs.GetBool("vHierarchy-focusEnabled", true); set => EditorPrefs.SetBool("vHierarchy-focusEnabled", value); }
        public static bool deleteEnabled { get => EditorPrefs.GetBool("vHierarchy-deleteEnabled", true); set => EditorPrefs.SetBool("vHierarchy-deleteEnabled", value); }
        public static bool expandCollapseEnabled { get => EditorPrefs.GetBool("vHierarchy-expandCollapseEnabled", true); set => EditorPrefs.SetBool("vHierarchy-expandCollapseEnabled", value); }
        public static bool collapseEverythingElseEnabled { get => EditorPrefs.GetBool("vHierarchy-collapseEverythingElseEnabled", true); set => EditorPrefs.SetBool("vHierarchy-collapseEverythingElseEnabled", value); }
        public static bool collapseEverythingEnabled { get => EditorPrefs.GetBool("vHierarchy-collapseEverythingEnabled", true); set => EditorPrefs.SetBool("vHierarchy-collapseEverythingEnabled", value); }

        public static bool componentMinimapEnabled { get => EditorPrefs.GetBool("vHierarchy-componentMinimapEnabled", true); set => EditorPrefs.SetBool("vHierarchy-componentMinimapEnabled", value); }
        public static bool iconsEnabled { get => EditorPrefs.GetBool("vHierarchy-iconsEnabled", true); set => EditorPrefs.SetBool("vHierarchy-iconsEnabled", value); }
        public static bool collapseAndLightingButtonsEnabled { get => EditorPrefs.GetBool("vHierarchy-collapseAndLightingButtonsEnabled", true); set => EditorPrefs.SetBool("vHierarchy-collapseAndLightingButtonsEnabled", value); }



        const string setActive = menuDir + "A to toggle active";
        const string focus = menuDir + "F to focus";
        const string delete = menuDir + "X to delete";
        const string expandCollapse = menuDir + "E to expand or collapse";
        const string collapseEverythingElse = menuDir + "Shift-E to collapse everything else";
        const string collapseEverything = menuDir + "Ctrl-Shift-E to collapse everything";

        const string icons = menuDir + "Custom icons via Alt-Click";
        const string componentMinimap = menuDir + "Component minimap";
        const string collapseAndLightingButtons = menuDir + "Collapse All and Lighting buttons ";




        [MenuItem(menuDir + "Shortcuts", false, 1)] static void dadsas() { }
        [MenuItem(menuDir + "Shortcuts", true, 1)] static bool dadsas123() => false;

        [MenuItem(setActive, false, 1)] static void dadsadadsas() => setActiveEnabled = !setActiveEnabled;
        [MenuItem(setActive, true, 1)] static bool dadsaddasadsas() { Menu.SetChecked(setActive, setActiveEnabled); return true; }

        [MenuItem(focus, false, 1)] static void dadsadasdadsas() => focusEnabled = !focusEnabled;
        [MenuItem(focus, true, 1)] static bool dadsadsaddasadsas() { Menu.SetChecked(focus, focusEnabled); return true; }

        [MenuItem(delete, false, 1)] static void dadsadsadasdadsas() => deleteEnabled = !deleteEnabled;
        [MenuItem(delete, true, 1)] static bool dadsaddsasaddasadsas() { Menu.SetChecked(delete, deleteEnabled); return true; }

        [MenuItem(expandCollapse, false, 1)] static void dadsadsadasdsadadsas() => expandCollapseEnabled = !expandCollapseEnabled;
        [MenuItem(expandCollapse, true, 1)] static bool dadsaddsasadadsdasadsas() { Menu.SetChecked(expandCollapse, expandCollapseEnabled); return true; }

        [MenuItem(collapseEverythingElse, false, 1)] static void dadsadsasdadasdsadadsas() => collapseEverythingElseEnabled = !collapseEverythingElseEnabled;
        [MenuItem(collapseEverythingElse, true, 1)] static bool dadsaddsdasasadadsdasadsas() { Menu.SetChecked(collapseEverythingElse, collapseEverythingElseEnabled); return true; }

        [MenuItem(collapseEverything, false, 1)] static void dadsadsdasadasdsadadsas() => collapseEverythingEnabled = !collapseEverythingEnabled;
        [MenuItem(collapseEverything, true, 1)] static bool dadsaddssdaasadadsdasadsas() { Menu.SetChecked(collapseEverything, collapseEverythingEnabled); return true; }




        [MenuItem(menuDir + "Features", false, 12)] static void daasddsas() { }
        [MenuItem(menuDir + "Features", true, 12)] static bool dadsdasas123() => false;

        [MenuItem(icons, false, 12)] static void dadsadaasdsdadsas() => iconsEnabled = !iconsEnabled;
        [MenuItem(icons, true, 12)] static bool dadsadsadadssaddasadsas() { Menu.SetChecked(icons, iconsEnabled); return true; }

        [MenuItem(componentMinimap, false, 12)] static void daadsdsadasdadsas() => componentMinimapEnabled = !componentMinimapEnabled;
        [MenuItem(componentMinimap, true, 12)] static bool dadsadasddasadsas() { Menu.SetChecked(componentMinimap, componentMinimapEnabled); return true; }

        [MenuItem(collapseAndLightingButtons, false, 12)] static void daadsdsadadsasdadsas() => collapseAndLightingButtonsEnabled = !collapseAndLightingButtonsEnabled;
        [MenuItem(collapseAndLightingButtons, true, 12)] static bool dadsadasdsaddasadsas() { Menu.SetChecked(collapseAndLightingButtons, collapseAndLightingButtonsEnabled); return true; }



        [MenuItem(menuDir + "Disable vHierarchy", false, 132)]
        static void das() => ToggleDefineDisabledInScript(typeof(VHierarchy));
        [MenuItem(menuDir + "Disable vHierarchy", true, 132)]
        static bool dassadc() { Menu.SetChecked(menuDir + "Disable vHierarchy", ScriptHasDefineDisabled(typeof(VHierarchy))); return true; }




    }
}
#endif