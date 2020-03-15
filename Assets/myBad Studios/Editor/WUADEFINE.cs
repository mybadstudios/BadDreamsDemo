using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public class WUADEFINE
{
	static WUADEFINE()
	{
		BuildTargetGroup btg = EditorUserBuildSettings.selectedBuildTargetGroup;
		string defines_field = PlayerSettings.GetScriptingDefineSymbolsForGroup(btg);
		List<string> defines = new List<string>(defines_field.Split(';'));
		if (!defines.Contains("WUA"))
		{
			defines.Add("WUA");
			PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, string.Join(";", defines.ToArray()));
		}
	}
}


