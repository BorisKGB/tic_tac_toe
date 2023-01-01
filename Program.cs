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
    return value;
}


int FieldSize = 3;
int Marks2Win = FieldSize;

int PlayModeDefault = 1;
string PlayModeHelp = @$"Game mod can be
1) robot vs human (default)
2) human vs human
3) robot vs robot
Please input game mode number (default:{PlayModeDefault}): ";
int PlayMode = GetPlayMode(PlayModeHelp, PlayModeDefault);

Console.WriteLine($"Current game mode {PlayMode} | Current field size {FieldSize}");
// Console.WriteLine($"Current field size {FieldSize}");
Console.WriteLine($"You need to create a row of {Marks2Win} elements to win");