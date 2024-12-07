Hra Dáma s AI - Uživatelský/Programátorský průvodce
1. Stručný popis
Tento projekt je grafická implementace klasické hry Dáma, vylepšená pomocí algoritmů umělé inteligence. Hra umožňuje hráču soutěžit proti počítači. AI může použít buď algoritmus Minimax, ořezávání Alpha-Beta, nebo kombinaci obou pro určení nejlepšího tahu. Hra se řídí standardními pravidly Dámy včetně nucených braní a povyšování na dámu.

2. Uživatelský průvodce
Tato sekce poskytuje pokyny, jak používat aplikaci hry Dáma.
Spuštění hry
Spuštění aplikace: Spusťte aplikaci spuštěním spustitelného souboru.
Hlavní okno: Hlavní okno zobrazuje hrací desku a ovládací prvky hry.
Hraní hry
Člověk vs. AI: Vyberte algoritmus AI z ComboBoxu (Minimax, Alpha-Beta nebo Kombinovaný). Hráč může provést tahy a AI odpoví na základě vybraného algoritmu.
Nucené braní: Pokud hráč může brát soupeřovu figurku, musí tak učinit. Hra toto pravidlo automaticky vynucuje.
Povyšování na Dámu: Když figurka dosáhne zadní řady soupeře, je povýšena na Dámu. Dámy se mohou pohybovat diagonálně vpřed i vzad.
Ovládací prvky
Výběr algoritmu: Použijte ComboBox pro výběr algoritmu AI. Možnosti jsou Minimax, Alpha-Beta a Kombinovaný.
Tlačítko reset: Kliknutím na tlačítko "Reset" restartujete hru.
Popisek aktuálního algoritmu: Zobrazuje aktuálně vybraný algoritmus AI.
Popisek vítěze: Zobrazuje vítěze hry po jejím skončení.
Konec Hry
Hra končí, když jednomu hráči nezbývají žádné figurky nebo žádné platné tahy. Vítěz je zobrazen v popisku vítěze.




3. Struktura Projektu
Projekt je strukturován jako aplikace Windows Forms napsaná v jazyce C#. Níže je uveden přehled struktury projektu, včetně hlavních tříd a jejich funkcí:
Soubory projektu:
MainForm.cs: Zpracovává grafické uživatelské rozhraní (GUI) a interakce uživatele.
Board.cs: Spravuje herní logiku, stav hrací desky a algoritmy AI.
Piece.cs: Reprezentuje jednotlivé figurky na hrací desce.
Move.cs: Struktura pro tahy.
AI.cs: Realizace umělé inteligence.
MainForm.cs
InitializeCustomComponents(): Inicializuje vlastní GUI komponenty, jako jsou ComboBox a Labely.
AlgorithmComboBox_SelectedIndexChanged(): Aktualizuje popisek aktuálního algoritmu při změně výběru v ComboBoxu.
ExecuteAIMove(): Provede tah AI na základě vybraného algoritmu.
ResetButton_Click(): Resetuje hrací desku.
CheckGameEnd(): Kontroluje, zda hra skončila, a aktualizuje popisek vítěze.
BoardPanel_MouseClick(object sender, MouseEventArgs e): Vyber piece a delani tahu.
CalculateValidMoves(int x, int y): Kalkuluje tahy pro vizualizace.
DrawBoard(Graphics g): Maluje board.
Board.cs
ApplyMove(Move move): Aplikuje daný tah na hrací desku.
GetAllPossibleMoves(PieceColor color): Vrací seznam všech možných tahů pro danou barvu.
GetValidMoves(int x, int y): Vrací seznam platných tahů pro figurku na zadané pozici.
HasCapture(PieceColor color, int x, int y): Kontroluje, zda figurka může brát jinou figurku.
IsValidMove(Move move): Kontroluje, zda je tah platný podle pravidel.
IsValidCaptureMove(Move move, PieceColor color, int directionsX, int directionsY): Kontroluje, zda je tah braním platný.
InitializeBoard(): Inicializuje pole.
HasAvailableCaptures(PieceColor color): Kontroluje, zda nektera figurka ma brat jinou figurku.
GetBestMove(PieceColor color, Board board, string algorithm): Pouziva metody z AI.cs pro vyber tahu.
IsGameOver(Board board): Vraci kdy hra se skoncila.
GetWinner(): Vraci kdo vyhral.
CountPieces(PieceColor color): Pocita figurky u kazdeho hrace.
Reset(): Resetuje pole.

