# how to create the Arabic font Atlas

open TextMesh Pro Font Asset Creator using these parameters:
```
Font Source: EA4S Arial.ttf
Font Size: Auto Sizing
Font padding: 5
Packing Method: Optimum
Atlas res: 1024x1024
Character Set: Unicode Range (Hex) with this Sequence:
20-7E,600-603,60B-615,61B,61E-61F,621-63A,640-65E,660-6DC,6DF-6E8,6EA-6FF,750-76D,FB50-FBB1,FBD3-FBE9,FBFC-FBFF,FC5E-FC62,FD3E-FD3F,FDF2,FDFC,FE80-FEFC```

Font Render Mode: Distance Field 16

Save as Antura Arabic SDF.asset
```

## Notes

the Unicode Range (Hex) is composed by:
* basic latin: 20-7E
* arabic: 600-603,60B-615,61B,61E-61F,621-63A,640-65E,660-6DC,6DF-6E8,6EA-6FF,750-76D,FB50-FBB1,FBD3-FBE9,FBFC-FBFF,FC5E-FC62,FD3E-FD3F,FDF2,FDFC,FE80-FEFC

## Arabic Letters

all IsoCodes
see: http://jrgraphix.net/r/Unicode/0600-06FF
http://jrgraphix.net/r/Unicode/FE70-FEFF
