# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.1.0] - 2022-05-23
This is the first release of this software.
### Added
- The core library
  - It contains the main execution code for the behaviour tree. In theory this is all you need to create behaviour tree nodes and execute them.
- The model library
  - This contains code to assist in the designing and construction of behaviour trees. It contains model objects that represent nodes in the behaviour tree that can be easily edited. Also it can use these models to create a behaviour tree from them that can be executed.
- The unity editor extension
  - This contains a editor window for unity to modify the behaviour tree in a visual way. It also allows you to store your behaviour trees as unity assets.

[Unreleased]: https://github.com/technologicalMayhem/BehaviourTree/compare/v1.0.0...HEAD
[0.1.0]: https://github.com/olivierlacan/keep-a-changelog/releases/tag/Release-v0.1.0