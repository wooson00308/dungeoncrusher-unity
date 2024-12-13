Support: Discord (https://discord.gg/K88zmyuZFD)

Online Documentation: https://inabstudios.gitbook.io/easy-trails-2d/
You can find more specific information on the asset functionality and setup process.


Do not delete EasyTrails2D/Materials folder, otherwise trails won't work in your build. Or you can include asset shaders In "Always Included Shaders" setting. 
You can easily explore demo scene to find out how to create cool effects with Easy Trails 2D.  If you don't know what given property does, you can refer to Properties or see the tooltip for the property.
After you create your desired effect you can save it's values to custom trails profile to reuse it later.
In order to make trail objects appear in good order, Z position of each object is slightly moved to the back. If you encounter any artifacts, try changing Depth Step.
Remember to keep Trail Order In Layer less than your sprite to keep trail behind the sprite.


Triggering trail
If you do not want your trail to be rendered always, you can set Automatic Trigger to Never, and use script functions to manage your trail.
//Starts rendering trail over a certain period.
//Returns true if trail was already rendering.
bool TriggerTrail(float time)
// Starts rendering trail till StopTriggeringTrail() is called.
public void StartTriggeringTrail()
// Ends rendering trail.
public virtual void StopTriggeringTrail()


Properties:
trailOrderInLayer - Trails renderer's order within a sorting layer.
depthStep - Step between Z position between each trail object.
depthMinValue - When trail object Z position reaches this value, position of the next one is set to 0.
hideTrailOnDisabled - If checked, trails will disaapear immadiately after disabling the game object.
trailObjectRefreshRate - How often each trail object is refreshed. Lower if you encounter loss in performance when using long and dense trails.
overrideSpriteColor - If checked, each trail object's sprite renderer color will be overriden.
overrideColor - Color to override trail object's sprite renderer color.
automaticTrigger - Should automatically begin rendering the trail on Start.
activationCondition - Should there will be any additional conditions on when trails should render.
velocityMagnitudeToSpawn - When the game objects exceed the velocity magnitude, the trails will be rendered.
velocityInLocalSpace - Should the velocity be calculated in local space.
trailSpawnCondition - Main spawn condition for rendering trails.
objectsPerSecond - How much trail objects should be spawned per second.
objectsSpawnedBetweenRefresh - How much trail objects should be spawned per one trail refresh, e.g when objectsPerSecond = 30, and objectsSpawnedBetweenRefresh = 2, you will have trails spawned 60 times per second.
distanceBetweenObjects - Distance between each trail object.
trailLifespan - Duration of the trail objects.
colorType - How color gradient affects trail appearance.
textureVisibility - When 0, texture is completely replaced by color, 1 means trail will not be affected by color.
colorOverTime - Gradient color with alpha channel that will be evaluated over lifespan of the trail.
scaleType - How scale of the trail should be affected over time.
scaleOverTime - Scale that will be evaluated over lifespan of the trail.

