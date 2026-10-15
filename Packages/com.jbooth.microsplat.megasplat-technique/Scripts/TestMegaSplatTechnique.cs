using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMegaSplatTechnique : MonoBehaviour
{
   public MeshFilter meshFilter;
   List<Color> colors;
   List<Vector4> texcoord0;

   float EncodeToFloat (Color enc)
   {
      var ex = (uint)(enc.r * 255);
      var ey = (uint)(enc.g * 255);
      var ez = (uint)(enc.b * 255);
      var ew = (uint)(enc.a * 255);
      var v = (ex << 24) + (ey << 16) + (ez << 8) + ew;
      return v / (256.0f * 256.0f * 256.0f * 256.0f);
   }

   Color DecodeToColor (float v)
   {
      var vi = (uint)(v * (256.0f * 256.0f * 256.0f * 256.0f));
      var ex = (int)(vi / (256 * 256 * 256) % 256);
      var ey = (int)((vi / (256 * 256)) % 256);
      var ez = (int)((vi / (256)) % 256);
      var ew = (int)(vi % 256);
      var e = new Color (ex / 255.0f, ey / 255.0f, ez / 255.0f, ew / 255.0f);
      return e;
   }

   int frame = 0;
   private void Update ()
   {
      meshFilter = GetComponent<MeshFilter>();
      if (meshFilter != null)
      {
         frame++;
         if (frame > 120)
         {
            frame = 0;

            var mesh = meshFilter.sharedMesh;
            if (colors == null)
            {
               colors = new List<Color> ();
               mesh.GetColors (colors);
            }
            if (texcoord0 == null)
            {
               texcoord0 = new List<Vector4>();
               mesh.GetUVs(0, texcoord0);
            }
            for (int i = 0; i < colors.Count; ++i)
            {
               Color c = colors [i];
               Vector4 texcoord = texcoord0[i];
               Color dec2 = DecodeToColor (c.g);
               Color dec3 = DecodeToColor (c.b);
               Color dec4 = DecodeToColor (c.a);
               Vector4 fx2 = DecodeToColor(texcoord.w);
               dec2.r = (float)UnityEngine.Random.Range (0, 4) / 255.0f; // index0
               
               dec2.g = UnityEngine.Random.Range (0, 4) / 255.0f; // index1
               dec2.b = UnityEngine.Random.Range (0.0f, 1.0f); // blend between 0 and 1
               // scatter
               dec3.r = UnityEngine.Random.Range (0, 1) / 255.0f;
               dec3.g = Mathf.Pow (UnityEngine.Random.Range (0.0f, 1.0f), 2.0f);

               // wetness
               dec4.g = UnityEngine.Random.Range (0.0f, 1.0f); // wetness

               // snow min/max
               fx2.x = UnityEngine.Random.Range(0.0f, 1.0f);
               fx2.y = fx2.x;

               c.g = EncodeToFloat (dec2);
               c.b = EncodeToFloat (dec3);
               c.a = EncodeToFloat (dec4);
               texcoord.w = EncodeToFloat(fx2);
               colors [i] = c;
               texcoord0[i] = texcoord;
            }
            mesh.SetColors (colors);
            mesh.SetUVs(0, texcoord0);
         }
      }
   }
}
