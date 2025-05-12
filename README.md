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
Per a la segona part, he desenvolupat un joc d'asteroides en consola on el jugador controla una nau que ha d'esquivar asteroides que cauen. La solució es basa en l'ús de múltiples tasques (Tasks) per gestionar de manera concurrent la lògica del joc, la renderització i la lectura del teclat, aconseguint així una experiència fluida i reactiva.

1. **Execució concurrent de tasques**
   - S'utilitzen diverses `Task` per executar en paral·lel:
     - **Renderització**: Una tasca que s'encarrega de pintar l'estat del joc a la consola a intervals regulars (20Hz).
     - **Lògica del joc**: Una altra tasca que actualitza la posició dels asteroides, comprova col·lisions i gestiona la puntuació (50Hz).
     - **Lectura de teclat**: Una tasca separada que escolta les tecles premudes per moure la nau o sortir del joc.
     - **Generació d'asteroides**: Una tasca addicional que afegeix nous asteroides periòdicament.
   - Totes aquestes tasques comparteixen estat mitjançant variables protegides per locks quan cal (per exemple, la llista d'asteroides).

2. **Sincronització i cancel·lació**
   - S'utilitza un `CancellationTokenSource` per poder aturar totes les tasques de manera coordinada quan el jugador perd o prem 'Q'.
   - La sincronització de la sortida a consola es fa amb locks per evitar que diferents tasques es trepitgin la sortida.

3. **Programació paral·lela i asíncrona**
   - **Paral·lelisme**: En aquest projecte, la concurrència s'aconsegueix mitjançant l'execució de diverses tasques en paral·lel, aprofitant els recursos del sistema per executar la lògica, la renderització i la lectura de teclat simultàniament. Això permet que cap funcionalitat bloquegi les altres.
   - **Asincronia**: Les tasques utilitzen `async/await` i `Task.Delay` per esperar intervals de temps sense bloquejar el fil principal, la qual cosa permet que el sistema operatiu gestioni eficientment els recursos i que el joc sigui més suau, fins i tot amb una sola CPU.
   - En resum, la solució combina programació paral·lela (múltiples tasques que poden executar-se realment alhora en màquines multicore) i asincronia (no bloquejar el fil mentre s'espera), aconseguint una execució eficient i responsiva.

### Enunciat 2: Com has executat les tasques per tal de pintar, calcular i escoltar el teclat al mateix temps? Has diferenciat entre programació paral·lela i asíncrona?

Per aconseguir que el joc sigui interactiu i fluid, he creat una tasca (`Task`) per a cadascuna de les funcionalitats principals: pintar la pantalla, calcular la lògica del joc i escoltar el teclat. Aquestes tasques s'executen de manera concurrent, compartint estat mitjançant variables protegides per locks quan cal. La lectura de teclat es fa en una tasca dedicada que no bloqueja la resta del joc. La renderització i la lògica del joc s'executen a intervals regulars mitjançant `Task.Delay`, que permet esperar sense bloquejar el fil. Això permet que totes les funcionalitats s'executin alhora, aprofitant tant el paral·lelisme (si el sistema ho permet) com l'asincronia per no bloquejar recursos. Així, el joc respon ràpidament a l'usuari i manté una execució eficient.

## Estructura del projecte (Part 2 - Asteroids)
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
T2-PR1 Part 2/                  # Arrel del projecte d'asteroids
├─ T2-PR1 Part 2.csproj         # Projecte C#
├─ Program.cs                   # Punt d'entrada i coordinació de tasques
├─ AsteroidGame.cs              # Lògica principal del joc (render, lògica, input, asteroides)
```

## Bibliografia
- Microsoft. (17 d’abril de 2025). Clase **System.Threading.Thread**. Microsoft Learn. Recuperat el 05 de maig de 2025 de [link](https://learn.microsoft.com/es-es/dotnet/fundamentals/runtime-libraries/system-threading-thread)
- Microsoft. (9 de gener de 2024). Clase **System.Threading.Tasks.Task**. Microsoft Learn. Recuperat el 08 de maig de 2025 de [link](https://learn.microsoft.com/es-es/dotnet/fundamentals/runtime-libraries/system-threading-tasks-task)
- Microsoft. (11 de gener de 2024). Clase **System.Threading.Monitor**. Microsoft Learn. Recuperat el 10 de maig de 2025 de [link](https://learn.microsoft.com/es-es/dotnet/fundamentals/runtime-libraries/system-threading-monitor)
- Microsoft. (22 de març de 2023). Paraula clau **async** (C# Reference). Microsoft Learn. Recuperat el 11 de maig de 2025 de [link](https://learn.microsoft.com/es-es/dotnet/csharp/language-reference/keywords/async)
- Microsoft. (1 de juliol de 2024). Operador **await**. Microsoft Learn. Recuperat el 11 de maig de 2025 de [link](https://learn.microsoft.com/es-es/dotnet/csharp/language-reference/operators/await)



