# Forza Model Tool
This is my very first coding project.

Basically all it does is swap one car part with another car part and modifies the ".carbin" so there are no texture problems. This tool was made for Forza Horizon 5 (might work on other Forza PC-Ports too)

# How to use
Select your game path first (where "ForzaHorizon5.exe" is located) by clicking on the `Path` button.

Once that's done you need to select your `Target Car`, this will be the car with the swapped part(s) in the end.

Next select your car part you want to replace aka `Target Model`.

Now do the same steps above with `Donator Car` and `Donator Model`. The `Donator Model` will be the car part you want on the `Target Car`.

Press `Swap Model` and a message should pop up that says the model swap was succesful.
In the background it will also modify the `Target Car` ".carbin" so the textures are also swapped.

You can swap as many car parts as you want, just be aware that if you try to replace the same swapped `Target Model` with a different `Donator Model` it will get overwritten.

When you are done swapping all the car parts you can create a zip by clicking on `Create Zip`. This will create a zip of your currently selected `Target Car`
and place it into `YOURGAMEPATH\media\stripped\mediaoverride\rc0\cars`. If there's already a .zip with the same name you will get asked if you want to replace it or not.

Note: The reason I put it in mediaoverride is to leave the original car zips untouched and allow easy removal of your "modded" car part(s).
If you don't want your swapped car part(s) anymore or do other changes to it you can just go to the path mentioned above and you will find the zip(s) there.

The last button is `X`. With this you can remove any leftover files + folders from the temp path (mentioned when you click on it) that you don't want or need anymore.
You can use this button to restart your progress however this won't remove any already created zip(s).

# Known issues
If both `Donator Car` and `Target Car` are the same car and you selected `Donator Car` first it won't load the `Target Model` list.
(This can easily be avoided by always selecting the `Target Car` first.)
