using System;
using System.Collections.Generic;
using System.Linq;
using DaiMangou.RadarBuilder3D.Editor;
using UnityEngine;
using UnityEngine.Rendering;

namespace DaiMangou.RadarBuilder3D
{

    [AddComponentMenu("Tools/DaiMangou/3D Radar")]

    public class _3DRadar : MonoBehaviour
    {
        public delegate void DoInstanceObjectCheck();

        /// <summary>
        /// trigger search for new objes for the radar to track as blips
        /// </summary>
        public static DoInstanceObjectCheck doInstanceObjectCheck;


        /// <summary>
        /// when set to true, will trigger a search for objects to track, once completed , it will reset it false
        /// </summary>
        internal bool BeginObjectInstanceCheck = true;

        public void OnEnable()
        {
            // subscrive to the event
            doInstanceObjectCheck += InstanceObjectCheck;

        }

        public void OnDisable()
        {
            // unsubscribe from the evnet
            doInstanceObjectCheck -= InstanceObjectCheck;
        }

        public void Start()
        {
            InstanceObjectCheck();
            ReadyCheck();
            SetBlipParentObject();

            //This is a patch that will be removed in 2 versions
            if (RadarDesign.renderingCamera != null)
            {
                RadarDesign.renderingCamera.orthographicSize = 2.3f;
            }

        }

        /// <summary>
        /// will trigger a search for objects to track
        /// 
        /// </summary>
        private void InstanceObjectCheck()
        {
            BeginObjectInstanceCheck = true;
        }
        /// <summary>
        /// 
        /// </summary>
        internal void ReadyCheck()
        {
            if (RadarDesign._3DSystemsWithScreenSpaceFunction && transform.parent)
            {
                Debug.LogWarning("3D Radar does not need to be the child of any object when in screen space mode. This may cause unanted effects");
            }
        }

        private void SetBlipParentObject()
        {
            RadarDesign.BlipsParentObject = transform.Find("Blips");

            if (RadarDesign.BlipsParentObject == null)
            {
                Debug.LogWarning("This version of the Radar Builder requires that an object named 'Blips' is made the child of your radar. This is done automatically when you create a new Radar");
                GameObject tempBLipsContiner = new GameObject("Blips");
                tempBLipsContiner.transform.parent = transform;
                tempBLipsContiner.transform.localPosition = Vector3.zero;
                RadarDesign.BlipsParentObject = tempBLipsContiner.transform;
                Debug.Log("Radar Builder has created a Temporary 'Blips' container object, Please create a new gameobject named 'Blips' and make it a child of your Radar gameobject");
            }
        }

        /// <summary>
        /// Called once perframe, ecch radar function too is called once per frame
        /// </summary>
        public void Update()
        {
            // set up our  renderingCamera and main Camera
            CameraSetup();
            // Determine the front facing direction of the radar
            Frontis();
            // set the dimentions of the radar 
            SetRadarDimentions();
            // manages the positioning of the radar
            SnapAndPositioning();
            //manage the poition, rotation and scale of the center blip
            CenterObjectBlip();
            // manage the position, rotation and scale for all other blips
            CheckAndSetBlips();
            // manages the Static and Realtime minimap systems
            Minimap();
            // manages specifically target objects rotations
            RotateSpecifics();
        }

        /// <summary>
        ///     here we setup main camera, this cript will constantly searn for your main camera or camera you wish to find IF it
        ///     is not yet found
        ///     during th abscence of a camera , no errors will be thrown.
        ///     However you will be warned if you do not set a 'Rendering camera' for your radar.
        /// </summary>
        public void CameraSetup()
        {
            // First we check if we are using the screen space function of the Radar
            if (RadarDesign._3DSystemsWithScreenSpaceFunction)
            {
                #region Setup the camera

                // if we dont already have a camera set up then do the following 
                if (!RadarDesign.camera)
                    if (!RadarDesign.ManualCameraSetup)
                        if (RadarDesign.UseMainCamera)
                            //
                            RadarDesign.camera = Camera.main;
                        else
                            try
                            {
                                // find some othr camera in the scene by tag
                                RadarDesign.camera =
                                    GameObject.FindWithTag(RadarDesign.CameraTag).GetComponent<Camera>();
                            }
                            catch
                            {
                                return;
                            }
                // throw ourself out of the function if no camera is found
                if (RadarDesign.camera == null)
                    return;

                #endregion

                #region Setup RenderCamera

                //
                if (RadarDesign.renderingCamera)
                {
                    //
                    RadarDesign.renderingCamera.transform.rotation = RadarDesign.camera.transform.rotation;
                }
                else // let the user know that a Render camera must be added
                {
                    Debug.LogWarning(
                        "Please specify a rendering camera, Your rendering camera was created when you created this radar ");
                    Debug.Break();
                }

                #endregion
            }
        }

