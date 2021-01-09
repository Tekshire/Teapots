/// <summary>
/// Developed by DaiMangou Games 
/// programmer and developer : Ruchmair Dixon
/// 2015
/// </summary>

using UnityEngine;
using UnityEditor;
using DaiMangou.RadarBuilder3D.Editor;
using DaiMangou.RadarBuilder3DEditor;
using System;
using System.Linq;

namespace DaiMangou.RadarBuilder3D
{


    public class _3DRadarBuilderEditorWindow : EditorWindow
    {

        [MenuItem("Tools/DaiMangou/3D Radar Builder")]
        private static void Init()
        {
            EditorWindow win = GetWindow(typeof(_3DRadarBuilderEditorWindow ));
            win.minSize = new Vector2(600, 250);
        }

        #region Variables
   
        private _3DRadar Current_Selected3D_Radar;
  
        private int Tab_Selection = 0;
        private GameObject _Selection()
        {
            try { return Selection.activeGameObject; }
            catch { return null; }

        }
        private Vector2 RadarBlipScrollPosition, DesignsScrollPosition;
        private GameObject RadarObject;
        private bool ShowHelpMessages;
        private Texture2D DaiMangouLogo, contactEmailIcon, tutorialIcon;

        private Texture2D
            On,
            Off,
            Optimizeicon,
            upArrow,
            downArrow,
            deleteConditionIcon,
            UITargetTrackerIcon, copyIcon,
            pasteIcon;

        private bool ShowContactArea;
        private string emailAddress, Subject, Message;

        public static Rect ScreenRect = new Rect();

        private RadarBlips3D copiedBlipData3D = new RadarBlips3D();
        #endregion


        public void OnEnable()
        {

            if (Application.HasProLicense())
                this.titleContent.image = GetResource.LoadDllImageResource(typeof(GetResource), "GeneralImageResources", "RadarIconlight", "png");
            else
                this.titleContent.image = GetResource.LoadDllImageResource(typeof(GetResource), "GeneralImageResources", "RadarIconDark", "png");


            this.titleContent.text = "3D Radar";

            DaiMangouLogo = GetResource.LoadDllImageResource(typeof(GetResource), "GeneralImageResources", "DaiMangouLogo", "png");


            On = Application.HasProLicense() ? GetResource.LoadDllImageResource(typeof(GetResource), "GeneralImageResources", "PowerOnpro", "png") : GetResource.LoadDllImageResource(typeof(GetResource), "GeneralImageResources", "PowerOn", "png");
            Off = Application.HasProLicense() ? GetResource.LoadDllImageResource(typeof(GetResource), "GeneralImageResources", "PowerOffpro", "png") : GetResource.LoadDllImageResource(typeof(GetResource), "GeneralImageResources", "PowerOff", "png");
            Optimizeicon = GetResource.LoadDllImageResource(typeof(GetResource), "GeneralImageResources", "optimizeIcon", "png");
            contactEmailIcon = GetResource.LoadDllImageResource(typeof(GetResource), "GeneralImageResources", "contactEmail", "png");
            tutorialIcon = GetResource.LoadDllImageResource(typeof(GetResource), "GeneralImageResources", "tutorialIcon", "png");
            upArrow = GetResource.LoadDllImageResource(typeof(GetResource), "GeneralImageResources", "upArrow", "png");
            downArrow = GetResource.LoadDllImageResource(typeof(GetResource), "GeneralImageResources", "downArrow", "png");
            deleteConditionIcon = GetResource.LoadDllImageResource(typeof(GetResource), "GeneralImageResources", "deleteConditionIcon", "png");
            UITargetTrackerIcon = GetResource.LoadDllImageResource(typeof(GetResource), "GeneralImageResources", "TargetTrackerIconL", "png");
            copyIcon = GetResource.LoadDllImageResource(typeof(GetResource), "GeneralImageResources", "CopyIcon", "png");
            pasteIcon = GetResource.LoadDllImageResource(typeof(GetResource), "GeneralImageResources", "PasteIcon", "png");



        }


        #region Creation Area
        void CreationArea()
        {


            if (!RadarObject) EditorGUILayout.HelpBox("There is no Radar selected , you will need to add one or select an existing radar. Thanks a bunch", MessageType.Info);







            var headerArea = ScreenRect.ToUpperLeft(0, 40, 0, 20);

            var emailaddres = "daimangou@gmail.com";
            var contactButtonArea = headerArea.PlaceUnder(25, 25, 0, 2);
            GUI.DrawTexture(contactButtonArea, contactEmailIcon);
            EditorGUI.TextArea(contactButtonArea.PlaceToRight(200, 20), emailaddres);
            var tutorialsButtonArea = contactButtonArea.PlaceUnder(0, 0, 0, 2);

            if (GUI.Button(tutorialsButtonArea, tutorialIcon, EditorStyles.label))
                Application.OpenURL("https://www.youtube.com/watch?v=k04iUM0JcCQ&list=PLn9dlpX8Ym8LIq7SpXoIU13AWiQex3i16");

            GUI.Label(tutorialsButtonArea.PlaceToRight(200, 20), "Tutorials");

            var MoreToolsButtonArea = tutorialsButtonArea.PlaceUnder(0, 0, 0, 4);
            if (GUI.Button(MoreToolsButtonArea, DaiMangouLogo, EditorStyles.label))
                Application.OpenURL("https://assetstore.unity.com/publishers/12333");
            GUI.Label(MoreToolsButtonArea.PlaceToRight(200, 0, 10), "Get more Awesome tools");


            var getUiTargetTrackerButton = MoreToolsButtonArea.PlaceUnder(0, 0, 0, 4);
            if (GUI.Button(getUiTargetTrackerButton, UITargetTrackerIcon, EditorStyles.label))
                Application.OpenURL("https://assetstore.unity.com/packages/tools/integration/ui-target-tracker-for-radar-builders-117360");
            GUI.Label(getUiTargetTrackerButton.PlaceToRight(250, 0, 10), "Get Free UI Target Tracker Extension");
            cursorchange();

            var _3DRadarSelectButton = ScreenRect.ToLowerLeft(150, 15);
            if (GUI.Button(_3DRadarSelectButton, "Create 3D System"))
                Create3DRadar();



        }
        #endregion