AI.cs
GetBestMoveMinimax(PieceColor color, int depth): 
Vstup: Současný stav hrací desky a barva hráče, který je na tahu.
Výstup: Nejlepší tah, který může hráč provést.
Postup:
Nastaví hloubku hledání na 5 (což určuje, kolik tahů dopředu se algoritmus podívá).
Inicializuje nejlepší hodnotu (bestValue) na nejmenší možnou hodnotu a nejlepší tah (bestMove) na null.
Získá všechny možné tahy pro daného hráče.
Pro každý možný tah:
Vytvoří novou kopii desky a aplikuje na ni tah.
Vyhodnotí tah pomocí funkce Minimax s hloubkou o 1 menší.
Pokud je hodnota tohoto tahu lepší než nejlepší dosavadní hodnota, aktualizuje nejlepší hodnotu a nejlepší tah.
Minimax(Board board, int depth, bool maximizingPlayer, PieceColor color): 
Vstup: Současný stav hrací desky, zbývající hloubka, boolean určující, zda se jedná o tah maximalizujícího hráče, a barva původního hráče.
Výstup: Hodnota, která odráží výhodnost pozice. (Rozdíl v počtu figurek hráču a soupeřu.)
Postup:
Pokud je hloubka 0 nebo je hra u konce, vyhodnotí desku pomocí funkce EvaluateBoard.
Získá všechny možné tahy pro aktuálního hráče.
Pokud je aktuální hráč maximalizující:
Inicializuje maximální hodnotu na nejmenší možnou hodnotu.
Pro každý možný tah:
Vytvoří novou kopii desky a aplikuje na ni tah.
Rekurzivně zavolá Minimax pro minimalizujícího hráče s hloubkou o 1 menší.
Aktualizuje maximální hodnotu.
Vrátí maximální hodnotu.
Pokud je aktuální hráč minimalizující:
Inicializuje minimální hodnotu na největší možnou hodnotu.
Pro každý možný tah:
Vytvoří novou kopii desky a aplikuje na ni tah.
Rekurzivně zavolá Minimax pro maximalizujícího hráče s hloubkou o 1 menší.
Aktualizuje minimální hodnotu.
Vrátí minimální hodnotu.

GetBestMoveAlphaBeta(PieceColor color, int depth): Určuje nejlepší tah pomocí ořezávání Alpha-Beta.
AlphaBeta(Board board, int depth, int alpha, int beta, bool maximizingPlayer, PieceColor color): Rekurzivní funkce Alpha-Beta.
GetBestMoveCombined(PieceColor color, int depth): Kombinuje Minimax a Alpha-Beta na základě hloubky.
EvaluateBoard(PieceColor color): Vyhodnocuje stav hrací desky a vrací skóre.
OpponentColor(PieceColor color): Vraci barvu protihrace.
GetAllPossibleMoves(Board board, PieceColor color): Vraci vsechny tahy.
GetPieceValidMoves(Board board, int x, int y): Vraci mozne tahy pro konkretni figurku.
GetValidMoves(Board board, int startX, int startY, PieceColor color): Vraci mozne tahy..
IsGameOver(Board board): Hleda kdyby hra se skončila.
Piece.cs
Piece: Reprezentuje figurku na hrací desce s vlastnostmi barvy a typu (Muž nebo Dáma).
Move.cs
Move: Reprezentuje tah z start point a end point.
Algoritmy AI
Minimax: Tento algoritmus rekurzivně prochází možné tahy až do určité hloubky (5 tahů dopředu) a vyhodnocuje každý stav hrací desky. Používá přitom strategii minimalizace ztrát (protihráč hraje optimálně) a maximalizace vlastního zisku. Tímto způsobem se snaží najít nejlepší možný tah v dané situaci.
Alpha-Beta Pruning: Tento algoritmus zlepšuje Minimax algoritmus tím, že eliminuje větve, které není nutné procházet. K tomu používá dvě hranice: alfa (nejlepší hodnota, kterou maximalizující hráč může zaručeně dosáhnout) a beta (nejlepší hodnota, kterou minimalizující hráč může zaručeně dosáhnout). Pokud se během prohledávání zjistí, že aktuální větev nemůže zlepšit nejlepší dosavadní výsledek, prohledávání této větve se přeruší (prořezání).
Kombinovaný: Používá Minimax pro malé hloubky a Alpha-Beta pro hlubší vyhledávání, čímž kombinuje silné stránky obou algoritmů.

