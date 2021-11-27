using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Common.Utils.Extensions
{
    public static class SceneUtils
    {
        public static Dictionary<int,Scene> GetLoadedScenesAndBuildIndex()
        {
            Dictionary<int, Scene> result = new Dictionary<int, Scene>();
            int countLoaded = SceneManager.sceneCount;
 
            for (int i = 0; i < countLoaded; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                result.Add(scene.buildIndex, scene);
            }

            return result;
        }

        public static int[] GetLoadedScenesByBuildIndex()
        {
            int countLoaded = SceneManager.sceneCount;
            int[] loadedScenes = new int[countLoaded];
 
            for (int i = 0; i < countLoaded; i++)
            {
                loadedScenes[i] = SceneManager.GetSceneAt(i).buildIndex;
            }

            return loadedScenes;
        }
    }
}