        #region IMGUI Interfce
        public void OnGUI()
        {
            ScreenRect = new Rect(0, 0, position.width, position.height);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Designs", EditorStyles.toolbarButton, new GUILayoutOption[] { GUILayout.Width(100) }))
            {
                Tab_Selection = 0;
            }
            if (GUILayout.Button("Blips", EditorStyles.toolbarButton, new GUILayoutOption[] { GUILayout.Width(100) }))
            {
                Tab_Selection = 1;
            }
            if (GUILayout.Button("Create", EditorStyles.toolbarButton, new GUILayoutOption[] { GUILayout.Width(100) }))
            {
                Tab_Selection = 2;

            }

            if (Tab_Selection == 1)
            {
                if (RadarObject)
                {

                    if (RadarObject.GetComponent<_3DRadar>() != null)
                    {
                        Current_Selected3D_Radar = RadarObject.GetComponent<_3DRadar>();
                        if (!Current_Selected3D_Radar) return;

                        Current_Selected3D_Radar.RadarDesign.TrackYPosition = GUILayout.Toggle(Current_Selected3D_Radar.RadarDesign.TrackYPosition, "Track Y Position", EditorStyles.toolbarButton, new GUILayoutOption[] { GUILayout.Width(100) });
                    }
                }

            }


            GUILayout.Box(" ", EditorStyles.toolbarButton, GUILayout.MaxWidth(ScreenRect.width));
            if (RadarObject)
            {
                if (RadarObject && RadarObject.GetComponent<_3DRadar>() != null)
                {

                    Current_Selected3D_Radar = RadarObject.GetComponent<_3DRadar>();

                    if (Current_Selected3D_Radar.RadarDesign == null) return;

                    Current_Selected3D_Radar.RadarDesign._3DSystemsWithScreenSpaceFunction = GUILayout.Toggle(Current_Selected3D_Radar.RadarDesign._3DSystemsWithScreenSpaceFunction, "Screen Space", EditorStyles.toolbarButton, new GUILayoutOption[] { GUILayout.Width(100) });

                    Current_Selected3D_Radar.RadarDesign._3DSystemsWithMinimapFunction = GUILayout.Toggle(Current_Selected3D_Radar.RadarDesign._3DSystemsWithMinimapFunction, "Minimap", EditorStyles.toolbarButton, new GUILayoutOption[] { GUILayout.Width(100) });

                }
            }

            GUILayout.EndHorizontal();

            #region Undo

            if (Current_Selected3D_Radar != null)
            {
                Undo.RecordObject(Current_Selected3D_Radar, "valb");
                EditorUtility.SetDirty(Current_Selected3D_Radar);
            }

            if (Event.current.type == EventType.ValidateCommand)
            {
                switch (Event.current.commandName)
                {
                    case "UndoRedoPerformed":

                        break;
                }
            }
            #endregion

            #region Call CreationArea
            if (RadarObject == null)
            {

                CreationArea();
            }
            #endregion

            #region 3D UI

            if (RadarObject && RadarObject.GetComponent<_3DRadar>() != null)
            {

                Current_Selected3D_Radar = RadarObject.GetComponent<_3DRadar>();



                switch (Tab_Selection)
                {
                    #region Radar Design and Settings
                    case 0:



                        DesignsScrollPosition = GUILayout.BeginScrollView(DesignsScrollPosition, false, false);
                        EditorGUILayout.Space();


                        #region Camera Section
                        if (Current_Selected3D_Radar.RadarDesign._3DSystemsWithScreenSpaceFunction)
                        {
                            HelpMessage("Here you set up your rendering camera and main camera");

                            Current_Selected3D_Radar.RadarDesign.ShowRenderCameraSettings = EditorGUILayout.Foldout(Current_Selected3D_Radar.RadarDesign.ShowRenderCameraSettings, "Camera Settings");

                            if (Current_Selected3D_Radar.RadarDesign.ShowRenderCameraSettings)
                            {
                                HelpMessage("This is the camers in front of which the radar will be displayed");

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Manual Camera Setup");
                                Current_Selected3D_Radar.RadarDesign.ManualCameraSetup = GUILayout.Toggle(Current_Selected3D_Radar.RadarDesign.ManualCameraSetup, "", GUILayout.Width(220));
                                GUILayout.EndHorizontal();

                                if (Current_Selected3D_Radar.RadarDesign.ManualCameraSetup)
                                {
                                    HelpMessage("you can drag and drop of assign a camera here via script");
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Camera");
                                    Current_Selected3D_Radar.RadarDesign.camera = (Camera)EditorGUILayout.ObjectField("", Current_Selected3D_Radar.RadarDesign.camera, typeof(Camera), true, GUILayout.Width(220));
                                    GUILayout.EndHorizontal();
                                }
                                else
                                {
                                    HelpMessage("Will always find and set the scenes Main Camera as the efault Camera  for the radar");

                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Always use main camera");
                                    Current_Selected3D_Radar.RadarDesign.UseMainCamera = GUILayout.Toggle(Current_Selected3D_Radar.RadarDesign.UseMainCamera, "", GUILayout.Width(220));
                                    GUILayout.EndHorizontal();

                                    HelpMessage("Finds a camera with the selected tag and uses it as the radars camera");

                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Use camera with tag");
                                    Current_Selected3D_Radar.RadarDesign.CameraTag = EditorGUILayout.TagField("", Current_Selected3D_Radar.RadarDesign.CameraTag, GUILayout.Width(220));
                                    GUILayout.EndHorizontal();
                                }

                                HelpMessage("YOU MUST HAVE A RENDERING CAMERA");
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Rendering Camera");
                                Current_Selected3D_Radar.RadarDesign.renderingCamera = (Camera)EditorGUILayout.ObjectField("", Current_Selected3D_Radar.RadarDesign.renderingCamera, typeof(Camera), true, GUILayout.Width(220));
                                GUILayout.EndHorizontal();
                            }

                            Separator();

                            EditorGUILayout.Space();
                        }

                        #endregion

                        #region Minimap Settings
                        if (Current_Selected3D_Radar.RadarDesign._3DSystemsWithMinimapFunction)
                        {
                            GUILayout.BeginHorizontal();
                            Current_Selected3D_Radar.RadarDesign.ShowMinimapSettings = EditorGUILayout.Foldout(Current_Selected3D_Radar.RadarDesign.ShowMinimapSettings, "Minimap Settings");
                            GUILayout.Space(300);
                            GUILayout.EndHorizontal();
                            if (Current_Selected3D_Radar.RadarDesign.ShowMinimapSettings)
                            {
                                if (Current_Selected3D_Radar.RadarDesign._3DSystemsWithMinimapFunction)
                                {

                                    HelpMessage("When set to Realtime, the minimap texture will be drawn from the curent scene . If static then a your predesigned map will be used");
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Minimap Type");
                                    Current_Selected3D_Radar.minimapModule.mapType = (MapType)EditorGUILayout.EnumPopup(Current_Selected3D_Radar.minimapModule.mapType, GUILayout.Width(220));
                                    GUILayout.EndHorizontal();

                                    if (Current_Selected3D_Radar.minimapModule.mapType != MapType.Realtime)
                                    {
                                        HelpMessage("The static texture2D mage that will be your map");
                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Minimap Texture");
                                        Current_Selected3D_Radar.minimapModule.MapTexture = (Sprite)EditorGUILayout.ObjectField(Current_Selected3D_Radar.minimapModule.MapTexture, typeof(Sprite), false, GUILayout.Width(220));
                                        GUILayout.EndHorizontal();

                                        HelpMessage("Sets the ratio which the radars internal system will use to ensue consistncy in your minimap");
                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Calibrate Minimap");
                                        Current_Selected3D_Radar.minimapModule.calibrate = EditorGUILayout.Toggle("", Current_Selected3D_Radar.minimapModule.calibrate, GUILayout.Width(220));
                                        GUILayout.EndHorizontal();

                                        HelpMessage("The order of the map sprite in the layer");
                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Order In layer");
                                        Current_Selected3D_Radar.minimapModule.OrderInLayer = EditorGUILayout.IntField(Current_Selected3D_Radar.minimapModule.OrderInLayer, GUILayout.Width(220));
                                        GUILayout.EndHorizontal();
                                    }
                                    else
                                    {
                                        HelpMessage("The render texture which will be used to pass the data from Realtime Minimap Camera to the Map");
                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Render Texture");
                                        Current_Selected3D_Radar.minimapModule.renderTexture = (RenderTexture)EditorGUILayout.ObjectField(Current_Selected3D_Radar.minimapModule.renderTexture, typeof(RenderTexture), true, GUILayout.Width(220));
                                        GUILayout.EndHorizontal();

                                        if (!Current_Selected3D_Radar.minimapModule.RealtimeMinimapCamera)
                                        {
                                            if (GUILayout.Button("Create Minimap Camera", GUILayout.Width(220)))
                                            {
                                                Current_Selected3D_Radar.minimapModule.RealtimeMinimapCamera = CreateRealtimeMinimapCamera(typeof(_3DRadar));

                                            }

                                        }
                                        else
                                        {
                                            HelpMessage("Your minimap camera ");
                                            GUILayout.BeginHorizontal();
                                            GUILayout.Label("Minimap Camera");
                                            Current_Selected3D_Radar.minimapModule.RealtimeMinimapCamera = (Camera)EditorGUILayout.ObjectField(Current_Selected3D_Radar.minimapModule.RealtimeMinimapCamera, typeof(Camera), true, GUILayout.Width(220));
                                            GUILayout.EndHorizontal();
                                        }
                                        HelpMessage("The y position of your Realtime Minimap Camera");
                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Camera Height");
                                        Current_Selected3D_Radar.minimapModule.CameraHeight = EditorGUILayout.FloatField(Current_Selected3D_Radar.minimapModule.CameraHeight, GUILayout.Width(220));
                                        GUILayout.EndHorizontal();
                                    }



                                    HelpMessage("the Material to be place on the Minimap, this must be a material must be able to be masked a,d its shader must allow for Texture images");
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Map Material");
                                    Current_Selected3D_Radar.minimapModule.MapMaterial = (Material)EditorGUILayout.ObjectField(Current_Selected3D_Radar.minimapModule.MapMaterial, typeof(Material), false, GUILayout.Width(220));
                                    GUILayout.EndHorizontal();

                                    HelpMessage("The material that will Mask the Map  in a circle");
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Mask Material");
                                    Current_Selected3D_Radar.minimapModule.MaskMaterial = (Material)EditorGUILayout.ObjectField(Current_Selected3D_Radar.minimapModule.MaskMaterial, typeof(Material), false, GUILayout.Width(220));
                                    GUILayout.EndHorizontal();

                                    HelpMessage("Places the map components on a specific layer at runtime");
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("On Layer"); Current_Selected3D_Radar.minimapModule.layer = EditorGUILayout.LayerField(Current_Selected3D_Radar.minimapModule.layer, GUILayout.Width(220));
                                    GUILayout.EndHorizontal();


                                    if (Current_Selected3D_Radar.minimapModule.calibrate && Current_Selected3D_Radar.minimapModule.mapType == MapType.Static)
                                    {
                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Map Scale");
                                        Current_Selected3D_Radar.minimapModule.MapScale = EditorGUILayout.FloatField("", Current_Selected3D_Radar.minimapModule.MapScale, GUILayout.Width(220));
                                        GUILayout.EndHorizontal();
                                    }

                                }

                            }
                            Separator();
                            EditorGUILayout.Space();
                        }
                        #endregion

                        #region Scale Settings
                        HelpMessage("The scale setting of your radar");
                        Current_Selected3D_Radar.RadarDesign.ShowScaleSettings = EditorGUILayout.Foldout(Current_Selected3D_Radar.RadarDesign.ShowScaleSettings, "Scale");

                        if (Current_Selected3D_Radar.RadarDesign.ShowScaleSettings)
                        {
                            HelpMessage("This will override the Radar Diameter value to make the radar be set to the default scale x,y,z in the inspector");

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Use Local Scale");
                            Current_Selected3D_Radar.RadarDesign.UseLocalScale = GUILayout.Toggle(Current_Selected3D_Radar.RadarDesign.UseLocalScale, "", GUILayout.Width(220));
                            GUILayout.EndHorizontal();

                            if (!Current_Selected3D_Radar.RadarDesign.UseLocalScale)
                            {
                                HelpMessage("Radar Diameter is the diameter of the radar");

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Radar Diameter");
                                GUILayout.Space(110);
                                Current_Selected3D_Radar.RadarDesign.RadarDiameter = EditorGUILayout.FloatField(" ", Mathf.Clamp(Current_Selected3D_Radar.RadarDesign.RadarDiameter, 0, Mathf.Infinity), GUILayout.MaxWidth(ScreenRect.width));
                                GUILayout.EndHorizontal();
                            }

                            HelpMessage("Anything outside of the tracking bounds will not be seen");

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Tracking Bounds");
                            GUILayout.Space(106);
                            Current_Selected3D_Radar.RadarDesign.TrackingBounds = EditorGUILayout.FloatField(" ", Mathf.Clamp(Current_Selected3D_Radar.RadarDesign.TrackingBounds, 0, Mathf.Infinity), GUILayout.MaxWidth(ScreenRect.width));
                            GUILayout.EndHorizontal();






                            HelpMessage("When selected , the scale of the radar once greater than or less than 1 ; will be ignored");

                            if (Current_Selected3D_Radar.RadarDesign._3DSystemsWithScreenSpaceFunction)
                            {
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Ignore Diameter Scale");
                                Current_Selected3D_Radar.RadarDesign.IgnoreDiameterScale = GUILayout.Toggle(Current_Selected3D_Radar.RadarDesign.IgnoreDiameterScale, "", GUILayout.Width(220));
                                GUILayout.EndHorizontal();
                            }

                            HelpMessage("Scene Scale is the zoom of the radar or how much units of space the radar can 'read'");

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Scene Scale");
                            GUILayout.Space(132);
                            Current_Selected3D_Radar.RadarDesign.SceneScale = EditorGUILayout.FloatField(" ", Mathf.Clamp(Current_Selected3D_Radar.RadarDesign.SceneScale, 1, Mathf.Infinity), GUILayout.MaxWidth(ScreenRect.width));
                            GUILayout.EndHorizontal();

                            HelpMessage("Anything inside this area will be culled(not seen)");

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Inner Culling Zone");
                            GUILayout.Space(95);
                            Current_Selected3D_Radar.RadarDesign.InnerCullingZone = EditorGUILayout.FloatField(" ", Mathf.Clamp(Current_Selected3D_Radar.RadarDesign.InnerCullingZone, 0, Current_Selected3D_Radar.RadarDesign.TrackingBounds), GUILayout.MaxWidth(ScreenRect.width));
                            GUILayout.EndHorizontal();
                        }
                        Separator();

                        EditorGUILayout.Space();
                        #endregion


                        #region Positioning  Section
                        if (Current_Selected3D_Radar.RadarDesign._3DSystemsWithScreenSpaceFunction)
                        {
                            HelpMessage("Position setting of your radar in screen space");

                            Current_Selected3D_Radar.RadarDesign.ShowPositioningSettings = EditorGUILayout.Foldout(Current_Selected3D_Radar.RadarDesign.ShowPositioningSettings, "Position");
                            if (Current_Selected3D_Radar.RadarDesign.ShowPositioningSettings)
                            {

                                #region Positioning settings
                                HelpMessage("Choose between 9 point snapping or Manual positioning");

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Radar Positioning");
                                Current_Selected3D_Radar.RadarDesign.radarPositioning = (RadarPositioning)EditorGUILayout.EnumPopup("", Current_Selected3D_Radar.RadarDesign.radarPositioning, GUILayout.Width(220));
                                GUILayout.EndHorizontal();

                                switch (Current_Selected3D_Radar.RadarDesign.radarPositioning)
                                {
                                    case RadarPositioning.Manual:
                                        HelpMessage("Position the radar manually on the x and y axis");

                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("X and Y position");
                                        Current_Selected3D_Radar.RadarDesign.RadarRect.position = EditorGUILayout.Vector2Field("", Current_Selected3D_Radar.RadarDesign.RadarRect.position, GUILayout.Width(220));
                                        GUILayout.EndHorizontal();

                                        break;
                                    case RadarPositioning.Snap:
                                        HelpMessage("Use our 9 point snapping  to snap the position of your radar to 9 dirent points on your screen");

                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Snap to the");
                                        Current_Selected3D_Radar.RadarDesign.snapPosition = (SnapPosition)EditorGUILayout.EnumPopup("", Current_Selected3D_Radar.RadarDesign.snapPosition, GUILayout.Width(220));
                                        GUILayout.EndHorizontal();

                                        HelpMessage("In order to correct for camera distortion of scene object closer to the corder of the screen , you can turn on this function to make snaps to the Top Left,Right, Bottom Left, Right, Middle Left, Right not distort");
                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Use Orthographic ForSide Snaps");
                                        Current_Selected3D_Radar.RadarDesign.UseOrthographicForSideSnaps = EditorGUILayout.Toggle("", Current_Selected3D_Radar.RadarDesign.UseOrthographicForSideSnaps, GUILayout.Width(220));
                                        GUILayout.EndHorizontal();
                                        break;

                                }
                                #endregion

                                #region FrontIs Settings
                                HelpMessage("Determine what the front facing direction of the radar is ");
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Front is");
                                Current_Selected3D_Radar.RadarDesign.frontIs = (FrontIs)EditorGUILayout.EnumPopup("", Current_Selected3D_Radar.RadarDesign.frontIs, GUILayout.Width(220));

                                GUILayout.EndHorizontal();
                                #endregion

                                HelpMessage("Pading on the X axis");
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("X Pading");
                                Current_Selected3D_Radar.RadarDesign.xPadding = EditorGUILayout.FloatField("", Current_Selected3D_Radar.RadarDesign.xPadding, GUILayout.Width(220));
                                GUILayout.EndHorizontal();

                                HelpMessage("Pading on the Y axis");
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Y Pading");
                                Current_Selected3D_Radar.RadarDesign.yPadding = EditorGUILayout.FloatField("", Current_Selected3D_Radar.RadarDesign.yPadding, GUILayout.Width(220));
                                GUILayout.EndHorizontal();



                            }
                            Separator();

                            EditorGUILayout.Space();
                        }
                        #endregion


                        #region Design Sprites

                        #region Setting and creating Rotation Targets
                        HelpMessage("You are able to target layers blips or parts of your radar and have them rotate in various ways");

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Rotation Targets");
                        Current_Selected3D_Radar.RadarDesign.RotationTargetsCount = EditorGUILayout.DelayedIntField("", Current_Selected3D_Radar.RadarDesign.RotationTargetsCount, GUILayout.Width(60));

                        if (Current_Selected3D_Radar.RadarDesign.temprottionTargtValue != Current_Selected3D_Radar.RadarDesign.RotationTargetsCount)
                        {
                            Current_Selected3D_Radar.RadarDesign.RotationTargets.Resize(Current_Selected3D_Radar.RadarDesign.RotationTargetsCount);
                            Current_Selected3D_Radar.RadarDesign.temprottionTargtValue = Current_Selected3D_Radar.RadarDesign.RotationTargetsCount;

                        }
                        GUILayout.EndHorizontal();

                        Separator();


                        #endregion

                        EditorGUILayout.Separator();
                        foreach (var rotationTarget in Current_Selected3D_Radar.RadarDesign.RotationTargets.Select((x, i) => new { value = x, index = i }))
                        {




                            try
                            {
                                GUILayout.BeginHorizontal();
                                string Target = (!rotationTarget.value.TargetedObject) ? (rotationTarget.value.target == TargetBlip.InstancedBlip) ? rotationTarget.value.InstancedTargetBlipname : "nothing " : rotationTarget.value.TargetedObject.name;
                                rotationTarget.value.ShowDesignSetings = EditorGUILayout.Foldout(rotationTarget.value.ShowDesignSetings, "Rotate " + Target);

                                GUILayout.FlexibleSpace();

                                if (GUILayout.Button(upArrow, "Label", GUILayout.Width(15)))
                                {
                                    if (rotationTarget.index != 0)
                                    {
                                        var temptarget = rotationTarget.value;
                                        Current_Selected3D_Radar.RadarDesign.RotationTargets[rotationTarget.index] = Current_Selected3D_Radar.RadarDesign.RotationTargets[rotationTarget.index - 1];
                                        Current_Selected3D_Radar.RadarDesign.RotationTargets[rotationTarget.index - 1] = temptarget;
                                    }
                                }
                                GUILayout.Space(5);

                                if (GUILayout.Button(downArrow, "Label", GUILayout.Width(15)))
                                {
                                    if (rotationTarget.index != Current_Selected3D_Radar.RadarDesign.RotationTargets.Count - 1)
                                    {
                                        var temptarget = rotationTarget.value;
                                        Current_Selected3D_Radar.RadarDesign.RotationTargets[rotationTarget.index] = Current_Selected3D_Radar.RadarDesign.RotationTargets[rotationTarget.index + 1];
                                        Current_Selected3D_Radar.RadarDesign.RotationTargets[rotationTarget.index + 1] = temptarget;
                                    }
                                }
                                GUILayout.Space(5);

                                if (GUILayout.Button(rotationTarget.value.UseRotationTarget ? On : Off, "Label", GUILayout.Width(20)))
                                    rotationTarget.value.UseRotationTarget = !rotationTarget.value.UseRotationTarget;

                                GUILayout.Space(5);

                                if (GUILayout.Button(deleteConditionIcon, "Label", GUILayout.Width(15)))
                                {
                                    Current_Selected3D_Radar.RadarDesign.RotationTargets.RemoveAt(rotationTarget.index);
                                    Current_Selected3D_Radar.RadarDesign.RotationTargetsCount -= 1;
                                    return;
                                }

                                GUILayout.EndHorizontal();

                                EditorGUI.BeginDisabledGroup(!rotationTarget.value.UseRotationTarget);

                                GUILayout.BeginHorizontal();
                                GUILayout.Space(20);
                                GUILayout.BeginVertical();

                                if (rotationTarget.value.ShowDesignSetings)
                                {

                                    #region Draw Radar Design Interface
                                    HelpMessage("Here you will set the objet(blip or otherwise) which you wish to have rotate ");

                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Rotate");
                                    rotationTarget.value.target = (TargetBlip)EditorGUILayout.EnumPopup(rotationTarget.value.target, GUILayout.Width(95));
                                    switch (rotationTarget.value.target)
                                    {

                                        case TargetBlip.ThisObject:

                                            rotationTarget.value.TargetedObject = (GameObject)EditorGUILayout.ObjectField(rotationTarget.value.TargetedObject, typeof(GameObject), true);

                                            break;
                                        case TargetBlip.InstancedBlip:

                                            rotationTarget.value.InstancedTargetBlipname = EditorGUILayout.TextField(rotationTarget.value.InstancedTargetBlipname);
                                            break;

                                    }



                                    rotationTarget.value.rotations = (Rotations)EditorGUILayout.EnumPopup(rotationTarget.value.rotations, GUILayout.Width(95));
                                    GUILayout.Label("to");

                                    rotationTarget.value.ObjectToTrack = (TargetObject)EditorGUILayout.EnumPopup(rotationTarget.value.ObjectToTrack, GUILayout.Width(95));

                                    switch (rotationTarget.value.ObjectToTrack)
                                    {
                                        case TargetObject.FindObject:
                                            rotationTarget.value.FindingName = EditorGUILayout.TextField(rotationTarget.value.FindingName);
                                            break;
                                        case TargetObject.ObjectWithTag:
                                            rotationTarget.value.tag = EditorGUILayout.TextField(rotationTarget.value.tag);
                                            break;
                                        case TargetObject.ThisObject:
                                            rotationTarget.value.Target = (GameObject)EditorGUILayout.ObjectField(rotationTarget.value.Target, typeof(GameObject), true);

                                            break;
                                        case TargetObject.InstancedBlip:
                                            rotationTarget.value.InstancedObjectToTrackBlipName = EditorGUILayout.TextField(rotationTarget.value.InstancedObjectToTrackBlipName);
                                            break;

                                    }
                                    GUILayout.EndHorizontal();

                                    HelpMessage("This value should usually  set to 90 for sprite objects");

                                    GUILayout.Label("Added Rotation (usually, x is set to 90 for sprites)");
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("X");
                                    rotationTarget.value.AddedXRotation = EditorGUILayout.FloatField(Mathf.Clamp(rotationTarget.value.AddedXRotation, 0, Mathf.Infinity));
                                    GUILayout.Label("Y");
                                    rotationTarget.value.AddedYRotation = EditorGUILayout.FloatField(Mathf.Clamp(rotationTarget.value.AddedYRotation, 0, Mathf.Infinity));
                                    GUILayout.Label("Z");
                                    rotationTarget.value.AddedZRotation = EditorGUILayout.FloatField(Mathf.Clamp(rotationTarget.value.AddedZRotation, 0, Mathf.Infinity));
                                    GUILayout.EndHorizontal();


                                    HelpMessage("Freze rotation on any axis");

                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Freeze");
                                    rotationTarget.value.FreezeX = GUILayout.Toggle(rotationTarget.value.FreezeX, "X");
                                    rotationTarget.value.FreezeY = GUILayout.Toggle(rotationTarget.value.FreezeY, "Y");
                                    rotationTarget.value.FreezeZ = GUILayout.Toggle(rotationTarget.value.FreezeZ, "Z");
                                    GUILayout.EndHorizontal();


                                    HelpMessage("Replace the rotation on any axis of your target object with that of the targeted object");

                                    GUILayout.Label("Retargeted Rotation");
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("X");
                                    rotationTarget.value.retargetedXRotation = (RetargetX)EditorGUILayout.EnumPopup(rotationTarget.value.retargetedXRotation);
                                    GUILayout.Label("Y");
                                    rotationTarget.value.retargetedYRotation = (RetargetY)EditorGUILayout.EnumPopup(rotationTarget.value.retargetedYRotation);
                                    GUILayout.Label("Z");
                                    rotationTarget.value.retargetedZRotation = (RetargetZ)EditorGUILayout.EnumPopup(rotationTarget.value.retargetedZRotation);
                                    GUILayout.EndHorizontal();


                                    HelpMessage("By what percent will rotation be reduced");

                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("% Damping");
                                    rotationTarget.value.RotationDamping = EditorGUILayout.FloatField(Mathf.Clamp(rotationTarget.value.RotationDamping, 0, 100), GUILayout.Width(220));
                                    GUILayout.EndHorizontal();



                                    #endregion
                                }
                                GUILayout.EndVertical();
                                GUILayout.EndHorizontal();
                                EditorGUI.EndDisabledGroup();
                                Separator();
                                EditorGUILayout.Separator();

                            }
                            catch
                            {

                            }


                        }
                        #endregion


                        HelpMessage("Turn on 'Visualize'. This show you your radar settings.");
                        Current_Selected3D_Radar.RadarDesign.Visualize = GUILayout.Toggle(Current_Selected3D_Radar.RadarDesign.Visualize, "Visualize");


                        ShowHelpMessages = GUILayout.Toggle(ShowHelpMessages, "Show Help Messages");

                        HelpMessage("These help messages will help you to set up your radar");

                        GUILayout.EndScrollView();





                        break;
                    #endregion

                    #region Blip Design UI
                    case 1:

                        RadarBlipScrollPosition = EditorGUILayout.BeginScrollView(RadarBlipScrollPosition, false, false);

                        #region Setting Blips

                        HelpMessage("This blip ; if set to active will always appear at the center of your radar.");
                        Current_Selected3D_Radar._RadarCenterObject3D.State = Current_Selected3D_Radar._RadarCenterObject3D.IsActive ? Current_Selected3D_Radar._RadarCenterObject3D.Tag + " is " + " Active" : Current_Selected3D_Radar._RadarCenterObject3D.Tag + " is " + " Inactive";

                        GUILayout.BeginHorizontal();
                        Current_Selected3D_Radar._RadarCenterObject3D.ShowBLipSettings = EditorGUILayout.Foldout(Current_Selected3D_Radar._RadarCenterObject3D.ShowBLipSettings, Current_Selected3D_Radar._RadarCenterObject3D.State);

                        if (Current_Selected3D_Radar._RadarCenterObject3D.BlipSprite)
                        {
                            GUILayout.FlexibleSpace();
                            GUILayout.Box(Current_Selected3D_Radar._RadarCenterObject3D.BlipSprite.texture, "Label",
                                GUILayout.Height(20), GUILayout.Width(20));
                            GUILayout.Space(70);
                            GUILayout.FlexibleSpace();
                        }

                        if (GUILayout.Button((Current_Selected3D_Radar._RadarCenterObject3D.IsActive) ? On : Off, "Label", GUILayout.Width(20))) Current_Selected3D_Radar._RadarCenterObject3D.IsActive = !Current_Selected3D_Radar._RadarCenterObject3D.IsActive;
                        GUILayout.EndHorizontal();
                        cursorchange();


                        if (Current_Selected3D_Radar._RadarCenterObject3D.ShowBLipSettings)
                        {
                            Separator();
                            EditorGUILayout.Space();

                            #region sprite
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(20);
                            GUILayout.BeginVertical();

                            HelpMessage("If using sprites as blips; set the Sprite and Material here");
                            Current_Selected3D_Radar._RadarCenterObject3D.ShowSpriteBlipSettings = EditorGUILayout.Foldout(Current_Selected3D_Radar._RadarCenterObject3D.ShowSpriteBlipSettings, Current_Selected3D_Radar._RadarCenterObject3D.Tag + " Sprite");

                            if (Current_Selected3D_Radar._RadarCenterObject3D.ShowSpriteBlipSettings)
                            {
                                #region Sprite Blip Design
                                GUILayout.BeginHorizontal();

                                GUILayout.BeginVertical();

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Sprite");
                                Current_Selected3D_Radar._RadarCenterObject3D.BlipSprite = (Sprite)EditorGUILayout.ObjectField(Current_Selected3D_Radar._RadarCenterObject3D.BlipSprite, typeof(Sprite), false, GUILayout.Width((Current_Selected3D_Radar._RadarCenterObject3D.BlipSprite) ? 170 : 220));
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Material");
                                Current_Selected3D_Radar._RadarCenterObject3D.BlipMaterial = (Material)EditorGUILayout.ObjectField("", Current_Selected3D_Radar._RadarCenterObject3D.BlipMaterial, typeof(Material), true, GUILayout.Width((Current_Selected3D_Radar._RadarCenterObject3D.BlipSprite) ? 170 : 220));
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Colour");
                                Current_Selected3D_Radar._RadarCenterObject3D.colour = EditorGUILayout.ColorField("", Current_Selected3D_Radar._RadarCenterObject3D.colour, GUILayout.Width((Current_Selected3D_Radar._RadarCenterObject3D.BlipSprite) ? 170 : 220));
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Order In Layer");
                                Current_Selected3D_Radar._RadarCenterObject3D.OrderInLayer = EditorGUILayout.IntField(Current_Selected3D_Radar._RadarCenterObject3D.OrderInLayer, GUILayout.Width(220));
                                GUILayout.EndHorizontal();

                                GUILayout.EndVertical();

                                try
                                {
                                    GUILayout.Box(Current_Selected3D_Radar._RadarCenterObject3D.BlipSprite.texture, GUILayout.Height(50), GUILayout.Width(50));
                                }
                                catch { }

                                GUILayout.EndHorizontal();
                                #endregion
                            }
                            GUILayout.EndVertical();
                            GUILayout.EndHorizontal();

                            Separator();

                            EditorGUILayout.Space();
                            #endregion

                            #region Mesh
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(20);
                            GUILayout.BeginVertical();
                            HelpMessage("If using mesh as blips; set the mesh and materials here");
                            Current_Selected3D_Radar._RadarCenterObject3D.ShowMeshBlipSettings = EditorGUILayout.Foldout(Current_Selected3D_Radar._RadarCenterObject3D.ShowMeshBlipSettings, Current_Selected3D_Radar._RadarCenterObject3D.Tag + " Mesh");
                            if (Current_Selected3D_Radar._RadarCenterObject3D.ShowMeshBlipSettings)
                            {
                                #region MeshBlipDesign

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Mesh");
                                Current_Selected3D_Radar._RadarCenterObject3D.mesh = (Mesh)EditorGUILayout.ObjectField(Current_Selected3D_Radar._RadarCenterObject3D.mesh, typeof(Mesh), false, GUILayout.Width(220));
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Material Count");
                                Current_Selected3D_Radar._RadarCenterObject3D.MatCount = Mathf.Clamp(EditorGUILayout.IntField("", Current_Selected3D_Radar._RadarCenterObject3D.MatCount, GUILayout.MaxWidth(200), GUILayout.Width(220)), 0, 8000);
                                GUILayout.EndHorizontal();

                                if (Event.current.keyCode == KeyCode.Return)
                                {
                                    Array.Resize(ref Current_Selected3D_Radar._RadarCenterObject3D.MeshMaterials, Current_Selected3D_Radar._RadarCenterObject3D.MatCount);

                                }


                                GUILayout.BeginVertical();
                                for (int i = 0; i < Current_Selected3D_Radar._RadarCenterObject3D.MeshMaterials.Count(); i++)
                                {
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Material " + i);
                                    try
                                    {
                                        Current_Selected3D_Radar._RadarCenterObject3D.MeshMaterials[i] = (Material)EditorGUILayout.ObjectField(Current_Selected3D_Radar._RadarCenterObject3D.MeshMaterials[i], typeof(Material), false, GUILayout.Width(220));
                                    }
                                    catch { }
                                    GUILayout.EndHorizontal();
                                }
                                GUILayout.EndVertical();


                                #endregion
                            }
                            GUILayout.EndVertical();
                            GUILayout.EndHorizontal();

                            Separator();

                            EditorGUILayout.Space();
                            #endregion

                            #region Prefab

                            GUILayout.BeginHorizontal();
                            GUILayout.Space(20);
                            GUILayout.BeginVertical();
                            HelpMessage("If using prefab as blips; se the prefab here");
                            Current_Selected3D_Radar._RadarCenterObject3D.ShowPrefabBlipSettings = EditorGUILayout.Foldout(Current_Selected3D_Radar._RadarCenterObject3D.ShowPrefabBlipSettings, Current_Selected3D_Radar._RadarCenterObject3D.Tag + " Prefab");
                            if (Current_Selected3D_Radar._RadarCenterObject3D.ShowPrefabBlipSettings)
                            {
                                #region PrefabBLipDesign
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Prefab");
                                Current_Selected3D_Radar._RadarCenterObject3D.prefab = (Transform)EditorGUILayout.ObjectField("", Current_Selected3D_Radar._RadarCenterObject3D.prefab, typeof(Transform), true, GUILayout.Width(220));
                                GUILayout.EndHorizontal();

                                #endregion
                            }
                            GUILayout.EndVertical();
                            GUILayout.EndHorizontal();

                            Separator();

                            EditorGUILayout.Space();
                            #endregion

                            #region Tracking Line
                            HelpMessage("Tracking lines allows you to visualize the y distance of your blips from the radar");
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(20);
                            GUILayout.BeginVertical();


                            string trackingLineState = (Current_Selected3D_Radar._RadarCenterObject3D.UseTrackingLine) ? "Active" : "Inactive";

                            GUILayout.BeginHorizontal();
                            Current_Selected3D_Radar._RadarCenterObject3D.ShowTrackingLineSettings = EditorGUILayout.Foldout(Current_Selected3D_Radar._RadarCenterObject3D.ShowTrackingLineSettings, "Tracking Line is " + trackingLineState);
                            if (GUILayout.Button((Current_Selected3D_Radar._RadarCenterObject3D.UseTrackingLine) ? On : Off, "Label", GUILayout.Width(20))) Current_Selected3D_Radar._RadarCenterObject3D.UseTrackingLine = !Current_Selected3D_Radar._RadarCenterObject3D.UseTrackingLine;
                            GUILayout.EndHorizontal();
                            cursorchange();

                            if (Current_Selected3D_Radar._RadarCenterObject3D.ShowTrackingLineSettings)
                            {
                                HelpMessage("Set the Colour and Material here. You can set any material");
                                #region Tracking Line Design
                                GUILayout.BeginVertical();

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Material");
                                Current_Selected3D_Radar._RadarCenterObject3D.TrackingLineMaterial = (Material)EditorGUILayout.ObjectField("", Current_Selected3D_Radar._RadarCenterObject3D.TrackingLineMaterial, typeof(Material), true, GUILayout.Width(220));
                                GUILayout.EndHorizontal();


                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Start Colour");
                                Current_Selected3D_Radar._RadarCenterObject3D.TrackingLineStartColour = EditorGUILayout.ColorField("", Current_Selected3D_Radar._RadarCenterObject3D.TrackingLineStartColour, GUILayout.Width(220));
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("End Colour");
                                Current_Selected3D_Radar._RadarCenterObject3D.TrackingLineEndColour = EditorGUILayout.ColorField("", Current_Selected3D_Radar._RadarCenterObject3D.TrackingLineEndColour, GUILayout.Width(220));
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Scale");
                                GUILayout.Space(153);
                                Current_Selected3D_Radar._RadarCenterObject3D.TrackingLineDimention = EditorGUILayout.FloatField(" ", Current_Selected3D_Radar._RadarCenterObject3D.TrackingLineDimention, GUILayout.MaxWidth(ScreenRect.width));
                                GUILayout.EndHorizontal();


                                #region BaseTracker Design
                                HelpMessage("Base Trackers are sprites which appear that the base of the reacking line and are optional");
                                GUILayout.BeginHorizontal();
                                Current_Selected3D_Radar._RadarCenterObject3D.ShowBaseTrackerSettings = EditorGUILayout.Foldout(Current_Selected3D_Radar._RadarCenterObject3D.ShowBaseTrackerSettings, "Base Tracker settings");
                                if (GUILayout.Button((Current_Selected3D_Radar._RadarCenterObject3D.UseBaseTracker) ? On : Off, "Label", GUILayout.Width(20))) Current_Selected3D_Radar._RadarCenterObject3D.UseBaseTracker = !Current_Selected3D_Radar._RadarCenterObject3D.UseBaseTracker;
                                GUILayout.EndHorizontal();
                                cursorchange();
                                if (Current_Selected3D_Radar._RadarCenterObject3D.ShowBaseTrackerSettings)
                                {
                                    GUILayout.BeginHorizontal();
                                    GUILayout.BeginVertical();
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Base Tracker");
                                    Current_Selected3D_Radar._RadarCenterObject3D.BaseTrackerSprite = (Sprite)EditorGUILayout.ObjectField(Current_Selected3D_Radar._RadarCenterObject3D.BaseTrackerSprite, typeof(Sprite), false, GUILayout.Width(220));
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Material");
                                    Current_Selected3D_Radar._RadarCenterObject3D.BaseTrackerMaterial = (Material)EditorGUILayout.ObjectField("", Current_Selected3D_Radar._RadarCenterObject3D.BaseTrackerMaterial, typeof(Material), true, GUILayout.Width(220));
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Colour");
                                    Current_Selected3D_Radar._RadarCenterObject3D.BaseTrackerColour = EditorGUILayout.ColorField("", Current_Selected3D_Radar._RadarCenterObject3D.BaseTrackerColour, GUILayout.Width(220));
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Size");
                                    Current_Selected3D_Radar._RadarCenterObject3D.BaseTrackerSize = EditorGUILayout.FloatField("", Current_Selected3D_Radar._RadarCenterObject3D.BaseTrackerSize, GUILayout.Width(220));
                                    GUILayout.EndHorizontal();

                                    GUILayout.EndVertical();

                                    try
                                    {
                                        GUILayout.Box(Current_Selected3D_Radar._RadarCenterObject3D.BaseTrackerSprite.texture, GUILayout.Height(50), GUILayout.Width(50));
                                    }
                                    catch
                                    {
                                        // nothing
                                    }
                                    GUILayout.EndHorizontal();


                                }
                                #endregion

                                GUILayout.EndVertical();

                                #endregion
                            }
                            GUILayout.EndVertical();
                            GUILayout.EndHorizontal();
                            Separator();
                            EditorGUILayout.Separator();
                            #endregion

                            #region Additional Options

                            GUILayout.BeginHorizontal();
                            GUILayout.Space(20);
                            GUILayout.BeginVertical();

                            HelpMessage("Displaying additional options for your blip");
                            Current_Selected3D_Radar._RadarCenterObject3D.ShowAdditionalOptions = EditorGUILayout.Foldout(Current_Selected3D_Radar._RadarCenterObject3D.ShowAdditionalOptions, "AdditionalOptions");
                            if (Current_Selected3D_Radar._RadarCenterObject3D.ShowAdditionalOptions)
                            {
                                HelpMessage("When eabled all " + Current_Selected3D_Radar._RadarCenterObject3D.Tag + "blips will not disppear when at they pass the bounderies of the radar, but will remain at the edge and will be scaled based on its distance from the center object");

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Always Show " + Current_Selected3D_Radar._RadarCenterObject3D.Tag + " in radar");
                                Current_Selected3D_Radar._RadarCenterObject3D.AlwaysShowCenterObject = GUILayout.Toggle(Current_Selected3D_Radar._RadarCenterObject3D.AlwaysShowCenterObject, "", GUILayout.Width(220));
                                GUILayout.EndHorizontal();

                            }

                            GUILayout.EndVertical();
                            GUILayout.EndHorizontal();

                            Separator();

                            EditorGUILayout.Space();

                            #endregion

                            /* #region Optimization 

                             GUILayout.BeginHorizontal();
                             GUILayout.Space(20);
                             GUILayout.BeginVertical();

                             HelpMessage("Options for optimization the radar proceses");
                             GUILayout.BeginHorizontal();
                             Current_Selected3D_Radar._RadarCenterObject3D.ShowOptimizationSettings = EditorGUILayout.Foldout(Current_Selected3D_Radar._RadarCenterObject3D.ShowOptimizationSettings, "Optimization Options");
                             GUILayout.Box(Optimizeicon, "Label", GUILayout.Width(120), GUILayout.Height(20));
                             GUILayout.EndHorizontal();
                             if (Current_Selected3D_Radar._RadarCenterObject3D.ShowOptimizationSettings)
                             {


                                     HelpMessage("Requires that doCenterObjectInstanceObjectCheck be called whenever you want to manually search for a enter object");
                                     GUILayout.BeginHorizontal();
                                     GUILayout.Label("Require Instance Object Check");
                                     Current_Selected3D_Radar._RadarCenterObject3D.RequireInstanceObjectCheck = EditorGUILayout.Toggle(Current_Selected3D_Radar._RadarCenterObject3D.RequireInstanceObjectCheck, GUILayout.Width(220));
                                     GUILayout.EndHorizontal();




                             }

                             GUILayout.EndVertical();
                             GUILayout.EndHorizontal();

                             Separator();

                             EditorGUILayout.Space();

                             #endregion
                             */


                            #region Scale And Rotation Settings


                            GUILayout.BeginHorizontal();
                            GUILayout.Space(20);
                            GUILayout.BeginVertical();

                            Current_Selected3D_Radar._RadarCenterObject3D.ShowGeneralSettings = EditorGUILayout.Foldout(Current_Selected3D_Radar._RadarCenterObject3D.ShowGeneralSettings, "Rotation and Scale");
                            if (Current_Selected3D_Radar._RadarCenterObject3D.ShowGeneralSettings)
                            {
                                #region Blip Scale Settings
                                if (!Current_Selected3D_Radar._RadarCenterObject3D.CenterObjectCanScaleByDistance)
                                {
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Scale");
                                    GUILayout.Space(153);
                                    Current_Selected3D_Radar._RadarCenterObject3D.BlipSize = EditorGUILayout.FloatField(" ", Current_Selected3D_Radar._RadarCenterObject3D.BlipSize, GUILayout.MaxWidth(ScreenRect.width));
                                    GUILayout.EndHorizontal();

                                }

                                /*  HelpMessage("Your Center Blip will always shrink when moving to the edge of the radar if this is not checkd");
                                  GUILayout.BeginHorizontal();
                                  GUILayout.Label("Autoscale Only At Border");
                                  Current_Selected3D_Radar._RadarCenterObject3D.AutoScaleOnlyAtBorder = GUILayout.Toggle(Current_Selected3D_Radar._RadarCenterObject3D.AutoScaleOnlyAtBorder, "", GUILayout.Width(220));
                                  GUILayout.EndHorizontal();

                                  HelpMessage("this value determines , be how much the blip will scale over distance");
                                  GUILayout.BeginHorizontal();
                                  GUILayout.Label("AutoScale Factor");
                                  Current_Selected3D_Radar._RadarCenterObject3D.AutoScaleFactor = EditorGUILayout.FloatField("", Current_Selected3D_Radar._RadarCenterObject3D.AutoScaleFactor, GUILayout.Width(220));
                                  GUILayout.EndHorizontal();*/


                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Scale By Distance");
                                Current_Selected3D_Radar._RadarCenterObject3D.CenterObjectCanScaleByDistance = GUILayout.Toggle(Current_Selected3D_Radar._RadarCenterObject3D.CenterObjectCanScaleByDistance, "", GUILayout.Width(220));
                                GUILayout.EndHorizontal();

                                if (Current_Selected3D_Radar._RadarCenterObject3D.CenterObjectCanScaleByDistance)
                                {
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Min Scale");
                                    Current_Selected3D_Radar._RadarCenterObject3D.BlipMinSize = EditorGUILayout.FloatField("", Current_Selected3D_Radar._RadarCenterObject3D.BlipMinSize, GUILayout.Width(220));
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Max Scale");
                                    Current_Selected3D_Radar._RadarCenterObject3D.BlipMaxSize = EditorGUILayout.FloatField("", Current_Selected3D_Radar._RadarCenterObject3D.BlipMaxSize, GUILayout.Width(220));
                                    GUILayout.EndHorizontal();



                                }
                                #endregion


                                #region Blip Rotation Settings

                                HelpMessage("Use custom rotation allows yo uto set a static rotation for all of these blip types");
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Use Custom Rotation");
                                Current_Selected3D_Radar._RadarCenterObject3D.UseCustomRotation = GUILayout.Toggle(Current_Selected3D_Radar._RadarCenterObject3D.UseCustomRotation, "", GUILayout.Width(220));
                                GUILayout.EndHorizontal();

                                if (Current_Selected3D_Radar._RadarCenterObject3D.UseCustomRotation && !Current_Selected3D_Radar._RadarCenterObject3D.IsTrackRotation)
                                {
                                    HelpMessage("Set a X, Y and Z rotation for this blip type");

                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Rotation");
                                    GUILayout.Label("X");
                                    Current_Selected3D_Radar._RadarCenterObject3D.CustomXRotation = EditorGUILayout.FloatField(Current_Selected3D_Radar._RadarCenterObject3D.CustomXRotation);
                                    GUILayout.Label("Y");
                                    Current_Selected3D_Radar._RadarCenterObject3D.CustomYRotation = EditorGUILayout.FloatField(Current_Selected3D_Radar._RadarCenterObject3D.CustomYRotation);
                                    GUILayout.Label("Z");
                                    Current_Selected3D_Radar._RadarCenterObject3D.CustomZRotation = EditorGUILayout.FloatField(Current_Selected3D_Radar._RadarCenterObject3D.CustomZRotation);
                                    GUILayout.EndHorizontal();
                                }

                                HelpMessage("Track Rotation allows for the blip to rotate and match the rotation of the tracked object");
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Track Rotation");
                                Current_Selected3D_Radar._RadarCenterObject3D.IsTrackRotation = GUILayout.Toggle(Current_Selected3D_Radar._RadarCenterObject3D.IsTrackRotation, "", GUILayout.Width(220));
                                GUILayout.EndHorizontal();


                                if (Current_Selected3D_Radar._RadarCenterObject3D.IsTrackRotation && !Current_Selected3D_Radar._RadarCenterObject3D.UseCustomRotation)
                                {
                                    HelpMessage("Freeze rotation through the x,y, or z axis");

                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Freeze");
                                    Current_Selected3D_Radar._RadarCenterObject3D.lockX = GUILayout.Toggle(Current_Selected3D_Radar._RadarCenterObject3D.lockX, "X");
                                    Current_Selected3D_Radar._RadarCenterObject3D.lockY = GUILayout.Toggle(Current_Selected3D_Radar._RadarCenterObject3D.lockY, "Y");
                                    Current_Selected3D_Radar._RadarCenterObject3D.lockZ = GUILayout.Toggle(Current_Selected3D_Radar._RadarCenterObject3D.lockZ, "Z");
                                    GUILayout.EndHorizontal();
                                }
                                #endregion
                                if (Current_Selected3D_Radar._RadarCenterObject3D.IsTrackRotation && Current_Selected3D_Radar._RadarCenterObject3D.UseCustomRotation)
                                {
                                    EditorGUILayout.HelpBox("Do not use 'Track Rotation' and 'Custom Rotation at the same time'", MessageType.Warning);
                                }
                            }
                            GUILayout.EndVertical();
                            GUILayout.EndHorizontal();

                            Separator();

                            EditorGUILayout.Space();

                            #region Universal Settings
                            HelpMessage("With the current settings,If this blip is active, it will represent your gameobject with the tag " + Current_Selected3D_Radar._RadarCenterObject3D.Tag + " on the layer " + Current_Selected3D_Radar._RadarCenterObject3D.Layer + " and will be at scale " + Current_Selected3D_Radar._RadarCenterObject3D.BlipSize);

                            GUILayout.BeginHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Create blip");
                            Current_Selected3D_Radar._RadarCenterObject3D._CreateBlipAs = (CreateBlipAs)EditorGUILayout.EnumPopup(Current_Selected3D_Radar._RadarCenterObject3D._CreateBlipAs, GUILayout.Width(100));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("With Tag"); Current_Selected3D_Radar._RadarCenterObject3D.Tag = EditorGUILayout.TagField(Current_Selected3D_Radar._RadarCenterObject3D.Tag);
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("On Layer"); Current_Selected3D_Radar._RadarCenterObject3D.Layer = EditorGUILayout.LayerField(Current_Selected3D_Radar._RadarCenterObject3D.Layer);
                            GUILayout.EndHorizontal();

                            GUILayout.EndHorizontal();
                            #endregion


                        }
                        #endregion
                        Separator();
                        GUILayout.Space(10);


                        #region Setting and creating Blips
                        GUILayout.BeginHorizontal();

                        GUILayout.Label("Number Of Other Blip Types");



                        Current_Selected3D_Radar.RadarDesign.BlipCount = EditorGUILayout.DelayedIntField("", Current_Selected3D_Radar.RadarDesign.BlipCount);
                        if (Current_Selected3D_Radar.RadarDesign.BlipCount != Current_Selected3D_Radar.RadarDesign.tempBipCountValue)
                        {
                            Current_Selected3D_Radar.Blips.Resize(Current_Selected3D_Radar.RadarDesign.BlipCount);
                            Current_Selected3D_Radar.RadarDesign.tempBipCountValue = Current_Selected3D_Radar.RadarDesign.BlipCount;
                        }


                        GUILayout.EndHorizontal();
                        #endregion
                        Separator();


                        #region Setting and creating All other Blips
                        EditorGUILayout.Separator();

                        foreach (var _Blip in Current_Selected3D_Radar.Blips.Select((x, i) => new { Value = x as RadarBlips3D, Index = i }))
                        {
                            try
                            {
                                _Blip.Value.State = _Blip.Value.IsActive ? _Blip.Value.Tag + " is " + "Active" : _Blip.Value.Tag + " is " + "Inactive";


                                GUILayout.BeginHorizontal();
                                _Blip.Value.ShowBLipSettings = EditorGUILayout.Foldout(_Blip.Value.ShowBLipSettings, _Blip.Value.State);



                                if (_Blip.Value.BlipSprite)
                                {
                                    GUILayout.FlexibleSpace();
                                    GUILayout.Box(_Blip.Value.BlipSprite.texture, "Label",
                                        GUILayout.Height(20), GUILayout.Width(20));
                                    GUILayout.FlexibleSpace();
                                }

                                if (GUILayout.Button((_Blip.Value.IsActive) ? On : Off, "Label", GUILayout.Width(20))) _Blip.Value.IsActive = !_Blip.Value.IsActive;

                                GUILayout.Space(5);
                                if (GUILayout.Button(new GUIContent(copyIcon, "Copy"), "Label", GUILayout.Width(15)))
                                {
                                    // copiedBlipData3D.DoRemoval = _Blip.Value.DoRemoval;
                                    //  copiedBlipData3D.Instanced = _Blip.Value.Instanced;
                                    copiedBlipData3D.IsActive = _Blip.Value.IsActive;
                                    copiedBlipData3D.ShowBLipSettings = _Blip.Value.ShowBLipSettings;
                                    copiedBlipData3D.ShowSpriteBlipSettings = _Blip.Value.ShowSpriteBlipSettings;
                                    copiedBlipData3D.ShowMeshBlipSettings = _Blip.Value.ShowMeshBlipSettings;
                                    copiedBlipData3D.ShowPrefabBlipSettings = _Blip.Value.ShowPrefabBlipSettings;
                                    copiedBlipData3D.IsTrackRotation = _Blip.Value.IsTrackRotation;
                                    copiedBlipData3D.BlipCanScleBasedOnDistance = _Blip.Value.BlipCanScleBasedOnDistance;
                                    copiedBlipData3D.ShowTrackingLineSettings = _Blip.Value.ShowTrackingLineSettings;
                                    copiedBlipData3D.UseTrackingLine = _Blip.Value.UseTrackingLine;
                                    copiedBlipData3D.UseBaseTracker = _Blip.Value.UseBaseTracker;
                                    copiedBlipData3D.ShowBaseTrackerSettings = _Blip.Value.ShowBaseTrackerSettings;
                                    copiedBlipData3D.lockX = _Blip.Value.lockX;
                                    copiedBlipData3D.lockY = _Blip.Value.lockY;
                                    copiedBlipData3D.lockZ = _Blip.Value.lockZ;
                                    copiedBlipData3D.UseLOD = _Blip.Value.UseLOD;
                                    copiedBlipData3D.ShowLODSettings = _Blip.Value.ShowLODSettings;
                                    copiedBlipData3D.ShowGeneralSettings = _Blip.Value.ShowGeneralSettings;
                                    copiedBlipData3D.ShowAdditionalOptions = _Blip.Value.ShowAdditionalOptions;
                                    copiedBlipData3D.AlwaysShowBlipsInRadarSpace = _Blip.Value.AlwaysShowBlipsInRadarSpace;
                                    copiedBlipData3D.ShowLowMeshSetings = _Blip.Value.ShowLowMeshSetings;
                                    copiedBlipData3D.ShowLowMeshSetings = _Blip.Value.ShowLowMeshSetings;
                                    copiedBlipData3D.ShowMediumMeshSettings = _Blip.Value.ShowMediumMeshSettings;
                                    copiedBlipData3D.ShowHighMeshSettings = _Blip.Value.ShowHighMeshSettings;
                                    copiedBlipData3D.ShowOptimizationSettings = _Blip.Value.ShowOptimizationSettings;
                                    copiedBlipData3D.SmoothScaleTransition = _Blip.Value.SmoothScaleTransition;
                                    copiedBlipData3D.UseCustomRotation = _Blip.Value.UseCustomRotation;
                                    copiedBlipData3D.KeepBlipsAboveRadarPlane = _Blip.Value.KeepBlipsAboveRadarPlane;
                                    copiedBlipData3D.UseHeightTracking = _Blip.Value.UseHeightTracking;


                                    copiedBlipData3D.icon = _Blip.Value.icon;

                                    copiedBlipData3D.BaseTracker = _Blip.Value.BaseTracker;

                                    copiedBlipData3D.prefab = _Blip.Value.prefab;

                                    copiedBlipData3D.State = _Blip.Value.State;
                                    copiedBlipData3D.Tag = _Blip.Value.Tag;

                                    copiedBlipData3D.SpriteMaterial = _Blip.Value.SpriteMaterial;

                                    copiedBlipData3D.mesh = _Blip.Value.mesh;
                                    copiedBlipData3D.Low = _Blip.Value.Low;
                                    copiedBlipData3D.Medium = _Blip.Value.Medium;
                                    copiedBlipData3D.High = _Blip.Value.High;

                                    copiedBlipData3D.tlm = _Blip.Value.tlm;
                                    copiedBlipData3D.btm = _Blip.Value.btm;


                                    copiedBlipData3D.MeshMaterials = new Material[] { };
                                    Array.Resize(ref copiedBlipData3D.MeshMaterials, _Blip.Value.MeshMaterials.Length);

                                    for (int q = 0; q < _Blip.Value.MeshMaterials.Length; q++)
                                    {
                                        copiedBlipData3D.MeshMaterials[q] = _Blip.Value.MeshMaterials[q];
                                    }

                                    copiedBlipData3D.colour = _Blip.Value.colour;
                                    copiedBlipData3D.TrackingLineStartColour = _Blip.Value.TrackingLineStartColour;
                                    copiedBlipData3D.TrackingLineEndColour = _Blip.Value.TrackingLineEndColour;
                                    copiedBlipData3D.BaseTrackerColour = _Blip.Value.BaseTrackerColour;

                                    copiedBlipData3D.BlipSize = _Blip.Value.BlipSize;
                                    // copiedBlipData3D.DynamicBlipSize = _Blip.Value.DynamicBlipSize;
                                    copiedBlipData3D.BlipMinSize = _Blip.Value.BlipMinSize;
                                    copiedBlipData3D.BlipMaxSize = _Blip.Value.BlipMaxSize;
                                    copiedBlipData3D.TrackingLineDimention = _Blip.Value.TrackingLineDimention;
                                    copiedBlipData3D.LowDistance = _Blip.Value.LowDistance;
                                    copiedBlipData3D.MediumDistance = _Blip.Value.MediumDistance;
                                    copiedBlipData3D.HighDistance = _Blip.Value.HighDistance;
                                    copiedBlipData3D.BaseTrackerSize = _Blip.Value.BaseTrackerSize;
                                    copiedBlipData3D.CustomXRotation = _Blip.Value.CustomXRotation;
                                    copiedBlipData3D.CustomYRotation = _Blip.Value.CustomYRotation;
                                    copiedBlipData3D.CustomZRotation = _Blip.Value.CustomZRotation;
                                    copiedBlipData3D.NumberOfBLips = _Blip.Value.NumberOfBLips;
                                    copiedBlipData3D.count = _Blip.Value.count;
                                    copiedBlipData3D.MatCount = _Blip.Value.MatCount;
                                    copiedBlipData3D.Layer = _Blip.Value.Layer;
                                    copiedBlipData3D._CreateBlipAs = _Blip.Value._CreateBlipAs;
                                    copiedBlipData3D.ObjectCount = _Blip.Value.ObjectCount;
                                    copiedBlipData3D.OrderInLayer = _Blip.Value.OrderInLayer;
                                    copiedBlipData3D.sortingLayer = _Blip.Value.sortingLayer;

                                    copiedBlipData3D.optimization = new OptimizationModule();

                                    copiedBlipData3D.optimization.poolSize = _Blip.Value.optimization.poolSize;
                                    copiedBlipData3D.optimization.SetPoolSizeManually = _Blip.Value.optimization.SetPoolSizeManually;
                                    copiedBlipData3D.optimization.objectFindingMethod = _Blip.Value.optimization.objectFindingMethod;
                                    copiedBlipData3D.optimization.RemoveBlipsOnTagChange = _Blip.Value.optimization.RemoveBlipsOnTagChange;
                                    copiedBlipData3D.optimization.RemoveBlipsOnDisable = _Blip.Value.optimization.RemoveBlipsOnDisable;
                                    copiedBlipData3D.optimization.RequireInstanceObjectCheck = _Blip.Value.optimization.RequireInstanceObjectCheck;
                                    copiedBlipData3D.optimization.RecalculatePoolSizeBasedOnFirstFoundObjects = _Blip.Value.optimization.RecalculatePoolSizeBasedOnFirstFoundObjects;


                                }

                                GUILayout.Space(5);

                                if (GUILayout.Button(new GUIContent(pasteIcon, "Paste"), "Label", GUILayout.Width(15)))
                                {
                                    // copiedBlipData3D.DoRemoval = copiedBlipData3D.DoRemoval;
                                    //  copiedBlipData3D.Instanced = copiedBlipData3D.Instanced;
                                    _Blip.Value.IsActive = copiedBlipData3D.IsActive;
                                    _Blip.Value.ShowBLipSettings = copiedBlipData3D.ShowBLipSettings;
                                    _Blip.Value.ShowSpriteBlipSettings = copiedBlipData3D.ShowSpriteBlipSettings;
                                    _Blip.Value.ShowMeshBlipSettings = copiedBlipData3D.ShowMeshBlipSettings;
                                    _Blip.Value.ShowPrefabBlipSettings = copiedBlipData3D.ShowPrefabBlipSettings;
                                    _Blip.Value.IsTrackRotation = copiedBlipData3D.IsTrackRotation;
                                    _Blip.Value.BlipCanScleBasedOnDistance = copiedBlipData3D.BlipCanScleBasedOnDistance;
                                    _Blip.Value.ShowTrackingLineSettings = copiedBlipData3D.ShowTrackingLineSettings;
                                    _Blip.Value.UseTrackingLine = copiedBlipData3D.UseTrackingLine;
                                    _Blip.Value.UseBaseTracker = copiedBlipData3D.UseBaseTracker;
                                    _Blip.Value.ShowBaseTrackerSettings = copiedBlipData3D.ShowBaseTrackerSettings;
                                    _Blip.Value.lockX = copiedBlipData3D.lockX;
                                    _Blip.Value.lockY = copiedBlipData3D.lockY;
                                    _Blip.Value.lockZ = copiedBlipData3D.lockZ;
                                    _Blip.Value.UseLOD = copiedBlipData3D.UseLOD;
                                    _Blip.Value.ShowLODSettings = copiedBlipData3D.ShowLODSettings;
                                    _Blip.Value.ShowGeneralSettings = copiedBlipData3D.ShowGeneralSettings;
                                    _Blip.Value.ShowAdditionalOptions = copiedBlipData3D.ShowAdditionalOptions;
                                    _Blip.Value.AlwaysShowBlipsInRadarSpace = copiedBlipData3D.AlwaysShowBlipsInRadarSpace;
                                    _Blip.Value.ShowLowMeshSetings = copiedBlipData3D.ShowLowMeshSetings;
                                    _Blip.Value.ShowLowMeshSetings = copiedBlipData3D.ShowLowMeshSetings;
                                    _Blip.Value.ShowMediumMeshSettings = copiedBlipData3D.ShowMediumMeshSettings;
                                    _Blip.Value.ShowHighMeshSettings = copiedBlipData3D.ShowHighMeshSettings;
                                    _Blip.Value.ShowOptimizationSettings = copiedBlipData3D.ShowOptimizationSettings;
                                    _Blip.Value.SmoothScaleTransition = copiedBlipData3D.SmoothScaleTransition;
                                    _Blip.Value.UseCustomRotation = copiedBlipData3D.UseCustomRotation;
                                    _Blip.Value.KeepBlipsAboveRadarPlane = copiedBlipData3D.KeepBlipsAboveRadarPlane;
                                    _Blip.Value.UseHeightTracking = copiedBlipData3D.UseHeightTracking;


                                    _Blip.Value.icon = copiedBlipData3D.icon;

                                    _Blip.Value.BaseTracker = copiedBlipData3D.BaseTracker;

                                    _Blip.Value.prefab = copiedBlipData3D.prefab;

                                    _Blip.Value.State = copiedBlipData3D.State;
                                    _Blip.Value.Tag = copiedBlipData3D.Tag;

                                    _Blip.Value.SpriteMaterial = copiedBlipData3D.SpriteMaterial;

                                    _Blip.Value.mesh = copiedBlipData3D.mesh;
                                    _Blip.Value.Low = copiedBlipData3D.Low;
                                    _Blip.Value.Medium = copiedBlipData3D.Medium;
                                    _Blip.Value.High = copiedBlipData3D.High;

                                    _Blip.Value.tlm = copiedBlipData3D.tlm;
                                    _Blip.Value.btm = copiedBlipData3D.btm;


                                    _Blip.Value.MeshMaterials = new Material[] { };
                                    Array.Resize(ref copiedBlipData3D.MeshMaterials, copiedBlipData3D.MeshMaterials.Length);

                                    for (int q = 0; q < copiedBlipData3D.MeshMaterials.Length; q++)
                                    {
                                        copiedBlipData3D.MeshMaterials[q] = copiedBlipData3D.MeshMaterials[q];
                                    }

                                    _Blip.Value.colour = copiedBlipData3D.colour;
                                    _Blip.Value.TrackingLineStartColour = copiedBlipData3D.TrackingLineStartColour;
                                    _Blip.Value.TrackingLineEndColour = copiedBlipData3D.TrackingLineEndColour;
                                    _Blip.Value.BaseTrackerColour = copiedBlipData3D.BaseTrackerColour;

                                    _Blip.Value.BlipSize = copiedBlipData3D.BlipSize;
                                    // copiedBlipData3D.DynamicBlipSize = copiedBlipData3D.DynamicBlipSize;
                                    _Blip.Value.BlipMinSize = copiedBlipData3D.BlipMinSize;
                                    _Blip.Value.BlipMaxSize = copiedBlipData3D.BlipMaxSize;
                                    _Blip.Value.TrackingLineDimention = copiedBlipData3D.TrackingLineDimention;
                                    _Blip.Value.LowDistance = copiedBlipData3D.LowDistance;
                                    _Blip.Value.MediumDistance = copiedBlipData3D.MediumDistance;
                                    _Blip.Value.HighDistance = copiedBlipData3D.HighDistance;
                                    _Blip.Value.BaseTrackerSize = copiedBlipData3D.BaseTrackerSize;
                                    _Blip.Value.CustomXRotation = copiedBlipData3D.CustomXRotation;
                                    _Blip.Value.CustomYRotation = copiedBlipData3D.CustomYRotation;
                                    _Blip.Value.CustomZRotation = copiedBlipData3D.CustomZRotation;
                                    _Blip.Value.NumberOfBLips = copiedBlipData3D.NumberOfBLips;
                                    _Blip.Value.count = copiedBlipData3D.count;
                                    _Blip.Value.MatCount = copiedBlipData3D.MatCount;
                                    _Blip.Value.Layer = copiedBlipData3D.Layer;
                                    _Blip.Value._CreateBlipAs = copiedBlipData3D._CreateBlipAs;
                                    _Blip.Value.ObjectCount = copiedBlipData3D.ObjectCount;
                                    _Blip.Value.OrderInLayer = copiedBlipData3D.OrderInLayer;
                                    _Blip.Value.sortingLayer = copiedBlipData3D.sortingLayer;

                                    _Blip.Value.optimization = new OptimizationModule();

                                    _Blip.Value.optimization.poolSize = copiedBlipData3D.optimization.poolSize;
                                    _Blip.Value.optimization.SetPoolSizeManually = copiedBlipData3D.optimization.SetPoolSizeManually;
                                    _Blip.Value.optimization.objectFindingMethod = copiedBlipData3D.optimization.objectFindingMethod;
                                    _Blip.Value.optimization.RemoveBlipsOnTagChange = copiedBlipData3D.optimization.RemoveBlipsOnTagChange;
                                    _Blip.Value.optimization.RemoveBlipsOnDisable = copiedBlipData3D.optimization.RemoveBlipsOnDisable;
                                    _Blip.Value.optimization.RequireInstanceObjectCheck = copiedBlipData3D.optimization.RequireInstanceObjectCheck;
                                    _Blip.Value.optimization.RecalculatePoolSizeBasedOnFirstFoundObjects = copiedBlipData3D.optimization.RecalculatePoolSizeBasedOnFirstFoundObjects;

                                }

                                GUILayout.Space(5);
                                if (GUILayout.Button(new GUIContent(deleteConditionIcon, "Delete"), "Label", GUILayout.Width(15)))
                                {
                                    Current_Selected3D_Radar.Blips.RemoveAt(_Blip.Index);
                                    Current_Selected3D_Radar.RadarDesign.BlipCount -= 1;
                                    return;
                                }

                                GUILayout.EndHorizontal();
                                cursorchange();


                                if (_Blip.Value.ShowBLipSettings)
                                {
                                    Separator();
                                    EditorGUILayout.Space();

                                    #region Sprite
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Space(20);
                                    GUILayout.BeginVertical();



                                    HelpMessage("If using sprites as blips; set the Sprite and Material here");

                                    _Blip.Value.ShowSpriteBlipSettings = EditorGUILayout.Foldout(_Blip.Value.ShowSpriteBlipSettings, _Blip.Value.Tag + " Sprite");
                                    if (_Blip.Value.ShowSpriteBlipSettings)
                                    {

                                        #region Sprite Blip Design
                                        GUILayout.BeginHorizontal();

                                        GUILayout.BeginVertical();

                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Sprite");
                                        _Blip.Value.BlipSprite = (Sprite)EditorGUILayout.ObjectField(_Blip.Value.BlipSprite, typeof(Sprite), false, GUILayout.Width(220));
                                        GUILayout.EndHorizontal();

                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Material");
                                        _Blip.Value.BlipMaterial = (Material)EditorGUILayout.ObjectField(_Blip.Value.BlipMaterial, typeof(Material), true, GUILayout.Width(220));
                                        GUILayout.EndHorizontal();

                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Colour");
                                        _Blip.Value.colour = EditorGUILayout.ColorField("", _Blip.Value.colour, GUILayout.Width(220));
                                        GUILayout.EndHorizontal();

                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Order In Layer");
                                        _Blip.Value.OrderInLayer = EditorGUILayout.IntField(_Blip.Value.OrderInLayer, GUILayout.Width(220));
                                        GUILayout.EndHorizontal();

                                        GUILayout.EndVertical();

                                        try
                                        {
                                            GUILayout.Box(_Blip.Value.BlipSprite.texture, GUILayout.Height(50), GUILayout.Width(50));
                                        }
                                        catch { }
                                        GUILayout.EndHorizontal();
                                        #endregion


                                    }
                                    GUILayout.EndVertical();
                                    GUILayout.EndHorizontal();

                                    Separator();

                                    EditorGUILayout.Space();
                                    #endregion

                                    #region Mesh
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Space(20);
                                    GUILayout.BeginVertical();
                                    HelpMessage("If using meshes as blips; set the Mesh and Materials here, Optional LODs and the distances at which each mesh is rendered in the radar");

                                    _Blip.Value.ShowMeshBlipSettings = EditorGUILayout.Foldout(_Blip.Value.ShowMeshBlipSettings, _Blip.Value.Tag + " Mesh");
                                    if (_Blip.Value.ShowMeshBlipSettings)
                                    {
                                        #region MeshBlipDesign

                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Mesh");
                                        _Blip.Value.mesh = (Mesh)EditorGUILayout.ObjectField(_Blip.Value.mesh, typeof(Mesh), false, GUILayout.Width(220));
                                        GUILayout.EndHorizontal();

                                        #region LOD area

                                        string statement = (_Blip.Value.UseLOD) ? " is on" : " is off";

                                        GUILayout.BeginHorizontal();
                                        _Blip.Value.ShowLODSettings = EditorGUILayout.Foldout(_Blip.Value.ShowLODSettings, _Blip.Value.Tag + " LOD" + statement);
                                        if (GUILayout.Button((_Blip.Value.UseLOD) ? On : Off, "Label", GUILayout.Width(20))) _Blip.Value.UseLOD = !_Blip.Value.UseLOD;
                                        GUILayout.EndHorizontal();
                                        cursorchange();

                                        if (_Blip.Value.ShowLODSettings)
                                        {
                                            Separator();
                                            EditorGUILayout.Separator();
                                            #region LowSettings
                                            _Blip.Value.ShowLowMeshSetings = EditorGUILayout.Foldout(_Blip.Value.ShowLowMeshSetings, "Low");
                                            if (_Blip.Value.ShowLowMeshSetings)
                                            {
                                                GUILayout.BeginHorizontal();
                                                GUILayout.Label("Mesh");
                                                _Blip.Value.Low = (Mesh)EditorGUILayout.ObjectField(_Blip.Value.Low, typeof(Mesh), false, GUILayout.Width(220));
                                                GUILayout.EndHorizontal();

                                                GUILayout.BeginHorizontal();
                                                GUILayout.Label("Distance");
                                                _Blip.Value.LowDistance = EditorGUILayout.FloatField(_Blip.Value.LowDistance, GUILayout.Width(220));
                                                GUILayout.EndHorizontal();
                                            }

                                            #endregion
                                            Separator();
                                            EditorGUILayout.Separator();
                                            #region MediumDistance
                                            _Blip.Value.ShowMediumMeshSettings = EditorGUILayout.Foldout(_Blip.Value.ShowMediumMeshSettings, "Medium");
                                            if (_Blip.Value.ShowMediumMeshSettings)
                                            {
                                                GUILayout.BeginHorizontal();
                                                GUILayout.Label("Mesh");
                                                _Blip.Value.Medium = (Mesh)EditorGUILayout.ObjectField(_Blip.Value.Medium, typeof(Mesh), false, GUILayout.Width(220));
                                                GUILayout.EndHorizontal();

                                                GUILayout.BeginHorizontal();
                                                GUILayout.Label("Distance");
                                                _Blip.Value.MediumDistance = EditorGUILayout.FloatField(_Blip.Value.MediumDistance, GUILayout.Width(220));
                                                GUILayout.EndHorizontal();
                                            }

                                            #endregion
                                            Separator();
                                            EditorGUILayout.Separator();
                                            #region HighDistance
                                            _Blip.Value.ShowHighMeshSettings = EditorGUILayout.Foldout(_Blip.Value.ShowHighMeshSettings, "High");
                                            if (_Blip.Value.ShowHighMeshSettings)
                                            {
                                                GUILayout.BeginHorizontal();
                                                GUILayout.Label("Mesh");
                                                _Blip.Value.High = (Mesh)EditorGUILayout.ObjectField(_Blip.Value.High, typeof(Mesh), false, GUILayout.Width(220));
                                                GUILayout.EndHorizontal();

                                                GUILayout.BeginHorizontal();
                                                GUILayout.Label("Distance");
                                                _Blip.Value.HighDistance = EditorGUILayout.FloatField(_Blip.Value.HighDistance, GUILayout.Width(220));
                                                GUILayout.EndHorizontal();
                                            }

                                            #endregion
                                            Separator();
                                            EditorGUILayout.Separator();


                                        }

                                        #endregion


                                        #region Define how many materials the mesh uses
                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Material Count");
                                        _Blip.Value.MatCount = Mathf.Clamp(EditorGUILayout.IntField("", _Blip.Value.MatCount, GUILayout.MaxWidth(200), GUILayout.Width(220)), 0, 8000);
                                        GUILayout.EndHorizontal();
                                        #endregion

                                        if (Event.current.keyCode == KeyCode.Return)
                                        {
                                            Array.Resize(ref _Blip.Value.MeshMaterials, _Blip.Value.MatCount);
                                        }

                                        GUILayout.BeginVertical();
                                        for (int i = 0; i < _Blip.Value.MeshMaterials.Count(); i++)
                                        {
                                            GUILayout.BeginHorizontal();
                                            GUILayout.Label("Material " + i);
                                            try
                                            {

                                                _Blip.Value.MeshMaterials[i] = (Material)EditorGUILayout.ObjectField(_Blip.Value.MeshMaterials[i], typeof(Material), false, GUILayout.Width(220));
                                            }
                                            catch { }
                                            GUILayout.EndHorizontal();

                                        }
                                        GUILayout.EndVertical();
                                        #endregion


                                    }
                                    GUILayout.EndVertical();
                                    GUILayout.EndHorizontal();
                                    Separator();

                                    EditorGUILayout.Space();
                                    #endregion

                                    #region prefab
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Space(20);
                                    GUILayout.BeginVertical();
                                    HelpMessage("If using sprites as blips; set the Prefab here");

                                    _Blip.Value.ShowPrefabBlipSettings = EditorGUILayout.Foldout(_Blip.Value.ShowPrefabBlipSettings, _Blip.Value.Tag + " Prefab");
                                    if (_Blip.Value.ShowPrefabBlipSettings)
                                    {
                                        #region Prefab BLip Design
                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Prefab");
                                        _Blip.Value.prefab = (Transform)EditorGUILayout.ObjectField("", _Blip.Value.prefab, typeof(Transform), true, GUILayout.Width(220));
                                        GUILayout.EndVertical();
                                        #endregion
                                    }
                                    GUILayout.EndVertical();
                                    GUILayout.EndHorizontal();

                                    Separator();

                                    EditorGUILayout.Separator();
                                    #endregion

                                    #region Tracking Line
                                    HelpMessage("Tracking lines allows you to visualize the y distance of your blips from the radar");
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Space(20);
                                    GUILayout.BeginVertical();


                                    string trackingLineState = (Current_Selected3D_Radar.Blips[_Blip.Index].UseTrackingLine) ? "Active" : "Inactive";

                                    GUILayout.BeginHorizontal();
                                    Current_Selected3D_Radar.Blips[_Blip.Index].ShowTrackingLineSettings = EditorGUILayout.Foldout(Current_Selected3D_Radar.Blips[_Blip.Index].ShowTrackingLineSettings, "Tracking Line is " + trackingLineState);
                                    if (GUILayout.Button((_Blip.Value.UseTrackingLine) ? On : Off, "Label", GUILayout.Width(20))) _Blip.Value.UseTrackingLine = !_Blip.Value.UseTrackingLine;

                                    GUILayout.EndHorizontal();
                                    cursorchange();

                                    if (Current_Selected3D_Radar.Blips[_Blip.Index].ShowTrackingLineSettings)
                                    {
                                        HelpMessage("Set the Colour and Material here. You can set any material");
                                        #region Tracking Line Design
                                        GUILayout.BeginVertical();

                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Material");
                                        Current_Selected3D_Radar.Blips[_Blip.Index].TrackingLineMaterial = (Material)EditorGUILayout.ObjectField("", Current_Selected3D_Radar.Blips[_Blip.Index].TrackingLineMaterial, typeof(Material), true, GUILayout.Width(220));
                                        GUILayout.EndHorizontal();


                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Start Colour");
                                        Current_Selected3D_Radar.Blips[_Blip.Index].TrackingLineStartColour = EditorGUILayout.ColorField("", Current_Selected3D_Radar.Blips[_Blip.Index].TrackingLineStartColour, GUILayout.Width(220));
                                        GUILayout.EndHorizontal();

                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("End Colour");
                                        Current_Selected3D_Radar.Blips[_Blip.Index].TrackingLineEndColour = EditorGUILayout.ColorField("", Current_Selected3D_Radar.Blips[_Blip.Index].TrackingLineEndColour, GUILayout.Width(220));
                                        GUILayout.EndHorizontal();

                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Scale");
                                        GUILayout.Space(153);
                                        Current_Selected3D_Radar.Blips[_Blip.Index].TrackingLineDimention = EditorGUILayout.FloatField(" ", Current_Selected3D_Radar.Blips[_Blip.Index].TrackingLineDimention, GUILayout.MaxWidth(ScreenRect.width));
                                        GUILayout.EndHorizontal();


                                        #region BaseTracker Design
                                        HelpMessage("Base Trackers are sprites which appear that the base of the reacking line and are optional");
                                        GUILayout.BeginHorizontal();
                                        Current_Selected3D_Radar.Blips[_Blip.Index].ShowBaseTrackerSettings = EditorGUILayout.Foldout(Current_Selected3D_Radar.Blips[_Blip.Index].ShowBaseTrackerSettings, "Base Tracker settings");
                                        if (GUILayout.Button((_Blip.Value.UseBaseTracker) ? On : Off, "Label", GUILayout.Width(20))) _Blip.Value.UseBaseTracker = !_Blip.Value.UseBaseTracker;
                                        GUILayout.EndHorizontal();
                                        cursorchange();
                                        if (Current_Selected3D_Radar.Blips[_Blip.Index].ShowBaseTrackerSettings)
                                        {
                                            GUILayout.BeginHorizontal();
                                            GUILayout.BeginVertical();
                                            GUILayout.BeginHorizontal();
                                            GUILayout.Label("Base Tracker");
                                            _Blip.Value.BaseTrackerSprite = (Sprite)EditorGUILayout.ObjectField(_Blip.Value.BaseTrackerSprite, typeof(Sprite), false, GUILayout.Width(220));
                                            GUILayout.EndHorizontal();

                                            GUILayout.BeginHorizontal();
                                            GUILayout.Label("Material");
                                            Current_Selected3D_Radar.Blips[_Blip.Index].BaseTrackerMaterial = (Material)EditorGUILayout.ObjectField("", Current_Selected3D_Radar.Blips[_Blip.Index].BaseTrackerMaterial, typeof(Material), true, GUILayout.Width(220));
                                            GUILayout.EndHorizontal();

                                            GUILayout.BeginHorizontal();
                                            GUILayout.Label("Colour");
                                            Current_Selected3D_Radar.Blips[_Blip.Index].BaseTrackerColour = EditorGUILayout.ColorField("", Current_Selected3D_Radar.Blips[_Blip.Index].BaseTrackerColour, GUILayout.Width(220));
                                            GUILayout.EndHorizontal();

                                            GUILayout.BeginHorizontal();
                                            GUILayout.Label("Size");
                                            Current_Selected3D_Radar.Blips[_Blip.Index].BaseTrackerSize = EditorGUILayout.FloatField("", Current_Selected3D_Radar.Blips[_Blip.Index].BaseTrackerSize, GUILayout.Width(220));
                                            GUILayout.EndHorizontal();

                                            GUILayout.EndVertical();

                                            try
                                            {
                                                GUILayout.Box(_Blip.Value.BaseTrackerSprite.texture, GUILayout.Height(50),
                                                    GUILayout.Width(50));
                                            }
                                            catch
                                            {
                                                // nothing

                                            }
                                            GUILayout.EndHorizontal();


                                        }
                                        #endregion

                                        GUILayout.EndVertical();

                                        #endregion
                                    }
                                    GUILayout.EndVertical();
                                    GUILayout.EndHorizontal();
                                    Separator();
                                    EditorGUILayout.Separator();
                                    #endregion

                                    #region Additional Options
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Space(20);
                                    GUILayout.BeginVertical();

                                    HelpMessage("Displaying additional options for your blip");
                                    Current_Selected3D_Radar.Blips[_Blip.Index].ShowAdditionalOptions = EditorGUILayout.Foldout(Current_Selected3D_Radar.Blips[_Blip.Index].ShowAdditionalOptions, "Additional Options");
                                    if (Current_Selected3D_Radar.Blips[_Blip.Index].ShowAdditionalOptions)
                                    {
                                        HelpMessage("When eabled all " + _Blip.Value.Tag + "blips will not disppear when at they pass the bounderies of the radar, but will remain at the edge and will be scaled based on its distance from the center object");
                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Always Show " + _Blip.Value.Tag + " in radar");
                                        Current_Selected3D_Radar.Blips[_Blip.Index].AlwaysShowBlipsInRadarSpace = GUILayout.Toggle(Current_Selected3D_Radar.Blips[_Blip.Index].AlwaysShowBlipsInRadarSpace, "", GUILayout.Width(220));
                                        GUILayout.EndHorizontal();

                                        if (!Current_Selected3D_Radar.Blips[_Blip.Index].UseHeightTracking && Current_Selected3D_Radar.Blips[_Blip.Index].KeepBlipsAboveRadarPlane)
                                            EditorGUILayout.HelpBox("Use Heigh Tracking is Off, so all blips will already be above /On the Radar plane", MessageType.Warning);

                                        HelpMessage("True by default, ensures that blips do not go under the radar");
                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Always show " + _Blip.Value.Tag + " above the radar plane");
                                        Current_Selected3D_Radar.Blips[_Blip.Index].KeepBlipsAboveRadarPlane = GUILayout.Toggle(Current_Selected3D_Radar.Blips[_Blip.Index].KeepBlipsAboveRadarPlane, "", GUILayout.Width(220));
                                        GUILayout.EndHorizontal();

                                        HelpMessage("True by default, ensures that blips track objects Y position");
                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Use height Tracking");
                                        Current_Selected3D_Radar.Blips[_Blip.Index].UseHeightTracking = GUILayout.Toggle(Current_Selected3D_Radar.Blips[_Blip.Index].UseHeightTracking, "", GUILayout.Width(220));
                                        GUILayout.EndHorizontal();
                                    }


                                    GUILayout.EndVertical();
                                    GUILayout.EndHorizontal();

                                    Separator();

                                    EditorGUILayout.Space();
                                    #endregion

                                    #region Optimization 

                                    GUILayout.BeginHorizontal();
                                    GUILayout.Space(20);
                                    GUILayout.BeginVertical();

                                    HelpMessage("Options for optimization the radar proceses");
                                    GUILayout.BeginHorizontal();
                                    Current_Selected3D_Radar.Blips[_Blip.Index].ShowOptimizationSettings = EditorGUILayout.Foldout(Current_Selected3D_Radar.Blips[_Blip.Index].ShowOptimizationSettings, "Optimization Options");
                                    GUILayout.Box(Optimizeicon, "Label", GUILayout.Width(120), GUILayout.Height(20));
                                    GUILayout.EndHorizontal();
                                    if (Current_Selected3D_Radar.Blips[_Blip.Index].ShowOptimizationSettings)
                                    {
                                        if (Current_Selected3D_Radar.Blips[_Blip.Index].optimization.objectFindingMethod == ObjectFindingMethod.Pooling)
                                        {
                                            HelpMessage("If you are spawning any new objects into the scene then call radar3D.DoInstanceObjectCheck() at instance or at the end of instancing");
                                        }


                                        if (Current_Selected3D_Radar.Blips[_Blip.Index].optimization.objectFindingMethod != ObjectFindingMethod.Recursive)
                                        {
                                            HelpMessage("This requires that you call ' _3DRadar.doInstanceObjectCheck() whenever you want to make the radar search for objects to create blips from. This can also be used to icrease your internal pool size if you need to track more objects'");

                                            HelpMessage(" If you know exactly ow many scene objects this blip should represet then you can set the pool size manually");
                                            GUILayout.BeginHorizontal();
                                            GUILayout.Label("Set Pool Size");
                                            Current_Selected3D_Radar.Blips[_Blip.Index].optimization.SetPoolSizeManually = EditorGUILayout.Toggle(Current_Selected3D_Radar.Blips[_Blip.Index].optimization.SetPoolSizeManually, GUILayout.Width(220));
                                            GUILayout.EndHorizontal();
                                            if (Current_Selected3D_Radar.Blips[_Blip.Index].optimization.SetPoolSizeManually)
                                            {
                                                HelpMessage("The mumber of scene objects that this blip will represent");
                                                GUILayout.BeginHorizontal();
                                                GUILayout.Label("Pool Size");
                                                Current_Selected3D_Radar.Blips[_Blip.Index].optimization.poolSize = EditorGUILayout.IntField(Current_Selected3D_Radar.Blips[_Blip.Index].optimization.poolSize, GUILayout.Width(220));
                                                GUILayout.EndHorizontal();

                                                HelpMessage("If your pool size is too large then the count will be calculated DOWN");
                                                GUILayout.BeginHorizontal();
                                                GUILayout.Label("Recalculate Pool Size On Start");
                                                Current_Selected3D_Radar.Blips[_Blip.Index].optimization.RecalculatePoolSizeBasedOnFirstFoundObjects = EditorGUILayout.Toggle(Current_Selected3D_Radar.Blips[_Blip.Index].optimization.RecalculatePoolSizeBasedOnFirstFoundObjects, GUILayout.Width(220));
                                                GUILayout.EndHorizontal();
                                            }

                                        }




                                        HelpMessage("This method allows you to use object pooling to store your blips");
                                        GUILayout.BeginHorizontal();
                                        var info = Current_Selected3D_Radar.Blips[_Blip.Index].optimization.objectFindingMethod == ObjectFindingMethod.Pooling ? "Use Pooling (Fast)" : (Current_Selected3D_Radar.Blips[_Blip.Index].optimization.RequireInstanceObjectCheck) ? "Use Recursive Object Finding (Fast)" : "Use Recursive Object Finding (Slower)";
                                        GUILayout.Label(info);
                                        Current_Selected3D_Radar.Blips[_Blip.Index].optimization.objectFindingMethod = (ObjectFindingMethod)EditorGUILayout.EnumPopup(Current_Selected3D_Radar.Blips[_Blip.Index].optimization.objectFindingMethod, GUILayout.Width(220));
                                        GUILayout.EndHorizontal();

                                        if (Current_Selected3D_Radar.Blips[_Blip.Index].optimization.objectFindingMethod == ObjectFindingMethod.Recursive)
                                        {
                                            GUILayout.BeginHorizontal();
                                            GUILayout.Label("Require Instance Object Check");
                                            if (Current_Selected3D_Radar.Blips[_Blip.Index].optimization.RequireInstanceObjectCheck)
                                                HelpMessage("This requires that you call ' _3DRadar.doInstanceObjectCheck() whenever you want to make the radar search for objects to create blips from. This can also be used to icrease your internal pool size if you need to track more objects'");
                                            Current_Selected3D_Radar.Blips[_Blip.Index].optimization.RequireInstanceObjectCheck = EditorGUILayout.Toggle(Current_Selected3D_Radar.Blips[_Blip.Index].optimization.RequireInstanceObjectCheck, GUILayout.Width(220));
                                            GUILayout.EndHorizontal();
                                        }

                                        HelpMessage("Allows blips to be removed whenever the object it represent  , changes its tag");
                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Remove Blip On Tag Change");
                                        Current_Selected3D_Radar.Blips[_Blip.Index].optimization.RemoveBlipsOnTagChange = EditorGUILayout.Toggle(Current_Selected3D_Radar.Blips[_Blip.Index].optimization.RemoveBlipsOnTagChange, GUILayout.Width(220));
                                        GUILayout.EndHorizontal();

                                        HelpMessage("Allows for the blip to be turned off when the object its represents is disabled");
                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Remove Blip On Disable");
                                        Current_Selected3D_Radar.Blips[_Blip.Index].optimization.RemoveBlipsOnDisable = EditorGUILayout.Toggle(Current_Selected3D_Radar.Blips[_Blip.Index].optimization.RemoveBlipsOnDisable, GUILayout.Width(220));
                                        GUILayout.EndHorizontal();


                                    }

                                    GUILayout.EndVertical();
                                    GUILayout.EndHorizontal();

                                    Separator();

                                    EditorGUILayout.Space();

                                    #endregion

                                    #region Scale and Rotation Settings

                                    GUILayout.BeginHorizontal();
                                    GUILayout.Space(20);
                                    GUILayout.BeginVertical();

                                    _Blip.Value.ShowGeneralSettings = EditorGUILayout.Foldout(_Blip.Value.ShowGeneralSettings, "Rotation and Scale");
                                    if (_Blip.Value.ShowGeneralSettings)
                                    {
                                        HelpMessage("Set the scale of the blip. If 'Scale by distance' is being used then the blps will scale in the radar based on their distance from the center object . the visibility of the scaling varies based on the size of the radar ");

                                        #region Blip Scale Settings
                                        if (!Current_Selected3D_Radar.Blips[_Blip.Index].BlipCanScleBasedOnDistance)
                                        {
                                            GUILayout.BeginHorizontal();
                                            GUILayout.Label("Scale");
                                            GUILayout.Space(153);
                                            Current_Selected3D_Radar.Blips[_Blip.Index].BlipSize = EditorGUILayout.FloatField(" ", Current_Selected3D_Radar.Blips[_Blip.Index].BlipSize, GUILayout.MaxWidth(ScreenRect.width));
                                            GUILayout.EndHorizontal();
                                        }
                                        else
                                        {
                                            GUILayout.BeginHorizontal();
                                            GUILayout.Label("Min Scale");
                                            Current_Selected3D_Radar.Blips[_Blip.Index].BlipMinSize = EditorGUILayout.FloatField("", Current_Selected3D_Radar.Blips[_Blip.Index].BlipMinSize, GUILayout.Width(220));
                                            GUILayout.EndHorizontal();

                                            GUILayout.BeginHorizontal();
                                            GUILayout.Label("Max Scale");
                                            Current_Selected3D_Radar.Blips[_Blip.Index].BlipMaxSize = EditorGUILayout.FloatField("", Current_Selected3D_Radar.Blips[_Blip.Index].BlipMaxSize, GUILayout.Width(220));
                                            GUILayout.EndHorizontal();
                                        }
                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Scale by distance");
                                        Current_Selected3D_Radar.Blips[_Blip.Index].BlipCanScleBasedOnDistance = GUILayout.Toggle(Current_Selected3D_Radar.Blips[_Blip.Index].BlipCanScleBasedOnDistance, "", GUILayout.Width(220));
                                        GUILayout.EndHorizontal();

                                        #endregion

                                        #region Rotation Settings

                                        HelpMessage("Use custom rotation allows yo uto set a static rotation for all of these blip types");
                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Use Custom Rotation");
                                        Current_Selected3D_Radar.Blips[_Blip.Index].UseCustomRotation = GUILayout.Toggle(Current_Selected3D_Radar.Blips[_Blip.Index].UseCustomRotation, "", GUILayout.Width(220));
                                        GUILayout.EndHorizontal();

                                        if (Current_Selected3D_Radar.Blips[_Blip.Index].UseCustomRotation && !Current_Selected3D_Radar.Blips[_Blip.Index].IsTrackRotation)
                                        {
                                            HelpMessage("Set a X, Y and Z rotation for this blip type");

                                            GUILayout.BeginHorizontal();
                                            GUILayout.Label("Rotation");
                                            GUILayout.Label("X");
                                            Current_Selected3D_Radar.Blips[_Blip.Index].CustomXRotation = EditorGUILayout.FloatField(Current_Selected3D_Radar.Blips[_Blip.Index].CustomXRotation);
                                            GUILayout.Label("Y");
                                            Current_Selected3D_Radar.Blips[_Blip.Index].CustomYRotation = EditorGUILayout.FloatField(Current_Selected3D_Radar.Blips[_Blip.Index].CustomYRotation);
                                            GUILayout.Label("Z");
                                            Current_Selected3D_Radar.Blips[_Blip.Index].CustomZRotation = EditorGUILayout.FloatField(Current_Selected3D_Radar.Blips[_Blip.Index].CustomZRotation);
                                            GUILayout.EndHorizontal();
                                        }

                                        HelpMessage("Track Rotation allows for the blip to rotate and match the rotation of the tracked object");

                                        GUILayout.BeginHorizontal();
                                        GUILayout.Label("Track Rotation");
                                        Current_Selected3D_Radar.Blips[_Blip.Index].IsTrackRotation = GUILayout.Toggle(Current_Selected3D_Radar.Blips[_Blip.Index].IsTrackRotation, "", GUILayout.Width(220));
                                        GUILayout.EndHorizontal();

                                        if (Current_Selected3D_Radar.Blips[_Blip.Index].IsTrackRotation && !Current_Selected3D_Radar.Blips[_Blip.Index].UseCustomRotation)
                                        {
                                            HelpMessage("Freeze rotation through the x,y, or z axis");

                                            GUILayout.BeginHorizontal();
                                            GUILayout.Label("Freeze");
                                            Current_Selected3D_Radar.Blips[_Blip.Index].lockX = GUILayout.Toggle(Current_Selected3D_Radar.Blips[_Blip.Index].lockX, "X");
                                            Current_Selected3D_Radar.Blips[_Blip.Index].lockY = GUILayout.Toggle(Current_Selected3D_Radar.Blips[_Blip.Index].lockY, "Y");
                                            Current_Selected3D_Radar.Blips[_Blip.Index].lockZ = GUILayout.Toggle(Current_Selected3D_Radar.Blips[_Blip.Index].lockZ, "Z");
                                            GUILayout.EndHorizontal();
                                        }
                                        #endregion

                                        if (Current_Selected3D_Radar.Blips[_Blip.Index].UseCustomRotation && Current_Selected3D_Radar.Blips[_Blip.Index].IsTrackRotation)
                                        {
                                            EditorGUILayout.HelpBox("Do not use 'Track Rotation' and 'Custom Rotation at the same time'", MessageType.Warning);
                                        }
                                    }
                                    GUILayout.EndVertical();
                                    GUILayout.EndHorizontal();

                                    Separator();

                                    EditorGUILayout.Space();
                                    #endregion


                                    #region Universal Settings
                                    HelpMessage("With the current settings,If this blip is active, it will represent your gameobject with the tag " + _Blip.Value.Tag + " on the layer " + _Blip.Value.Layer + " and will be at scale " + Current_Selected3D_Radar.Blips[_Blip.Index].BlipSize);

                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Create Blip");

                                    _Blip.Value._CreateBlipAs = (CreateBlipAs)EditorGUILayout.EnumPopup(_Blip.Value._CreateBlipAs, GUILayout.Width(100));

                                    GUILayout.BeginHorizontal(); GUILayout.Label("From Object Tagged"); _Blip.Value.Tag = EditorGUILayout.TagField(_Blip.Value.Tag); GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal(); GUILayout.Label("On Layer"); _Blip.Value.Layer = EditorGUILayout.LayerField(_Blip.Value.Layer); GUILayout.EndHorizontal();

                                    GUILayout.EndHorizontal();


                                    #endregion
                                }

                                Separator();
                                EditorGUILayout.Separator();
                            }
                            catch { }
                        }
                        #endregion

                        EditorGUILayout.EndScrollView();


                        break;
                    #endregion

                    #region Make New Radar
                    case 2:
                        CreationArea();

                        break;
                        #endregion
                }
            }

            #endregion

            #endregion

        }
        #endregion

