# BehaviourTree
A implementation of behaviour trees in C# with Unity editor support.

I created this since I didn't find a satisfactory way to create behaviour trees in unity. It's still a bit rough around the edges but mostly feature complete (for now at least).

Some of the things that need improving on, in no paticular order:
- Editor > Exception Handling: 
  - The editor throws exceptions in a bunch of situations (like deleting the behaviour tree you are currently working on) and I need to add proper exception handling to those cases.
- Core > Runtime States: 
  - It would be very helpful if for diagnostic purposes you could see what nodes are in which states and what part of the tree is currently being executed.
- Core | Editor > Runtime Editing: 
  - It could be really cool to make edits to the behaviour tree whilst it's executing. It's not really high priority right now but it could be a worthwhile addition.
- Editor > Blackboard Runtime Inspector: 
  - It would be very useful if you could see the actual values during runtime.
- General > Automation: 
  - Add some way to automatically put all the dependencies and stuff in one output folder. Right now this is a bit of a mess.
- General > Tests: 
  - Add tests to the project so that i know ahead of time if a change i might make breaks things.
- General > Wiki: 
  - Some explanation on how to actually use this.

This is the first bigger thing I have made for Unity so I would love to receive feedback on this.
