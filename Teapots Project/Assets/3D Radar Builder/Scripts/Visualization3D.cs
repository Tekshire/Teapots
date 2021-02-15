using UnityEngine;

namespace DaiMangou.RadarBuilder3D.Editor
{
#if (UNITY_EDITOR)
    public class Visualization3D : MonoBehaviour
    {
        private _3DRadar Radar;
        public void OnDrawGizmos()
        {
            transform.hideFlags = HideFlags.None;
            if (!Radar) Radar = this.GetComponent<_3DRadar>();
            if (Radar.RadarDesign == null) return;
            if (!Radar.RadarDesign.Visualize) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, Radar.RadarDesign.TrackingBounds * transform.localScale.x);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, Radar.RadarDesign.InnerCullingZone * transform.localScale.x);
            if (Radar.RadarDesign.UseLocalScale) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, Radar.RadarDesign.RadarDiameter);
        }
    }
#endif
}