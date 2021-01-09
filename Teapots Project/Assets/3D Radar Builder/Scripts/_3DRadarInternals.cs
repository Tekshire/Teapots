using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace DaiMangou.RadarBuilder3D
{


    #region SnapPosition
    [System.Serializable]
    /// <summary>
    /// Contains 9 values relating to areas of a rect.
    /// </summary>
    public enum SnapPosition
    {

        TopLeft = 0,
        TopRight = 1,
        BottomLeft = 2,
        BottomRight = 3,
        TopMiddle = 4,
        BottomMiddle = 5,
        LeftMiddle = 6,
        RightMiddle = 7,
        Center = 8

    }
    #endregion

    #region ThisScreen
    /// <summary>
    /// The current screen
    /// </summary>

    public static class ThisScreen
    {


        /// <summary>
        /// The current screen rect
        /// </summary>

        public static Rect ScreenRect
        {


            get
            {

                return new Rect(0, 0, Screen.width, Screen.height);
            }
        }

        static ThisScreen()
        {

        }


    }
    #endregion

    #region Extentions

    #region RectExtensions

    // [Info("Ruchmair Dixon", ".NET3.5", "Version 1.0.a")]
    /// <summary>
    ///     Extensions of the Rect class
    /// </summary>
    public static class RectExt
    {
        public static Vector2 TopLeft(this Rect rect)
        {
            return new Vector2(rect.x, rect.y);
        }

        public static Vector2 TopRight(this Rect rect)
        {
            return new Vector2(rect.x + rect.width, rect.y);
        }

        public static Vector2 BottomLeft(this Rect rect)
        {
            return new Vector2(rect.x, rect.y + rect.height);
        }

        public static Vector2 BottomRight(this Rect rect)
        {
            return new Vector2(rect.x + rect.width, rect.y + rect.height);
        }

        public static Rect ScaleSizeBy(this Rect rect, float scale)
        {
            return rect.ScaleSizeBy(scale, rect.center);
        }

        public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
        {
            var result = rect;
            result.x -= pivotPoint.x;
            result.y -= pivotPoint.y;
            result.xMin *= scale;
            result.xMax *= scale;
            result.yMin *= scale;
            result.yMax *= scale;
            result.x += pivotPoint.x;
            result.y += pivotPoint.y;
            return result;
        }

        public static Rect ScaleSizeBy(this Rect rect, Vector2 scale)
        {
            return rect.ScaleSizeBy(scale, rect.center);
        }

        public static Rect ScaleSizeBy(this Rect rect, Vector2 scale, Vector2 pivotPoint)
        {
            var result = rect;
            result.x -= pivotPoint.x;
            result.y -= pivotPoint.y;
            result.xMin *= scale.x;
            result.xMax *= scale.x;
            result.yMin *= scale.y;
            result.yMax *= scale.y;
            result.x += pivotPoint.x;
            result.y += pivotPoint.y;
            return result;
        }

        /// <summary>
        ///     Will set <paramref name="rect" /> to be positioned at the mouse position
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Rect SetToMouseCenter(this Rect rect)
        {
            var result = rect;
            result.center = Event.current.mousePosition;
            return result;
        }

        /// <summary>
        ///     will set <paramref name="rect" /> to be positioned at the mouse positiom at the <paramref name="rect" /> center and
        ///     scaled by <paramref name="scale" /> with its pivot point at  <paramref name="pivotPoint" />
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="scale"></param>
        /// <param name="pivotPoint"></param>
        /// <returns></returns>
        public static Rect SetToMouseCenter(this Rect rect, float scale, Vector2 pivotPoint)
        {
            var result = rect;
            result.x = Event.current.mousePosition.x - result.width / 2;
            result.y = Event.current.mousePosition.y - result.height / 2;
            result.xMin *= scale;
            result.xMax *= scale;
            result.yMin *= scale;
            result.yMax *= scale;
            result.x += pivotPoint.x;
            result.y += pivotPoint.y;
            return result;
        }

        /// <summary>
        ///     Will set create a Rect to be of <paramref name="width" /> and <paramref name="height" /> at the center of
        ///     <paramref name="rect" /> with <paramref name="xPaddin" /> and <paramref name="yPadding" />
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="xPadding"></param>
        /// <param name="yPadding"></param>
        /// <returns></returns>
        public static Rect ToCenter(this Rect rect, float width = 0, float height = 0, float xPadding = 0,
            float yPadding = 0)
        {
            var result = rect;
            var rectWidth = rect.width;
            var rectHeight = rect.height;
            result.width = width == 0 ? rectWidth : width;
            result.height = height == 0 ? rectHeight : height;
            result.position = new Vector2(rect.x + rectWidth / 2 - result.width / 2 + xPadding,
                rect.y + rectHeight / 2 - result.height / 2 + yPadding);

            return result;
        }

        /// <summary>
        ///     Will set create a Rect to be of <paramref name="width" /> and <paramref name="height" /> at the upper right of
        ///     <paramref name="rect" /> with <paramref name="xPaddin" /> and <paramref name="yPadding" />
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="xPadding"></param>
        /// <param name="yPadding"></param>
        /// <returns></returns>
        public static Rect ToUpperRight(this Rect rect, float width = 0, float height = 0, float xPadding = 0,
            float yPadding = 0)
        {
            var result = rect;
            var rectWidth = rect.width;
            var rectHeight = rect.height;
            result.width = width == 0 ? rectWidth : width;
            result.height = height == 0 ? rectHeight : height;
            result.x = rect.x + rectWidth - result.width + xPadding;
            result.y = rect.y + yPadding;

            return result;
        }

        /// <summary>
        ///     Will set create a Rect to be of <paramref name="width" /> and <paramref name="height" /> at the lower right
        ///     <paramref name="rect" /> with <paramref name="xPaddin" /> and <paramref name="yPadding" />
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="xPadding"></param>
        /// <param name="yPadding"></param>
        /// <returns></returns>
        public static Rect ToLowerRight(this Rect rect, float width = 0, float height = 0, float xPadding = 0,
            float yPadding = 0)
        {
            var result = rect;
            var rectWidth = rect.width;
            var rectHeight = rect.height;
            result.width = width == 0 ? rectWidth : width;
            result.height = height == 0 ? rectHeight : height;
            result.x = rect.x + rectWidth - result.width + xPadding;
            result.y = rect.y + rectHeight - result.height + yPadding;

            return result;
        }

        /// <summary>
        ///     Will set create a Rect to be of <paramref name="width" /> and <paramref name="height" /> at the upper left of
        ///     <paramref name="rect" /> with <paramref name="xPaddin" /> and <paramref name="yPadding" />
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="xPadding"></param>
        /// <param name="yPadding"></param>
        /// <returns></returns>
        public static Rect ToUpperLeft(this Rect rect, float width = 0, float height = 0, float xPadding = 0,
            float yPadding = 0)
        {
            var result = rect;
            var rectWidth = rect.width;
            var rectHeight = rect.height;
            result.x = rect.x + xPadding;
            result.y = rect.y + yPadding;
            result.width = width == 0 ? rectWidth : width;
            result.height = height == 0 ? rectHeight : height;
            return result;
        }

        /// <summary>
        ///     Will set create a Rect to be of <paramref name="width" /> and <paramref name="height" /> at the lower left of
        ///     <paramref name="rect" /> with <paramref name="xPaddin" /> and <paramref name="yPadding" />
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="xPadding"></param>
        /// <param name="yPadding"></param>
        /// <returns></returns>
        public static Rect ToLowerLeft(this Rect rect, float width = 0, float height = 0, float xPadding = 0,
            float yPadding = 0)
        {
            var result = rect;
            var rectWidth = rect.width;
            var rectHeight = rect.height;
            result.width = width == 0 ? rectWidth : width;
            result.height = height == 0 ? rectHeight : height;
            result.x = rect.x + xPadding;
            result.y = rect.y + rectHeight - result.height + yPadding;

            return result;
        }

        /// <summary>
        ///     Will set create a Rect to be of <paramref name="width" /> and <paramref name="height" /> at the top center of
        ///     <paramref name="rect" /> with <paramref name="xPaddin" /> and <paramref name="yPadding" />
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="xPadding"></param>
        /// <param name="yPadding"></param>
        /// <returns></returns>
        public static Rect ToCenterTop(this Rect rect, float width = 0, float height = 0, float xPadding = 0,
            float yPadding = 0)
        {
            var result = rect;
            var rectWidth = rect.width;
            var rectHeight = rect.height;
            result.width = width == 0 ? rectWidth : width;
            result.height = height == 0 ? rectHeight : height;
            result.position = new Vector2(rect.x + rectWidth / 2 - result.width / 2 + xPadding, rect.y + yPadding);
            return result;
        }

        /// <summary>
        ///     Will set create a Rect to be of <paramref name="width" /> and <paramref name="height" /> at the bottom center of
        ///     <paramref name="rect" /> with <paramref name="xPaddin" /> and <paramref name="yPadding" />
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="xPadding"></param>
        /// <param name="yPadding"></param>
        /// <returns></returns>
        public static Rect ToCenterBottom(this Rect rect, float width = 0, float height = 0, float xPadding = 0,
            float yPadding = 0)
        {
            var result = rect;
            var rectWidth = rect.width;
            var rectHeight = rect.height;
            result.width = width == 0 ? rectWidth : width;
            result.height = height == 0 ? rectHeight : height;
            result.position = new Vector2(rect.x + rectWidth / 2 - result.width / 2 + xPadding,
                rect.y + rectHeight - result.height + yPadding);

            return result;
        }

        /// <summary>
        ///     Will set create a Rect to be of <paramref name="width" /> and <paramref name="height" /> at the left center of
        ///     <paramref name="rect" /> with <paramref name="xPaddin" /> and <paramref name="yPadding" />
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="xPadding"></param>
        /// <param name="yPadding"></param>
        /// <returns></returns>
        public static Rect ToCenterLeft(this Rect rect, float width = 0, float height = 0, float xPadding = 0,
            float yPadding = 0)
        {
            var result = rect;
            var rectWidth = rect.width;
            var rectHeight = rect.height;
            result.width = width == 0 ? rectWidth : width;
            result.height = height == 0 ? rectHeight : height;
            result.position = new Vector2(rect.x + xPadding,
                rect.y + rectHeight / 2 - height / 2 + yPadding);

            return result;
        }

        /// <summary>
        ///     Will set create a Rect to be of <paramref name="width" /> and <paramref name="height" /> at the right center of
        ///     <paramref name="rect" /> with <paramref name="xPaddin" /> and <paramref name="yPadding" />
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="xPadding"></param>
        /// <param name="yPadding"></param>
        /// <returns></returns>
        public static Rect ToCenterRight(this Rect rect, float width = 0, float height = 0, float xPadding = 0,
            float yPadding = 0)
        {
            var result = rect;
            var rectWidth = rect.width;
            var rectHeight = rect.height;
            result.width = width == 0 ? rectWidth : width;
            result.height = height == 0 ? rectHeight : height;
            result.position = new Vector2(rect.x + xPadding + (rectWidth - width),
                rect.y + rectHeight / 2 - height / 2 + yPadding);
            return result;
        }

        /// <summary>
        ///     Increases the size of <paramref name="rect" /> by <paramref name="width" /> and <paramref name="height" /> and
        ///     position of  <paramref name="rect" /> by <paramref name="x" /> and <paramref name="y" />
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Rect AddRect(this Rect rect, float x, float y, float width = 0, float height = 0)
        {
            var result = rect;
            result.x = rect.x + x;
            result.y = rect.y + y;
            result.width = rect.width + width;
            result.height = rect.height + height;

            return result;
        }

        public static Rect PlaceUnder(this Rect rect, float width = 0, float height = 0, float xPadding = 0,
            float yPadding = 0)
        {
            var result = rect;
            var rectWidth = rect.width;
            var rectHeight = rect.height;
            result.x = rect.x + xPadding;
            result.y = rect.y + yPadding + rectHeight;
            result.width = width == 0 ? rectWidth : width;
            result.height = height == 0 ? rectHeight : height;
            return result;
        }

        public static Rect PlaceToRight(this Rect rect, float width = 0, float height = 0, float xPadding = 0,
            float yPadding = 0)
        {
            var result = rect;
            var rectWidth = rect.width;
            var rectHeight = rect.height;
            result.width = width == 0 ? rectWidth : width;
            result.height = height == 0 ? rectHeight : height;
            result.x = rect.x + rectWidth + xPadding;
            result.y = rect.y + yPadding;

            return result;
        }


        //  public static void getRect<T>()
    }

    #endregion

    #region Vector2 Extensions
    /// <summary>
    /// Extension of the Vector2 Class
    /// </summary>
    public static class Vector2Ext
    {

        /// <summary>
        /// Adds paddint on the x and y of <paramref name="vector2"/> by <paramref name="<xPading"/> and <paramref name="yPading"/>
        /// </summary>
        /// <param name="vector2"></param>
        /// <param name="xPadding"></param>
        /// <param name="yPadding"></param>
        /// <returns></returns>
        public static Vector2 AddPadding(this Vector2 vector2, float xPadding, float yPadding)
        {
            Vector2 result = vector2;

            result.x = vector2.x + xPadding;
            result.y = vector2.y + yPadding;

            return vector2;
        }

    }

    #endregion

    #region Vector3 Extensions
    /// <summary>
    /// Extensions of the Vecor3 Class
    /// </summary>
    public static class Vector3Ext
    {
        /// <summary>
        /// Modifies the x,y,z values of <paramref name="vector3"/> by <paramref name="x"/>  , <paramref name="y"/> , <paramref name="z"/>
        /// </summary>
        /// <param name="vector3"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3 Modify(this Vector3 vector3, float x, float y, float z)
        {
            Vector3 result = vector3;
            result.x = result.x + x;
            result.y = result.y + y;
            result.z = result.z + z;

            return result;

        }
    }

    #endregion

    #region ListExtensions
    /// <summary>
    /// Extension of the List classs
    /// </summary>
    public static class ListExt
    {
        /// <summary>
        /// Resize <paramref name="list"/> by <paramref name="size"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="size"></param>
        /// <param name="c"></param>
        public static void Resize<T>(this List<T> list, int size, T c = default(T))
        {
            int cur = list.Count;
            if (size < cur)
                list.RemoveRange(size, cur - size);
            else if (size > cur)
                list.AddRange(Enumerable.Repeat(c, size - cur));
        }
        /// <summary>
        /// Add multiple values to <paramref name="List"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="elements"></param>
        public static void AddMany<T>(this List<T> list, params T[] elements)
        {
            list.AddRange(elements);
        }

    }
    #endregion
    #endregion

    #region DMMath Struct
    /// <summary>
    /// DMMath functions 
    /// </summary>
    public class DMMath
    {
        public const float Mv = 2.287128713f;
        public static float REQS(float screenHeight, float valiue)
        {
            return screenHeight / valiue;
        }
        public static float REQS(float screenHeight)
        {
            return screenHeight / Mv;
        }
        public const float ScalingConstant = 0.00215f;

    }



    #endregion

    namespace Editor
    {
        #region Radar Builder
        /// <summary>
        /// Determining what defines the forward facing position of the radar 
        /// </summary>
        public enum FrontIs
        {
            /// <summary>
            /// 
            /// </summary>
            East = 0,
            /// <summary>
            /// 
            /// </summary>
            West = 1,
            /// <summary>
            /// 
            /// </summary>
            North = 2,
            /// <summary>
            /// 
            /// </summary>
            South = 3
        }
  
        /// <summary>
        /// change the X rotation the the y or z
        /// </summary>
        public enum RetargetX
        {
            /// <summary>
            /// change the x rotation to the y rotation
            /// </summary>
            X_to_Y,
            /// <summary>
            /// change the x rotation to the z rotation
            /// </summary>
            X_to_Z,
            /// <summary>
            /// use no rotation retargeting
            /// </summary>
            none

        }
        /// <summary>
        /// change the y rotation to the x or z
        /// </summary>
        public enum RetargetY
        {

            /// <summary>
            /// change the y rotation to the x rotation
            /// </summary>
            Y_to_X,
            /// <summary>
            /// change the y rotation to the z rotation
            /// </summary>
            Y_to_Z,
            /// <summary>
            /// use no rotation retargeting
            /// </summary>
            none

        }
        /// <summary>
        /// chage the z rotation to the x or y
        /// </summary>
        public enum RetargetZ
        {

            /// <summary>
            /// change the z rotation the the X rotation
            /// </summary>
            Z_to_X,
            /// <summary>
            /// change the z rotation to the x rotation
            /// </summary>
            Z_to_Y,
            /// <summary>
            /// use no rotation retargeting
            /// </summary>
            none

        }
        /// <summary>
        /// Selection between Inverse rotation and Proportional rotation
        /// </summary>
        public enum Rotations
        {
            /// <summary>
            /// 
            /// </summary>
            Proportional = 0,
            /// <summary>
            /// 
            /// </summary>
            Inverse = 1

        }
        /// <summary>
        /// Selection of the way in which you wish to select and object
        /// </summary>
        public enum TargetObject
        {
            /// <summary>
            /// 
            /// </summary>
            FindObject = 0,
            /// <summary>
            /// 
            /// </summary>
            ObjectWithTag = 1,
            /// <summary>
            /// 
            /// </summary>
            ThisObject = 2,
            /// <summary>
            /// 
            /// </summary>
            InstancedBlip = 3
        }
        /// <summary>
        /// The Targeted blips 
        /// </summary>
        public enum TargetBlip
        {
            /// <summary>
            /// 
            /// </summary>
            ThisObject = 0,
            /// <summary>
            /// 
            /// </summary>
            InstancedBlip = 1
        }
        /// <summary>
        ///  A selection between Sprite,Mesh and Prefab
        /// </summary>
        public enum CreateBlipAs
        {
            /// <summary>
            /// 
            /// </summary>
            AsSprite = 0,
            /// <summary>
            /// 
            /// </summary>
            AsMesh = 1,
            /// <summary>
            /// 
            /// </summary>
            AsPrefab = 2
        }
        /// <summary>
        /// The method by which we wish to use to position the radar
        /// </summary>
        public enum RadarPositioning
        {
            /// <summary>
            /// 
            /// </summary>
            Snap = 0,
            /// <summary>
            /// 
            /// </summary>
            Manual = 1

        }
        /// <summary>
        /// Choose between Realtime minimap or a stati minimap
        /// </summary>
        public enum MapType
        {
            /// <summary>
            /// Will tell the Rada r builder to use real time images as MinimapTexture
            /// </summary>
            Realtime = 0,
            /// <summary>
            /// Will tell Radar Builder to use a Static predesigned image as Minmap texture
            /// </summary>
            Static = 1
        }
        /// <summary>
        /// 
        /// </summary>
        public enum ObjectFindingMethod
        {
            /// <summary>
            /// Determines if we will be using recursive gameobject find. call WARNING, THIS IS A SLOW PROCESS
            /// </summary>
            Recursive,
            /// <summary>
            ///  If true, you will NEED to call _3DRadar.radar3D.DoInstanceObjectCheck() at instance or at the end of instancing
            /// </summary>
            Pooling
        }
        /// <summary>
        /// 
        /// </summary>
        public enum CenterObjectFindingMethod
        {
            /// <summary>
            /// Determines if we will be using recursive gameobject find.
            /// </summary>
            Recursive,
            /// <summary>
            ///  If true, you will NEED to call _3DRadar.radar3D.DoCenterInstanceObjectCheck() at instance or at the end of instancing
            /// </summary>
            Single
        }



        /// <summary>
        /// <para> Defines the object which is to be targeted and the way in which it must be rotated </para>
        /// </summary>
        [System.Serializable]
        public class RotationTarget
        {
            public bool UseRotationTarget = true;

            /// <summary>
            /// called only from editor , and is not necessary at runtime 
            /// </summary>
            public bool ShowDesignSetings;
            /// <summary>
            /// When true , the z rotation will be the same as the Y rotation
            /// </summary>
            public bool UseY;
            /// <summary>
            /// Freeze rotation around particular axis
            /// </summary>
            public bool FreezeX, FreezeY, FreezeZ;
            /// <summary>
            /// Damping used to control rotation of particular design layer
            /// </summary>
            public float RotationDamping;
            /// <summary>
            /// the string tag you define 
            /// </summary>
            public string tag;
            /// <summary>
            /// the name of the object you wish to find
            /// </summary>
            public string FindingName;
            /// <summary>
            /// The name of the instanced object  you wish to target
            /// </summary>
            public string InstancedObjectToTrackBlipName;
            /// <summary>
            /// the name of the instanced blip you wish to track
            /// </summary>
            public string InstancedTargetBlipname;
            /// <summary>
            /// Selection between Inverse rotation and Proportional rotation
            /// </summary>
            public Rotations rotations;
            /// <summary>
            ///  This may be a blip or any other object in scene
            /// </summary>
            public GameObject TargetedObject;
            /// <summary>
            /// the object whose rotation you wish to target
            /// </summary>
            public GameObject Target;
            /// <summary>
            /// Selection of the way in which you wish to select and object
            /// </summary>
            public TargetObject ObjectToTrack = TargetObject.ThisObject;
            /// <summary>
            /// The blip you wish to target
            /// </summary>
            public TargetBlip target = TargetBlip.ThisObject;
            /// <summary>
            /// change the x rotation to the y o z
            /// </summary>
            public RetargetX retargetedXRotation = RetargetX.none;
            /// <summary>
            ///  change the y rotation to the x or z 
            /// </summary>
            public RetargetY retargetedYRotation = RetargetY.none;
            /// <summary>
            /// change the z rotation to the x or y
            /// </summary>
            public RetargetZ retargetedZRotation = RetargetZ.none;
            /// <summary>
            /// X rotation value added to the rotation target
            /// </summary>
            public float AddedXRotation;
            /// <summary>
            /// Y rotation value added to the rotation target
            /// </summary>
            public float AddedYRotation;
            /// <summary>
            /// Z rotation value added to the rotation target
            /// </summary>
            public float AddedZRotation;

        }
        /// <summary>
        /// Minimap Module 
        /// </summary>
        [System.Serializable]
        public class MiniMapModule
        {
            /// <summary>
            /// Choose between Realtime minimap or a stati minimap
            /// </summary>
            public MapType mapType = MapType.Realtime;
            /// <summary>
            /// Texture to be used for static minimaps
            /// </summary>
            public Sprite MapTexture;
            /// <summary>
            /// take in a sprite value and will use it as the mask for the minimap
            /// </summary>
            public Sprite CustomMapMaskShape;
            // public bool autogenerateMaps;
            /// <summary>
            /// Check if the map has been generated
            /// </summary>
            [NonSerialized]
            public bool generated;
            //public bool UseVector3 = true;
            /// <summary>
            /// Determine if the static minimap is being calibrated 
            /// </summary>
            public bool calibrate;
            /// <summary>
            /// if teue , will allow you to set a custom map mask sprite
            /// </summary>
            public bool UseCustomMapMaskShape;
            /// <summary>
            /// true when a new static map texture is set
            /// </summary>
            public bool Refreshedmap = true;
            /// <summary>
            /// the objet which will use the Map texture and Masked Material
            /// </summary>
            public GameObject Map;
            /// <summary>
            /// the object which will use the mask material
            /// </summary>
            public GameObject Mask;
            /// <summary>
            /// The parent gameobject of the Map
            /// </summary>
            public GameObject MapPivot;
            //   public GameObject CenterObject;
            /// <summary>
            /// Cashe of the SceneScale vlaue
            /// </summary>
            public float SavedSceneScale;
            /// <summary>
            /// The value set during calibratin of ststic minimap
            /// </summary>
            public float MapScale = 1;
            /// <summary>
            /// Cashe of the MapScale vlaue
            /// </summary>
            public float SavedMapScale;
            /// <summary>
            /// Determines by what rate the minmap is scales at rintime
            /// </summary>
            public float Scalingfactor;
            //public Vector3 CenterPosition;
            /// <summary>
            /// Masked material
            /// </summary>
            public Material MapMaterial;
            /// <summary>
            /// Mask Material
            /// </summary>
            public Material MaskMaterial;
            /// <summary>
            /// The layer on which the minimap will be rendered
            /// </summary>
            public LayerMask layer;
            /// <summary>
            /// the RenderTexture to be used with the realtime minimap
            /// </summary>
            public RenderTexture renderTexture;
            /// <summary>
            /// The camera reading the RenderTexture for the Minimap
            /// </summary>
            public Camera RealtimeMinimapCamera;
            /// <summary>
            /// the position of the RealtimeMinimapCamera in the Y axis
            /// </summary>
            public float CameraHeight;
            /// <summary>
            /// the order in layer of the blip
            /// </summary>
            public int OrderInLayer = -1;
            /// <summary>
            /// Internal use only
            /// </summary>
            public Vector3 StartPosition = Vector3.zero;
            Texture2D CalculateTexture(int h, int w, float r, float cx, float cy, Texture2D sourceTex)
            {
                //  Color[] c = sourceTex.GetPixels(0, 0, sourceTex.width, sourceTex.height);
                Texture2D b = new Texture2D(h, w);
                for (int i = (int)(cx - r); i < cx + r; i++)
                {
                    for (int j = (int)(cy - r); j < cy + r; j++)
                    {
                        float dx = i - cx;
                        float dy = j - cy;
                        float d = Mathf.Sqrt(dx * dx + dy * dy);
                        if (d <= r)
                            b.SetPixel(i - (int)(cx - r), j - (int)(cy - r), sourceTex.GetPixel(i, j));
                        else
                            b.SetPixel(i - (int)(cx - r), j - (int)(cy - r), Color.clear);
                    }
                }
                b.Apply();
                return b;
            }

            /// <summary>
            /// generates are sprite specificially used for the mask layer of the Radar
            /// </summary>
            /// <returns></returns>
            public Sprite MaskSprite()
            {


                Sprite mask = Sprite.Create(CalculateTexture(200, 200, 100, 0.5f, 0.5f, new Texture2D(200, 200)),
        new Rect(0, 0, 200, 200),
        new Vector2(0.5f, 0.5f));
                return mask;



            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public Mesh ProceduralMapQuad()
            {
                Mesh TempMesh = new Mesh();
                Vector3[] verts;
                int[] triangles;
                Vector3[] Normals;
                Vector2[] UVs;

                verts = new Vector3[4];
                verts[0] = new Vector3(-1, -1, 0);
                verts[1] = new Vector3(1, -1, 0);
                verts[2] = new Vector3(-1, 1, 0);
                verts[3] = new Vector3(1, 1, 0);

                triangles = new int[6];
                triangles[0] = 0;
                triangles[1] = 2;
                triangles[2] = 1;
                triangles[3] = 1;
                triangles[4] = 2;
                triangles[5] = 3;

                Normals = new Vector3[4];
                Normals[0] = -Vector3.forward;
                Normals[1] = -Vector3.forward;
                Normals[2] = -Vector3.forward;
                Normals[3] = -Vector3.forward;

                UVs = new Vector2[4];
                UVs[0] = new Vector2(0, 0);
                UVs[1] = new Vector2(1, 0);
                UVs[2] = new Vector2(0, 1);
                UVs[3] = new Vector2(1, 1);

                TempMesh.vertices = verts;
                TempMesh.triangles = triangles;
                TempMesh.normals = Normals;
                TempMesh.uv = UVs;

                return TempMesh;
            }

        }
        /// <summary>
        /// Options for optimizing  the radars functions
        /// </summary>
        [System.Serializable]
        public class OptimizationModule
        {
            /// <summary>
            ///  pool size for objects you wish to track
            /// </summary>
            public int poolSize;
            /// <summary>
            /// Determines if the blip will be using pooling
            /// </summary>
            public bool SetPoolSizeManually = false;
            /// <summary>
            /// Options for usng two diferent object finding methods
            /// </summary>
            public ObjectFindingMethod objectFindingMethod = ObjectFindingMethod.Recursive;

            /// <summary>
            /// if true , blips will be removed if the object they track has lost its original tag
            /// </summary>
            public bool RemoveBlipsOnTagChange;
            /// <summary>
            /// if true , blips will be removed if the object they track has been disabled
            /// </summary>
            public bool RemoveBlipsOnDisable;
            /// <summary>
            /// if true and you are using Recursive optimization method then you can call _3DRadar.radar3D.doInstanceObjectCheck() trigger object search;
            /// </summary>
            public bool RequireInstanceObjectCheck;

            /// <summary>
            /// By setting this to true, you can ensure that evne if your pool value at atart is greater then the actual amount of objects that can be found , your pool value will be reset to the amount of objects found ao that recusrsive searching is avoided
            /// </summary>
            public bool RecalculatePoolSizeBasedOnFirstFoundObjects;

        }

        #region 3D


        #region _3DRadarBlips Class
        /// <summary>
        /// 
        /// </summary>
        [System.Serializable]
        public class RadarBlips3D
        {

            /// <summary>
            /// Tell the blip to do a removal of blips if the Recursive optimization method is used 
            /// </summary>
            [NonSerialized]
            public bool DoRemoval = false;
            /// <summary>
            /// checks if all blips have ben instanced
            /// </summary>
            [NonSerialized]
            public bool Instanced;
            /// <summary>
            /// check if the blip is set turned on or off
            /// </summary>
            public bool IsActive;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public bool ShowBLipSettings;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public bool ShowSpriteBlipSettings;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public bool ShowMeshBlipSettings;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public bool ShowPrefabBlipSettings;
            /// <summary>
            /// Determines if the blip will be tracking the rotation of its target
            /// </summary>
            public bool IsTrackRotation;
            /// <summary>
            /// Determines if th blips can scale by distance
            /// </summary>
            public bool BlipCanScleBasedOnDistance;
            /// <summary>
            /// INTERNL USE ONLY
            /// </summary>
            public bool ShowTrackingLineSettings;
            /// <summary>
            /// Determines if we should use tracking lines or not.
            /// </summary>
            public bool UseTrackingLine;
            /// <summary>
            /// Determines if we should use basetrackers or not
            /// </summary>
            public bool UseBaseTracker;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public bool ShowBaseTrackerSettings;
            /// <summary>
            /// Determines if the X rotation of the tracked object should be locked to 0
            /// </summary>
            public bool lockX;
            /// <summary>
            /// Determines if the Y rotation of the tracked object should be locked to 0
            /// </summary>
            public bool lockY;
            /// <summary>
            /// Determines if the Z rotation of the tracked object should be locked to 0
            /// </summary>
            public bool lockZ;
            /// <summary>
            /// Determines if the mesh blip will use a the Radar Builder LOD system
            /// </summary>
            public bool UseLOD;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public bool ShowLODSettings;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public bool ShowGeneralSettings;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public bool ShowAdditionalOptions;
            /// <summary>
            /// determines if the blip should always remeing inside the radar 
            /// </summary>
            public bool AlwaysShowBlipsInRadarSpace;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public bool ShowLowMeshSetings;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public bool ShowMediumMeshSettings;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public bool ShowHighMeshSettings;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public bool ShowOptimizationSettings;
            /// <summary>
            /// if you are using Always Show and Scale By Distance , this will ensure that you have a smooth ttansition from the moment your blip passes the Tracking Bounds to the moment is is scales to its minimaum scale
            /// </summary>
            public bool SmoothScaleTransition;
            /// <summary>
            ///  Set to true if you wish to give the blips a custom rotation
            /// </summary>
            public bool UseCustomRotation;
            /// <summary>
            ///  ensures that the blips do not go under the radar
            /// </summary>
            public bool KeepBlipsAboveRadarPlane;
            /// <summary>
            /// will trak objects through their y position
            /// </summary>
            public bool UseHeightTracking = true;

            public Sprite BlipSprite
            {
                get
                {

                    if (icon == null)
                        icon = Resources.Load<Sprite>("Default Sprites/Blip/DefaultBlipSprite");
                    return icon;
                }
                set
                {


                    icon = value;
                }
            }
            /// <summary>
            /// The blip icon if the blip is a sprite
            /// </summary>
            public Sprite icon;



            public Sprite BaseTrackerSprite
            {
                get
                {

                    if (BaseTracker == null)
                        BaseTracker = Resources.Load<Sprite>("Default Sprites/Base Tracker/DefaultBaseTrackerSprite");
                    return BaseTracker;
                }
                set
                {


                    BaseTracker = value;
                }
            }
            /// <summary>
            /// The base tracker sprite
            /// </summary>
            public Sprite BaseTracker;

            /// <summary>
            /// Prefab blip
            /// </summary>
            public Transform prefab;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public string State = "";
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public string Tag = "Untagged";

            public Material BlipMaterial
            {
                get
                {
                    if (SpriteMaterial == null)
                        SpriteMaterial = new Material(Shader.Find("Sprites/Default"));

                    return SpriteMaterial;
                }
                set
                {
                    SpriteMaterial = value;
                }
            }
            /// <summary>
            /// The material used for the sprite blip
            /// </summary>
            public Material SpriteMaterial;

            public Material TrackingLineMaterial
            {
                get
                {
                    if (tlm == null)
                        tlm = Resources.Load<Material>("Default Materials/TrackingLine");


                    return tlm;
                }
                set
                {
                    tlm = value;
                }
            }

            /// <summary>
            /// The material used for the tracking line
            /// /// </summary>
            [UnityEngine.Serialization.FormerlySerializedAs("TrackingLineMaterial")]
            public Material tlm;

            /// <summary>
            /// The material used for the base tracker
            /// </summary>
            public Material BaseTrackerMaterial
            {
                get
                {
                    if (btm == null)
                        btm = new Material(Shader.Find("Sprites/Default"));

                    return btm;
                }
                set
                {
                    btm = value;
                }
            }

            [UnityEngine.Serialization.FormerlySerializedAs("BaseTrackerMaterial")]
            public Material btm;
            /// <summary>
            /// The mesh blip mesh when LOD is not active
            /// </summary>
            public Mesh mesh;
            /// <summary>
            /// The low poly mesh when LOD is active
            /// </summary>
            public Mesh Low;
            /// <summary>
            /// The medium poly count mesh when the LOD is active
            /// </summary>
            public Mesh Medium;
            /// <summary>
            /// The high poly count mesh when the LOD is active
            /// </summary>
            public Mesh High;
            /// <summary>
            /// All mesh materials usd by the Mesh
            /// </summary>
            public Material[] MeshMaterials = new Material[1];
            /// <summary>
            /// THe colour of the material
            /// </summary>
            public Color colour = new Color(1F, 0.6F, 0F, 0.8F);
            /// <summary>
            /// The colour start of the tracking line
            /// </summary>
            public Color TrackingLineStartColour = new Color(1F, 0.435F, 0F, 0.5F);
            /// <summary>
            /// The end colour of the tracking line
            /// </summary>
            public Color TrackingLineEndColour = new Color(1F, 0.435F, 0F, 0.5F);
            /// <summary>
            /// The colour ued by the base tracker material
            /// </summary>
            public Color BaseTrackerColour = new Color(1F, 0.435F, 0F, 0.5F);
            /// <summary>
            /// The size of the blip
            /// </summary>
            public float BlipSize = 1;
            /// <summary>
            ///  The default minimum scale of the blip
            /// </summary>
            public const float DynamicBlipSize = 0.025f;
            /// <summary>
            /// The minimum size of the blip
            /// </summary>
            public float BlipMinSize = 0.5f;
            /// <summary>
            /// The maximum size of the blip
            /// </summary>
            public float BlipMaxSize = 1;
            /// <summary>
            /// The width of the tracking line
            /// </summary>
            public float TrackingLineDimention = 0.02F;
            /// <summary>
            /// The distance at which the LOW mesh will replace the current mesh of the mesh blip
            /// </summary>
            public float LowDistance;
            /// <summary>
            /// The distance at which the MEDIUM mesh will replace the current mesh of the mesh blip
            /// </summary>
            public float MediumDistance;
            /// <summary>
            /// The distance at which the HIGH mesh will replace the current mesh of the mesh blip
            /// </summary>
            public float HighDistance;
            /// <summary>
            /// The scale of th base tracker
            /// </summary>
            public float BaseTrackerSize = 0.5f;
            /// <summary>
            /// Custom X Rotation For BLips
            /// </summary>
            public float CustomXRotation = 0;
            /// <summary>
            /// Custom Y Rotation For BLips
            /// </summary>
            public float CustomYRotation = 0;
            /// <summary>
            /// Custom Z Rotation For BLips
            /// </summary>
            public float CustomZRotation = 0;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public int NumberOfBLips;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public int count;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public int MatCount = 1;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public int Layer = 0;
            /// <summary>
            /// A list of All tracking lines
            /// </summary>
            [NonSerialized]
            public List<GameObject> TrackingLineObject = new List<GameObject>();
            /// <summary>
            /// A list of the objects being tracked
            /// </summary>
            [NonSerialized]
            public List<GameObject> gos = new List<GameObject>();
            /// <summary>
            /// A list of the actual blips you see in your radar
            /// </summary>
            [NonSerialized]
            public List<Transform> RadarObjectToTrack = new List<Transform>();
            /// <summary>
            /// 
            /// </summary>
            [NonSerialized]
            public List<GameObject> BaseTrackers = new List<GameObject>();
            /// <summary>
            /// Determines what the blip should be created as , prefab or sprite
            /// </summary>
            public CreateBlipAs _CreateBlipAs;
            /// <summary>
            ///  records the amount of tracked objects in the radr far this blip type
            /// </summary>
            [NonSerialized]
            public int ObjectCount = -1;
            /// <summary>
            /// the order in layer of the blip
            /// </summary>
            public int OrderInLayer = 1;
            /// <summary>
            ///  Sorting layer of the sprite blip
            /// </summary>
            public SortingLayer sortingLayer;
            /// <summary>
            /// Methods of optimizing radar performance
            /// </summary>
            public OptimizationModule optimization = new OptimizationModule();
            /// <summary>
            ///  Line renderer for the tracking line
            /// </summary>
            //  [NonSerialized]
            //public LineRenderer TrackingLine = new LineRenderer();

            // public List<BlipObject> blipObjects = new List<BlipObject>();

            // public Dictionary<GameObject, UnityEngine.Object[]> BlipContent;

            //   public LODStyle LODstyle = LODStyle.Radial;

        }

        #endregion

        #region CenterObject class
        /// <summary>
        /// 
        /// </summary>
        [System.Serializable]
        public class RadarCenterObject3D
        {
            /// <summary>
            /// checks if all blips have ben instanced
            /// </summary>
            [NonSerialized]
            public bool Instanced;
            public bool IsActive;
            /// <summary>
            ///  INTERNAL USE ONLY
            /// </summary>
            public bool ShowBLipSettings;
            /// <summary>
            ///  INTERNAL USE ONLY
            /// </summary>
            public bool ShowSpriteBlipSettings;
            /// <summary>
            ///  INTERNAL USE ONLY
            /// </summary>
            public bool ShowMeshBlipSettings;
            /// <summary>
            ///  INTERNAL USE ONLY
            /// </summary>
            public bool ShowPrefabBlipSettings;
            /// <summary>
            /// Determines if the blip will be tracking the rotation of its target
            /// </summary>
            public bool IsTrackRotation;
            /// <summary>
            /// Determines if the X rotation of the tracked object should be locked to 0
            /// </summary>
            public bool lockX;
            /// <summary>
            /// Determines if the Y rotation of the tracked object should be locked to 0
            /// </summary>
            public bool lockY;
            /// <summary>
            /// Determines if the Z rotation of the tracked object should be locked to 0
            /// </summary>
            public bool lockZ;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public bool ShowGeneralSettings;
            /// <summary>
            /// Determines if the enter object or center blip should alwats be shown in th radar 
            /// </summary>
            public bool AlwaysShowCenterObject;
            /// <summary>
            /// Determines if the center object (center blip) can scale by distance
            /// </summary>
            public bool CenterObjectCanScaleByDistance;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public bool ShowAdditionalOptions;
            /// <summary>
            /// INTERNL USE ONLY
            /// </summary>
            public bool ShowTrackingLineSettings;
            /// <summary>
            /// Determines if we should use tracking lines or not.
            /// </summary>
            public bool UseTrackingLine;
            /// <summary>
            /// Determines if we should use basetrackers or not
            /// </summary>
            public bool UseBaseTracker;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public bool ShowBaseTrackerSettings;
            /// <summary>
            /// if you are using Always Show and Scale By Distance , this will ensure that you have a smooth ttansition from the moment your blip passes the Tracking Bounds to the moment is is scales to its minimaum scale
            /// </summary>
            public bool SmoothScaleTransition;
            /// <summary>
            ///  Set to true if you wish to give the center blip a custom rotation
            /// </summary>
            public bool UseCustomRotation;

            public Sprite BlipSprite
            {
                get
                {

                    if (icon == null)
                        icon = Resources.Load<Sprite>("Default Sprites/Blip/DefaultPlayerBlipSprite");
                    return icon;
                }
                set
                {


                    icon = value;
                }
            }
            /// <summary>
            /// The blip icon if the blip is a sprite
            /// </summary>
            public Sprite icon;

            public Sprite BaseTrackerSprite
            {
                get
                {

                    if (BaseTracker == null)
                        BaseTracker = Resources.Load<Sprite>("Default Sprites/Base Tracker/DefaultBaseTrackerSprite");
                    return BaseTracker;
                }
                set
                {


                    BaseTracker = value;
                }
            }
            /// <summary>
            /// The base tracker sprite
            /// </summary>
            public Sprite BaseTracker;

            /// <summary>
            /// Prefab blip
            /// </summary>
            public Transform prefab;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public string State = "";
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public string Tag = "Player";

            public Material BlipMaterial
            {
                get
                {
                    if (SpriteMaterial == null)
                        SpriteMaterial = new Material(Shader.Find("Sprites/Default"));

                    return SpriteMaterial;
                }
                set
                {
                    SpriteMaterial = value;
                }
            }
            /// <summary>
            /// The material used for the sprite blip
            /// </summary>
            public Material SpriteMaterial;


            public Material TrackingLineMaterial
            {
                get
                {
                    if (tlm == null)
                    {
                        tlm = Resources.Load<Material>("Default Materials/TrackingLine");
                    }

                    return tlm;
                }
                set
                {
                    tlm = value;
                }
            }

            /// <summary>
            /// The material used for the tracking line
            /// /// </summary>
            [UnityEngine.Serialization.FormerlySerializedAs("TrackingLineMaterial")]
            public Material tlm;

            /// <summary>
            /// The material used for the base tracker
            /// </summary>

            public Material BaseTrackerMaterial
            {
                get
                {
                    if (btm == null)
                        btm = new Material(Shader.Find("Sprites/Default"));

                    return btm;
                }
                set
                {
                    btm = value;
                }
            }

            [UnityEngine.Serialization.FormerlySerializedAs("BaseTrackerMaterial")]
            public Material btm;
            /// <summary>
            /// The mesh blip mesh when LOD is not active
            /// </summary>
            public Mesh mesh;
            /// <summary>
            /// All mesh materials usd by the Mesh
            /// </summary>
            public Material[] MeshMaterials = new Material[1];
            /// <summary>
            /// THe colour of the material
            /// </summary>
            public Color colour = new Color(1F, 0.435F, 0F, 0.5F);
            /// <summary>
            /// The colour start of the tracking line
            /// </summary>
            public Color TrackingLineStartColour = new Color(1F, 0.435F, 0F, 0.5F);
            /// <summary>
            /// The end colour of the tracking line
            /// </summary>
            public Color TrackingLineEndColour = new Color(1F, 0.435F, 0F, 0.5F);
            /// <summary>
            /// The colour ued by the base tracker material
            /// </summary>
            public Color BaseTrackerColour = new Color(1F, 0.435F, 0F, 0.5F);
            /// <summary>
            /// The size of the blip
            /// </summary>
            public float BlipSize = 1;
            /// <summary>
            /// Custom X Rotation For Center Blip
            /// </summary>
            public float CustomXRotation = 0;
            /// <summary>
            /// Custom Y Rotation For  Center Blip
            /// </summary>
            public float CustomYRotation = 0;
            /// <summary>
            /// Custom Z Rotation For  Center Blip
            /// </summary>
            public float CustomZRotation = 0;
            /// <summary>
            /// The width of the tracking line
            /// </summary>
            public float TrackingLineDimention = 0.2f;
            /// <summary>
            ///  The default minimum scale of the blip
            /// </summary>
            public const float DynamicBlipSize = 0.025f;
            /// <summary>
            /// The minimum size of the blip
            /// </summary>
            public float BlipMinSize = 0.5f;
            /// <summary>
            /// The maximum size of the blip
            /// </summary>
            public float BlipMaxSize = 1;
            /// <summary>
            /// The scale of th base tracker
            /// </summary>
            public float BaseTrackerSize = 0.5f;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public int Layer = 0;
            /// <summary>
            /// the order in layer of the blip
            /// </summary>
            public int OrderInLayer = 1;
            /// <summary>
            /// The blip at the center of the radar 
            /// </summary>
            [NonSerialized]
            public GameObject CenterBlip;
            /// <summary>
            /// The object being tracked to and used to represent the CenterBlip
            /// </summary>
            [NonSerialized]
            public Transform CenterObject;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public int MatCount = 1;
            /// <summary>
            /// Determines what the blip should be created as , prefab or sprite
            /// </summary>
            public CreateBlipAs _CreateBlipAs;
            /// <summary>
            /// Object which will sit on the y plane of the radar at all time 
            /// </summary>
            [NonSerialized]
            public GameObject BaseTrackerObject;
            /// <summary>
            /// Line wich will indicate distance in height from the centerobject to the radar
            /// </summary
            [NonSerialized]
            public GameObject TrackingLine;


        }

        #endregion

        #region Radar Design Class
        /// <summary>
        /// 
        /// </summary>
        [System.Serializable]
        public class RadarDesign3D
        {


            /// <summary>
            /// This is the Diameter of the radar, this value will directly change the scale of the Radars child object "Designs" once UseSceneScale is false
            /// </summary>
            public float RadarDiameter = 1;
            /// <summary>
            /// This is the amound of the scene that the radar is able to 'see' in order to collect dats on things to track and display
            /// </summary>
            public float SceneScale = 100.0f;
            /// <summary>
            /// The range in which all blips can be shown in the radar
            /// </summary>
            public float TrackingBounds = 1;
            /// <summary>
            ///  The diameter of the zone at the center of the radar in which all blips will ce culled 
            /// </summary>
            public float InnerCullingZone = 0f;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public float RadarRotationOffset = 0f;
            /// <summary>
            /// Do not replace this value
            /// </summary>
            public const float ConstantRadarRenderDistance = 4;
            /// <summary>
            /// The padding on the x and Y axis of the radar system
            /// </summary>
            public float xPadding, yPadding;
            /// <summary>
            /// Determins if the radar will ise Manual position or Snap Positioning
            /// </summary>
            public RadarPositioning radarPositioning = RadarPositioning.Snap;
            /// <summary>
            /// Determines where in scren space the radar system will be positioned
            /// </summary>
            public SnapPosition snapPosition = SnapPosition.BottomMiddle;
            /// <summary>
            /// Determining what defines the forward facing position of the radar 
            /// </summary>
            public FrontIs frontIs = FrontIs.North;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public Rect RadarRect, SnappedRect = new Rect(0, 0, 200, 200);
            // public bool LiveEditing;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            [UnityEngine.Serialization.FormerlySerializedAsAttribute("Count")]
            public int BlipCount = 0;
            public int tempBipCountValue;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            [UnityEngine.Serialization.FormerlySerializedAs("DesignsCount")]
            public int RotationTargetsCount = 0;
            public int temprottionTargtValue;
            /// <summary>
            /// Determines if we should use the scale of the Radar "Designs" child object instead of the RadarDiameter 
            /// </summary>
            public bool UseLocalScale;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public bool Visualize = true;
            /// <summary>
            /// Determines if the tracking bounds values will always be the same as 
            /// </summary>
            public bool LinkToTrackingBounds;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public bool ShowScaleSettings;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public bool ShowRenderCameraSettings;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public bool ShowPositioningSettings;
            /// <summary>
            /// When true, the radar ; diameter (Sale of the Radars "Designs" child object) when scales to a vlue greater or less than one 
            /// will not prompt the radar system to reposition itslf automatically to maintain a correct position in screen space
            /// 
            /// </summary>
            public bool IgnoreDiameterScale = false;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public bool ManualCameraSetup;
            /// <summary>
            /// determines if we will be using the gameobject in the scne with the tag "Main Camera"
            /// </summary>
            public bool UseMainCamera;
            /// <summary>
            /// Determines if the 3D Radar will also be using the screen space system
            /// </summary>
            public bool _3DSystemsWithScreenSpaceFunction;
            /// <summary>
            /// Determines if the radar can also be a minimap
            /// </summary>
            public bool _3DSystemsWithMinimapFunction;
            /// <summary>
            /// INTERNAL USE ONLY 
            /// </summary>
            public bool ShowMinimapSettings;
            /// <summary>
            /// This makes the Render Camera go into orthographics mode to correct for deault camera distortion
            /// </summary>
            public bool UseOrthographicForSideSnaps = false;
            /// <summary>
            /// Will no longer track the z position of objects and will insted pass tracked objects Y position into the z position of blips
            /// </summary>
            public bool TrackYPosition = false;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            //  public GameObject DesignsObject;
            /// <summary>
            /// Parent objct which will hold all the blips
            /// </summary>
            public Transform BlipsParentObject;
            /// <summary>
            /// The camera which will be the camera your player views the world through at any time 
            /// </summary>
            public Camera camera;
            /// <summary>
            /// The camera whuch will only render radar systems, (These camera are automatically created for you)
            /// </summary>
            public Camera renderingCamera;
            /// <summary>
            /// INTERNAL USE ONLY
            /// </summary>
            public string CameraTag = "MainCamera";
            /// <summary>
            /// The list of Rotation targets 
            /// </summary>
            public List<RotationTarget> RotationTargets = new List<RotationTarget>();
            /// <summary>
            /// The pan of the blips in the radar
            /// </summary>
            public Vector3 Pan = new Vector3();
            // private bool _3DSystemWithMinimap;


        }


        #endregion

        #endregion


        #endregion
    }
}