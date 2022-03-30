# BehaviourTree
A implementation of behaviour trees in C# with Unity editor support.

I created this since I found satisfactory way to create behaviour trees in unity. It's still a bit rough around edges but mostly feature complete (for now at least).

Some of the things that need improving on, in no paticular order:
- Editor > Exception Handling: The editor throws exceptions in a bunch of situations (like deleting the behaviour tree you are currently working on) and I need to add proper exception handling to those cases.
- Editor > Reformat Tree: I want to add the possiblity to have the editor reposition all the nodes in a nice and concise manner.
- Core > Runtime States: It would be very helpful if for diagnostic purposes you could see what nodes are in which states and what part of tree is currently being executed.
- Core | Editor > Runtime State Editing: It could be really cool to make edits to the behaviour tree whilst it's executing. It's not really high priority right now but it could be a worthwhile addition.
- Editor > Blackboard Inspector: Adding a inspector that let's you track what blackboard values exists and of what types they are as well as which nodes use them would be very helpful. Aditionally it would be very useful if you could see the actual values during runtime.
- Model > Support for Extensions: It would be great if the model had support for arbitrary data that might be added to if someone expands on this or they just don't make much sense to add to the base implemenations.
- General: Add some way to automatically put all the depencies and stuff in one output folder. Right now this is a bit of a mess.

This is the first bigger thing I have made for Unity so I would love to receive feedback on this.
