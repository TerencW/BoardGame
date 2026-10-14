# Instructions
- Please use C#, Java, Kotlin to solve the problem  stated below.

## Here are a few tips:
- Include unit/end-to-end tests in your solution. We recommend using TDD to solve these problems, where possible.
- Keep your functions/classes small.
- Demonstrate a good understanding of clean code, object oriented skills, and SOLID principles.
- Submit production-ready code that is clean and easy to understand with appropriate documentation.

## Rules:
- You may NOT use any external libraries that directly solve this problem, but you may use external libraries or tools for building or testing purposes. Specifically, you may use unit-testing libraries or build tools available for your chosen language (e.g., JUnit, NUnit, etc.).
- You should NOT include any executable attachments, including those with .exe or .lib extensions. System security is very important to us and certain file extensions will be blocked for security purposes, resulting in delays to your application. We need to be able to run and build your code ourselves, so please submit your code as a zipped file of source code and supporting files, without any compiled code. If you're submitting in C#, please do not submit your code as a .msi file.
- Please include a brief explanation of your design and assumptions, along with your code, as well as detailed instructions to run your application. Also include the environment required to run the application, eg. Windows, Linux.
- We assess a number of things including your application code design, your tests and general readability. While these are relatively small problems, we expect you to submit what you believe is production-quality code; code that you'd be able to run, maintain, and evolve. You don't need to gold plate your solution, however we are looking for something more than a bare-bones algorithm.

# Problem statement: Sudoku App
Write a program that allows a user to play Sudoku on the command line. The program should support puzzle generation, user interaction and validation.

- The game should begin by displaying the Sudoku grid in a readable format, showing 30 pre-filled numbers and empty cells (represented by _ )

- The user can:
	- Enter a number into a specific cell (e.g., B3 7 means place number 7 in row B, column 3).
	- Clear a cell (e.g., C5 clear).
	- Request a hint (e.g., hint) — the program should reveal one correct number.
	- Check the current grid for validity (no duplicates in rows, columns, or subgrids).
	- Quit the game at any time.

- Each move should be validated:
	- The user cannot modify pre-filled cells.
	- The number must be between 1–9.

- The game ends when the grid is completely and correctly filled.

## Game play

### Sucess example
```
Welcome to Sudoku!

Here is your puzzle:
    1 2 3 4 5 6 7 8 9
  A 5 3 _ _ 7 _ _ _ _
  B 6 _ _ 1 9 5 _ _ _
  C _ 9 8 _ _ _ _ 6 _
  D 8 _ _ _ 6 _ _ _ 3
  E 4 _ _ 8 _ 3 _ _ 1
  F 7 _ _ _ 2 _ _ _ 6
  G _ 6 _ _ _ _ 2 8 _
  H _ _ _ 4 1 9 _ _ 5
  I _ _ _ _ 8 _ _ 7 9

Enter command (e.g., A3 4, C5 clear, hint, check):
A3 4

Move accepted.

Current grid:
    1 2 3 4 5 6 7 8 9
  A 5 3 4 _ 7 _ _ _ _
  B 6 _ _ 1 9 5 _ _ _
  C _ 9 8 _ _ _ _ 6 _
  D 8 _ _ _ 6 _ _ _ 3
  E 4 _ _ 8 _ 3 _ _ 1
  F 7 _ _ _ 2 _ _ _ 6
  G _ 6 _ _ _ _ 2 8 _
  H _ _ _ 4 1 9 _ _ 5
  I _ _ _ _ 8 _ _ 7 9

Enter command (e.g., A3 4, C5 clear, hint, check):
check
No rule violations detected.

Enter command (e.g., A3 4, C5 clear, hint, check):
hint
Hint: Cell E5 = 5

.
.
.

Enter command (e.g., A3 4, C5 clear, hint, check):
I4 2

Move accepted.

Current grid:
    1 2 3 4 5 6 7 8 9
  A 5 3 4 6 7 8 9 1 2
  B 6 7 2 1 9 5 3 4 8
  C 1 9 8 3 4 2 5 6 7
  D 8 5 9 7 6 1 4 2 3
  E 4 2 6 8 5 3 7 9 1
  F 7 1 3 9 2 4 8 5 6
  G 9 6 1 5 3 7 2 8 4
  H 2 8 7 4 1 9 6 3 5
  I 3 4 5 2 8 6 1 7 9

You have successfully completed the Sudoku puzzle!
Press any key to play again...
```

