Here's how to use the **CreateEditableAsset** tool in Unity:

1. **Add the Attribute**: Attach the `CreateEditableAsset` attribute to a field in your MonoBehaviour or ScriptableObject where you want to manage a ScriptableObject.

   ```csharp
   using Core.Attributes;

   public class MyComponent : MonoBehaviour
   {
       [CreateEditableAsset]
       public MyScriptableObject myAsset;
   }
   ```

2. **Customize Options**: You can customize the behavior of the attribute by passing parameters. For example:

   ```csharp
   [CreateEditableAsset(AddLabel = true, SetWidth = 150)]
   public MyScriptableObject myAsset;
   ```

3. **Inspector Usage**: In the Unity Inspector, you'll see an object field with options to create a new ScriptableObject or clone an existing one. You can also toggle quick edit mode to modify the ScriptableObject directly.

4. **Create and Clone**: Use the "New" button to create a new ScriptableObject of the specified type or use the "Clone" button to duplicate an existing asset. A save panel will prompt you to specify a location for the new asset.

5. **Edit Directly**: Click the quick edit button to open the inspector for the selected ScriptableObject directly within the current inspector, allowing for easy modifications.

This workflow streamlines asset management, making it efficient for data-driven projects in Unity.