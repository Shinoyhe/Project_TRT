__________________________________________________________________________________________

Package "F Texture Tools"
Version 1.1.2

Made by FImpossible Creations - Filip Moeglich
https://assetstore.unity.com/publishers/37262
http://www.fimpossiblecreations.pl
FImpossibleGames@Gmail.com

__________________________________________________________________________________________

Youtube: https://www.youtube.com/channel/UCDvDWSr6MAu1Qy9vX4w8jkw
Twitter (@FimpossibleC): https://twitter.com/FImpossibleC
Facebook: https://www.facebook.com/FImpossibleCreations

___________________________________________________

To use F Texture Tools, click on any texture file in your project with right mouse button, 
then go to "FImpossible Creations (probably at the bottom or top of the list) -> Texture Tools -> And here list of tools
(Supported formats .jpg, .png, .exr, .tga)

Check the user manual file for all details!

__________________________________________________________________________________________
Description:

F Texture Tools is a pack of handy tools for use within the Unity Editor.
With these tools, you can perform common operations on texture files
right inside the Unity Editor, without switching to different image editing programs.

The plugin includes Texture Editing Window Tools and Unity File Options.
In the editing windows, you can adjust the parameters of the algorithms and quickly preview the result, 
allowing you to generate the best texture variant for your use case.

The plugin features algorithms like Seamless Texture Generator (edges stamp), 
Color Equalizer (delighter+texturizer), Color Replacer (selective color editor), 
RGBA Channel Generator, and other RGBA Channel tools, 
as well as quick file scaling tools (don't get it wrong - it's not an AI upscale, it's basic lanczos scaling).

This is essentially a group of tools that I found really helpful
in speeding up my work within the Unity Editor. I hope you'll find them useful too!

Tools In The Package:

• Seamless Texture Generator (edges stamp)
- Possibility to stamp edges using other texture
• Texture Equalizer (work on too dark/overexposed areas on the texture)
- Possibility to paint choosen areas with other texture
• Color Replacer (selective color hue shift / saturation modify)
• Channeled Generator (Generate png file with selected R G B A channels)
- Useful for generating single texture files, with different channels for shaders
- Helper filters per channel, like add/multiply/roughness to smoothness etc.

• Any texture file to PNG converter
• Channel inserter (put some texture into R/G/B/A channel of other texture)
• RGBA Extractor (Extract RGBA channels of selected file into separated grayscale textures)
• Quick Scale tools (scale to any resolution / scale to power of 2 etc.)
• Extension function to convert material's textures into PNGs and remove source textures files

• Full source code included
• Base class of custom inspector window script, which can be expanded for custom texture editing algorithms (seamless texture generator, texture equalizer and color replacer is using this class as base)


______________________________________________________________

Version 1.1.2
- Tweaked initial sizes for the tools windows

Version 1.1.1
- Added possibility for pre and post blur for normal map generator
- Added 'Blur Normal Map' option for normal map tool window

Version 1.1
- Added Normal Map Tool Window (picture to normal map / DirextX <-> OpenGL conversion)
- Added Blending Tool Window (blending textures/blending normal maps)

Version 1.0.0.1
- Added masking mode to the Color Replacer Window
- Added 'Extra Factor' parameter to the Color Replacer Window
- Added texturize mode to the Color Replacer Window
- Added tiling feature to the texturize modes
- Added selective channel export toggles for Channels Extractor window
- Added "Export Grayscale" button for masking modes
- Now when exporting using "Result As New File" button, will not override previously generated variant image file with same button

Version 1.0.0
-Initial version
