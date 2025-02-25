### What is Improved First Person Character Controller?

This project is an enhanced version of the [Unity asset Starter Assets - FirstPerson](https://assetstore.unity.com/packages/essentials/starter-assets-firstperson-updates-in-new-charactercontroller-pa-196525). It improves the base functionality by refining values, improving code clarity, enhancing the input system with additional keys and useful functions, sensitivity conversion from famous games like CSGO or Valorant, sliding mechanics, and upgrading to the 3.1.2 version of Cinemachine with improved settings.

### How to Install

##### Unity Package: Manual Cinemachine update

1. Download the Cinemachine version 3.1.2 package from the Unity Asset Store 
2. Import <code>FirstPersonCharacterController.unitypackage</code> into your Unity project
3. Add <code>Runtime/Prefabs/PlayerToUnpack.prefab</code> to the scene, right-click it, and select Prefab > Unpack Completely.
4. Move <code>Main Camera</code>, <code>Cinemachine Follow Camera</code>, and <code>Player</code> to the top of the hierarchy.
5. Delete the empty gameobject <code>PlayerToUnpack</code> from the scene.
6. Replace the original camera by removing it and using the newly added one.
7. Create a <code>Player</code> layer and assign it to both <code>Player</code> and <code>PlayerCapsule</code> objects.

###
##### Package Manager: If further script changes are wanted:
1. Copy the web URL of this repository ending with .git. 
2. Clone this repository in your machine.
3. Open the Package Manager in Unity.
4. Click the '+' button and select Install Package from disk.
5. Select package.json from the copied repository.
6. Resources now should be in <code>Packages/improvedfirstpersoncontroller</code>.
7. Select the three folders inside Runtime, cut and paste them in your desired Assets folder.
8. Add <code>/Prefabs/PlayerToUnpack.prefab</code> to the scene, right-click it, and select Prefab > Unpack Completely.
9. Move <code>Main Camera</code>, <code>Cinemachine Follow Camera</code>, and <code>Player</code> to the top of the hierarchy.
10. Delete the empty gameobject <code>PlayerToUnpack</code> from the scene.
11. Replace the original camera by removing it and using the newly added one.
12. Create a <code>Player</code> layer and assign it to both <code>Player</code> and <code>PlayerCapsule</code> objects.

###
##### Package Manager: If no script changes are wanted:
1. Copy the web URL of this repository ending with .git. 
2. Open the Package Manager in Unity.
3. Click the '+' button and select Install Package from Git URL.
4. Paste the copied URL and click Install.
5. Resources now should be in <code>Packages/com.spdev.improvedfirstpersoncontroller</code>.
6. Add <code>Runtime/Prefabs/PlayerToUnpack.prefab</code> to the scene, right-click it, and select Prefab > Unpack Completely.
7. Move <code>Main Camera</code>, <code>Cinemachine Follow Camera</code>, and <code>Player</code> to the top of the hierarchy.
8. Delete the empty gameobject <code>PlayerToUnpack</code> from the scene.
9.  Replace the original camera by removing it and using the newly added one.
10. Create a <code>Player</code> layer and assign it to both <code>Player</code> and <code>PlayerCapsule</code> objects.

### What does the original Unity package include?

The original Unity package provides the following features:

- Input System: Pre-configured to handle movement, looking around, jumping, and sprinting.

- Character Controller: Three scripts that handle player movement and interaction.

- Player Input: A dedicated script for processing player input actions.

### What's Added?

This improved version includes several enhancements and additional features, such as:

- Expanded Input System:

    - Attack functionality

    - Possibility to convert sensitivity from famous games

    - Interaction key

    - Ground angle detection

    - Automatic sliding on steep slopes

    - Crouching

- On-Screen Crosshair: Added for improved aiming and gameplay experience.

- Code Optimization: The original scripts have been refined for better readability and performance.

- Cinemachine Upgrade: Updated to the latest version (3.1.2), which includes major changes in classes and script structures.

- Improved Prefab configuration.

### What's Removed?

Certain features from the original package have been removed to streamline and optimize the experience:

- Arrow Key Movement: Removed to prioritize modern control schemes.

- Analog Control for Gamepads: Removed for a simplified input handling approach.

- UI Canvas overlays for mobile (Joystick and Touch Zone).

- Playground Scene and level prototyping Prefabs.

### How do I bring the sensitivity from other game into Unity?

- Change SensitivityConvertion inside the FirstPersonController to use the desired game's sensitivity system.
- Use a simple rule of 3 to translate some of the listed below SensitivityConvertion examples to a different game by getting the same sensitivity in [Same Aim](https://www.mouse-sensitivity.com/)

- Here's some SensitivityConvertion values for some games:
- CS:GO / APEX LEGENDS: <code>0.436</code>
- VALORANT: <code>1.387</code>
- FORTNITE: <code>0.11</code>
- CALL OF DUTY MWIII / MWII / WARZONE 2.0: <code>0.131</code>

Example: I myself have a CSGO sensitivity of 2.3625 so in Unity I have that sens in CameraSensitivity and 0.436 as SensitivityConvertion.
If I wanted to use my Valorant sensitivity inside Unity, now I should put 0.743 as CameraSensitivity (obtained by using [Same Aim](https://www.mouse-sensitivity.com/)) and 1.387 as SensitivityConvertion.
If I wanted to get the SensitivityConvertion of Overwatch, I would go to [Same Aim](https://www.mouse-sensitivity.com/), get my sensitivity in that game (7.88) and apply a rule of 3, getting as 0.131 Overwatch's SensitivityConvertion

### Additional Information

For further details or inquiries, feel free to reach out.
📧 sergio.perez.fdez0@gmail.com
