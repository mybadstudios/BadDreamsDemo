using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public class WUSKUDEFINE
{
	static WUSKUDEFINE()
	{
		BuildTargetGroup btg = EditorUserBuildSettings.selectedBuildTargetGroup;
		string defines_field = PlayerSettings.GetScriptingDefineSymbolsForGroup(btg);
		List<string> defines = new List<string>(defines_field.Split(';'));
		if (!defines.Contains("WUSKU"))
		{
			defines.Add("WUSKU");
			PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, string.Join(";", defines.ToArray()));
		}
	}
}


