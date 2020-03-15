using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public class WULDEFINE
{
	static WULDEFINE()
	{
		BuildTargetGroup btg = EditorUserBuildSettings.selectedBuildTargetGroup;
		string defines_field = PlayerSettings.GetScriptingDefineSymbolsForGroup(btg);
		List<string> defines = new List<string>(defines_field.Split(';'));
		if (!defines.Contains("WUL"))
		{
			defines.Add("WUL");
			PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, string.Join(";", defines.ToArray()));
		}
	}
}


