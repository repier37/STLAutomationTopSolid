# STLAutomationTopSolid
TopSolid automation to export a part file to a slic3r.

Currently, only tested with prusaSlic3r.

The automation will export the first shape in the detailed representation to STL, then will start the programm set in the config file, and load the stl file to it.

If you wish this application support another slic3r, please feel free to create an issue.


# Installation
Place the exe and the config file to the bin folder of your Topsolid install.

Open the config file and change the key "Chemin" to the path of your slic3r exe
    <appSettings>
      <add key="Chemin" value ="Absolute\Path\To\prusa-slicer.exe"/>
    </appSettings>

In TopSolid open Tools=>Customize, then use the "Add a file to execute section" and point to the STLAutomation.exe
![image](https://github.com/repier37/STLAutomationTopSolid/assets/28806724/1537d346-d64e-45f4-94b3-d6a5aeacd62b)

Then drag drop the icon to an area of TopSolid
![image](https://github.com/repier37/STLAutomationTopSolid/assets/28806724/fadea0a1-a56f-49d3-af1b-807220f0eca7)
