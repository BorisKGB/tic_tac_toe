int GetPlayMode(string helpText, int defaultValue)
{
    int value;
    Console.Write(helpText);
    string _string = Console.ReadLine();
    if (! int.TryParse(_string, out value))
    {
        value = _string == "" ? defaultValue : 0;
    }
    if (value < 1 || value > 3)
    {
        Console.WriteLine($"Incorrect input '{_string}', game mode will be set to '{defaultValue}'");
        value = defaultValue;
    }
    if (value != 2)
    {
        Console.WriteLine("This version implement only 'human vs human' game mode, enable it for you");
        value = 2;
    }
    return value;
}

char[][] CreateGameField(int size = 3, string initState = "")
{
    char[][] Field = new char[size][];
    // Create empty field
    for (int i=0; i < size; i++)
    {
        Field[i] = new char[size];
        for (int j=0; j < size; j++)
        {
            Field[i][j] = ' ';
        }
    }
    if (initState != "")
    {
        // WARN, no initState string validation, assume that it is always correct
        for (int i=0; i< initState.Length; i++)
        {
            Field[i/size][i%size] = initState[i];
        }
    }
    return Field;
}

void PrintGameField(char[][] Field, string headerNames)
{
    string header = "   |";
    for (int i=0;i<Field.Length;i++)
    {
        if (i == Field.Length -1)
        {
            header += $" {headerNames[i]}";
        }
        else
        {
            header += $" {headerNames[i]} |";
        }
    }
    string table = "";
    for (int i = 0; i<Field.Length;i++)
    {
        table += $" {i+1} |";
        for (int j =0; j<Field[i].Length; j++)
        {
            if (j == Field[i].Length-1)
            {
                table += $" {Field[i][j]}";
            }
            else
            {
                table += $" {Field[i][j]} |";
            }
        }
        table += "\n";
    }
    Console.WriteLine(header);
    Console.WriteLine(table);
}

void AddMark(char[][] GameField, char Mark, int positionX, int positionY)
{
    GameField[positionX][positionY] = Mark;
}

bool AbleToAddMark(char[][] GameField, int[] result)
{
    if (result[0] < 0 || result[1] < 0)
    {
        return false;
    }
    else if (result[0] > GameField.Length-1 || result[1] > GameField.Length-1)
    {
        return false;
    }
    else if (GameField[result[0]][result[1]] != ' ')
    {
        return false;
    }
    return true;
}

int[] AskPlayerForCoordinates(char[][] GameField, string columnNames, string PlayerName, char PlayerMark)
{
    int [] result = new int[2] {-1, -1};
    string input;
    while (result[0] < 0 || result[1] < 0)
    {
        Console.Write($"{PlayerName} turn, input coordinates to place '{PlayerMark}'> ");
        input = Console.ReadLine().ToLower();
        if (input.Length < 2)
        {
            input = "  ";
        }
        result[1] = columnNames.IndexOf(input[0]);
        int.TryParse(input.Substring(1), out result[0]);
        // decrease resultX by one cause we counting from 0, but displaing from 1
        result[0] = result[0] - 1;
        if (!AbleToAddMark(GameField, result))
        {
            Console.WriteLine($"Coordinates '{input}' incorrect, try again. Example: 'a1'");
            result[0] = -1;
        }
    }
    return result;
}

int CheckMarksInRow(char[][] Field, int[] pointCoordinates, int direction, int rowLength)
//bool IsEqualMarksInRow(char[][] Field, int[] pointCoordinates, int direction, int rowLength, bool ignoreEmpty)
// Check if {rowLength} cells in {Field} in {direction} from(including) {pointCoordinates} are equal
// possible directions
//  0 Going through the first axis
//  1 Going through the second axis
//  2 Going obliquely up
//  3 Going obliquely down
// return
// 1 if all fields are equal (win strike)
// 0 if all fields are equal or empty (possible to do win strike)
// -1 if there are different symbols in row
{
    int[][] points = new int[rowLength][];
    char firstMark = Field[pointCoordinates[0]][pointCoordinates[1]];
    switch (direction)
    {
        case 0:
            for (int i=0;i<rowLength;i++)
            {
                points[i] = new int[] {pointCoordinates[0]+i, pointCoordinates[1]};
            }
            break;
        case 1:
            for (int i=0;i<rowLength;i++)
            {
                points[i] = new int[] {pointCoordinates[0], pointCoordinates[1]+i};
            }
            break;
        case 2:
            for (int i=0;i<rowLength;i++)
            {
                points[i] = new int[] {pointCoordinates[0]+i, pointCoordinates[1]+i};
            }
            break;
        case 3:
            for (int i=0;i<rowLength;i++)
            {
                points[i] = new int[] {pointCoordinates[0]-i, pointCoordinates[1]+i};
            }
            break;
    }
    int equalCount = 2;
    for (int point=1;point<points.Length;point++)
    {
        if (Field[points[point][0]][points[point][1]] == firstMark )
        {
            equalCount += 2;
        }
        else if (Field[points[point][0]][points[point][1]] == ' ')
        {
            equalCount += 1;
        }
        else
        {
            equalCount = 0;
            break;
        }
    }
    if (equalCount == rowLength * 2)
    {
        return 1;
    }
    if (equalCount > 0)
    {
        return 0;
    }
    else
    {
        return -1;
    }
}

