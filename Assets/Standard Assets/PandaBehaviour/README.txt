
Panda Behaviour - copyright (c) 2015 Eric Begue


==== OVERVIEW ====
Panda Behaviour is a script-based Behaviour Tree engine for Unity, available as a GameObject component.

The behaviour of a GameObject is defined by writing BT scripts, using a minimalist built-in language to describe the Behaviour Tree structure and its execution flow. A Behaviour Tree consists of a hierarchy of nodes where the deepest node are tasks, which are implemented in C# and invoked from the BT scripts.

The Behaviour Tree is compactly display as a color coded text directly within the Inspector. A Life View allows visualization and debugging of the Behaviour Tree at run-time, providing relevant information at a glance.


==== GETTING STARTED ====

A good starting point is to open the example scenes, play them and observe the running Behaviour Trees in the Inspector by selecting a GameObject of interest. In the inspector, you can double click on a task in the Code Viewer to open the corresponding C# implementation.

The examples are located in the same folder containing this README file. The examples are ordered from simple to more elaborated.

The Panda Behaviour component can be added to any game object from the Inspector by clicking on "Add Component" then by selecting Scripts > Panda > Panda Behaviour.

The Panda Behvaviour component requires BT script to be functional. You can create new BT script by right clicking in a asset folder in the Project tab then by selecting Create > Panda BT Script.

A BT script can invoke tasks defined in any MonoBehaviour attached to the Game Object. A task can be a boolean field, a boolean property or a method returning void having [Task] attribute. 


==== LINK ====

For more documentations, examples and tutorials please visit:

http://www.pandabehaviour.com

