# T2. PR1. Threads i Tasks

## Solució proposada Part 1
Per resoldre el problema del **sopar dels reis** he implementat les decisions següents:

1. **Prevenció d'interbloquejos**  
   - Faig servir un `Monitor` per a sincronitzar l'accés als recursos (pals) i evitar interbloquejos.

2. **Evitar fam (starvation)**  
   - Faig servir un `Stopwatch` per a controlar el temps que porta esperant per menjar.

3. **Sortida gràfica i colors**  
   - Cada fil té un color de text únic per identificar-lo.  
   - Cada estat (pensant, fam, menjant, recollint o deixant chopsticks) té un color de fons diferent per fer més clara la traça.
   - **Sincronització de la sortida**: S'utilitza un lock quan s'escriu a la consola per garantir que només un fil imprimeix alhora. Això evita que els colors de diferents fils es barregin o es mostrin incorrectament, millorant la llegibilitat de la sortida.

4. **Estadístiques i CSV**  
   - Són recollides el nombre de menjars (`MealCount`), el temps màxim d'espera (`MaxHungerTime`) i el temps total bloquejat (`TotalBlockedTime`).  
   - Al final, es mostren per consola i s'exporten a un fitxer `dining_stats.csv`.

### Enunciat 1: Com has fet per evitar interbloquejos i que ningú passes fam?

1. **Prevenció d'interbloquejos**  
   - Assigno una jerarquia fixa als pals (chopsticks) per ID. Abans de menjar, cada fil adquireix els dos pals en ordre creixent d'ID (menor primer, després major).  
   - Utilitzo `Monitor.TryEnter` amb timeout 0 per evitar bloquejos infinits. Si no s'adquireix el segon pal, alliberem el primer immediatament. D'aquesta manera eliminem la condició de **circular wait infinit**.

2. **Evitar fam (starvation)**  
   - Cada fil comprova el temps que porta esperant per menjar mitjançant un `Stopwatch`.  
   - Si supera els 15 s d'espera, la simulació detecta fam i s'interromp per garantir que cap fil hagi estat massa temps sense menjar.  
   - En la pràctica, el temps d'espera és curt (reintents cada 100 ms) i, gràcies a l'ordre consistent dels bloquejos, cap fil experimenta fam prolongada.


## Solució proposada Part 2

## Estructura del projecte
```
T2. PR1. Threads i Tasks/          # Arrel del projecte
├─ T2-PR1.csproj                  # Projecte C#
├─ T2. PR1. Threads i Tasks.sln   # Solució Visual Studio
├─ Program.cs                     # Punt d'entrada i coordinació
├─ README.md                      # Documentació (aquest fitxer)
├─ Models/                        # Classes de domini
│  ├─ Chopstick.cs                # Representa un pal de menjar amb lock
│  └─ Guest.cs                    # Lògica de cada comensal (fil)
├─ Services/                      # Serveis de sincronització i estadístiques
│  ├─ ChopstickManager.cs         # Gestió segura de pals (xopes)
│  └─ StatsManager.cs             # Recollida i exportació de dades a CSV
└─ HelperClasses/                 # Utilitats auxiliars
   └─ MyMath.cs                   # Generador de nombres aleatoris
```



