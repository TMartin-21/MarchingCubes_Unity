# Felület generálása marching cubes algoritmussal és vízfelszín szimuláció Unity játékmotorral

A projekt a szakdolgozatomban kitűzött feladat megvalósítása:

- A terep formáját compute shaderrel implementált simplex zaj határozza meg.

- Felület generálása marching cubes algoritmussal, chunkokra bontva. A felület egy terephez hasonló zöld-barna színátmenetet kapott egy egyszerű shader segítségével. A projektbe bekerült egy egyszerű sűrűségfüggvény is, mely a legenerált felület formáját határozza meg:

<p align="center">
    <img src="/Screenshot 2023-11-29 191657-1.png" width="400">
</p>

<p align="center">
    <img src="/Screenshot 2023-09-26 001928.png" width="400">
</p>

<p align="center">
    <img src="/Screenshot 2023-09-25 021544.png" width="400">
</p>

<p align="center">
    <img src="/image.png" width="400">
</p>
 
- Annak érdekében, hogy a legenerált felületet egy kicsit érdekesebbé tegyük, ne legyen annyira egyhangú, adhatunk hozzá vízfelületet. Ennek egy gyors és egyszerű megoldása egy sima sík felvétele, amin mozgatunk egy normálmap textúrát, esetleg többet kombinálva. Viszont ha minél kisebb szögben nézünk a síkra, fel fog tűnni, hogy csupán textúrákat mozgatunk egy teljesen lapos felületen. Ez egy kicsit illúzió romboló lenne. Ahhoz hogy elkerüljük, folyamatosan mozgathatjuk időben a felületet egy szinusz hullám szerint. Azonban van egy ennél is jobb megoldás: egy teljes  vízfelület szimulációja, normálmap nélkül, csupán szinusz hullámok alkalmazásával. Megfelelő árnyalással együtt egy fodrozódó vízfelület benyomását érhetjük el a módszer segítségével:

<p align="center">
    <img src="/Screenshot 2023-12-04 234226.png" width="400">
</p>

- Terep és a vízfelület együtt:

<p align="center">
    <img src="/Screenshot 2023-11-04 220648.png" width="400">
</p>

<p align="center">
    <img src="/Screenshot 2023-12-06 004931.png" width="400">
</p>

Látható, hogy a megoldás nem tökéletes, hisz eltávolodva a vízfelülettől ismmétlődő minta figyelhető meg (tiling). Ez elkerülhetetlen, ha periódikus fügvényekkel dolgozunk, azonban van módszer látszólagos elrejtésére (pl Fourier-transzformációk segítségével). Viszont ezek a módszerek akár egy teljes szakdolgozat témáját kitehetnék, így a projektben nem kerültek megvalósításra.