# Fancy Weather API

This is the repository for the Fancy_Weather_API, a Lethal Company API to extend to "fancy weathers" monitor display of the General Improvements mod.

This is done by scanning new weathers animations inside txt files and patching General Improvements to inject these animation inside the code.

You can post some suggestions or issues in the 'Issues' tab.

This API was made in collaboration with `@BADQUEST_`.
<!--
## More info
You can check more info on Thunderstore:
https://thunderstore.io/c/lethal-company/p/BADQUEST/todo/
-->

## Dependencies

https://thunderstore.io/c/lethal-company/p/ShaosilGaming/GeneralImprovements/

https://thunderstore.io/c/lethal-company/p/mrov/WeatherRegistry/

## How to use the API

### Folder structure

In your mod folder, create a "plugins" folder, and inside it place the .dll and create a "ASCII_Anim" folder : This is where all .txt files should be located.

```
*your main folder*
    - plugins
        - ASCII_Anim
            - *all animation txt files here*
        - FancyWeatherAPI.dll
    - CHANGELOG.md
    - icon.png
    - manifest.json
    - README.md
```

### Animation files

Animations files can be named whatever you want, but it's important that they are .txt files. You can create as many files as you want, each containing one or many different animations.

The format of the file is the following :

```
Parameters
Name: Snowfall
LightningOverlay: false

Animation

/~~~~~~\
\~~~~~~/
. + * .
 * + * .

/~~~~~~\
\~~~~~~/
 + . + *
. + * .

/~~~~~~\
\~~~~~~/
. + * .
 + . + *

/~~~~~~\
\~~~~~~/
 * + * +
. + * .

/~~~~~~\
\~~~~~~/
* . + .
 * + * +

/~~~~~~\
\~~~~~~/
 * + * .
* . + .
```

As you can see in the example above, there is 2 main areas in the file.

### **Parameters**

This is the place where specific parameters are specified, such as the Name of the weather.

- **Name:** Use this parameter to define the name of the targeted weather to display this animation. This needs to match the weather name in-game perfectly in order to work. **(this parameter is required)**

- **LightningOverlay:** Set this parameter to true to activate the special Stormy thunder overlay on top of your animation. **(this parameter is optional)**

Each time a new Parameters area is detected in the file, there will be a new animation scanning. This means you can actually enter 2 or more animations by writting in succession a Parameters area, then an Animation area, then other Parameters and Animation areas.

### **Animation**

This is the place where you can define the ASCII animation frame by frame. Each frame is separated by a blank line.

- Each frame cannot have more than 4 lines and each line max character limit will depend on the characters you are using
- When you create any custom ASCII animations, use a program like Open Office or Microsoft Word (or https://write-box.appspot.com/) then copy paste the resulting animation inside the formated .txt file for this mod
- This is because the monitor font is proportional, meaning each character does not take the same pixel space when it is displayed, so some characters will take more spaces than others. *For example, the `\` take twice as more space than the `.`*
- Thus, using a classic text editor like Notepad is not recommanded as it will not display the characters at their correct pixel size

### Compatibility

This mod uses WeatherRegistery to detect the current weather.

This means, the API is compatible with combined and progressing weather types as well as with copies of original weathers.