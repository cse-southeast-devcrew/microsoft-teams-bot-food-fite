namespace FoodFite.Commanding
{
    using System;
    using Microsoft.Bot.Schema;

    public enum Commands
    {
        Undefined = 0,
        CreateCafeteria
    }

    public class Command
    {
        public Commands BaseCommand { get; protected set; }

        public Command(Commands baseCommand)
        {
            if (baseCommand == Commands.Undefined) throw new ArgumentNullException("The base command must be defined");

            BaseCommand = baseCommand;  
        }

        public static Command FromMessageActivity(IMessageActivity messageActivity)
        {
            return FromString(messageActivity.Text?.Trim());
        }

        public static Command FromString(string commandAsString)
        {
            if (string.IsNullOrWhiteSpace(commandAsString)) return null;

            string[] commandAsStringArray = commandAsString.Split(' ');

            Command command = null;
            int baseCommandIndex = -1;

            for (int i = 1; i < commandAsStringArray.Length; ++i)
            {
                Commands baseCommand = StringToCommand(commandAsStringArray[i].Trim());

                if (baseCommand != Commands.Undefined)
                {
                    command = new Command(baseCommand);
                    baseCommandIndex = i;
                    break;
                }
            }

            // TODO: Handle taking "Create Cafeteria" into "CreateCafetria"

            return command;
        }

        public static Commands StringToCommand(string commandAsString)
        {
            if (Enum.TryParse(commandAsString, out Commands result)) return result;

            foreach (Commands command in Enum.GetValues(typeof(Commands)))
            {
                if (command.ToString().ToLower().Equals(commandAsString.ToLower())) return command;
            }

            return Commands.Undefined;
        }
    }
}