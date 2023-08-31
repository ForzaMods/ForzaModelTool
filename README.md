# Forza Model Tool
This is my very first coding project.

Basically all it does is swap one car part with another car part and modifies the ".carbin" so there are no texture problems.

This tool was made for Forza Horizon 5 and requires .NET 7 (might work on other Forza PC-Ports too)

Update 2.0.0: Now you can swap aftermarket wheels with car wheels. (Or optionally create car wheels as an add-on, read down for more infos)

# How to use (Model Swap)
1. Select your game path (where "ForzaHorizon5.exe" is located) by clicking on the `Path` button.

2. Once that's done you need to select your `Target Car` (this will be the car with the swapped part(s) in the end).

3. Next select your car part you want to replace aka `Target Model`.
Now do the same steps above with `Donator Car` and `Donator Model`. The `Donator Model` will be the car part that you want on the `Target Car`.

4. Press `Swap Model` and a message should pop up that says the model swap was succesful.
In the background it will also modify the `Target Car` ".carbin" so the that textures are swapped as well (this needs to be done to avoid texture issues).
You can swap as many car parts as you want, just be aware that if you try to replace the same swapped `Target Model` with a different `Donator Model` it will get overwritten.

5. When you are done swapping all the car parts you can create a zip by clicking on `Create Zip`. This will create a zip of your currently selected `Target Car`
and place it into `YOURGAMEPATH\media\stripped\mediaoverride\rc0\cars`. If there's already a .zip with the same name you will get asked if you want to replace it or not.

Note: The reason I put it in mediaoverride is to leave the original car zips untouched and allow easy removal of your "modded" car part(s).
If you don't want your swapped car part(s) anymore or do other changes to it you can just go to the path mentioned above and you will find the zip(s) there.

6. If you wish to remove any leftover files + folders in `Documents\Forza Model Tool` you can do so by clicking the last button `X`. 
You can also use this button to restart your model-swapping progress, however this won't remove any already created zip(s).

# How to use (Wheel Swap)
1. Select your game path (where "ForzaHorizon5.exe" is located) by clicking on the `Path` button.

2. Click on `Wheel Swap` button and select the car wheel that you want.

3. (Optional) Select the car wheel variant that you want. Some cars have deeper wheel depth in the front/rear, you can select it here. (if you are not sure what to pick this tool will do it for you)

4. (Optional) Tick the `Add-on Wheel` checkbox if you'd like to create a new car wheel zip instead of replacing existing aftermarket wheels.
   If you don't have access to the game database (gamedb.slt) this option will be useless to you.
   If you do have access to it you need to make some changes in `List_Wheels`, `WheelAnnotations` and optionally create a new category in `WheelCategories` if you want the car wheel to appear in-game.

5. Choose the Aftermarket wheel that you wish to replace. This option is disabled if `Add-on Wheel` is ticked.

6. When you are done choosing your wheel, click the `Swap Wheel` button and it will create a zip in `GAMEPATH\media\Stripped\MediaOverride\RC0\Cars\_library\scene\wheels`.
   Start your game and navigate to the aftermarket wheel that you wanted to replace, you should see the new car wheel that you selected.

# Known issues
There might be issues if you try to swap something like `CARNAME_skeleton.modelbin` and `ProxyLOD.modelbin`.

Some aftermarket wheels in the selection may be unavailable in the game, you can thank the Forza Developers for leaving unused/unfinished files everywhere.

Possibly more things that i either forgot or don't know about. Let me know if you find anything.

# Credits
Das - Creator of this tool

szaamerik - Sharing me his method on how to easily switch views

draff - His tool `Easy Wheel Replace` gave me the inspiration to create this tool
