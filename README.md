# Code Samples of Liam O'Donnell-Carey
This repo features some code samples from my time working on Vesper from 2024-2025 as a Senior Unity Programmer. For context, Vesper was a Unity project built by a team of around 12 to be a sandbox for user-generated content, where users would write a narrative script and we would use a combination of procedurally generated content and pre-built mechanics and systems to build a playable game world for them. My work often involved building out new game mechanics or adding new tooling to help the team at large, two examples of which I've included here. 

Unfortunately I no longer have access to code demonstrating the breadth of my experience in areas like building new gameplay obstacles and AI bot behaviour tooling in collaboration with design teams on Fall Guys, but I am happy to discuss those and more during a call if desired. 

## Modular Character Building System

This was a set of in-editor tooling built by me to facilitate the art team in creating new character prefabs from discrete interchangeable body parts they imported into the project. The tool allows the artist to cycle through all available body parts and combine and recolour them as desired, and then save out the end result as a prefab ready to be used in the game. The tool also features live animation and facial blendshape previews to help ensure everything is working as intended. You can watch a [short narrated video here](https://youtu.be/j3swiKHbKfc) demonstrating what this tool looks like in-editor.

## Interaction Markers System

This was a system built by me to facilitate the addition of interaction markers for characters, landmarks, and items into our game worlds, as well as in-editor tooling for our design team to use to tweak the settings for those interaction markers at runtime on a level-by-level basis. You can watch a [short narrated video here](https://www.youtube.com/watch?v=u4Ii9Fb8nyQ) demonstrating what this looks like in-game and explaining the tooling.

NB: The runtime creation and updating of interaction markers was done in a very wide-ranging script (UniverseModeController.cs), so for the sake of brevity and clarity I've trimmed that script down to only the parts that assist in understanding how the interaction marker system works, hence why there is not a full level instantiation flow and the like.
