# EA4S_WordDrawings - how to create the Drawings font

the art department provides a single grid with all drawings (rename it in SVG/EA4S_WordDrawings_grid.svg

1. Open it with Affinity Designer (maybe we could use Inkscape but i'm not sure it exports slices). i check that every layer is named as the word_id in the Sheet.
2. Export all groups/layers as slices as SVG (for web) into the SVG/todo
3. Import all these single SVG files into Glyph (could be any OTF font maker), assigning the Unicode to the Sheet and moving the svg file into the "used" folder
4. make sure that every unicode id in Google Sheet WordData is unique
5. Export the OTF font into `/Assets/_app/Fonts/EA4S_WordDrawings.otf`
6. open TextMesh Pro Font Asset Creator using these parameters:
Font Source: EA4S_WordDrawings.otf
Font Size: Auto Sizing
Font padding: 5
Packing Method: Optimum
Atlas res: 1024x1024
Character Set: Unicode Range (Hex) with this Sequence:
21,22,23,27,30,31,32,33,34,35,36,37,38,39,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,152,153,175,177,2022,2026,2A,2C,2E,3A,3F,4A,4B,4C,4D,4E,4F,5A,5C,6A,6B,6C,6D,6E,6F,7A,B7,BF,C0,C1,C2,C4,C5,C6,C7,C8,C9,CA,CB,CC,CD,CE,CF,D0,D1,D2,D3,D4,D5,D6,D8,D9,DA,DB,DC,DD,DE,DF,E0,E1,E2,E3,E4,E5,E6,E7,E8,E9,EA,EB,EC,ED,EE,EF,F0,F1,F2,F3,F4,F5,F6,F8,F9,FA,FB,FC,FD,FE,FF

Font Render Mode: Distance Field 16

# letter_id - unicode(Hex) table

anger	41
apple	42
apricot	43
back	45
banana	46
bank	47
basketball	48
bear	49
bed	4A
belly	4B
berry	4C
bird	4D
book	4E
books	4F
box	50
boy	51
brain	52
bread	53
breeze	54
broad_bean	55
bus	44
camel	56
carpenter	57
carrot	58
cat	59
chair	5A
cheese	61
cherry	62
chest	63
chickpea	64
classroom	30
cloud	65
costume	67
cucumber	68
cupboard	C1
deer	69
doctor	6A
dog	6B
door	6C
duck	31
eagle	6D
ear	6E
elephant	6F
engineer	F2
eye	70
face	71
farmer	72
field	73
fig	74
finger	75
flag	76
flower	77
foot	CC
football	78
forest	79
fridge	7A
furniture	32
garden	C2
garlic	C4
girl	C0
glue	C5
granddaughter	33
grandfather	34
grandmother	35
grandson	36
grapes	C6
ground	37
hair	C7
hand	D0
head	CA
hospital	CB
hour	38
house	C8
icecream	CD
islands	CE
juice	39
knowledge	5C
leg	CF
light	B7
lion	D1
lips	2022
meat	D3
moon	D4
mother	D6
mountain	D2
mouse	D8
mouth	D5
nail	152
needle	DE
nest	DA
nose	DB
notebook	DC
oil	D9
onion	DD
paper	E1
parrot	E2
pear	E4
pearl	E0
pencil	E5
plum	E3
policeman	E6
potato	E7
rain	F0
rice	E9
river	EA
room	3A
salt	EB
scientist	2C
scissors	E8
sister	EE
sky	2026
snow	EF
sound	EC
sun	F1
table	F3
tea	F4
teacher	F6
the_coat	66
the_engineer	F2
the_hat	C9
the_playground	21
the_policeman	E6
the_scientist	2C
the_shirt	ED
the_shoe	23
the_trousers	F8
the_worker	27
tiger	F5
toilet	153
tomato	FE
tooth	DF
tower	FA
tree	FB
triangle	FC
vegetables	2E
voice	F9
washing_machine	175
watermelon	3F
whale	FD
window	177
winter	BF
wolf	FF
woman	22
worker	27
yogurt	2A
