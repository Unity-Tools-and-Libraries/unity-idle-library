## 1. Introduction
This package provides a framework for the creation of idle/incremental games in Unity.

Currently, it consists of a backend engine which implements the logic and rules; you are responsible for creating the UI and wiring it to the engine.

## 2. Quickstart
Here are some simple instructions to get you started.

To add to your Unity project, download the zip of the project, unzip it and add the contents to your project in a single Folder.

First, you must generate the configuration your engine instance will use:

* Create an instance of IdleFramework.GameConfigurationBuilder and configure it using the following methods:
	* AddResourceDefinition to add definitions of resources you can spend.
	* AddProducerDefinition to add definitions of things that produce resources.

After you are done with the builder, call Build to get a GameConfiguration.

Now, instantiate an IdleFramework.IdleEngine with your configuration; now your engine is ready.

To advance time within the engine, call the Update method on the instance. You are responsible for calling Update at the rate you wish to advance, with the caveat that the engine will not process calls that occur less than 100ms from the last.

To unconditionally change the amount of a resource, call ChangeEntityQuantity on your IdleEngine instance. ChangeEntityQuantity adds or subtracts the given quantity from the current quantity.

To unconditionally set the amount of a resource, call SetResourceQuantity with the key of the entity and new quantity. This discards the previous quantity and unconditionally set it to the new amount.

To buy more of a resource while spending resources, call BuyEntity, with the key of the entity and quantity to attempt to buy. Buying differs from Change and Set in that it checks for if the costs and requirements are met.

## 3. Entities
Entities are the primary "things" that the player will use to achieve their ends in the game.

There are two kinds of entites: normal entities and singleton entities.

Normal entities are things which the player will accrue in large numbers. For example, in a "Civilization" type game the resources the player gains, such as food, gold, research, etc. would all be "normal entities".

Singleton entities are things which the player will have which are distinct from each other. In an RPG style game, party members would likely be defined as singleton entities. Singleton entities are defined as templates, with each instance being a distinct object.

## 4. Hooks
Hooks provide a standardized interface for a developer to customize fundamental elements of how the engine works and watch for when things occur within the engine.

The following events within the engine exist and can have hooks associated with them:
* Entity Production

## I. Future Features
* Engine hooks - This will allow custom user-defined code that can be run when things occur inside the engine that is more sophisticated than what can be done declaratively.
	- Entity purchase
	- Entity production input
	- Entity production output
* Tutorial system
* Metrics - Track meta information over the course of play
* Minimum and maximum quantities - Set caps and floors on entity quantities
* Tracking of property modifiers - For e.g. tooltips
* Custom properties - Add support for custom properties on the engine and entities.
* Read-only views of entity, engine state - So state can be queries without allowing for modification.
* Actions which occur over a period of time - E.g. clicking to build something where it does not complete instantly.
* Singleton entities - For things like upgrades/research, characters, achievements, etc.
* UI Component generation framework
* Offline mechanics
* Support for prestige mechanics
* Auto-buy for entities.
* Custom logging - Default Unity logging is barebones.
* Web saving
* Scheduled/random events
* Event system
* Entity production chains - define chains where entities feed into each other.
* Random events
