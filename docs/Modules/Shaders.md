# Shaders

## General

The following are simple high performance shaders created for environment art (such as trees, houses),
and Antura's customization pieces.

* **Diffuse** - Opaque, Lambert lighting, Colored, single texture
	* Sample: Letters Emoticons
* **DiffuseJustColor** - Opaque, Lambert lighting, Colored, no texture
	* Sample: quite every environment object (e.g. mountains, trees)
* **Specular** - Opaque, BlinnPhong lighting, Colored, single texture
	* Sample: Living Letters
* **SpecularDual** - As Specular, but renders both sides
	* Sample: Reward Flass Punk
* **SpecularJustColor** - Opaque, BlinnPhong lighting, Colored, no texture
	* Sample: Clouds in Tobogan
* **Transparent** - Very simple unlit transparent
* **TransparentBothSides** - As Transparent, but renders both sides
	* Sample: Reward Candy Stick, Reward Flass Punk


* **Glass**
 
## Antura
* **AnturaDog** - The 1st material used by Antura's model. It allows us to change the hue of Antura's fabric without using different textures; it also provide a simplified lighting model compared to the Standard shader, in order to support mobile platforms.
* **AnturaDecals** - The 2nd material used by Antura's model. It allows decals (e.g. the smile patch on Antura's back) and body to change color without the need of multiple textures and independently of Antura's fabric.


## Image Effects
* **MobileBlur** - used by ReadingGame to blur the text behind the lens; though this is known to be a low performance effect on mobile, it is not applied for each frame. On the contrary, the image effect is rendered once each time the text changes.

* **VignettingSimpleShader** - used in each game scene, it simulates the vignetting effect by just rendering a fullscreen quad in which pixels that are closer to the center of the screen are rendered with higher transparency than the others. It does not read from the framebuffer, so its render weight is roughly the same of an unlit semi-transparent quad without any texture. It also provide a way to create fade in/fade out effects.

## Minigames

### ReadingGame/Alphabet Song
* BlurredPaste
* MagnifyingGlass
* TreeSlicesSprite
* **GlassShines** - used by the *sunshine* effect, which is visible when the magnifying glass is positioned on the correct part of the song.

### Tobogan
* **MeterDot** - This shader is used to render the red dashed line which measures the tower's height.
* **EnlargingTube** - This shader is used to add an opening/closing animation to the pipes.
* **TransparentColor** - Used by pipes' placeholders. It renders an unlit semi-transparent mesh.

### Mixed Letters
* MobileParticleAdd_Alpha8

### TextMesh Pro

The following list includes the custom shaders accompanying the TextMesh Pro plug-in:

* TMP_Bitmap
* TMP_Bitmap-Mobile
* TMP_SDF
* TMP_SDF Overlay
* TMP_SDF-Mobile
* TMP_SDF-Mobile Masking
* TMP_SDF-Mobile Overlay
* TMP_SDF-Surface
* TMP_SDF-Surface-Mobile
* TMP_SDF_SL
* TMP_Sprite

## Misc
* **Confetti** - Used in Ending Scene by colored confetti
* **Glitter** - Used in Ending Scene by shining glitters

### Unknown
* **TransparentMasker** - 

