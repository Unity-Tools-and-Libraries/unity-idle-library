## 1. Introduction
This package provides a framework for the creation of idle/incremental games.

## 2. Quickstart
Here are some simple instructions to get you started.
First, you must generate the configuration your engine instance will use:

* Create an instance of IdleFramework.GameConfigurationBuilder and configure it using the following methods:
	* AddResourceDefinition to add definitions of resources you can spend.
	* AddProducerDefinition to add definitions of things that produce resources.

After you are done with the builder, call Build to get a GameConfiguration.

Now, instantiate an IdleFramework.IdleEngine with your configuration; now your engine is ready.

To advance time within the engine, call the Update method on the instance. You are responsible for calling Update at the rate you wish to advance, with the caveat that the engine will not process calls that occur less than 100ms from the last.

To unconditionally change the amount of a resource, call ChangeEntityQuantity on your IdleEngine instance. ChangeEntityQuantity adds or subtracts the given quantity from the current quantity.

To unconditionally set the amount of a resource, call SetResourceQuantity with the key of the entity and new quantity. This discards the previous quantity and unconditionally set it to the new amount.

To buy more of a resource, call BuyEntity, with the key of the entity and quantity to attempt to buy. Buying differs from Change and Set in that it checks for if the costs and requirements are met.

## 3. Future Features
Conditionally available entities
