using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
[CustomEditor(typeof(Trails2D))]
[CanEditMultipleObjects]
public class Trails2DEditor : Editor
{
	private Trails2D myTarget;

	SerializedProperty trailOrderInLayer,
	trailObjectRefreshRate,
	activationCondition,
	velocityMagnitudeToSpawn,
	trailSpawnCondition,
	objectsPerSecond,
	distanceBetweenObjects,
	trailLifespan,
	colorType,
	textureVisibility,
	colorOverTime,
	scaleType,
	scaleOverTime,
	scaleOverTimeX,
	scaleOverTimeY,
	overrideSpriteColor,
	automaticTrigger,
	trailsProfile,
		velocityInLocalSpace,
		objectsSpawnedBetweenRefresh,
		 depthStep,
	depthMinValue,
	hideTrailOnDisabled,
	overrideColor;

	void OnEnable()
	{
		FindProperties();
	}

	private void FindProperties()
	{
		trailOrderInLayer = serializedObject.FindProperty("trailOrderInLayer");
		trailObjectRefreshRate = serializedObject.FindProperty("trailObjectRefreshRate");
		activationCondition = serializedObject.FindProperty("activationCondition");
		velocityMagnitudeToSpawn = serializedObject.FindProperty("velocityMagnitudeToSpawn");
		trailSpawnCondition = serializedObject.FindProperty("trailSpawnCondition");
		objectsPerSecond = serializedObject.FindProperty("objectsPerSecond");
		distanceBetweenObjects = serializedObject.FindProperty("distanceBetweenObjects");
		trailLifespan = serializedObject.FindProperty("trailLifespan");
		colorType = serializedObject.FindProperty("colorType");
		textureVisibility = serializedObject.FindProperty("textureVisibility");
		colorOverTime = serializedObject.FindProperty("colorOverTime");
		scaleType = serializedObject.FindProperty("scaleType");
		scaleOverTime = serializedObject.FindProperty("scaleOverTime");
		scaleOverTimeX = serializedObject.FindProperty("scaleOverTimeX");
		scaleOverTimeY = serializedObject.FindProperty("scaleOverTimeY");
		overrideSpriteColor = serializedObject.FindProperty("overrideSpriteColor");
		overrideColor = serializedObject.FindProperty("overrideColor");
		automaticTrigger = serializedObject.FindProperty("automaticTrigger");
		trailsProfile = serializedObject.FindProperty("trailsProfile");
		velocityInLocalSpace = serializedObject.FindProperty("velocityInLocalSpace");
		objectsSpawnedBetweenRefresh = serializedObject.FindProperty("objectsSpawnedBetweenRefresh");
		depthStep = serializedObject.FindProperty("depthStep");
		depthMinValue = serializedObject.FindProperty("depthMinValue");
		hideTrailOnDisabled = serializedObject.FindProperty("hideTrailOnDisabled");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		myTarget = (Trails2D)target;

		using (new EditorGUILayout.HorizontalScope())
		{
			DrawProperty(trailsProfile);
			if (GUILayout.Button("Create"))
			{
				Trails2DProfile instance = CreateInstance("Trails2DProfile") as Trails2DProfile;
				instance.Save(myTarget);

				var currentPath = Application.dataPath;
				var path = EditorUtility.SaveFilePanelInProject("Save profile", "Trails2DProfile", "asset", "Set file path for the profile.", currentPath);

				if (!string.IsNullOrEmpty(path))
				{
					AssetDatabase.CreateAsset(instance, path);
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
					EditorUtility.FocusProjectWindow();
					Selection.activeObject = instance;

					myTarget.CreateNewProfile(instance);
				}
			}

		}

		using (new EditorGUILayout.HorizontalScope())
		{
			if (trailsProfile.propertyType == SerializedPropertyType.ObjectReference)
			{
				if (trailsProfile.objectReferenceValue != null)
				{
					if (GUILayout.Button("Save"))
					{
						myTarget.SaveToProfile();
					}
					if (GUILayout.Button("Load"))
					{
						Undo.RecordObject(target, "Load From Profile");
						myTarget.LoadFromProfile();
					}
				}
			}
		}

		DrawProperty(trailOrderInLayer);
		DrawProperty(depthStep);
		DrawProperty(depthMinValue);
		DrawProperty(hideTrailOnDisabled);
		DrawProperty(trailObjectRefreshRate);
		DrawProperty(overrideSpriteColor);
		if (overrideSpriteColor.boolValue)
		{
			DrawProperty(overrideColor);
		}

		DrawProperty(automaticTrigger);

		DrawProperty(activationCondition);
		Trails2D.ActivationCondition autoTrigger = (Trails2D.ActivationCondition)activationCondition.enumValueIndex;
		switch (autoTrigger)
		{
			case Trails2D.ActivationCondition.VelocityMagnitude:
				DrawProperty(velocityMagnitudeToSpawn);
				DrawProperty(velocityInLocalSpace);
				break;
			case Trails2D.ActivationCondition.None:
				break;
		}

		DrawProperty(trailSpawnCondition);
		Trails2D.TrailSpawnCondition spawnCondition = (Trails2D.TrailSpawnCondition)trailSpawnCondition.enumValueIndex;
		switch (spawnCondition)
		{
			case Trails2D.TrailSpawnCondition.Distance:
				DrawProperty(distanceBetweenObjects);
				break;
			case Trails2D.TrailSpawnCondition.TimeInterval:
				DrawProperty(objectsPerSecond);
				DrawProperty(objectsSpawnedBetweenRefresh);
				break;
		}

		DrawProperty(trailLifespan);

		DrawProperty(colorType);
		Trails2D.ColorType color = (Trails2D.ColorType)colorType.enumValueIndex;
		switch (color)
		{
			case Trails2D.ColorType.Multiply:
				break;
			case Trails2D.ColorType.Replace:
				DrawProperty(textureVisibility);
				break;
		}

		DrawProperty(colorOverTime);

		DrawProperty(scaleType);
		Trails2D.ScaleType scale = (Trails2D.ScaleType)scaleType.enumValueIndex;
		switch (scale)
		{
			case Trails2D.ScaleType.None:
				break;
			case Trails2D.ScaleType.SeparateAxes:
				scaleOverTimeX.animationCurveValue = EditorGUILayout.CurveField(scaleOverTimeX.animationCurveValue);
				scaleOverTimeY.animationCurveValue = EditorGUILayout.CurveField(scaleOverTimeY.animationCurveValue);

				break;
			case Trails2D.ScaleType.BothAxes:
				scaleOverTime.animationCurveValue = EditorGUILayout.CurveField(scaleOverTime.animationCurveValue);
				break;
		}

		serializedObject.ApplyModifiedProperties();
	}

	static Gradient SafeGradientValue(SerializedProperty sp)
	{
		System.Reflection.BindingFlags instanceAnyPrivacyBindingFlags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
		System.Reflection.PropertyInfo propertyInfo = typeof(SerializedProperty).GetProperty(
			"gradientValue",
			instanceAnyPrivacyBindingFlags,
			null,
			typeof(Gradient),
			new Type[0],
			null
		);
		if (propertyInfo == null)
			return null;

		Gradient gradientValue = propertyInfo.GetValue(sp, null) as Gradient;
		return gradientValue;
	}

	void DrawProperty(SerializedProperty obj)
	{
		EditorGUILayout.PropertyField(obj);
	}

}
