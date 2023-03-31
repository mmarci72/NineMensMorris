using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malom.Persistence
{
    public class MalomFileDataAccess : IMalomDataAccess
    {
        public async Task<MalomTable> LoadAsync(String path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    MalomTable table = new MalomTable();
                    int player = Int32.Parse(await reader.ReadLineAsync() ?? String.Empty);
                    string[] stateStr = (await reader.ReadLineAsync() ?? String.Empty).Split(" ");
                    int[] state = stateStr.Select(s => Int32.Parse(s)).ToArray();
                    table.CurrentPlayer = player == 1 ? Values.Player1 : Values.Player2;
                    table.GameStepCount = state[0];
                    table.CurrentNumberOfPieces = state[1];
                    table.Player1NumberOfPieces = state[2];
                    table.Player2NumberOfPieces = state[3];
                    string[] numbers = new string[8];
                    for (Int32 i = 0; i < 3; i++)
                    {
                        string line = await reader.ReadLineAsync() ?? String.Empty;
                        numbers = line.Split(' ');

                        for (Int32 j = 0; j < 8; j++)
                        {
                            Values value = Int32.Parse(numbers[j]) == 0
                                ? Values.Empty
                                : Int32.Parse(numbers[j]) == 1 ? Values.Player1 : Values.Player2;
                            table.SetValue(j+i*8, value);
                        }
                    }

                    return table;
                }
            }
            catch 
            {
                throw new MalomDataException();
            }
        }

        public async Task SaveAsync(String path, MalomTable table) 
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    await writer.WriteLineAsync(table.CurrentPlayer == Values.Player1 ? "1" :"2");
                    await writer.WriteLineAsync(table.GameStepCount + " " + table.CurrentNumberOfPieces + " " + table.Player1NumberOfPieces + " " + table.Player2NumberOfPieces);

                    for (int i = 0; i < 8; i++)
                    {
                        await writer.WriteAsync(((table.GetValue(i) == Values.Player1) ? "1" :
                            table.GetValue(i) == Values.Player2 ? "2" : "0") + " ");
                    }

                    await writer.WriteLineAsync();

                    for (int i = 8; i < 16; i++)
                    {
                        await writer.WriteAsync(((table.GetValue(i) == Values.Player1) ? "1" :
                            table.GetValue(i) == Values.Player2 ? "2" : "0") + " ");
                    }

                    await writer.WriteLineAsync();

                    for (int i = 16; i < 24; i++)
                    {
                        await writer.WriteAsync(((table.GetValue(i) == Values.Player1) ? "1" :
                            table.GetValue(i) == Values.Player2 ? "2" : "0") + " ");

                    }

                }
            }
            catch
            {
                throw new MalomDataException();
            }
        }

    }
}
