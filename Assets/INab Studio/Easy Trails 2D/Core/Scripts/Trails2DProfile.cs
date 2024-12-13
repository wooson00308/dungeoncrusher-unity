using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "TrailsProfile")]
public class Trails2DProfile : ScriptableObject
{
    public int objectsSpawnedBetweenRefresh;
    public bool hideTrailOnDisabled;
    public float depthStep = 0.001f;
    public float depthMinValue = -1f;
    public int trailOrderInLayer;
    public float trailObjectRefreshRate;
    public bool overrideSpriteColor;
    public Color overrideColor;
    public Trails2D.AutomaticTrigger automaticTrigger;
    public Trails2D.ActivationCondition activationCondition;
    public float velocityMagnitudeToSpawn;
    public Trails2D.TrailSpawnCondition trailSpawnCondition;
    public float objectsPerSecond;
    public float distanceBetweenObjects;
    public float trailLifespan;
    public Trails2D.ColorType colorType;
    public float textureVisibility;
    public Gradient colorOverTime;
    public Trails2D.ScaleType scaleType;
    public AnimationCurve scaleOverTime = new AnimationCurve();
    public AnimationCurve scaleOverTimeX = new AnimationCurve();
    public AnimationCurve scaleOverTimeY = new AnimationCurve();

    public bool velocityInLocalSpace = false;

    public void Save(Trails2D trails)
    {
        velocityInLocalSpace = trails.velocityInLocalSpace;
        objectsSpawnedBetweenRefresh = trails.objectsSpawnedBetweenRefresh;
        depthStep = trails.depthStep;
        depthMinValue = trails.depthMinValue;
        hideTrailOnDisabled = trails.hideTrailOnDisabled;
        trailOrderInLayer = trails.trailOrderInLayer;
        trailObjectRefreshRate = trails.trailObjectRefreshRate;
        activationCondition = trails.activationCondition;
        velocityMagnitudeToSpawn = trails.velocityMagnitudeToSpawn;
        trailSpawnCondition = trails.trailSpawnCondition;
        objectsPerSecond = trails.objectsPerSecond;
        distanceBetweenObjects = trails.distanceBetweenObjects;
        trailLifespan = trails.trailLifespan;
        colorType = trails.colorType;
        textureVisibility = trails.textureVisibility;
        scaleType = trails.scaleType;
        overrideSpriteColor = trails.overrideSpriteColor;
        automaticTrigger = trails.automaticTrigger;
        overrideColor = trails.overrideColor;

        colorOverTime = new Gradient();
        colorOverTime.alphaKeys = trails.colorOverTime.alphaKeys;
        colorOverTime.colorKeys = trails.colorOverTime.colorKeys;
        colorOverTime.mode = trails.colorOverTime.mode;

        scaleOverTime = CopyComponent(trails.scaleOverTime);
        scaleOverTimeX = CopyComponent(trails.scaleOverTimeX);
        scaleOverTimeY = CopyComponent(trails.scaleOverTimeY);
    }

    AnimationCurve CopyComponent(AnimationCurve original)
    {
        var dst = new AnimationCurve();
        dst.keys = original.keys;
        dst.postWrapMode = original.postWrapMode;
        dst.preWrapMode = original.preWrapMode;
        return dst;
    }
}
