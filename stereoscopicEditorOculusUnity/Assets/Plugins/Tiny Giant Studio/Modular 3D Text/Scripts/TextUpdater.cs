using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace TinyGiantStudio.Text
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [HelpURL("https://ferdowsur.gitbook.io/modular-3d-text/text/text-updater")]
    public class TextUpdater : MonoBehaviour
    {
#if UNITY_EDITOR
        [HideInInspector]
        [SerializeField]
        private int openTime = 0;
#endif

        Modular3DText Text => GetComponent<Modular3DText>();


#if UNITY_EDITOR
        [SerializeField, HideInInspector] int GUIID;
#endif

        [ExecuteAlways]
        private void Awake()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                this.enabled = true;

            // If a text with combined mesh is duplicated, they have a sharedmesh. If one of those is deleted, like when updating the text on one of them, both are destroyed, that is, the shared mesh is destroyed
            // This is used to make a new copy
            #region Duplication Check
            if (GUIID == 0)
            {
                GUIID = gameObject.GetInstanceID();
            }
            else if (GUIID != gameObject.GetInstanceID())
            {
                GameObject old = EditorUtility.InstanceIDToObject(GUIID) as GameObject;
                if (old != null)
                {
                    MeshFilter original = old.GetComponent<MeshFilter>();
                    MeshFilter myMeshFilter = GetComponent<MeshFilter>();

                    if (myMeshFilter != null && original != null && myMeshFilter != original)
                    {
                        if (myMeshFilter.sharedMesh == original.sharedMesh)
                        {
                            myMeshFilter.sharedMesh = new Mesh()
                            {
                                vertices = myMeshFilter.sharedMesh.vertices,
                                triangles = myMeshFilter.sharedMesh.triangles,
                                normals = myMeshFilter.sharedMesh.normals,
                                tangents = myMeshFilter.sharedMesh.tangents,
                                bounds = myMeshFilter.sharedMesh.bounds,
                                uv = myMeshFilter.sharedMesh.uv
                            };
                        }
                    }
                }

                GUIID = gameObject.GetInstanceID();
            }
            #endregion  Duplication Check

            if (openTime < 5)
            {
                openTime++;
                if (openTime < 2)
                    return;
            }
            PrefabUtility.prefabInstanceUpdated += OnPrefabInstanceUpdated;
#endif

            if (!Text)
                return;

            if (EmptyText(Text))
                Text.UpdateText();
        }



#if UNITY_EDITOR
        void OnPrefabInstanceUpdated(GameObject instance)
        {
            EditorApplication.delayCall += () => CheckPrefab();
        }

        private void CheckPrefab()
        {
            if (!this)
                return;

            if (Text == null)
                return;

            if (!Text.Font)
                return;

            bool prefabConnected = PrefabUtility.GetPrefabInstanceStatus(this.gameObject) == PrefabInstanceStatus.Connected;
            if (prefabConnected)
            {
                EditorApplication.delayCall += () => UpdateText();
            }
        }

        private void UpdateText()
        {
            if (!this)
                return;

            if (Text == null)
                return;

            if (Text)
            {
                if (!Text.updatedAfterStyleUpdateOnPrefabInstances)
                {
                    if (Text.debugLogs)
                        Debug.Log("Text updated due to prefab update.");
                    Text.CleanUpdateText();
                    Text.updatedAfterStyleUpdateOnPrefabInstances = true;//buggy
                }
            }
        }
#endif



        private bool EmptyText(Modular3DText text)
        {
            if (string.IsNullOrEmpty(text.Text))
            {
                return false;
            }

            if (gameObject.GetComponent<MeshFilter>())
            {
                if (gameObject.GetComponent<MeshFilter>().sharedMesh != null)
                {
                    return false;
                }
            }

            if (text.characterObjectList.Count > 0)
            {
                for (int i = 0; i < text.characterObjectList.Count; i++)
                {
                    if (text.characterObjectList[i])
                    {
                        return false;
                    }
                }
            }


            return true;
        }
    }
}