        /// <summary>
        ///     Here we determine which direction we wish for our radars front to be
        ///     For example, If you ser Frontis to be East then all blips moving forward through the Z axis will appear to be
        ///     moving to the east in your radar .
        ///     This makes the function very useful for various games
        /// </summary>
        private void Frontis()
        {
            #region Front is 

            if (RadarDesign._3DSystemsWithScreenSpaceFunction)
                switch (RadarDesign.frontIs)
                {
                    case FrontIs.North:
                        RadarDesign.RadarRotationOffset = 0;
                        break;
                    case FrontIs.East:
                        RadarDesign.RadarRotationOffset = 90;

                        break;
                    case FrontIs.South:
                        RadarDesign.RadarRotationOffset = 180;

                        break;
                    case FrontIs.West:
                        RadarDesign.RadarRotationOffset = 270;

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            RadarDesign.BlipsParentObject.transform.localEulerAngles = new Vector3(0, RadarDesign.RadarRotationOffset, 0);

            #endregion
        }

        /// <summary>
        ///     Scale the radar lie any other object or by usign the  RadarDiameter Value when UseLocalScale is true
        /// </summary>
        private void SetRadarDimentions()
        {
            #region Setting Radar Dimentions

            //NEVER DIRECTLY MANIPULATE THE SCALE OF THIS OBJECT
            var diameter = RadarDesign.RadarDiameter;
            transform.localScale = RadarDesign.UseLocalScale
                ? transform.localScale
                : new Vector3(diameter, diameter, diameter);

            #endregion
        }

        /// <summary>
        ///     Snap position allows for the radar to be positioned in 9 points of the screen
        /// </summary>
        private void SnapAndPositioning()
        {
            if (RadarDesign._3DSystemsWithScreenSpaceFunction)
            {
                RadarDesign.SnappedRect.size = new Vector2(DMMath.REQS(Screen.height, DMMath.Mv),
                    DMMath.REQS(Screen.height, DMMath.Mv));

                #region Snap&Manual positioning

                // for every increase or decrease of 0.01 in radar diameter above or below 1 will result in  the amount of times * 0.0024096385
                var scale = transform.localScale.x;

                var autoScalingForChangeInDiameter = Mathf.Abs((1 - scale) / 0.01f) * DMMath.ScalingConstant;
                // Do not change this value

                var posOffSetByScale = RadarDesign.IgnoreDiameterScale
                    ? 0
                    : scale < 1
                        ? Screen.height * autoScalingForChangeInDiameter
                        : scale > 1 ? -(Screen.height * autoScalingForChangeInDiameter) : 0;


                switch (RadarDesign.radarPositioning)
                {
                    case RadarPositioning.Manual:
                        RadarDesign.SnappedRect.position = new Vector2(RadarDesign.RadarRect.xMax,
                            -RadarDesign.RadarRect.yMax + Screen.height);

                        transform.position =
                            RadarDesign.renderingCamera.ScreenToWorldPoint(
                                new Vector3(RadarDesign.SnappedRect.center.x + RadarDesign.xPadding,
                                    RadarDesign.SnappedRect.center.y - RadarDesign.SnappedRect.height -
                                    RadarDesign.yPadding, RadarDesign3D.ConstantRadarRenderDistance));

                        break;
                    case RadarPositioning.Snap:
                        switch (RadarDesign.snapPosition)
                        {
                            case SnapPosition.TopLeft:
                                RadarDesign.SnappedRect =
                                    ThisScreen.ScreenRect.ToUpperLeft(RadarDesign.SnappedRect.width,
                                            RadarDesign.SnappedRect.width, -posOffSetByScale, posOffSetByScale)
                                        .AddRect(0, Screen.height - RadarDesign.SnappedRect.size.y);
                                if (RadarDesign.UseOrthographicForSideSnaps)
                                    RadarDesign.renderingCamera.orthographic = true;
                                else
                                    RadarDesign.renderingCamera.orthographic = false;
                                break;
                            case SnapPosition.TopRight:
                                RadarDesign.SnappedRect =
                                    ThisScreen.ScreenRect.ToUpperRight(RadarDesign.SnappedRect.width,
                                            RadarDesign.SnappedRect.width, posOffSetByScale, posOffSetByScale)
                                        .AddRect(0, Screen.height - RadarDesign.SnappedRect.size.y);
                                if (RadarDesign.UseOrthographicForSideSnaps)
                                    RadarDesign.renderingCamera.orthographic = true;
                                else
                                    RadarDesign.renderingCamera.orthographic = false;
                                break;
                            case SnapPosition.BottomLeft:
                                RadarDesign.SnappedRect =
                                    ThisScreen.ScreenRect.ToLowerLeft(RadarDesign.SnappedRect.width,
                                            RadarDesign.SnappedRect.width, -posOffSetByScale, -posOffSetByScale)
                                        .AddRect(0, -Screen.height + RadarDesign.SnappedRect.size.y);
                                if (RadarDesign.UseOrthographicForSideSnaps)
                                    RadarDesign.renderingCamera.orthographic = true;
                                else
                                    RadarDesign.renderingCamera.orthographic = false;

                                break;
                            case SnapPosition.BottomRight:
                                RadarDesign.SnappedRect =
                                    ThisScreen.ScreenRect.ToLowerRight(RadarDesign.SnappedRect.width,
                                            RadarDesign.SnappedRect.width, posOffSetByScale, -posOffSetByScale)
                                        .AddRect(0, -Screen.height + RadarDesign.SnappedRect.size.y);
                                if (RadarDesign.UseOrthographicForSideSnaps)
                                    RadarDesign.renderingCamera.orthographic = true;
                                else
                                    RadarDesign.renderingCamera.orthographic = false;
                                break;
                            case SnapPosition.Center:
                                RadarDesign.SnappedRect = ThisScreen.ScreenRect.ToCenter(RadarDesign.SnappedRect.width,
                                    RadarDesign.SnappedRect.width);
                                RadarDesign.renderingCamera.orthographic = false;
                                break;
                            case SnapPosition.LeftMiddle:
                                RadarDesign.SnappedRect =
                                    ThisScreen.ScreenRect.ToCenterLeft(RadarDesign.SnappedRect.width,
                                        RadarDesign.SnappedRect.width, -posOffSetByScale);
                                if (RadarDesign.UseOrthographicForSideSnaps)
                                    RadarDesign.renderingCamera.orthographic = true;
                                else
                                    RadarDesign.renderingCamera.orthographic = false;
                                break;
                            case SnapPosition.RightMiddle:
                                RadarDesign.SnappedRect =
                                    ThisScreen.ScreenRect.ToCenterRight(RadarDesign.SnappedRect.width,
                                        RadarDesign.SnappedRect.width, posOffSetByScale);
                                if (RadarDesign.UseOrthographicForSideSnaps)
                                    RadarDesign.renderingCamera.orthographic = true;
                                else
                                    RadarDesign.renderingCamera.orthographic = false;
                                break;
                            case SnapPosition.BottomMiddle:
                                RadarDesign.SnappedRect =
                                    ThisScreen.ScreenRect.ToCenterBottom(RadarDesign.SnappedRect.width,
                                            RadarDesign.SnappedRect.width, 0, -posOffSetByScale)
                                        .AddRect(0, -Screen.height + RadarDesign.SnappedRect.size.y);
                                RadarDesign.renderingCamera.orthographic = false;
                                break;
                            case SnapPosition.TopMiddle:
                                RadarDesign.SnappedRect =
                                    ThisScreen.ScreenRect.ToCenterTop(RadarDesign.SnappedRect.width,
                                            RadarDesign.SnappedRect.width, 0, posOffSetByScale)
                                        .AddRect(0, Screen.height - RadarDesign.SnappedRect.size.y);
                                RadarDesign.renderingCamera.orthographic = false;
                                break;
                        }
                        transform.localPosition = RadarDesign.renderingCamera.ScreenToWorldPoint(new Vector3(
                            RadarDesign.SnappedRect.center.x + RadarDesign.xPadding,
                            RadarDesign.SnappedRect.center.y - RadarDesign.yPadding,
                            RadarDesign3D.ConstantRadarRenderDistance));


                        break;
                }

                #endregion
            }
        }

        /// <summary>
        ///     Here we create the center object blip if one is to be created, else we fall back to using the radar itself (this
        ///     transform) as the center object
        /// </summary>
        private void CenterObjectBlip()
        {
            #region CenterObject BLip



            if (_RadarCenterObject3D.IsActive)
            {
                #region instancing center blip

                if (!_RadarCenterObject3D.Instanced)
                {
                    // if we are creating the center object as a sprite
                    var blip = new GameObject();

                    #region Sprite

                    if (_RadarCenterObject3D.UseTrackingLine)
                    {
                        var pivot = new GameObject("TrackingLinePivot");
                        pivot.transform.parent = blip.transform;
                        pivot.transform.localPosition = Vector3.zero;
                        pivot.transform.localRotation = new Quaternion(0, 0, 0, 0);
                        pivot.transform.localScale = Vector3.one;
                        pivot.transform.gameObject.layer = _RadarCenterObject3D.Layer;
                        CreateCenterObjectTrackingLine(_RadarCenterObject3D, pivot);
                        pivot.hideFlags = HideFlags.HideAndDontSave;
                        _RadarCenterObject3D.TrackingLine = pivot;

                        if (_RadarCenterObject3D.UseBaseTracker)
                        {
                            var baseTracker = new GameObject("BaseTracker");
                            baseTracker.AddComponent<SpriteRenderer>();
                            var baseTrackerRenderer = baseTracker.GetComponent<SpriteRenderer>();
                            baseTracker.transform.parent = pivot.transform;
                            baseTrackerRenderer.sprite = _RadarCenterObject3D.BaseTrackerSprite;
                            baseTrackerRenderer.color = _RadarCenterObject3D.BaseTrackerColour;
                            baseTrackerRenderer.material = _RadarCenterObject3D.BaseTrackerMaterial;
                            baseTracker.gameObject.layer = _RadarCenterObject3D.Layer;
                            baseTracker.transform.localScale = new Vector3(_RadarCenterObject3D.BaseTrackerSize,
                                _RadarCenterObject3D.BaseTrackerSize, _RadarCenterObject3D.BaseTrackerSize);
                            baseTracker.transform.localRotation = Quaternion.Euler(90F, 0, 0);
                            _RadarCenterObject3D.BaseTrackerObject = baseTracker;
                        }
                    }

                    if (_RadarCenterObject3D._CreateBlipAs == CreateBlipAs.AsSprite)
                    {
                        blip.transform.parent = RadarDesign.BlipsParentObject;
                        blip.name = _RadarCenterObject3D.Tag;
                        blip.transform.position = transform.position;
                        blip.gameObject.AddComponent<SpriteRenderer>();
                        var blipSpriteRenderer = blip.GetComponent<SpriteRenderer>();
                        blipSpriteRenderer.material = _RadarCenterObject3D.BlipMaterial;
                        blipSpriteRenderer.color = _RadarCenterObject3D.colour;
                        blipSpriteRenderer.sprite = _RadarCenterObject3D.BlipSprite;
                        blipSpriteRenderer.sortingOrder = _RadarCenterObject3D.OrderInLayer;
                        blip.GetComponent<Transform>().gameObject.layer = _RadarCenterObject3D.Layer;
                        _RadarCenterObject3D.CenterBlip = blip;
                        _RadarCenterObject3D.Instanced = true;
                    }

                    #endregion

                    // if we are creating the center object as a mesh

                    #region Mesh

                    if (_RadarCenterObject3D._CreateBlipAs == CreateBlipAs.AsMesh)
                    {
                        blip.transform.parent = RadarDesign.BlipsParentObject;
                        blip.name = _RadarCenterObject3D.Tag;
                        blip.transform.position = transform.position;
                        blip.gameObject.AddComponent<MeshFilter>();
                        blip.gameObject.GetComponent<MeshFilter>().mesh =
                            _RadarCenterObject3D.mesh;
                        blip.gameObject.AddComponent<MeshRenderer>();
                        var blipMeshRenderer = blip.GetComponent<MeshRenderer>();
                        blipMeshRenderer.materials =
                            _RadarCenterObject3D.MeshMaterials;
                        blipMeshRenderer.shadowCastingMode =
                            ShadowCastingMode.Off;
                        blipMeshRenderer.receiveShadows = false;
                        blip.GetComponent<Transform>().gameObject.layer =
                            _RadarCenterObject3D.Layer;
                        _RadarCenterObject3D.CenterBlip = blip;
                        _RadarCenterObject3D.Instanced = true;
                    }

                    #endregion

                    // if we are creating the center object as a prefab

                    #region Prefab

                    if (_RadarCenterObject3D._CreateBlipAs == CreateBlipAs.AsPrefab)
                    {
                        blip =
                            Instantiate(_RadarCenterObject3D.prefab, transform.position, Quaternion.identity).gameObject;
                        blip.transform.parent = RadarDesign.BlipsParentObject;
                        blip.transform.position = transform.position;
                        blip.name = _RadarCenterObject3D.Tag;
                        blip.GetComponent<Transform>().gameObject.layer =
                            _RadarCenterObject3D.Layer;
                        _RadarCenterObject3D.CenterBlip = blip;
                        _RadarCenterObject3D.Instanced = true;
                    }

                    #endregion


                }

                #endregion

                #region Setting CenterObject


                try
                {
                    _RadarCenterObject3D.CenterObject =
                        GameObject.FindGameObjectWithTag(_RadarCenterObject3D.Tag).transform;
                }
                catch
                {
                    _RadarCenterObject3D.CenterObject = !RadarDesign._3DSystemsWithScreenSpaceFunction
                        ? transform
                        : RadarDesign.camera.transform;
                }


                #endregion

                #region calculate distance and determine action to take based on the center blip distance 

                // var distance = Vector3.Distance(transform.position + (RadarDesign.Pan / RadarDesign.SceneScale), transform.position);
                var distance = Vector3.Distance(Vector3.zero, RadarDesign.Pan) / RadarDesign.SceneScale;

                // we calculate the distance between the blip and this transform 
                var centerBlipTransform = _RadarCenterObject3D.CenterBlip.transform;


                var currentWorkingCenterObjectBlipActiveState =
                    _RadarCenterObject3D.CenterBlip.gameObject.activeInHierarchy;

                if (distance > RadarDesign.TrackingBounds)
                {
                    // here we check if AlwaysShowCenterObject is true. If it is, we will let the center object remain in the radar 
                    if (_RadarCenterObject3D.AlwaysShowCenterObject)
                    {
                        if (!currentWorkingCenterObjectBlipActiveState)
                            _RadarCenterObject3D.CenterBlip.gameObject.SetActive(true);
                    }
                    else
                    {
                        if (currentWorkingCenterObjectBlipActiveState)
                            _RadarCenterObject3D.CenterBlip.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (!currentWorkingCenterObjectBlipActiveState)
                        _RadarCenterObject3D.CenterBlip.gameObject.SetActive(true);
                }

                #endregion

                #region Position Tracking

                var centerObjectOffset =
    (!_RadarCenterObject3D.AlwaysShowCenterObject) ?
    (RadarDesign.Pan / RadarDesign.SceneScale)
    :
    Vector3.ClampMagnitude(RadarDesign.Pan / RadarDesign.SceneScale, RadarDesign.TrackingBounds);

                centerBlipTransform.localPosition = transform.InverseTransformPoint(transform.position + centerObjectOffset) * transform.lossyScale.x;

                #endregion

                #region Rotation tracking

                if (_RadarCenterObject3D.IsTrackRotation)
                {
                    centerBlipTransform.localEulerAngles = new Vector3(

                    #region Cleck if we should lock X Rotation
                        _RadarCenterObject3D.lockX ? 0 : _RadarCenterObject3D.CenterObject.transform.eulerAngles.x,

                    #endregion
                    #region Cleck if we should lock Y Rotation
                        _RadarCenterObject3D.lockY ? 0 : _RadarCenterObject3D.CenterObject.transform.eulerAngles.y,

                    #endregion
                    #region Cleck if we should lock Z Rotation
                        _RadarCenterObject3D.lockZ ? 0 : _RadarCenterObject3D.CenterObject.transform.eulerAngles.z
                    #endregion

                    );
                    if (_RadarCenterObject3D.UseTrackingLine)
                        _RadarCenterObject3D.TrackingLine.transform.rotation = transform.rotation;
                }
                else if (_RadarCenterObject3D.UseCustomRotation)
                {
                    centerBlipTransform.localEulerAngles = new Vector3(_RadarCenterObject3D.CustomXRotation, _RadarCenterObject3D.CustomYRotation, _RadarCenterObject3D.CustomZRotation);
                    if (_RadarCenterObject3D.UseTrackingLine)
                        _RadarCenterObject3D.TrackingLine.transform.rotation = transform.rotation;
                }
                else
                {
                    centerBlipTransform.rotation = new Quaternion(0, 0, 0, 0);
                    if (_RadarCenterObject3D.UseTrackingLine)
                        _RadarCenterObject3D.TrackingLine.transform.rotation = new Quaternion(0, 0, 0, 0);
                }

                #endregion

                #region Scale Tracking



                var workingBlipScale = _RadarCenterObject3D.CenterObjectCanScaleByDistance ? _RadarCenterObject3D.BlipMinSize : _RadarCenterObject3D.BlipSize;

                var dynamicScale =
                    (distance < RadarDesign.TrackingBounds) ?

                    _RadarCenterObject3D.CenterObjectCanScaleByDistance ?
                               Mathf.Clamp((_RadarCenterObject3D.BlipMaxSize - (_RadarCenterObject3D.BlipMinSize * ((distance) / RadarDesign.TrackingBounds)))
                             , _RadarCenterObject3D.BlipMinSize
                             , _RadarCenterObject3D.BlipMaxSize)

                             :
                             _RadarCenterObject3D.BlipSize
                             :
                             _RadarCenterObject3D.AlwaysShowCenterObject ?
                              Mathf.Clamp(workingBlipScale - (workingBlipScale * (distance - RadarDesign.TrackingBounds))
                                        , workingBlipScale
                                        , workingBlipScale)
                                        :
                                        _RadarCenterObject3D.BlipSize
                             ;



                centerBlipTransform.localScale = new Vector3(dynamicScale, dynamicScale, dynamicScale);


                #endregion

                #region Tracking Lines

                if (!_RadarCenterObject3D.UseTrackingLine) return;


                if (distance > RadarDesign.TrackingBounds)
                {
                    if (_RadarCenterObject3D.UseTrackingLine) _RadarCenterObject3D.TrackingLine.SetActive(false);
                }
                else
                {
                    if (_RadarCenterObject3D.UseTrackingLine) _RadarCenterObject3D.TrackingLine.SetActive(true);
                }

                var currentWorkingTrackingLine = _RadarCenterObject3D.TrackingLine;
                var currentWorkingBaseTracker = _RadarCenterObject3D.UseBaseTracker ? _RadarCenterObject3D.BaseTrackerObject : null;

                var start = new Vector3
                (
                    0,
                    (transform.InverseTransformPoint(centerBlipTransform.position).y / dynamicScale)

                    , 0
                );
                var end = Vector3.zero;
                //Set the begin and the end of the line renderer
                var line = currentWorkingTrackingLine.GetComponent<LineRenderer>();
                line.SetPosition(0, -start);
                line.SetPosition(1, end);

                #region BaseTracker(incicates the point where the base of the tracking line meets the radar)
                if (_RadarCenterObject3D.UseBaseTracker)
                {
                    if (currentWorkingBaseTracker)
                        currentWorkingBaseTracker.transform.localPosition = -start;
                }

                #endregion

                #endregion 


            }
            else
            {
                if (_RadarCenterObject3D.CenterBlip != null)
                    if (_RadarCenterObject3D.CenterBlip.gameObject.activeInHierarchy)
                        _RadarCenterObject3D.CenterBlip.gameObject.SetActive(false);

                if (RadarDesign._3DSystemsWithScreenSpaceFunction &&
                    _RadarCenterObject3D.CenterObject != RadarDesign.camera.transform)
                    _RadarCenterObject3D.CenterObject = RadarDesign.camera.transform;

                if (!RadarDesign._3DSystemsWithScreenSpaceFunction && _RadarCenterObject3D.CenterObject != transform)
                    _RadarCenterObject3D.CenterObject = transform;
            }


            #endregion
        }

        /// <summary>
        /// </summary>
        /// <param name="item"></param>
        /// <param name="pivot"></param>
        private static void CreateCenterObjectTrackingLine(RadarCenterObject3D item, GameObject pivot)
        {
            var line = pivot.AddComponent<LineRenderer>();

            line.positionCount = 2;
            line.endWidth =
                line.startWidth = item.TrackingLineDimention;
            line.useWorldSpace = false;
            // trackingLine.useLightProbes = false;
            line.receiveShadows = false;
            line.shadowCastingMode = ShadowCastingMode.Off;
            line.startColor = item.TrackingLineStartColour;
            line.endColor = item.TrackingLineEndColour;
            if (item.TrackingLineMaterial)
                line.material = item.TrackingLineMaterial;
        }

        /// <summary>
        ///     Here we go about creating all other radar blips
        /// </summary>
        private void CheckAndSetBlips()
        {
            #region Check and Set Blips

            // var outerBounds = RadarDesign.TrackingBounds * transform.lossyScale.x;
            //  var innerBounds = RadarDesign.InnerCullingZone * transform.lossyScale.x;
            // we go through all the blip type you create and then...
            foreach (var item in Blips)
            {
                var trackedObjectCount = item.gos.Count;
                var trackedRadarObjectCount = item.RadarObjectToTrack.Count;
                #region Removal of BLips when using Recursive Optimization 

                if (item.optimization.objectFindingMethod == ObjectFindingMethod.Recursive)
                {
                    for (var i = 0; i < trackedObjectCount; i++)
                    {
                        var currentWorkingObject = item.gos[i];


                        if (currentWorkingObject == null)
                        {
                            DestroyImmediate(item.RadarObjectToTrack[i].gameObject);
                            item.DoRemoval = true;
                        }

                        if (item.optimization.RemoveBlipsOnTagChange)
                            if (currentWorkingObject != null)
                                if (currentWorkingObject.tag != item.Tag)
                                {

                                    DestroyImmediate(item.RadarObjectToTrack[i].gameObject);
                                    item.DoRemoval = true;
                                }

                        if (item.optimization.RemoveBlipsOnDisable)
                            if (currentWorkingObject != null)
                                if (!currentWorkingObject.activeInHierarchy)
                                {

                                    DestroyImmediate(item.RadarObjectToTrack[i].gameObject);
                                    item.DoRemoval = true;
                                }
                    }

                    if (item.DoRemoval)
                    {
                        item.RadarObjectToTrack.RemoveAll(x => x == null);
                        item.TrackingLineObject.RemoveAll(x => x == null);
                        item.BaseTrackers.RemoveAll(x => x == null);
                    }
                }



                #endregion

                #region Find all objects that we want to track 

                switch (item.optimization.objectFindingMethod)
                {
                    case ObjectFindingMethod.Pooling:
                        if (item.optimization.SetPoolSizeManually)
                        {
                            if (BeginObjectInstanceCheck || trackedObjectCount < item.optimization.poolSize)
                            {
                                item.gos = GameObject.FindGameObjectsWithTag(item.Tag).ToList(); //DO AT END OF FRAME !!
                                if (item.optimization.RecalculatePoolSizeBasedOnFirstFoundObjects)
                                    if (item.optimization.poolSize > trackedObjectCount)
                                        item.optimization.poolSize = trackedObjectCount = item.gos.Count;


                            }
                        }
                        else
                        {
                            if (BeginObjectInstanceCheck)
                            {
                                item.gos = GameObject.FindGameObjectsWithTag(item.Tag).ToList(); //DO AT END OF FRAME !!

                            }
                        }

                        break;
                    case ObjectFindingMethod.Recursive:
                        if (item.optimization.RequireInstanceObjectCheck)
                        {

                            if (BeginObjectInstanceCheck || item.DoRemoval)
                            {
                                item.gos = GameObject.FindGameObjectsWithTag(item.Tag).ToList(); //DO AT END OF FRAME !!

                            }
                        }
                        else
                        {

                            item.gos = GameObject.FindGameObjectsWithTag(item.Tag).ToList(); //DO AT END OF FRAME !!
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                #endregion


                #region Ensure that our tracking line count and TrackedObjects= count are always correct ONLY FOR RECURSIVE OPTIMIZATION METHOD

                if (item.DoRemoval)
                    item.DoRemoval = false;

                trackedObjectCount = item.gos.Count;
                #endregion


                // if the blip type is set to active the following will execute
                if (item.IsActive)
                {
                    #region Instancing blips

                    // we will begin populating the list which will hold the blips by checking if the amount of things in the blip list is equal to the amount of theing we want to track
                    for (var a = trackedRadarObjectCount; a < trackedObjectCount; a++)
                    {

                        #region Sprites

                        if (item._CreateBlipAs == CreateBlipAs.AsSprite)
                        {
                            var blip = new GameObject();


                            //here we check if we need to add a pivot gameobject to the blip . This is for blips that use tracking lines
                            if (item.UseTrackingLine)
                            {
                                var pivot = new GameObject("TrackingLinePivot");
                                pivot.transform.parent = blip.transform;
                                pivot.transform.localPosition = Vector3.zero;
                                pivot.transform.localRotation = new Quaternion(0, 0, 0, 0);
                                pivot.transform.localScale = Vector3.one;
                                pivot.transform.gameObject.layer = item.Layer;
                                CreateTrackingLine(item, pivot);
                                pivot.hideFlags = HideFlags.HideAndDontSave;
                                item.TrackingLineObject.Add(pivot);


                                if (item.UseBaseTracker)
                                {
                                    var baseTracker = new GameObject("BaseTracker").AddComponent<SpriteRenderer>();
                                    var baseTrackerRenderer = baseTracker.GetComponent<SpriteRenderer>();
                                    baseTracker.transform.parent = pivot.transform;
                                    baseTrackerRenderer.sprite = item.BaseTrackerSprite;
                                    baseTrackerRenderer.color = item.BaseTrackerColour;
                                    baseTrackerRenderer.sharedMaterial = item.BaseTrackerMaterial;
                                    baseTracker.gameObject.layer = item.Layer;
                                    baseTracker.transform.localScale = new Vector3(item.BaseTrackerSize,
                                        item.BaseTrackerSize, item.BaseTrackerSize);
                                    baseTracker.transform.localRotation = Quaternion.Euler(90F, 0, 0);
                                    item.BaseTrackers.Add(baseTracker.gameObject);
                                }
                            }


                            blip.transform.parent = RadarDesign.BlipsParentObject;
                            blip.transform.position = transform.position;
                            blip.name = item.Tag;
                            blip.AddComponent<SpriteRenderer>();
                            blip.GetComponent<SpriteRenderer>().sharedMaterial = item.BlipMaterial;
                            blip.GetComponent<SpriteRenderer>().color = item.colour;
                            blip.GetComponent<SpriteRenderer>().sprite = item.BlipSprite;
                            blip.GetComponent<Transform>().gameObject.layer = item.Layer;
                            blip.GetComponent<SpriteRenderer>().sortingOrder = item.OrderInLayer;
                            blip.transform.localScale = new Vector3(item.BlipSize, item.BlipSize, item.BlipSize);
                            item.RadarObjectToTrack.Add(blip.transform);
                            //  item.BlipContent.Add(a, new UnityEngine.Object[] { });


                        }

                        #endregion

                        #region Mesh

                        if (item._CreateBlipAs == CreateBlipAs.AsMesh)
                        {
                            var blip = new GameObject();
                            //here we check if we need to add a pivot gameobject to the blip . This is for blips that use tracking lines
                            if (item.UseTrackingLine)
                            {
                                var pivot = new GameObject("TrackingLinePivot");
                                pivot.transform.parent = blip.transform;
                                pivot.transform.localPosition = Vector3.zero;
                                pivot.transform.localRotation = new Quaternion(0, 0, 0, 0);
                                pivot.transform.localScale = Vector3.one;
                                pivot.layer = item.Layer;
                                CreateTrackingLine(item, pivot);
                                pivot.hideFlags = HideFlags.HideAndDontSave;
                                item.TrackingLineObject.Add(pivot);
                                if (item.UseBaseTracker)
                                {
                                    var baseTracker = new GameObject("BaseTracker").AddComponent<SpriteRenderer>();
                                    var baseTrackerRenderer = baseTracker.GetComponent<SpriteRenderer>();
                                    baseTracker.transform.parent = pivot.transform;
                                    baseTrackerRenderer.sprite = item.BaseTrackerSprite;
                                    baseTrackerRenderer.color = item.BaseTrackerColour;
                                    baseTrackerRenderer.material = item.BaseTrackerMaterial;
                                    baseTracker.gameObject.layer = item.Layer;
                                    baseTracker.transform.localScale = new Vector3(item.BaseTrackerSize,
                                        item.BaseTrackerSize, item.BaseTrackerSize);
                                    baseTracker.transform.localRotation = Quaternion.Euler(90F, 0, 0);
                                    item.BaseTrackers.Add(baseTracker.gameObject);
                                }
                            }
                            blip.transform.parent = RadarDesign.BlipsParentObject;
                            blip.transform.position = transform.position;
                            blip.name = item.Tag;
                            blip.AddComponent<MeshFilter>();
                            blip.GetComponent<MeshFilter>().mesh = item.mesh;
                            blip.AddComponent<MeshRenderer>();
                            var blipMeshRenderer = blip.GetComponent<MeshRenderer>();
                            blipMeshRenderer.materials = item.MeshMaterials;
                            blipMeshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                            blipMeshRenderer.receiveShadows = false;
                            blip.GetComponent<Transform>().gameObject.layer = item.Layer;
                            blip.transform.localScale = new Vector3(item.BlipSize, item.BlipSize, item.BlipSize);
                            item.RadarObjectToTrack.Add(blip.transform);



                        }

                        #endregion

                        #region Prefab

                        if (item._CreateBlipAs != CreateBlipAs.AsPrefab) continue;
                        {
                            var blip = Instantiate(item.prefab, transform.position, Quaternion.identity);
                            //here we check if we need to add a pivot gameobject to the blip . This is for blips that use tracking lines
                            if (item.UseTrackingLine)
                            {
                                var pivot = new GameObject("TrackingLinePivot");
                                pivot.transform.parent = blip.transform;
                                pivot.transform.localPosition = Vector3.zero;
                                pivot.transform.localRotation = new Quaternion(0, 0, 0, 0);
                                pivot.transform.localScale = Vector3.one;
                                pivot.layer = item.Layer;
                                CreateTrackingLine(item, pivot);
                                pivot.hideFlags = HideFlags.HideAndDontSave;
                                item.TrackingLineObject.Add(pivot);
                                if (item.UseBaseTracker)
                                {
                                    var baseTracker = new GameObject("BaseTracker").AddComponent<SpriteRenderer>();
                                    var baseTrackerRenderer = baseTracker.GetComponent<SpriteRenderer>();
                                    baseTracker.transform.parent = pivot.transform;
                                    baseTrackerRenderer.sprite = item.BaseTrackerSprite;
                                    baseTrackerRenderer.color = item.BaseTrackerColour;
                                    baseTrackerRenderer.material = item.BaseTrackerMaterial;
                                    baseTracker.gameObject.layer = item.Layer;
                                    baseTracker.transform.localScale = new Vector3(item.BaseTrackerSize,
                                        item.BaseTrackerSize, item.BaseTrackerSize);
                                    baseTracker.transform.localRotation = Quaternion.Euler(90F, 0, 0);
                                    item.BaseTrackers.Add(baseTracker.gameObject);
                                }
                            }
                            blip.transform.parent = RadarDesign.BlipsParentObject;
                            blip.transform.position = transform.position;
                            blip.name = item.Tag;
                            blip.GetComponent<Transform>().gameObject.layer = item.Layer;
                            blip.transform.localScale = new Vector3(item.BlipSize, item.BlipSize, item.BlipSize);
                            item.RadarObjectToTrack.Add(blip.transform);


                        }

                        #endregion
                    }

                    #endregion

                    if (item.RadarObjectToTrack.Count < trackedObjectCount)
                        return;
                    // we then sort through all the object in the gos list
                    for (var i = 0; i < trackedObjectCount; i++)
                    {
                        var currentWorkingObject = item.gos[i];
                        var currentWorkingRadarObjectToTrack = item.RadarObjectToTrack[i];
                        var currentWorkingRadarObjectToTrackGameObject = currentWorkingRadarObjectToTrack.gameObject;
                        var isRadarObjectToTrackActiveInHeirarchy =
                            currentWorkingRadarObjectToTrackGameObject.activeInHierarchy;
                        var currentWorkingTrackingLine = item.UseTrackingLine
                            ? item.TrackingLineObject[i]
                            : null;
                        var currentWorkingBaseTracker = item.UseBaseTracker ? item.BaseTrackers[i] : null;

                        #region Removal of blips when using Pooling optimization mode

                        if (item.optimization.objectFindingMethod == ObjectFindingMethod.Pooling)
                        {
                            // if you ae using pooling then you should not be destroying your objets anyway. if you do. this annoying code will run
                            if (currentWorkingObject == null)
                            {
                                // remove and add back to the list

                                // Move tracking lines , blips and base trackers to the front of their respective lists This is a fast process
                                item.TrackingLineObject.Remove(currentWorkingTrackingLine);
                                item.TrackingLineObject.Add(currentWorkingTrackingLine);

                                if (item.UseTrackingLine && item.UseBaseTracker)
                                {
                                    item.BaseTrackers.Remove(currentWorkingBaseTracker);
                                    item.BaseTrackers.Add(currentWorkingBaseTracker);
                                }
                                item.RadarObjectToTrack.Remove(currentWorkingRadarObjectToTrack);
                                item.RadarObjectToTrack.Add(currentWorkingRadarObjectToTrack);
                                foreach (var b in item.RadarObjectToTrack)
                                    b.gameObject.SetActive(false);

                                item.gos = GameObject.FindGameObjectsWithTag(item.Tag).ToList();
                                //DO AT END OF FRAME !! // dont let it come to this :( EVEN THOUGH THIS IS REALLY FASE :P
                                // in case you destroy objects that should be pooled we reset the pool size.
                                item.optimization.poolSize = item.gos.Count;




                                return;
                            }


                            if (item.optimization.RemoveBlipsOnTagChange)
                                if (currentWorkingObject != null)
                                    if (currentWorkingObject.tag != item.Tag)
                                    {
                                        // remove and add back to the list

                                        // Move tracking lines , blips and base trackers to the front of their respective lists This is a fast process
                                        item.TrackingLineObject.Remove(currentWorkingTrackingLine);
                                        item.TrackingLineObject.Add(currentWorkingTrackingLine);
                                        if (item.UseTrackingLine && item.UseBaseTracker)
                                        {
                                            item.BaseTrackers.Remove(currentWorkingBaseTracker);
                                            item.BaseTrackers.Add(currentWorkingBaseTracker);
                                        }
                                        item.RadarObjectToTrack.Remove(currentWorkingRadarObjectToTrack);
                                        item.RadarObjectToTrack.Add(currentWorkingRadarObjectToTrack);
                                        foreach (var b in item.RadarObjectToTrack)
                                            b.gameObject.SetActive(false);

                                        item.gos = GameObject.FindGameObjectsWithTag(item.Tag).ToList();
                                        //DO AT END OF FRAME !! // dont let it come to this :( EVEN THOUGH THIS IS REALLY FASE :P
                                        // in case you destroy objects that should be pooled we reset the pool size.
                                        item.optimization.poolSize = item.gos.Count;



                                        return;
                                    }

                            if (item.optimization.RemoveBlipsOnDisable)
                                if (currentWorkingObject != null)
                                    if (!currentWorkingObject.activeInHierarchy)
                                        currentWorkingRadarObjectToTrackGameObject.SetActive(false);
                        }

                        #endregion


                        var currentWorkingObjectTransform = currentWorkingObject.transform;
                        var currentWorkingTrackingLinePosition = currentWorkingTrackingLine
                            ? currentWorkingTrackingLine.transform
                            : null;
                        var trackedObjectsPoition = currentWorkingObjectTransform.position;
                        var currentWorkingRadarObjectToTrackTransform = currentWorkingRadarObjectToTrack.transform;
                        var centerObjectsPosition = _RadarCenterObject3D.CenterObject.transform.position;
                        var thisTransformPosition = transform.position;
                        var currentRadarObjectToTrackPosition = currentWorkingRadarObjectToTrack.position;

                        #region calculate distance and determine action to take based on the blip distance 



                        // the distance each blip is from the center of the radar is calculated now that we have access to everything we want to track

                        var distance = Vector3.Distance(centerObjectsPosition, trackedObjectsPoition + RadarDesign.Pan) / RadarDesign.SceneScale;

                        if (distance > RadarDesign.TrackingBounds)
                        {
                            if (item.AlwaysShowBlipsInRadarSpace)
                            {
                                if (!isRadarObjectToTrackActiveInHeirarchy && currentWorkingObject.activeInHierarchy)
                                    currentWorkingRadarObjectToTrackGameObject.SetActive(true);

                                if (item.UseTrackingLine)
                                    currentWorkingTrackingLine.SetActive(false);
                            }
                            else
                            {
                                currentWorkingRadarObjectToTrackGameObject.SetActive(false);
                            }

                        }
                        else
                        {
                            if (RadarDesign.InnerCullingZone > 0)
                            {
                                if (distance < RadarDesign.InnerCullingZone)
                                {
                                    if (isRadarObjectToTrackActiveInHeirarchy)
                                        currentWorkingRadarObjectToTrackGameObject.SetActive(false);
                                }
                            }
                            if (distance >= RadarDesign.InnerCullingZone && currentWorkingObject.activeInHierarchy)
                            {
                                if (!isRadarObjectToTrackActiveInHeirarchy)
                                    currentWorkingRadarObjectToTrackGameObject.SetActive(true);


                                if (item.UseTrackingLine)
                                    if (!currentWorkingTrackingLine.activeInHierarchy)
                                        currentWorkingTrackingLine.SetActive(true);
                            }
                        }






                        #endregion

                        #region Position Tracking


                        var playerpos = (centerObjectsPosition - RadarDesign.Pan) / RadarDesign.SceneScale;

                        var radarBlipPos = trackedObjectsPoition / RadarDesign.SceneScale + thisTransformPosition;




                        var localYOffset = item.UseHeightTracking ? (!item.KeepBlipsAboveRadarPlane ? transform.InverseTransformPoint(radarBlipPos - playerpos).y * transform.lossyScale.x : Mathf.Clamp(transform.InverseTransformPoint(radarBlipPos - playerpos).y * transform.lossyScale.x, 0, Mathf.Infinity)) : 0;
                        // nV3 just mean new vector3
                        var nV3 = new Vector3(
                            transform.InverseTransformPoint(radarBlipPos - playerpos).x * transform.lossyScale.x,
                            localYOffset,
                           RadarDesign.TrackYPosition ? (transform.InverseTransformPoint(radarBlipPos - playerpos).y * -1) * transform.lossyScale.x : transform.InverseTransformPoint(radarBlipPos - playerpos).z * transform.lossyScale.x);


                        currentWorkingRadarObjectToTrackTransform.localPosition = (!item.AlwaysShowBlipsInRadarSpace) ?
                           nV3 : Vector3.ClampMagnitude(nV3, RadarDesign.TrackingBounds);



                        /*  currentWorkingRadarObjectToTrackTransform.localPosition = (!item.AlwaysShowBlipsInRadarSpace) ?

                              transform.InverseTransformPoint(radarBlipPos - playerpos) * transform.lossyScale.x
                              :
                              Vector3.ClampMagnitude(transform.InverseTransformPoint(radarBlipPos - playerpos) * transform.lossyScale.x, RadarDesign.TrackingBounds);*/


                        #endregion

                        #region Rotation Tracking

                        if (item.IsTrackRotation)
                        {
                            currentWorkingRadarObjectToTrackTransform.localEulerAngles = new Vector3(

                            #region check if we should lock X Rotation
                                        item.lockX ? 0 : currentWorkingObjectTransform.eulerAngles.x,

                            #endregion
                            #region check if we should lock Y Rotation+
                                        item.lockY ? 0 : currentWorkingObjectTransform.eulerAngles.y,

                            #endregion
                            #region check if we should lock Z Rotation
                                        item.lockZ ? 0 : currentWorkingObjectTransform.eulerAngles.z)
                                ;

                            #endregion

                            if (item.UseTrackingLine)
                                currentWorkingTrackingLinePosition.rotation = transform.rotation;
                        }
                        else if (item.UseCustomRotation)
                        {

                            currentWorkingRadarObjectToTrackTransform.localEulerAngles = new Vector3(item.CustomXRotation, item.CustomYRotation, item.CustomZRotation);
                            if (item.UseTrackingLine)
                                currentWorkingTrackingLinePosition.rotation = transform.rotation;
                        }
                        else
                        {
                            currentWorkingRadarObjectToTrackTransform.rotation = new Quaternion(0, 0, 0, 0);
                            if (item.UseTrackingLine)
                                currentWorkingTrackingLinePosition.rotation = new Quaternion(0, 0, 0, 0);
                        }

                        #endregion

                        #region Scale Trcking
                        var workingBlipScale = item.BlipCanScleBasedOnDistance ? item.BlipMinSize : item.BlipSize;

                        var dynamicScale =
                            (distance < RadarDesign.TrackingBounds) ?

                            item.BlipCanScleBasedOnDistance ?
                                       Mathf.Clamp((item.BlipMaxSize - (item.BlipMaxSize * ((distance) / RadarDesign.TrackingBounds)))
                                     , item.BlipMinSize
                                     , item.BlipMaxSize)

                                     :
                                     item.BlipSize
                                     :
                                     item.AlwaysShowBlipsInRadarSpace ?
                                      Mathf.Clamp(workingBlipScale - (workingBlipScale * (distance - RadarDesign.TrackingBounds))
                                                , workingBlipScale
                                                , workingBlipScale)
                                                :
                                                item.BlipSize
                                     ;



                        currentWorkingRadarObjectToTrackTransform.localScale = new Vector3(dynamicScale, dynamicScale, dynamicScale);



                        #endregion

                        #region Tracking Lines

                        if (item.UseTrackingLine)
                        {

                            var start = new Vector3
                            (
                                0,
                                (transform.InverseTransformPoint(currentWorkingRadarObjectToTrackTransform.position)
                                     .y / dynamicScale)
                                , 0
                            );


                            var end = Vector3.zero;

                            //Set the begin and the end of the line renderer
                            var line = currentWorkingTrackingLine.GetComponent<LineRenderer>();
                            line.SetPosition(0, -start);
                            line.SetPosition(1, end);

                            #region BaseTracker(incicates the point where the base of the tracking line meets the radar)
                            if (item.UseBaseTracker)
                            {
                                if (currentWorkingBaseTracker)
                                    currentWorkingBaseTracker.transform.localPosition = -start;
                            }

                            #endregion
                        }

                        #endregion

                        #region Flip the blip once it crosses into - or +

                        if (item._CreateBlipAs == CreateBlipAs.AsSprite)
                        {
                            var blipSpriteRenderer =
                                currentWorkingRadarObjectToTrack.GetComponent<SpriteRenderer>();
                            // caclulate opposite side height to determine if the blip should be flipped

                            if (transform.InverseTransformPoint(currentRadarObjectToTrackPosition).y /
                                RadarDesign.SceneScale > 0 &&
                                blipSpriteRenderer.flipY)
                                blipSpriteRenderer.flipY = false;
                            if (transform.InverseTransformPoint(currentRadarObjectToTrackPosition).y /
                                RadarDesign.SceneScale < 0 &&
                                blipSpriteRenderer.flipY == false)
                                blipSpriteRenderer.flipY = true;
                        }

                        #endregion

                        #region Set mesh and use LOD if LOD is to be used
                        // Debug.Log(item.UseLOD);
                        if (item.UseLOD && item._CreateBlipAs == CreateBlipAs.AsMesh)
                        {

                            if (distance > item.MediumDistance)
                                currentWorkingRadarObjectToTrack.GetComponent<MeshFilter>().mesh = item.Low;

                            if (distance < item.MediumDistance && distance > item.HighDistance)
                                currentWorkingRadarObjectToTrack.GetComponent<MeshFilter>().mesh = item.Medium;

                            if (distance < item.HighDistance)
                                currentWorkingRadarObjectToTrack.GetComponent<MeshFilter>().mesh = item.High;
                        }

                        #endregion


                    }
                }
                else
                {
                    #region turn off all blips if blip type is set to be inactive

                    if (trackedRadarObjectCount != 0)
                    {

                        for (var i = 0; i < trackedObjectCount; i++)
                        {

                            item.RadarObjectToTrack[i].gameObject.SetActive(false);
                        }

                    }

                    #endregion
                }

                if (item.RadarObjectToTrack.Count > trackedObjectCount)
                {
                    var difCount = item.RadarObjectToTrack.Count - trackedObjectCount;

                    for (int x = 0; x < difCount; x++)
                    {
                        Destroy(item.RadarObjectToTrack[x].gameObject);
                        item.RadarObjectToTrack[x] = null;
                        if (item.UseTrackingLine)
                        {
                            Destroy(item.TrackingLineObject[x].gameObject);
                            item.TrackingLineObject[x] = null;
                        }

                    }
                    item.RadarObjectToTrack.RemoveAll(x => x == null);
                    if (item.UseTrackingLine)
                        item.TrackingLineObject.RemoveAll(x => x == null);
                }

            }

            #endregion

            // finally just set the BeginObjectInstanceCheck to false if it was true

            if (BeginObjectInstanceCheck) BeginObjectInstanceCheck = false;



        }

        /// <summary>
        ///     called inside CheckAndSetBLips at the mment a blip is created that uses tracking lines
        /// </summary>
        /// <param name="item"></param>
        /// <param name="pivot"></param>
        private static void CreateTrackingLine(RadarBlips3D item, GameObject pivot)
        {
            var line = pivot.AddComponent<LineRenderer>();

            line.positionCount = 2;
            line.endWidth =
                line.startWidth = item.TrackingLineDimention;
            line.useWorldSpace = false;
            // trackingLine.useLightProbes = false;
            line.receiveShadows = false;
            line.shadowCastingMode = ShadowCastingMode.Off;
            line.startColor = item.TrackingLineStartColour;
            line.endColor = item.TrackingLineEndColour;
            if (item.TrackingLineMaterial)
                line.sharedMaterial = item.TrackingLineMaterial;
        }

        /// <summary>
        ///     here we set up and reposition the minimap
        /// </summary>
        public void Minimap()
        {
            if (!RadarDesign._3DSystemsWithMinimapFunction) return;
            var centerObjectsPosition = _RadarCenterObject3D.CenterObject.transform.position;
            // this was t obe used to reposition blips in the 3D Radar when the 3D  radar is in screen space, uses minimaps , tracking lines and is rotated at an angle but such a correction is not possible to this code can be removed.
            // the minimap and the blips for the 3D Radar are fundimentally different tracking systems , sure they can be combined but 
            var xRotationValue = transform.localEulerAngles.x > 180 ? -(360 - transform.localEulerAngles.x) : transform.localEulerAngles.x;
            var zRotationValue = transform.localEulerAngles.z > 180 ? -(360 - transform.localEulerAngles.z) : transform.localEulerAngles.z;
            var rotationOffset = new Vector3(zRotationValue * -0.065666666f, 0, xRotationValue * 0.065666666f);


            #region creating the minimap

            if (!minimapModule.generated)
            {
                var miniMap = new GameObject("MiniMap");
                miniMap.transform.parent = transform;
                miniMap.transform.localScale = Vector3.one;
                miniMap.transform.localPosition = Vector2.zero;
                var mapPivot = new GameObject("map Pivot");
                mapPivot.transform.parent = miniMap.transform;
                mapPivot.transform.localScale = Vector3.one;
                minimapModule.MapPivot = mapPivot;
                mapPivot.transform.localPosition = Vector3.zero;
                var map = new GameObject("map");
                map.transform.parent = mapPivot.transform;
                map.transform.localPosition = Vector2.zero;
                var mask = new GameObject("Minimap Mask");
                mask.transform.parent = miniMap.transform;
                mask.transform.localScale = Vector3.one;
                minimapModule.Mask = mask;
                var maskSpriteRenderer = mask.AddComponent<SpriteRenderer>();
                maskSpriteRenderer.sprite = minimapModule.MaskSprite();
                maskSpriteRenderer.sortingOrder = minimapModule.OrderInLayer;
                maskSpriteRenderer.sharedMaterial = minimapModule.MaskMaterial;
                mask.transform.Rotate(new Vector2(90, 0), Space.World);
                mask.transform.localPosition = new Vector3(0, -0.01f, 0);
                mask.layer = minimapModule.layer;
                minimapModule.Map = map;
                minimapModule.Map.transform.Rotate(new Vector2(90, 0), Space.World);

                if (minimapModule.mapType == MapType.Realtime)
                {
                    map.AddComponent<MeshFilter>().sharedMesh = minimapModule.ProceduralMapQuad();
                    // create a new masked material on the fly so that we dont overwrite any preexisting matrials in the project
                    // and use it as the map material
                    var mapRenderTextureMaterial = new Material(minimapModule.MapMaterial.shader)
                    {
                        mainTexture = minimapModule.renderTexture
                    };
                    // use the render texture as the main texture
                    // Add a mesh renderer and set its shared materia lto be the MapRenderTextureMaterial
                    var mapRrenderer = map.AddComponent<MeshRenderer>();
                    //
                    mapRrenderer.sharedMaterial = mapRenderTextureMaterial;
                    //
                    mapRrenderer.receiveShadows = false;
                    //
                    mapRrenderer.lightProbeUsage = LightProbeUsage.Off;
                    //
                    mapRrenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
                    //
                    mapRrenderer.sortingOrder = minimapModule.OrderInLayer;
                }
                else
                {
                    var mapSpriteRenderer = map.AddComponent<SpriteRenderer>();
                    mapSpriteRenderer.sprite = minimapModule.MapTexture;
                    mapSpriteRenderer.sharedMaterial = minimapModule.MapMaterial;
                    mapSpriteRenderer.sortingOrder = minimapModule.OrderInLayer;
                }

                map.layer = minimapModule.layer;
                minimapModule.SavedSceneScale = RadarDesign.SceneScale;
                minimapModule.SavedMapScale = minimapModule.MapScale;
                // here we set the scaling factor by taking the scaled map value and dividing scene scale into it, this gives us the 1 to 1 ratio of map size to scene scale at saved map scale
                if (!minimapModule.calibrate)
                    minimapModule.Scalingfactor = minimapModule.SavedMapScale / minimapModule.SavedSceneScale;
                minimapModule.generated = true;
            }

            #endregion

            #region Map Positioning

            if (minimapModule.mapType == MapType.Static)
            {
                if (minimapModule.Refreshedmap)
                {
                    minimapModule.StartPosition = centerObjectsPosition;
                    minimapModule.Refreshedmap = false;
                }

                minimapModule.Map.transform.localPosition = -(new Vector3((centerObjectsPosition.x - minimapModule.StartPosition.x) - RadarDesign.Pan.x, 0, (centerObjectsPosition.z - minimapModule.StartPosition.z) - RadarDesign.Pan.z) + rotationOffset)
                                                            / RadarDesign.SceneScale;
            }
            else
                minimapModule.RealtimeMinimapCamera.transform.position = new Vector3(
                    centerObjectsPosition.x - RadarDesign.Pan.x,
                    centerObjectsPosition.y - RadarDesign.Pan.y + minimapModule.CameraHeight,
                    centerObjectsPosition.z - RadarDesign.Pan.z) + rotationOffset;

            #endregion

            #region Map Scale

            if (minimapModule.calibrate) return;
            var changeInSceneScale = RadarDesign.SceneScale - minimapModule.SavedSceneScale;


            var scale = minimapModule.SavedMapScale -
                        changeInSceneScale * minimapModule.Scalingfactor /
                        (RadarDesign.SceneScale / minimapModule.SavedSceneScale);
            if (minimapModule.mapType == MapType.Static)
                minimapModule.Map.transform.localScale = new Vector3(scale, scale, scale);
            else
                minimapModule.RealtimeMinimapCamera.orthographicSize = RadarDesign.SceneScale;

            #endregion

            #region Map rotation
            minimapModule.MapPivot.transform.localEulerAngles = new Vector3(0, RadarDesign.RadarRotationOffset, 0);
            #endregion
        }

        /// <summary>
        ///  This allows you to set a new static map texture from a custom script
        /// </summary>
        /// <param name="sprite"></param>
        public void SwapStaticMinimapTexture(Sprite sprite)
        {
            minimapModule.Map.GetComponent<SpriteRenderer>().sprite = sprite;
            minimapModule.Refreshedmap = true;
        }

        /// <summary>
        ///     Here we determine what 'Design layers' are to be rotates and what object rotation they will match proportionally |
        ///     inversely and with a damping
        /// </summary>
        private void RotateSpecifics()
        {
            #region Rotating specific design layers

            if (RadarDesign.RotationTargets.Count <= 0) return;
            foreach (var rotationTarget in RadarDesign.RotationTargets)
            {
                if (rotationTarget.UseRotationTarget)
                {
                    // to ensure that we do not contine to set target objects we ch3ck if the target object is null, if it is then we do the following
                    if (!rotationTarget.Target)
                        switch (rotationTarget.ObjectToTrack)
                        {
                            case TargetObject.FindObject:
                                try
                                {
                                    if (rotationTarget.FindingName.Length > 0)
                                        rotationTarget.Target = GameObject.Find(rotationTarget.FindingName);
                                }
                                catch
                                {
                                    // ignored
                                }
                                break;
                            case TargetObject.ObjectWithTag:
                                try
                                {
                                    if (rotationTarget.tag.Length > 0)
                                        rotationTarget.Target = GameObject.FindWithTag(rotationTarget.tag);
                                }
                                catch
                                {
                                    // ignored
                                }
                                break;
                            case TargetObject.InstancedBlip:
                                try
                                {
                                    if (rotationTarget.InstancedObjectToTrackBlipName.Length > 0)
                                        rotationTarget.Target =
                                            RadarDesign.BlipsParentObject.transform.Find(rotationTarget.InstancedObjectToTrackBlipName)
                                                .gameObject;
                                }
                                catch
                                {
                                    // ignored
                                }
                                break;
                            case TargetObject.ThisObject:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                    switch (rotationTarget.target)
                    {
                        case TargetBlip.InstancedBlip:
                            try
                            {
                                if (rotationTarget.InstancedTargetBlipname.Length > 0)
                                    rotationTarget.TargetedObject =
                                     RadarDesign.BlipsParentObject.transform.Find(rotationTarget.InstancedTargetBlipname).gameObject;
                            }
                            catch
                            {
                                // ignored
                            }
                            break;
                        case TargetBlip.ThisObject:
                            // should already has targetobjectassigned
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (!rotationTarget.TargetedObject || !rotationTarget.Target) continue;
                    float x = 0, y = 0, z = 0;



                    switch (rotationTarget.retargetedXRotation)
                    {
                        case RetargetX.none:
                            x = rotationTarget.Target.transform.eulerAngles.x + rotationTarget.AddedXRotation;
                            break;
                        case RetargetX.X_to_Y:
                            x = rotationTarget.Target.transform.eulerAngles.y + rotationTarget.AddedYRotation;
                            break;
                        case RetargetX.X_to_Z:
                            x = rotationTarget.Target.transform.eulerAngles.z + rotationTarget.AddedZRotation;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    switch (rotationTarget.retargetedYRotation)
                    {
                        case RetargetY.none:
                            y = rotationTarget.Target.transform.eulerAngles.y + rotationTarget.AddedYRotation;
                            break;
                        case RetargetY.Y_to_X:
                            y = rotationTarget.Target.transform.eulerAngles.x + rotationTarget.AddedXRotation;
                            break;
                        case RetargetY.Y_to_Z:
                            y = rotationTarget.Target.transform.eulerAngles.z + rotationTarget.AddedZRotation;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    switch (rotationTarget.retargetedZRotation)
                    {
                        case RetargetZ.none:
                            z = rotationTarget.Target.transform.eulerAngles.z + rotationTarget.AddedZRotation;
                            break;
                        case RetargetZ.Z_to_X:
                            z = rotationTarget.Target.transform.eulerAngles.x + rotationTarget.AddedXRotation;
                            break;
                        case RetargetZ.Z_to_Y:
                            z = rotationTarget.Target.transform.eulerAngles.y + rotationTarget.AddedYRotation;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    switch (rotationTarget.rotations)
                    {
                        case Rotations.Proportional:


                            rotationTarget.TargetedObject.transform.eulerAngles = ((new Vector3(
                                rotationTarget.FreezeX
                                    ? rotationTarget.AddedXRotation : x,
                                 rotationTarget.FreezeY
                                    ? rotationTarget.AddedYRotation : y,
                                 rotationTarget.FreezeZ
                                    ? rotationTarget.AddedZRotation : z) / 100) * (100 - rotationTarget.RotationDamping));




                            break;

                        case Rotations.Inverse:

                            rotationTarget.TargetedObject.transform.eulerAngles = ((new Vector3(
                                      rotationTarget.FreezeX
                                          ? rotationTarget.AddedXRotation : x,
                                       rotationTarget.FreezeY
                                          ? rotationTarget.AddedYRotation : y,
                                       rotationTarget.FreezeZ
                                          ? rotationTarget.AddedZRotation : z) / 100) * (100 - rotationTarget.RotationDamping)) * -1;

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            #endregion
        }



        #region variables (View source code listed aove for reference to these 4 classes, if you wish to replave these variables you will have to rename the classes to prevent clashing )
        /// <summary>
        /// The object being displayed at the center of your radar
        /// </summary>
        [HideInInspector]
        public RadarCenterObject3D _RadarCenterObject3D;

        /// <summary>
        /// all other radar blips
        /// </summary>
        [HideInInspector]
        public List<RadarBlips3D> Blips = new List<RadarBlips3D>();

        /// <summary>
        ///  Radar Design area
        /// </summary>
        [HideInInspector]
        public RadarDesign3D RadarDesign = new RadarDesign3D();

        /// <summary>
        /// Minimap settings
        /// </summary>
        [HideInInspector]
        public MiniMapModule minimapModule = new MiniMapModule();

        #endregion
    }




}