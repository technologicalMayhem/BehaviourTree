using System.Linq;
using UnityEditor;

namespace BehaviourTrees.UnityEditor
{
    /// <summary>
    ///     Hooks into the import pipeline to notify the editor of asset changes.
    /// </summary>
    public class AssetDatabaseChanges : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (EditorWindow.HasOpenInstances<BehaviourTreeEditor>())
            {
                var editor = BehaviourTreeEditor.GetOrOpen();
                if (deletedAssets.Contains(editor.TreeContainerPath)) editor.UnloadTree();
            }
        }
    }
}