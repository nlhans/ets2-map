#Euro Truck Simulator 2 Map

This repository is middle-ware for software developers to integrate ETS2 maps into their widgets. It will also help with building a route advisor.

At the current state this library is ALPHA, meaning that it can read the ETS2 map but with some quirks and limitations. There is work required to make it easy to use, fast to load or adaptable to mods.

Please see the ticket system to review any known issues or work that is left open.

The demo application can be used to check if exported files from ETS2 are set up correctly, and how the map can be rendered onto a widget. 

##Assets

ETS2 maps are built basically by 2 main assets:

- Roads; these of course is the stuff you drive on.
- Prefabs; these are "prebuilt packages" that can be companies, garages, but most importantly junctions and other "road glue" for map editors to create wicked roads.

All other objects in the game are basically auxiliary to navigation maps. 

As of now the software uses some LUT (Look Up Tables) to identify what type of road or prefab corresponds to in-game values. These are shipped in this repository. It is very likely these may need to be updated per game version and/or expanded for mods that use their own prefabs.

##Setting up

In order to set-up for a demo, you need to manually extract the following:

- Raw map information. This is located in base.scs at base/map/europe/ (or within a mod). Use the [http://www.eurotrucksimulator2.com/mod_tools.php](SCS extractor) to extract def.scs and extract the map data. Put all *.base files in SCS/europe/. 

- Prefab information. These are also located in the base.scs. Extract this file as well with the SCS extractor, and locate the base/prefab/ folder. Put all *.ppd files in SCS/prefab. There are some duplicates; just ignore these because this has not been supported yet.

- 2 SII def files; these are located in def.scs. Extract this file and locate def/world/road_look.sii and def/world/prefab.sii. Put them in the SII prefab location, and add the "LUT1.19-" to them.

During initialisation you need to point to these 3 directories. In the demo this is done at Ets2MapDemo.cs. Relocate the project map to where you will be using this GIT repository.

The LUT files are shipped in this repository. These files are translation tables to convert the game ID's to prefab names.

The loading of SCS Europe map (without Scandinavia DLC) requires some time. The mapper loads all sectors at once and keeps objects in memory. The map parser also uses a fail-safe method of searching for items which is rather CPU intensive. Therefor the loading process is paralleled to all threads your machine has, and will keep them busy at 100% for a brief time. On a Intel i5 3570 machine it takes 10 seconds to load up the map in Debug mode, and about 7 seconds in Release mode.