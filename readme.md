This project is no longer under development. Unity just sucks too much.

## 1. Introduction
This package provides a framework for the creation of idle/incremental games in Unity.

Currently, it consists of a backend engine which implements the logic and rules; you are responsible for creating the UI and wiring it to the engine.

![Coverage](/CodeCoverage/Report/badge_linecoverage.png?raw=true)

## 2. Quickstart
Here are some simple instructions to get you started.

To add the library to your project, add https://github.com/Unity-Tools-and-Libraries/unity-idle-library.git?path=Assets as a git dependency in the Package Manager, as well as those listed under Additional Dependencies, below.

First, you must generate the configuration your engine instance will use:

* Create an instance of IdleFramework.GameConfigurationBuilder and configure it using the following methods:

After you are done with the builder, call Build to get a GameConfiguration.

Now, instantiate an IdleFramework.IdleEngine with your configuration; now your engine is ready.

To advance time within the engine, call the Update method on the instance. You are responsible for calling Update at the rate you wish to advance, with the caveat that the engine will not process calls that occur less than 100ms from the last; instead, the time accumulates.

To unconditionally change the amount of a resource, get the entity via `GetEntity()` on your engine system and then use `ChangeQuantity`

To unconditionally set the amount of a resource, use `SetQuantity` instead of `ChangeQuantity`.

To buy more of a resource while spending resources, call `Buy` on the entity. Buying differs from Change and Set in that it checks for if the costs and requirements are met.

## 3. Additional dependencies
This package depends on the following additional packages:
* https://github.com/Unity-Tools-and-Libraries/unity-lua-scripting-module
* https://github.com/Unity-Tools-and-Libraries/unity-simple-logging

In addition, you must add the [Addressables package](https://docs.unity3d.com/Packages/com.unity.addressables@1.21/manual/index.html), available in the Unity-provided packages.

Because the Unity Package Manager cannot resolve transitive dependencies of git repository dependencies, you must add these packages manually.

## I. Future Features
* Metrics - Track meta information over the course of play
* Tracking of property modifiers - For e.g. tooltips
* Read-only views - So state can be queries without allowing for modification.
* Actions which occur over a period of time - E.g. clicking to build something where it does not complete instantly.
* Special entities for common mechanics
	* Upgrades
* UI Component generation framework
	* Repeat List Component
	* Spinner Component
* Offline mechanics
* Support for prestige mechanics
* Auto-buy for entities.
* Web saving
* Scheduled/random events
* Event system
* Entity production chains - define chains where entities feed into each other.
* UI Components
