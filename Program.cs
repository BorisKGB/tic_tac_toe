
int FieldSize = 3;
int Marks2Win = FieldSize;

int PlayModeDefault = 1;
int PlayMode;
Console.Write(@$"Game mod can be
1) robot vs human (default)
2) human vs human
3) robot vs robot
Please input game mode number (default:{PlayModeDefault}): ");

string _PlayMode = Console.ReadLine();
if (! int.TryParse(_PlayMode, out PlayMode))
{
    PlayMode = 0;
}

if (PlayMode < 1 || PlayMode > 3)
{
    Console.WriteLine($"Incorrect input '{_PlayMode}', game mode will be set to '1'");
    PlayMode = PlayModeDefault;
 }

Console.WriteLine($"Current game mode {PlayMode}");
