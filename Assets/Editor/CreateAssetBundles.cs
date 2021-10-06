using UnityEditor;

public class CreateAssetBundles
{

	[MenuItem("Riders X/Create Stage")]
	static void BuildAllAssetBundles()
	{
		BuildPipeline.BuildAssetBundles("Assets/Riders X/Stage/BUILD", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
	}
}