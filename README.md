# <img src="GanntChartLogo.svg" width="28"> GanttChartGUI 

<img src="https://github.com/user-attachments/assets/5398d290-37b5-4993-a22c-ba49db2a38ad" width="1000">

## Cel projektu
- wizualizacja informacji o działaniu różnego rodzaju zespołów ratowniczych i specjalnych w postaci czytelnego wykresu - wykresu Gantt'a.

## Technologie
- **Język :** C#
- **Biblioteka :** Windows Presentation Foundation (WPF)
- **Framework :** .NET 8.0

## Format danych wejściowych
Aplikacja przetwarza dane z plików tekstowych (`.txt`). Każdy wiersz reprezentuje jeden zespół i jego aktywności. Elementy są oddzielone znakiem `|`:
```
NazwaZespołu|NazwaAktywności|CzasRozpoczęcia|DługośćTrwania|[NastępnaAktywność|...]
```

Gdzie:
- `NazwaZespołu` - nazwa zespołu ratowniczego *(string)*
- `NazwaAktywności` - nazwa aktywności *(string)*
- `GodzinaPoczątku` - godzina rozpoczęcia *(string)* w formacie HH:mm (np. 08:30)
- `CzasTrwania` - czas trwania aktywności w minutach *(int)* (np: 90)

Jeden zespół może wykonywać wiele aktywności w trakcie danego dnia. Kolejne aktywności podawane są jako kolejne argumenty w linii danego zespołu.
Przykład:
```
Straż Pożarna|Gaszenie pożaru magazynu|08:30|90|Kontrola hydrantów|11:00|45|Ćwiczenia z użyciem drabin|13:15|60|Obsługa festynu rodzinnego|18:00|240
```

## Struktura projektu

### Parser
Zawiera metody do przetwarzania pliku z danymi. Sprawdza jego poprawność. W przypadku błędów zwraca odpowiedni wyjątek.

-  `ParsingException` - ogólny wyjątek parsowania po którym dziedziczy reszta
-  `InvalidActivityDataException` - niepoprawny format aktywności
-  `DuplicateTeamNameException` - zduplikowana nazwa zespołu
-  `DuplicateActivityException` - zduplikowana nazwa aktywności
-  `NoActivitiesException` - brak zdefiniowanych aktywności
-  `OverlappingActivitiesException` - nakładające się czasowo aktywności

### Interfejs
Aplikacja wykorzystuje customowe kontrolki WPF (dostępne w `Project/Views/UserControls`) do stworzenia interfejsu użytkownika:

#### 1. Pasek narzędzi
- Kontrolka `Logo` wyświetlająca graficzne logo aplikacji
- Przycisk wczytywania pliku z danymi
- Przyciski kontroli okna (minimalizuj, maksymalizuj, zamknij)

#### 2. Panel komunikatów
- Wyświetla informacje o błędach w danych

#### 3. Oś czasu
- Kontrolka `TimeAxis` reprezentująca skalę czasową (główna podziałka godzinowa, dodatkowa pomocnicza co 10 min)
- Kontrolka `TeamLabel` (tu: opisuje oś y)

#### 4. Panel zespołów: `TeamLabel` `TimeGrid`
- Kontrolka `TeamLabel` wyświetla nazwy zespołów
- Kontrolka `TimeGrid` wizualizuje bloki aktywności zespołów
- Wykorzystuje wbudowaną kontrolkę `ScrollViewer` aby obsłużyć większą liczbę zespołów
- Etykiety aktywności wyświetlane są naprzemiennie nad i pod blokami

## Uruchamianie

### Program
1. Otwórz solucję `Project.sln` w Visual Studio
2. Skompiluj projekt w trybie `Release`
3. Uruchom aplikację jednym z dwóch sposobów:
- Bezpośrednio z Visual Studio
- Przez plik wykonywalny: `Project/bin/Release/net8.0-windows/GUI.exe`
4. Użyj przycisku z belki górnej aby załadować dane z komputera

### Testy
1. Otwórz solucję `Project.sln` w Visual Studio
2. Wybierz opcję (Test > Test Explorer) z belki górnej VS
3. Kliknij "Run All Tests ..." aby uruchomić wszystkie testy

Testy sprawdzają działanie parsera oraz walidację danych

### Przykładowe dane
W katalogu `/ExampleData` znajdują się przykładowe scenariusze dla aplikacji. Obejmują:

#### Scenariusze pozytywne
-  `dane_poprawne.txt`
-  `dane_poprawne2.txt`

#### Scenariusze negatywne
-  `dane_duplikat_nazwy_zespolu.txt`
-  `dane_nakladajace_sie_aktywnosci.txt`
-  `dane_niekompletne_dane_aktywnosci.txt`
-  `dane_zespol_bez_aktywnosci.txt`
-  `dane_puste.txt`
