using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Trails2D : MonoBehaviour
{
    public enum ColorType
    {
        Multiply,
        Replace
    }

    public enum ScaleType
    {
        None,
        SeparateAxes,
        BothAxes
    }

    public enum TrailSpawnCondition
    {
        Distance,
        TimeInterval
    }

    public enum ActivationCondition
    {
        VelocityMagnitude,
        None,
    }

    public enum AutomaticTrigger
    {
        Start,
        Never
    }

    public Trails2DProfile trailsProfile;

    [Tooltip("Trails renderer's order within a sorting layer.")]
    public int trailOrderInLayer = 0;
    [Tooltip("Step between Z position between each trail object.")]
    public float depthStep = 0.0001f;
    [Tooltip("When trail object Z position reaches this value, position of the next one is set to 0.")]
    public float depthMinValue = -1f;
    [Tooltip("If checked, trails will disaapear immadiately after disabling the game object.")]
    public bool hideTrailOnDisabled = true;

    [Tooltip("How often each trail object is refreshed. Lower if you encounter loss in performance when using long and dense trails.")]
    [Range(5, 60), Space] public float trailObjectRefreshRate = 60;
    [Tooltip("If checked, each trail object's sprite renderer color will be overriden.")]
    public bool overrideSpriteColor = false;
    [Tooltip("Color to override trail object's sprite renderer color.")]
    public Color overrideColor = Color.white;

    [Tooltip("Should automatically begin rendering the trail on Start.")]
    [Space] public AutomaticTrigger automaticTrigger;

    [Tooltip("Should there will be any additional conditions on when trails should render.")]
    [Space] public ActivationCondition activationCondition;
    [Tooltip("When the game objects exceed the velocity magnitude, the trails will be rendered.")]
    public float velocityMagnitudeToSpawn = 2f;
    [Tooltip("Should the velocity be calculated in local space.")]
    public bool velocityInLocalSpace = false;

    [Tooltip("Main spawn condition for rendering trails.")]
    [Space] public TrailSpawnCondition trailSpawnCondition;
    [Tooltip("How much trail objects should be spawned per second.")]
    [Range(0, 60)] public float objectsPerSecond = 1;
    [Tooltip("How much trail objects should be spawned per one trail refresh, e.g when objectsPerSecond = 30, and objectsSpawnedBetweenRefresh = 2, you will have trails spawned 60 times per second.")]
    [Range(1, 10)] public int objectsSpawnedBetweenRefresh = 1;
    [Tooltip("Distance between each trail object.")]
    public float distanceBetweenObjects = 1;

    [Tooltip("Duration of the trail objects.")]
    public float trailLifespan = 2;

    [Tooltip("How color gradient affects trail appearance.")]
    [Space] public ColorType colorType;
    [Tooltip("When 0, texture is completely replaced by color, 1 means trail will not be affected by color.")]
    [Range(0, 1)] public float textureVisibility = 0;

    [Tooltip("Gradient color with alpha channel that will be evaluated over lifespan of the trail.")]
    public Gradient colorOverTime = new Gradient
    {
        colorKeys = new GradientColorKey[] { new GradientColorKey(Color.white, 0), new GradientColorKey(Color.white, 1) },
        alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(0, 1) }
    };

    [Tooltip("How scale of the trail should be affected over time.")]
    [Space] public ScaleType scaleType;
    [Tooltip("Scale in X and Y axis that will be evaluated over lifespan of the trail.")]
    public AnimationCurve scaleOverTime = AnimationCurve.Constant(0, 1, 1);
    [Tooltip("Scale in X axis that will be evaluated over lifespan of the trail.")]
    public AnimationCurve scaleOverTimeX = AnimationCurve.Constant(0, 1, 1);
    [Tooltip("Scale in Y axis that will be evaluated over lifespan of the trail.")]
    public AnimationCurve scaleOverTimeY = AnimationCurve.Constant(0, 1, 1);

    static protected readonly string shPropColor = "_TrailColor";
    static protected readonly string shPropTextureAffect = "_TextureAffect";
    static protected readonly string shPropScaleX = "_TrailScaleX";
    static protected readonly string shPropScaleY = "_TrailScaleY";

    static protected readonly string shTrail2D = "Trails/2DTrail";
    static protected readonly string shTrail2DBlend = "Trails/2DTrailBlend";

    private SpriteRenderer spriteRenderer;

    private Material trailMaterial;
    private Material trailMaterialBlend;

    private Vector3 previousPosition;
    private Vector3 lastPositionSpawn;

    private GameObject objectsHolder;

    private Stack<GameObject> activeTrailObjects = new Stack<GameObject>();
    private Stack<GameObject> disabledTrailObjects = new Stack<GameObject>();

    private bool conditionsMet = false;
    private bool isTriggered = false;
    private float distancePassed = 0;
    private float TrailRefreshRate;
    private Coroutine drawTrailContinous;
    private float currentDepthPositionZ = 0;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastPositionSpawn = transform.position;

        trailMaterialBlend = new Material(Shader.Find(shTrail2DBlend));
        trailMaterial = new Material(Shader.Find(shTrail2D));

        if (velocityInLocalSpace)
        {
            previousPosition = transform.localPosition;
        }
        else
        {
            previousPosition = transform.position;
        }

        switch (automaticTrigger)
        {
            case AutomaticTrigger.Start:
                isTriggered = true;
                break;
            case AutomaticTrigger.Never:
                isTriggered = false;
                break;
        }

        objectsHolder = new GameObject("TrailObjectsHolder");
    }

    private void OnDisable()
    {
        if (hideTrailOnDisabled)
        {
            if (objectsHolder != null)
            {
                DestroyImmediate(objectsHolder);
            }
        }

        StopAllCoroutines();

    }

    private void OnEnable()
    {

        if (hideTrailOnDisabled)
        {
            objectsHolder = new GameObject("TrailObjectsHolder");
        }
        else
        {
            if (objectsHolder != null) Destroy(objectsHolder);

            StopAllCoroutines();

            objectsHolder = new GameObject("TrailObjectsHolder");
        }

        drawTrailContinous = StartCoroutine(DrawTrailContinous());

    }

    private void Update()
    {
        float distance;
        if (velocityInLocalSpace)
        {
            distance = (transform.localPosition - previousPosition).magnitude;
        }
        else
        {
            distance = (transform.position - previousPosition).magnitude;
        }

        float velocity = distance / Time.deltaTime;
        switch (activationCondition)
        {
            case ActivationCondition.VelocityMagnitude:
                if (velocity > velocityMagnitudeToSpawn)
                {
                    conditionsMet = true;
                }
                else
                {
                    conditionsMet = false;
                }
                break;
            case ActivationCondition.None:
                conditionsMet = true;
                break;
        }

        switch (trailSpawnCondition)
        {
            case TrailSpawnCondition.Distance:
                if (!conditionsMet) break;
                if (!isTriggered) break;
                objectsSpawnedBetweenRefresh = 1;
                distancePassed += distance;
                if (distancePassed > distanceBetweenObjects)
                {
                    distancePassed = 0;
                    SpawnTrailObject();
                };
                if (drawTrailContinous != null)
                {
                    StopCoroutine(drawTrailContinous);
                    drawTrailContinous = null;
                }
                break;
            case TrailSpawnCondition.TimeInterval:
                if (drawTrailContinous == null)
                {
                    drawTrailContinous = StartCoroutine(DrawTrailContinous());
                }
                break;
        }

        if (velocityInLocalSpace)
        {
            previousPosition = transform.localPosition;
        }
        else
        {
            previousPosition = transform.position;
        }
    }

    private IEnumerator DrawTrailContinous()
    {
        while (true)
        {
            TrailRefreshRate = 1 / objectsPerSecond;
            if (conditionsMet && isTriggered)
            {
                SpawnTrailObject();
            }
            yield return new WaitForSeconds(TrailRefreshRate);
        }

    }

    private IEnumerator TriggerTrailEnumerator(float timeActive)
    {
        TrailRefreshRate = 1 / objectsPerSecond;

        isTriggered = true;

        while (timeActive > 0)
        {
            timeActive -= TrailRefreshRate;

            yield return new WaitForSeconds(TrailRefreshRate);

        }
        isTriggered = false;
    }

    T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        var dst = destination.GetComponent(type) as T;
        if (!dst) dst = destination.AddComponent(type) as T;
        var fields = type.GetFields();
        foreach (var field in fields)
        {
            if (field.IsStatic) continue;
            field.SetValue(dst, field.GetValue(original));
        }
        var props = type.GetProperties();
        foreach (var prop in props)
        {
            if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
            prop.SetValue(dst, prop.GetValue(original, null), null);
        }
        return dst as T;
    }

    private void SpawnTrailObject()
    {
        float MaterialRefreshRate = 1 / trailObjectRefreshRate;

        for (int i = 1; i <= objectsSpawnedBetweenRefresh; i++)
        {
            float lerpTime = (float)i / objectsSpawnedBetweenRefresh;
            float timeOffset = MaterialRefreshRate / lerpTime;

            GameObject obj;
            if (disabledTrailObjects.Count > 0)
            {
                obj = disabledTrailObjects.Pop();
                obj.SetActive(true);
            }
            else
            {
                obj = new GameObject($"TrailObject {nameof(spriteRenderer)}");
                activeTrailObjects.Push(obj);
            }

            if (objectsHolder == null) continue;

            obj.transform.parent = objectsHolder.transform;
            var currentPosition = Vector3.Lerp(lastPositionSpawn, transform.position, lerpTime);
            currentDepthPositionZ += depthStep;
            if (currentDepthPositionZ > -depthMinValue) currentDepthPositionZ = 0;
            obj.transform.position = currentPosition - new Vector3(0, 0, currentDepthPositionZ);
            obj.transform.rotation = transform.rotation;
            obj.transform.localScale = transform.lossyScale;

            SpriteRenderer renderer = CopyComponent(spriteRenderer, obj);
            renderer.sortingOrder = trailOrderInLayer;


            if (overrideSpriteColor) renderer.color = overrideColor;

            MaterialPropertyBlock mtb = new MaterialPropertyBlock();

            renderer.GetPropertyBlock(mtb);
            if (colorType == ColorType.Replace)
            {
                mtb.SetFloat(shPropTextureAffect, textureVisibility);
                renderer.material = trailMaterialBlend;
            }
            else
            {
                renderer.material = trailMaterial;
            }

            renderer.SetPropertyBlock(mtb);

            StartCoroutine(AnimateTrailObject(renderer, mtb, obj.transform, MaterialRefreshRate, timeOffset));
        }

        lastPositionSpawn = transform.position;
    }

    private IEnumerator AnimateTrailObject(SpriteRenderer spriteRenderer, MaterialPropertyBlock materialPropertyBlock, Transform transform, float refreshRate, float timeOffset)
    {
        float value = 1 - timeOffset;
        var startScale = transform.localScale;
        Vector3 changedScale = new Vector3();

        float rate = refreshRate / trailLifespan;
        while (value > 0)
        {
            value -= rate;
            materialPropertyBlock.SetColor(shPropColor, colorOverTime.Evaluate(1 - value));
            switch (scaleType)
            {
                case ScaleType.BothAxes:
                    float scale = scaleOverTime.Evaluate(1 - value);
                    changedScale.x = startScale.x * scale;
                    changedScale.y = startScale.y * scale;
                    transform.localScale = changedScale;
                    break;
                case ScaleType.SeparateAxes:
                    changedScale.x = startScale.x * scaleOverTimeX.Evaluate(1 - value);
                    changedScale.y = startScale.y * scaleOverTimeY.Evaluate(1 - value);
                    transform.localScale = changedScale;
                    break;
            }
            spriteRenderer.SetPropertyBlock(materialPropertyBlock);
            yield return new WaitForSeconds(refreshRate);
        }

        yield return new WaitForSeconds(refreshRate);

        spriteRenderer.gameObject.SetActive(false);
        disabledTrailObjects.Push(spriteRenderer.gameObject);
    }


    /// <summary>
    /// Starts rendering trail over a certain period.
    /// </summary>
    ///      /// <param name="time">The time for which the trail will be rendered. </param>
    /// <returns>Returns true if trail was already rendering.</returns>
    public virtual bool TriggerTrail(float time)
    {
        bool wasActive = isTriggered;

        StartCoroutine(TriggerTrailEnumerator(time));
        return wasActive;
    }


    /// <summary>
    /// Starts rendering trail.
    /// </summary>
    public virtual void StartTriggeringTrail()
    {
        isTriggered = true;
    }

    /// <summary>
    /// Ends rendering trail.
    /// </summary>
    public virtual void StopTriggeringTrail()
    {
        isTriggered = false;

    }

    public void CreateNewProfile(Trails2DProfile profile)
    {
        trailsProfile = profile;
    }

    public void SaveToProfile()
    {
        trailsProfile.Save(this);
    }


    /// <summary>
    /// Replaces component values with profile asset values.
    /// </summary>
    public void LoadFromProfile()
    {
        var clone = trailsProfile;

        velocityInLocalSpace = clone.velocityInLocalSpace;
        objectsSpawnedBetweenRefresh = clone.objectsSpawnedBetweenRefresh;
        depthStep = clone.depthStep;
        depthMinValue = clone.depthMinValue;
        hideTrailOnDisabled = clone.hideTrailOnDisabled;

        trailOrderInLayer = clone.trailOrderInLayer;
        trailObjectRefreshRate = clone.trailObjectRefreshRate;
        activationCondition = clone.activationCondition;
        velocityMagnitudeToSpawn = clone.velocityMagnitudeToSpawn;
        trailSpawnCondition = clone.trailSpawnCondition;
        objectsPerSecond = clone.objectsPerSecond;
        distanceBetweenObjects = clone.distanceBetweenObjects;
        trailLifespan = clone.trailLifespan;
        colorType = clone.colorType;
        textureVisibility = clone.textureVisibility;
        scaleType = clone.scaleType;
        overrideSpriteColor = clone.overrideSpriteColor;
        automaticTrigger = clone.automaticTrigger;
        overrideColor = clone.overrideColor;

        colorOverTime.alphaKeys = clone.colorOverTime.alphaKeys;
        colorOverTime.colorKeys = clone.colorOverTime.colorKeys;
        colorOverTime.mode = clone.colorOverTime.mode;

        CopyAnimationCurve(ref scaleOverTime, clone.scaleOverTime);
        CopyAnimationCurve(ref scaleOverTimeX, clone.scaleOverTimeX);
        CopyAnimationCurve(ref scaleOverTimeY, clone.scaleOverTimeY);


    }

    void CopyAnimationCurve(ref AnimationCurve original, AnimationCurve copy)
    {
        original.keys = copy.keys;
        original.postWrapMode = copy.postWrapMode;
        original.preWrapMode = copy.preWrapMode;
    }
}
