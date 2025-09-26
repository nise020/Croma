// Assets/Editor/MeshFoliagePainter.cs
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MeshFoliagePainter : EditorWindow
{
    [System.Serializable]
    public class Variation
    {
        public GameObject prefab;
        public float weight = 1f;
        public Vector2 scaleRange = new Vector2(0.9f, 1.2f);
        public float yOffset = 0f;
        public bool alignToNormal = true;          // 표면 기울기에 맞출지
        public bool randomYawOnly = true;          // Yaw만 랜덤 회전

        // ⬇ 추가된 옵션들
        public enum UpAxis { X, Y, Z, MinusX, MinusY, MinusZ }
        public UpAxis upAxis = UpAxis.Y;           // 이 프리팹의 '위' 축
        public Vector3 rotationOffsetEuler = Vector3.zero; // 추가 회전 보정(도 단위)
    }

    // ---- UI State ----
    public Transform parent;
    public List<Variation> palette = new List<Variation>();
    public LayerMask surfaceMask = ~0;
    public float brushRadius = 3f;
    public float densityPer10m2 = 8f;            // 10m^2 당 개수
    public float minSeparation = 1.2f;           // 최소 간격
    public float maxSlopeDeg = 35f;
    public int seed = 12345;
    public bool snapToHit = true;                // 히트 지점 기준 배치
    public bool makeStatic = true;
    public bool showGizmo = true;

    // ---- Runtime ----
    private bool isPainting;
    private bool isErasing;
    private Vector3 lastHitPos;
    private System.Random rnd;
    private HashSet<Transform> cachedChildren = new HashSet<Transform>();
    private SpatialHash spatial;
    private double lastStrokeTime;

    [MenuItem("Tools/Foliage/Mesh Foliage Painter %#f")]
    static void Open() => GetWindow<MeshFoliagePainter>("Foliage Painter");

    void OnEnable()
    {
        SceneView.duringSceneGui += DuringSceneGUI;
        rnd = new System.Random(seed);
        BuildCache();
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
        parent = (Transform)EditorGUILayout.ObjectField("Parent (optional)", parent, typeof(Transform), true);
        makeStatic = EditorGUILayout.Toggle("Mark Static", makeStatic);

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Surface & Rules", EditorStyles.boldLabel);
        surfaceMask = LayerMaskField("Surface Mask", surfaceMask);
        maxSlopeDeg = EditorGUILayout.Slider("Max Slope (deg)", maxSlopeDeg, 0, 60);
        snapToHit = EditorGUILayout.Toggle("Snap To Hit", snapToHit);

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Brush", EditorStyles.boldLabel);
        brushRadius = EditorGUILayout.Slider("Radius (m)", brushRadius, 0.3f, 20f);
        densityPer10m2 = EditorGUILayout.Slider("Density (/10m²)", densityPer10m2, 0.1f, 50f);
        minSeparation = EditorGUILayout.Slider("Min Separation (m)", minSeparation, 0.1f, 5f);
        seed = EditorGUILayout.IntField("Random Seed", seed);
        showGizmo = EditorGUILayout.Toggle("Show Brush Gizmo", showGizmo);

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Palette (variations)", EditorStyles.boldLabel);
        SerializedObject so = new SerializedObject(this);
        EditorGUILayout.PropertyField(so.FindProperty(nameof(palette)), true);
        so.ApplyModifiedProperties();

        EditorGUILayout.Space(8);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Rebuild Cache")) BuildCache();
            if (GUILayout.Button("Clear Selection Cache")) { cachedChildren.Clear(); spatial?.Clear(); }
        }

        EditorGUILayout.HelpBox(
            "Scene 뷰: Left Drag = Paint, Shift+Drag = Erase\n" +
            "[ / ] = 반경 -, +  /  - / = 밀도 -, +\n" +
            "표면에는 MeshCollider(또는 Collider)가 있어야 합니다.", MessageType.Info);
    }

    // ------- Scene GUI -------
    void DuringSceneGUI(SceneView sv)
    {
        Event e = Event.current;
        var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        if (!Physics.Raycast(ray, out var hit, 10000f, surfaceMask, QueryTriggerInteraction.Ignore))
        {
            DrawCursor(null);
            return;
        }

        DrawCursor(hit);
        HandleHotkeys(e);

        if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
        {
            isErasing = e.shift;
            isPainting = !isErasing;
            lastHitPos = hit.point;
            lastStrokeTime = EditorApplication.timeSinceStartup;
            GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
            e.Use();
        }
        else if (e.type == EventType.MouseDrag && GUIUtility.hotControl != 0)
        {
            if (isPainting) PaintStroke(hit);
            else if (isErasing) EraseStroke(hit);

            lastHitPos = hit.point;
            e.Use();
        }
        else if (e.type == EventType.MouseUp && GUIUtility.hotControl != 0)
        {
            isPainting = isErasing = false;
            GUIUtility.hotControl = 0;
            e.Use();
        }
    }

    void PaintStroke(RaycastHit hit)
    {
        EnsureParent();

        if (EditorApplication.timeSinceStartup - lastStrokeTime < 0.05) return;
        lastStrokeTime = EditorApplication.timeSinceStartup;

        float area = Mathf.PI * brushRadius * brushRadius;
        int target = Mathf.CeilToInt(densityPer10m2 * (area / 10f));
        int tries = Mathf.Clamp(target * 6, 32, 4096);
        int placed = 0;

        Undo.IncrementCurrentGroup();
        int group = Undo.GetCurrentGroup();

        for (int i = 0; i < tries && placed < target; i++)
        {
            Vector2 r = RandomInCircle(rnd) * brushRadius;
            Vector3 p = hit.point + hit.transform.right * r.x + hit.transform.forward * r.y + Vector3.up * 200f;

            if (!Physics.Raycast(p, Vector3.down, out var h, 1000f, surfaceMask, QueryTriggerInteraction.Ignore))
                continue;

            if (Vector3.Angle(h.normal, Vector3.up) > maxSlopeDeg) continue;
            if (!spatial.CanPlace(h.point, minSeparation)) continue;

            var varr = PickVariation();
            if (varr == null || varr.prefab == null) continue;

            var go = (GameObject)PrefabUtility.InstantiatePrefab(varr.prefab);
            Undo.RegisterCreatedObjectUndo(go, "Paint Foliage");
            if (parent) go.transform.SetParent(parent, true);

            Vector3 pos = snapToHit ? h.point + Vector3.up * varr.yOffset : h.collider.transform.position;
            go.transform.position = pos;

            // ---- 회전(업데이트된 로직) ----
            Quaternion rot = Quaternion.identity;

            // ① 프리팹의 '위 축'을 표면 노멀에 맞추기
            Vector3 prefabUp = AxisVector(varr.upAxis);
            if (varr.alignToNormal)
                rot = Quaternion.FromToRotation(prefabUp, h.normal);

            // ② Yaw 랜덤(기울임을 적용했다면 노멀 축 기준)
            Vector3 yawAxis = varr.alignToNormal ? h.normal : Vector3.up;
            if (varr.randomYawOnly)
                rot = Quaternion.AngleAxis((float)rnd.NextDouble() * 360f, yawAxis) * rot;
            else
                rot *= RandomRotation(rnd);

            // ③ 추가 보정
            rot = rot * Quaternion.Euler(varr.rotationOffsetEuler);

            go.transform.rotation = rot;

            // 스케일
            float s = Mathf.Lerp(varr.scaleRange.x, varr.scaleRange.y, (float)rnd.NextDouble());
            go.transform.localScale = Vector3.one * s;

            if (makeStatic)
                GameObjectUtility.SetStaticEditorFlags(go, StaticEditorFlags.BatchingStatic | StaticEditorFlags.OccludeeStatic | StaticEditorFlags.OccluderStatic);

            spatial.Register(go.transform.position);
            cachedChildren.Add(go.transform);

            placed++;
        }

        Undo.CollapseUndoOperations(group);
    }

    void EraseStroke(RaycastHit hit)
    {
        if (parent == null) return;

        var unique = new List<Transform>();
        var seen = new HashSet<Transform>();
        foreach (Transform t in parent)
        {
            if (t == null) continue;
            float d2 = (t.position - hit.point).sqrMagnitude;
            if (d2 <= brushRadius * brushRadius && !seen.Contains(t))
            {
                unique.Add(t);
                seen.Add(t);
            }
        }
        unique.Sort((a, b) =>
            (a.position - hit.point).sqrMagnitude.CompareTo((b.position - hit.point).sqrMagnitude));

        int removeMax = Mathf.Min(50, unique.Count);
        if (removeMax == 0) return;

        Undo.IncrementCurrentGroup();
        int group = Undo.GetCurrentGroup();
        for (int i = 0; i < removeMax; i++)
        {
            var tr = unique[i];
            spatial.Unregister(tr.position);
            Undo.DestroyObjectImmediate(tr.gameObject);
        }
        Undo.CollapseUndoOperations(group);
    }

    void DrawCursor(RaycastHit? hit)
    {
        if (!showGizmo) return;

        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
        Handles.color = new Color(0.2f, 0.8f, 0.4f, 0.4f);

        if (hit.HasValue)
        {
            var h = hit.Value;
            Handles.DrawWireDisc(h.point, h.normal, brushRadius);
            Handles.DrawSolidDisc(h.point, h.normal, 0.06f);
        }
    }

    void HandleHotkeys(Event e)
    {
        if (e.type != EventType.KeyDown) return;

        if (e.keyCode == KeyCode.LeftBracket) { brushRadius = Mathf.Max(0.1f, brushRadius - 0.2f); e.Use(); }
        if (e.keyCode == KeyCode.RightBracket) { brushRadius += 0.2f; e.Use(); }

        if (e.keyCode == KeyCode.Minus) { densityPer10m2 = Mathf.Max(0.1f, densityPer10m2 - 0.5f); e.Use(); }
        if (e.keyCode == KeyCode.Equals) { densityPer10m2 += 0.5f; e.Use(); }
    }

    Variation PickVariation()
    {
        if (palette == null || palette.Count == 0) return null;
        float total = 0f;
        foreach (var v in palette) total += Mathf.Max(0f, v.weight);
        if (total <= 0f) return null;

        float r = (float)rnd.NextDouble() * total;
        float acc = 0f;
        foreach (var v in palette)
        {
            acc += Mathf.Max(0f, v.weight);
            if (r <= acc) return v;
        }
        return palette[palette.Count - 1];
    }

    void EnsureParent()
    {
        if (parent != null) return;
        var go = GameObject.Find("FoliageRoot");
        if (!go) go = new GameObject("FoliageRoot");
        parent = go.transform;
    }

    void BuildCache()
    {
        if (parent == null)
        {
            cachedChildren.Clear();
            spatial = new SpatialHash(minSeparation);
            return;
        }

        cachedChildren.Clear();
        foreach (Transform t in parent)
            if (t) cachedChildren.Add(t);

        spatial = new SpatialHash(minSeparation);
        foreach (var t in cachedChildren)
            spatial.Register(t.position);
    }

    static Vector2 RandomInCircle(System.Random rnd)
    {
        float u = (float)rnd.NextDouble();
        float v = (float)rnd.NextDouble();
        float r = Mathf.Sqrt(u);
        float theta = 2f * Mathf.PI * v;
        return new Vector2(r * Mathf.Cos(theta), r * Mathf.Sin(theta));
    }

    static Quaternion RandomRotation(System.Random rnd)
    {
        float rx = (float)rnd.NextDouble() * 360f;
        float ry = (float)rnd.NextDouble() * 360f;
        float rz = (float)rnd.NextDouble() * 360f;
        return Quaternion.Euler(rx, ry, rz);
    }

    // ⬇ 추가: UpAxis를 벡터로
    static Vector3 AxisVector(Variation.UpAxis a) => a switch
    {
        Variation.UpAxis.X => Vector3.right,
        Variation.UpAxis.Y => Vector3.up,
        Variation.UpAxis.Z => Vector3.forward,
        Variation.UpAxis.MinusX => Vector3.left,
        Variation.UpAxis.MinusY => Vector3.down,
        Variation.UpAxis.MinusZ => Vector3.back,
        _ => Vector3.up
    };

    static LayerMask LayerMaskField(string label, LayerMask selected)
    {
        var layers = UnityEditorInternal.InternalEditorUtility.layers;
        var layerNumbers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
            layerNumbers[i] = LayerMask.NameToLayer(layers[i]);

        int maskWithoutEmpty = 0;
        for (int i = 0; i < layerNumbers.Length; i++)
            if (((1 << layerNumbers[i]) & selected.value) > 0)
                maskWithoutEmpty |= (1 << i);

        maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers);
        int newMask = 0;
        for (int i = 0; i < layerNumbers.Length; i++)
            if ((maskWithoutEmpty & (1 << i)) > 0)
                newMask |= (1 << layerNumbers[i]);

        selected.value = newMask;
        return selected;
    }

    // --- simple spatial hash for distance checks ---
    class SpatialHash
    {
        readonly float cell;
        readonly Dictionary<Vector3Int, List<Vector3>> map = new();

        public SpatialHash(float minDist)
        {
            cell = Mathf.Max(0.1f, minDist) * 0.7071f;
        }

        Vector3Int Key(Vector3 p)
        {
            return new Vector3Int(
                Mathf.FloorToInt(p.x / cell),
                Mathf.FloorToInt(p.y / cell),
                Mathf.FloorToInt(p.z / cell));
        }

        public void Register(Vector3 p)
        {
            var k = Key(p);
            if (!map.TryGetValue(k, out var list)) { list = new List<Vector3>(); map[k] = list; }
            list.Add(p);
        }

        public void Unregister(Vector3 p)
        {
            var k = Key(p);
            if (map.TryGetValue(k, out var list)) list.Remove(p);
        }

        public bool CanPlace(Vector3 p, float minDist)
        {
            float d2 = minDist * minDist;
            var k = Key(p);
            for (int x = -1; x <= 1; x++)
                for (int y = -1; y <= 1; y++)
                    for (int z = -1; z <= 1; z++)
                    {
                        var kk = new Vector3Int(k.x + x, k.y + y, k.z + z);
                        if (!map.TryGetValue(kk, out var list)) continue;
                        for (int i = 0; i < list.Count; i++)
                        {
                            if ((list[i] - p).sqrMagnitude < d2) return false;
                        }
                    }
            return true;
        }

        public void Clear() => map.Clear();
    }
}
#endif
