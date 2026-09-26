using System;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

// PORT: ferramenta de diagnostico - lista os parametros dos efeitos do mixer como o editor os ve.
public static class PortMixerDump
{
    public static void Dump()
    {
        var asm = typeof(AssetPostprocessor).Assembly;
        var defs = asm.GetType("UnityEditor.Audio.MixerEffectDefinitions", true);
        var getParams = defs.GetMethod("GetEffectParameters", BindingFlags.Public | BindingFlags.Static);
        var sb = new StringBuilder();
        foreach (var name in new[] { "SFX Reverb", "Send", "Receive", "Attenuation" })
        {
            var arr = (Array)getParams.Invoke(null, new object[] { name });
            sb.Append("PORTDUMP DEF " + name + ":");
            foreach (var p in arr)
                sb.Append(" [" + p.GetType().GetField("name").GetValue(p) + "=" + p.GetType().GetField("defaultValue").GetValue(p) + "]");
            sb.AppendLine();
        }
        var ctlType = asm.GetType("UnityEditor.Audio.AudioMixerEffectController", true);
        foreach (var o in AssetDatabase.LoadAllAssetsAtPath("Assets/AudioMixerController/SoundMixer.mixer"))
        {
            if (o.GetType() != ctlType) continue;
            var en = (string)ctlType.GetProperty("effectName").GetValue(o);
            if (en != "SFX Reverb") continue;
            var so = new SerializedObject(o);
            var ps = so.FindProperty("m_Parameters");
            sb.Append("PORTDUMP INST " + en + ":");
            for (int i = 0; i < ps.arraySize; i++)
                sb.Append(" " + ps.GetArrayElementAtIndex(i).FindPropertyRelative("m_ParameterName").stringValue);
            sb.AppendLine();
        }
        Debug.Log(sb.ToString());
    }
}