        #region Separator
        void Separator()
        {
            var area = GUILayoutUtility.GetLastRect();
            GUI.DrawTexture(new Rect(0, area.position.y + area.height, position.width, 1), Application.HasProLicense() ? Textures.DuskLightest : Textures.DuskLighter);
}
        #endregion

        #region Help Messages
        void HelpMessage(string message)
        {
            if (ShowHelpMessages) { EditorGUILayout.HelpBox(message, MessageType.Info); }
        }
        #endregion

        #region OnInspector Update
        public void OnInspectorUpdate()
        {
            Repaint();
            if (_Selection())
                if (_Selection().GetComponent<_3DRadar>())
                {
                    Repaint();
                    RadarObject = _Selection();
                    Repaint();
                }

        }
        #endregion

        void cursorchange()
        {
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
            }
        }

        #region Create 3D Radar
        void Create3DRadar()
        {

            GameObject RadarInstance = new GameObject("3D Radar");
            RadarInstance.AddComponent<_3DRadar>();
            RadarInstance.AddComponent<Visualization3D>();
            Selection.activeGameObject = RadarInstance;
            GameObject DesignsParent = new GameObject("Designs");
            DesignsParent.transform.parent = RadarInstance.transform;
            GameObject BlipsContainer = new GameObject("Blips");
            BlipsContainer.transform.parent = RadarInstance.transform;
            GameObject DefaultDesign = new GameObject("Default3DRadarSprite");
            DefaultDesign.transform.parent = DesignsParent.transform;
            DefaultDesign.transform.Rotate(new Vector2(90, 0), Space.World);
            DefaultDesign.AddComponent<SpriteRenderer>();
            DefaultDesign.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Default Sprites/Radar/Default3DRadarSprite");
            CreateRenderCamera(RadarInstance);




            Tab_Selection = 0;

        }
        #endregion

