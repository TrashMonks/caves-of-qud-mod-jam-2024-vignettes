# Changelog
This document lists the changes made to this pack to support Caves of Qud's 1.0.4 release.

## Updated mods
These mods had updates since the last time the pack was updated:
* A Man has Fallen into the Sacred Well in the Six Day Stilt!
* Give Me A Sign
* The Cask of Rose Wine
* Water Bandits
* Xoteotin, Mechanical Guardian

## Updated wish menu
* Added the mod jam wish menu to the in-game wish menu (Debug > Wish Menu, bound to <kbd>shift</kbd>-<kbd>W</kbd> by default).
* Renamed the jam wish menu to `vignetteswishmenu`.
* Removed the default wish menu bind (can still be bound manually by Debug > Vignettes Mash-Up Wish Menu).

## Fixed errors and warnings
Please note that a best-effort was made to maintain functionality, but a surface-level lack of errors and warnings was prioritized over correctness.
* Ambivalent Animators
  * Made a number of changes related to the Spring Molting opinions changes
  * Added extra error logging
* Astral Medusae
  * Cleaned up obsolescence warnings
  * Fixed error
* Cacophony
  * Cleaned up obsolescence warnings
* Give Me A Sign
  * Cleaned up obsolescence warnings
* Templar Outpost
  * Renamed `JoppaWorldNew8.rpm` to `QudWorldMap.rpm`
* The Cask of Rose Wine
  * Cleaned up obsolescence warnings
* The Perfect Cup
  * Cleaned up obsolescence warnings
  * Made the unidentified cup an odd trinket (there are no small trinkets anymore)
* Water Bandits
  * Correct typo in XML (`Beetle Jerky ` to `Beetle Jerky`)
* Xoteotin, Mechanical Guardian
  * Cleaned up obsolescence warnings
  
## Split out merged directories and files
Starting with version 1.0.4, Qud no longer requires a central `Textures/` folder or requires map files to be in the root directory.

