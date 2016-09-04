﻿using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Mesh/Combine Children")]
public class CombineChildren : MonoBehaviour {

    public void Combine() {
        Matrix4x4 myTransform = transform.worldToLocalMatrix;
        Dictionary<string, List<CombineInstance>> combines = new Dictionary<string, List<CombineInstance>>();
        Dictionary<string, Material> namedMaterials = new Dictionary<string, Material>();
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        List<GameObject> toDestroy = new List<GameObject>();
        foreach (var meshRenderer in meshRenderers) {
            foreach (var material in meshRenderer.sharedMaterials)
                if (material != null && !combines.ContainsKey(material.name)) {
                    combines.Add(material.name, new List<CombineInstance>());
                    namedMaterials.Add(material.name, material);
                }
        }

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        foreach (var filter in meshFilters) {
            toDestroy.Add(filter.gameObject);
            if (filter.sharedMesh == null)
                continue;
            var filterRenderer = filter.GetComponent<Renderer>();
            if (filterRenderer == null || filterRenderer.sharedMaterial == null)
                continue;
            if (filterRenderer.sharedMaterials.Length > 1)
                continue;
            CombineInstance ci = new CombineInstance {
                mesh = filter.sharedMesh,
                transform = myTransform * filter.transform.localToWorldMatrix
            };
            combines[filterRenderer.sharedMaterial.name].Add(ci);

            DestroyImmediate(filterRenderer);
        }

        foreach (Material m in namedMaterials.Values) {
            var go = new GameObject(m.name);
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            var filter = go.AddComponent<MeshFilter>();
            filter.mesh.CombineMeshes(combines[m.name].ToArray(), true, true);

            var arenderer = go.AddComponent<MeshRenderer>();
            arenderer.material = m;

            go.AddComponent<MeshCollider>();
        }

        foreach (GameObject g in toDestroy) {
            DestroyImmediate(g);
        }
    }
}