// TIP, this function check only squared fields 
// and assume that win streak equal field size
int CheckGameStatus(char[][] GameField, string PlayerMarks, int Marks2Win)
// possible return values
// -2 game can continue
// -1 draw (no way to win for any player)
// 0/1 number of player who win, PlayerMarks char index in this case
{
    int result = -2;
    int possibleRows = 0;
    int emptyFields = 0;
    int tmpLineStatus;
    //for each PlayerMark
    for (int markNum=0; markNum<PlayerMarks.Length; markNum++)
    {
        for (int row=0; row<GameField.Length; row++)
        {
            // check first axis row
            if (GameField[row][0] == PlayerMarks[markNum])
            {
                // Always check horizontal lines
                tmpLineStatus = CheckMarksInRow(GameField, new int[] {row, 0}, 1, Marks2Win);
                if (tmpLineStatus == 1)
                {
                    return markNum;
                }
                else if (tmpLineStatus == 0)
                {
                    possibleRows += 1;
                }
                // If it make sense check oblique lines
                if (row == 0)
                {
                    tmpLineStatus = CheckMarksInRow(GameField, new int[] {row, 0}, 2, Marks2Win);
                    if (tmpLineStatus == 1)
                    {
                        return markNum;
                    }
                    else if (tmpLineStatus == 0)
                    {
                        possibleRows += 1;
                    }
                }
                if (row == GameField.Length-1)
                {
                    tmpLineStatus = CheckMarksInRow(GameField, new int[] {row, 0}, 3, Marks2Win);
                    if (tmpLineStatus == 1)
                    {
                        return markNum;
                    }
                    else if (tmpLineStatus == 0)
                    {
                        possibleRows += 1;
                    }
                }
            }
            else if (GameField[row][0] == ' ')
            {
                emptyFields += 1;
            }
            // check second axis row
            if (GameField[0][row] == PlayerMarks[markNum])
            {
                // Always check vertical lines
                tmpLineStatus = CheckMarksInRow(GameField, new int[] {0, row}, 0, Marks2Win);
                if (tmpLineStatus == 1)
                {
                    return markNum;
                }
                else if (tmpLineStatus == 0)
                {
                    possibleRows += 1;
                }
                // do not need to check oblique lines in this part
            }
            else if (GameField[row][0] == ' ')
            {
                emptyFields += 1;
            }
        }

    }
    // check if trere is still a point to continue game (draw)
    if (emptyFields < (GameField.Length + GameField[0].Length) && possibleRows == 0)
    {
        result = -1;
    }
    else
    {
        result = -2;
    }
    return result;
}

// This limit possible Game field size to 26, or to fieldHeaderNames.Length
string fieldHeaderNames = "abcdefghijklmnopqrstuvwxyz";

int FieldSize = 3;
int Marks2Win = FieldSize;

int PlayModeDefault = 1;
string PlayModeHelp = @$"Game mod can be
1) robot vs human (default)
2) human vs human
3) robot vs robot
---
For input coordinates put Letter, then number. 'A1' as example
Please input game mode number (default:{PlayModeDefault}): ";
int PlayMode = GetPlayMode(PlayModeHelp, PlayModeDefault);

Console.WriteLine($"Current game mode {PlayMode} | Current field size {FieldSize}");
Console.WriteLine($"You need to create a row of {Marks2Win} elements to win");

char[][] GameState = CreateGameField(FieldSize);

PrintGameField(GameState, fieldHeaderNames);

string[] players = new String[2] {"player1", "player2"};
string playerMarks = "xo";
int currentPlayer = 0;
bool gameEnd = false;
// game result:
// -2 default value, will be like that untill game ends
// -1 draw
// 0/1 number of player who win
int gameResult = -2;
int[] moveCoordinates;
while (!gameEnd)
{
    // determine who moves (human or robot) to get mark coordinates
    if (players[currentPlayer] == "player1" || players[currentPlayer] == "player2")
    {
        moveCoordinates = AskPlayerForCoordinates(GameState, fieldHeaderNames, players[currentPlayer], playerMarks[currentPlayer]);
    }
    else
    {
        // currently not used. no robot players implement for now
        moveCoordinates = new int[2] {0, 0};
    }
    AddMark(GameState, playerMarks[currentPlayer], moveCoordinates[0], moveCoordinates[1]);

    // for now game newer end
    gameResult = CheckGameStatus(GameState, playerMarks, Marks2Win);
    if (gameResult >= -1 && gameResult <= 1)
    {
        gameEnd = true;
    }
    PrintGameField(GameState, fieldHeaderNames);
    currentPlayer = (currentPlayer + 1)%2;
}

if (gameResult == -1)
{
    Console.WriteLine("Draw");
}
else
{
    Console.WriteLine($"{players[gameResult]} wins");
}