### Invalid move example
```
Here is your puzzle:
    1 2 3 4 5 6 7 8 9
  A 5 3 _ _ 7 _ _ _ _
  B 6 _ _ 1 9 5 _ _ _
  C _ 9 8 _ _ _ _ 6 _
  D 8 _ _ _ 6 _ _ _ 3
  E 4 _ _ 8 _ 3 _ _ 1
  F 7 _ _ _ 2 _ _ _ 6
  G _ 6 _ _ _ _ 2 8 _
  H _ _ _ 4 1 9 _ _ 5
  I _ _ _ _ 8 _ _ 7 9

Enter command (e.g., A3 4, C5 clear, hint, check, quit):
A1 6

Invalid move. A1 is pre-filled.

Current grid:
    1 2 3 4 5 6 7 8 9
  A 5 3 _ _ 7 _ _ _ _
  B 6 _ _ 1 9 5 _ _ _
  C _ 9 8 _ _ _ _ 6 _
  D 8 _ _ _ 6 _ _ _ 3
  E 4 _ _ 8 _ 3 _ _ 1
  F 7 _ _ _ 2 _ _ _ 6
  G _ 6 _ _ _ _ 2 8 _
  H _ _ _ 4 1 9 _ _ 5
  I _ _ _ _ 8 _ _ 7 9

Enter command (e.g., A3 4, C5 clear, hint, check):
  
```


### Violation example
```
Here is your puzzle:
    1 2 3 4 5 6 7 8 9
  A 5 3 _ _ 7 _ _ _ _
  B 6 _ _ 1 9 5 _ _ _
  C _ 9 8 _ _ _ _ 6 _
  D 8 _ _ _ 6 _ _ _ 3
  E 4 _ _ 8 _ 3 _ _ 1
  F 7 _ _ _ 2 _ _ _ 6
  G _ 6 _ _ _ _ 2 8 _
  H _ _ _ 4 1 9 _ _ 5
  I _ _ _ _ 8 _ _ 7 9

Enter command (e.g., A3 4, C5 clear, hint, check, quit):
A3 3

Move accepted.

Current grid:
    1 2 3 4 5 6 7 8 9
  A 5 3 3 _ 7 _ _ _ _
  B 6 _ _ 1 9 5 _ _ _
  C _ 9 8 _ _ _ _ 6 _
  D 8 _ _ _ 6 _ _ _ 3
  E 4 _ _ 8 _ 3 _ _ 1
  F 7 _ _ _ 2 _ _ _ 6
  G _ 6 _ _ _ _ 2 8 _
  H _ _ _ 4 1 9 _ _ 5
  I _ _ _ _ 8 _ _ 7 9

Enter command (e.g., A3 4, C5 clear, hint, check):
check
Number 3 already exists in Row A.
```

### Violation example 2
```
Here is your puzzle:
    1 2 3 4 5 6 7 8 9
  A 5 3 _ _ 7 _ _ _ _
  B 6 _ _ 1 9 5 _ _ _
  C _ 9 8 _ _ _ _ 6 _
  D 8 _ _ _ 6 _ _ _ 3
  E 4 _ _ 8 _ 3 _ _ 1
  F 7 _ _ _ 2 _ _ _ 6
  G _ 6 _ _ _ _ 2 8 _
  H _ _ _ 4 1 9 _ _ 5
  I _ _ _ _ 8 _ _ 7 9

Enter command (e.g., A3 4, C5 clear, hint, check, quit):
C1 5

Move accepted.

Current grid:
    1 2 3 4 5 6 7 8 9
  A 5 3 _ _ 7 _ _ _ _
  B 6 _ _ 1 9 5 _ _ _
  C 5 9 8 _ _ _ _ 6 _
  D 8 _ _ _ 6 _ _ _ 3
  E 4 _ _ 8 _ 3 _ _ 1
  F 7 _ _ _ 2 _ _ _ 6
  G _ 6 _ _ _ _ 2 8 _
  H _ _ _ 4 1 9 _ _ 5
  I _ _ _ _ 8 _ _ 7 9

Enter command (e.g., A3 4, C5 clear, hint, check):
check
Number 5 already exists in Column 1.
```

### Violation example 3
```
Here is your puzzle:
    1 2 3 4 5 6 7 8 9
  A 5 3 _ _ 7 _ _ _ _
  B 6 _ _ 1 9 5 _ _ _
  C _ 9 8 _ _ _ _ 6 _
  D 8 _ _ _ 6 _ _ _ 3
  E 4 _ _ 8 _ 3 _ _ 1
  F 7 _ _ _ 2 _ _ _ 6
  G _ 6 _ _ _ _ 2 8 _
  H _ _ _ 4 1 9 _ _ 5
  I _ _ _ _ 8 _ _ 7 9

Enter command (e.g., A3 4, C5 clear, hint, check, quit):
B3 8

Move accepted.

Current grid:
    1 2 3 4 5 6 7 8 9
  A 5 3 _ _ 7 _ _ _ _
  B 6 _ 8 1 9 5 _ _ _
  C _ 9 8 _ _ _ _ 6 _
  D 8 _ _ _ 6 _ _ _ 3
  E 4 _ _ 8 _ 3 _ _ 1
  F 7 _ _ _ 2 _ _ _ 6
  G _ 6 _ _ _ _ 2 8 _
  H _ _ _ 4 1 9 _ _ 5
  I _ _ _ _ 8 _ _ 7 9

Enter command (e.g., A3 4, C5 clear, hint, check):
check
Number 8 already exists in the same 3×3 subgrid.
```
