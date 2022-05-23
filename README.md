# BehaviourTree
A implementation of behaviour trees in C# with Unity editor support.

I created this since I didn't find a satisfactory way to create behaviour trees in unity. It's still a bit rough around the edges but mostly feature complete.

## Usage
To use this just simply get the archive from the [releases](https://github.com/technologicalMayhem/BehaviourTree/releases) page and extract it. Then in Unity open the package manager, click on the plus and select 'Add package from disk'.  
Alternatively you can also clone the repository and run 'createPackage.ps1'. This creates the same folder as you would get by unzipping the release archive. It can then be added the same way to Unity.

## Roadmap
There are some ways in how I want to improve this. They are, in no particular order:
- Editor
  - **Runtime Debugging**: It would be very helpful if for diagnostic purposes you could see what nodes are in which states and what part of the tree is currently being executed as well as what values are currently set on the blackboard.
  - **Runtime Editing**: It could be really cool to make edits to the behaviour tree whilst it's executing. It's not really high priority right now but it could be a worthwhile addition.
  - **Better Selector**: The current drag selector in the editor doesn't work properly at zoom level other than fully zoomed in.
  - **Inspector Actions**: Allow nodes to send actions to the inspector that when used might make some more complicated changes that can't be made with basic editor boxes.
- General
  - **Automation**: Add some way to automatically put all the dependencies and stuff in one output folder. Right now this is a bit of a mess.
  - **Tests**: Add tests to the project so that i know ahead of time if a change i might make breaks things.
  - **Wiki**: Some explanation on how to actually use this.

This is the first bigger thing I have made for Unity so I would love to receive feedback on this.