        #region CreateRender Camera
        void CreateRenderCamera(GameObject radar)
        {
            GameObject RenderCamera = new GameObject("Render Camera");
            RenderCamera.AddComponent<Camera>();
            RenderCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
            RenderCamera.GetComponent<Camera>().farClipPlane = 265;
            RenderCamera.GetComponent<Camera>().nearClipPlane = 0.01f;
            RenderCamera.GetComponent<Camera>().depth = 100;
            RenderCamera.GetComponent<Camera>().cullingMask = 1;
            RenderCamera.GetComponent<Camera>().orthographicSize = 2.3f;

                radar.GetComponent<_3DRadar>().RadarDesign.renderingCamera = RenderCamera.GetComponent<Camera>();

        }
        #endregion


        #region Create Realitime Minimap Camera

        Camera CreateRealtimeMinimapCamera(Type radarType)
        {
            GameObject RenderCamera = new GameObject("Realtime Minimap Camera");
            RenderCamera.transform.Rotate(new Vector2(90, 0), Space.World);
            RenderCamera.AddComponent<Camera>();
            RenderCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            RenderCamera.GetComponent<Camera>().farClipPlane = 50;
            RenderCamera.GetComponent<Camera>().nearClipPlane = 0.01f;
            RenderCamera.GetComponent<Camera>().depth = 0;
            RenderCamera.GetComponent<Camera>().orthographic = true;
            try
            {

                RenderCamera.GetComponent<Camera>().targetTexture = Current_Selected3D_Radar.minimapModule.renderTexture;
            }
            catch
            {
                Debug.LogWarning("You have no set up a render texture your Realtime Minimap Camera yet");
            }
            return RenderCamera.GetComponent<Camera>();
        }
        #endregion


    }

}