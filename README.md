# Graphs
This packet of code allows you to work with basic graph. Included a special editor extension that allows you to edit  graph directly from the scene tab. Important: this graph is space-oriented, based on 2D dimension, so you must use it for such systems as roads, paths and etc in game space. Also it has built-in pathfinding, that works really fast because was optimized. algorythm 


#Usage
To start work with this graph, just drag&drop this packet of code to your Unity project. Use directive:

using Graphs;

to access the namespace and start working with graphs.

If you want to edit graph directly from the scene and save this structure, create an empty GameObject and attach a 'PathData' component to it.
And now just select this GameObject, and you will see a special dashboard shown in the scene tab. WARNING: Only 1 component must be attached 
to 1 GameObject at once!!
