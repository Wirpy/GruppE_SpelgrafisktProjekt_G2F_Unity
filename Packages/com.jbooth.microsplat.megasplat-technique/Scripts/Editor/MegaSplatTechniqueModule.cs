//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//
// Impliments MegaSplat technique for 256 textures in MicroSplat
//////////////////////////////////////////////////////


using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections.Generic;
using System.Text;

namespace JBooth.MicroSplat
{
#if __MICROSPLAT__
   [InitializeOnLoad]
   public class MegaSplatTechniqueModule : FeatureDescriptor
   {
      const string sDefine = "__MICROSPLAT_MEGA__";
      static MegaSplatTechniqueModule ()
      {
         MicroSplatDefines.InitDefine (sDefine);
      }
      [PostProcessSceneAttribute (0)]
      public static void OnPostprocessScene ()
      {
         MicroSplatDefines.InitDefine (sDefine);
      }

      public override string ModuleName ()
      {
         return "MegaSplat Technique";
      }
      public override string GetHelpPath()
      {
         return "https://docs.google.com/document/d/1vD_LY5zxA_C78pDNdsC8xoA4HOezzPsVk9pz7ER9Gso/edit?usp=sharing";
      }

      public enum DefineFeature
      {
         _MEGASPLAT,
         _MEGASPLATTEXTURE,
         kNumFeatures,
      }


      // Can we template these somehow?
      static Dictionary<DefineFeature, string> sFeatureNames = new Dictionary<DefineFeature, string> ();
      public static string GetFeatureName (DefineFeature feature)
      {
         string ret;
         if (sFeatureNames.TryGetValue (feature, out ret))
         {
            return ret;
         }
         string fn = System.Enum.GetName (typeof (DefineFeature), feature);
         sFeatureNames [feature] = fn;
         return fn;
      }

      public static bool HasFeature (string [] keywords, DefineFeature feature)
      {
         string f = GetFeatureName (feature);
         for (int i = 0; i < keywords.Length; ++i)
         {
            if (keywords [i] == f)
               return true;
         }
         return false;
      }

      public override string GetVersion ()
      {
         return "3.9";
      }

      enum MegaSplatDataSource
      {
         None,
         Vertex,
         Texture
      }
      MegaSplatDataSource dataSource = MegaSplatDataSource.None;


      public override void DrawFeatureGUI (MicroSplatKeywords keywords)
      {
         
      }

      static GUIContent CMegaTexture = new GUIContent("Control Texture", "A control texture with 2 indexes and a blend weight in RGB, uncompressed");
      public override void DrawShaderGUI (MicroSplatShaderGUI shaderGUI, MicroSplatKeywords keywords, Material mat, MaterialEditor materialEditor, MaterialProperty [] props)
      {
         if (dataSource == MegaSplatDataSource.Texture && mat.HasProperty("_MegaSplatTexture"))
         {
            if (MicroSplatUtilities.DrawRollup("MegaSplat Technique"))
            {
               materialEditor.TexturePropertySingleLine(CMegaTexture, shaderGUI.FindProp("_MegaSplatTexture", props));
            }
         }
         // revision propdata
         var pd = MicroSplatShaderGUI.FindOrCreatePropTex(mat);
         if (pd.maxTextures == 32 && dataSource != MegaSplatDataSource.None)
         {
            pd.maxTextures = 256;
            EditorUtility.SetDirty(pd);
         }
         else if (pd.maxTextures == 256 && dataSource == MegaSplatDataSource.None)
         {
            pd.maxTextures = 32;
            EditorUtility.SetDirty(pd);
         }
      }

      public override string [] Pack ()
      {
         List<string> features = new List<string> ();

         return features.ToArray ();
      }
      public override void Unpack (string [] keywords)
      {
         dataSource = MegaSplatDataSource.None;
         if (HasFeature (keywords, DefineFeature._MEGASPLAT))
         {
            dataSource = MegaSplatDataSource.Vertex;
         }
         else if (HasFeature(keywords, DefineFeature._MEGASPLATTEXTURE))
         {
            dataSource = MegaSplatDataSource.Texture;
         }
      }

      TextAsset megasplat_func_vertex;
      TextAsset megasplat_func_texture;
      public override void InitCompiler (string [] paths)
      {
         for (int i = 0; i < paths.Length; ++i)
         {
            string p = paths [i];
            if (p.EndsWith ("microsplat_megasplat_func.txt"))
            {
               megasplat_func_vertex = AssetDatabase.LoadAssetAtPath<TextAsset> (p);
            }
            if (p.EndsWith("microsplat_megasplat_func_texture.txt"))
            {
               megasplat_func_texture = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
            }
         }
      }

      public override void WriteFunctions (string[] features, StringBuilder sb)
      {
         if (dataSource == MegaSplatDataSource.Vertex)
         {
            sb.AppendLine (megasplat_func_vertex.text);
         }
         else if (dataSource == MegaSplatDataSource.Texture)
         {
            sb.AppendLine(megasplat_func_texture.text);
         }
      }

      public override void WriteProperties (string [] features, StringBuilder sb)
      {
         if (dataSource == MegaSplatDataSource.Texture)
         {
            sb.AppendLine("   _MegaSplatTexture(\"Mega Texture\", 2D) = \"black\" {}");
         }
      }

      public override void ComputeSampleCounts (string [] features, ref int arraySampleCount, ref int textureSampleCount, ref int maxSamples, ref int tessellationSamples, ref int depTexReadLevel)
      {
        
      }

      public override void DrawPerTextureGUI (int index, MicroSplatKeywords keywords, Material mat, MicroSplatPropData propData)
      {
         
      }
   }

#endif
}