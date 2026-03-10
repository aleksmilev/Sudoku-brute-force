using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SudokuBruteForce.Models;

namespace SudokuBruteForce.Helpers
{
    public class ApiHelper
    {
        private static readonly HttpClient httpClient = new HttpClient();
        public static async Task<Grid?> GetRandomGrid(string difficulty = "easy")
        {
            try
            {
                string difficultyParam = difficulty.ToLower();
                if (difficultyParam != "easy" && difficultyParam != "medium" && difficultyParam != "hard")
                    difficultyParam = "easy";

                string endpoint = $"https://sugoku.onrender.com/board?difficulty={difficultyParam}";

                HttpResponseMessage response = await httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to fetch grid from API. Status: {response.StatusCode}");
                    return null;
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();                
                if (string.IsNullOrWhiteSpace(jsonResponse))
                {
                    Console.WriteLine("Failed to fetch grid from API. Empty response.");
                    return null;
                }

                Grid? gridFromApi = ParseApiResponse(jsonResponse, difficulty);
                if (gridFromApi == null)
                {
                    Console.WriteLine("Failed to parse grid from API response.");
                }
                return gridFromApi;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching grid from API: {ex.Message}");
                return null;
            }
        }

        public static Grid? GetGridFromJson(string jsonGrid, string name = "Custom", string difficulty = "Easy")
        {
            try
            {
                return new Grid(jsonGrid, name, difficulty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing grid JSON: {ex.Message}");
                return null;
            }
        }

        private static Grid? ParseApiResponse(string jsonResponse, string difficulty)
        {
            try
            {
                JObject root = JObject.Parse(jsonResponse);
                JToken? board = root["board"];
                if (board != null && board.Type == JTokenType.Array)
                {
                    List<List<int>> gridData = new List<List<int>>();
                    foreach (JToken row in board)
                    {
                        if (row.Type == JTokenType.Array)
                        {
                            List<int> rowData = new List<int>();
                            foreach (JToken cell in row)
                            {
                                rowData.Add(cell.Value<int>());
                            }
                            gridData.Add(rowData);
                        }
                    }

                    if (gridData.Count > 0 && gridData[0].Count > 0)
                    {
                        string gridJson = Newtonsoft.Json.JsonConvert.SerializeObject(gridData);
                        return new Grid(gridJson, "API Generated", CapitalizeFirst(difficulty));
                    }
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string CapitalizeFirst(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            
            return char.ToUpper(str[0]) + str.Substring(1).ToLower();
        }

        public static List<Grid> GetDefaultGrid()
        {

            List<Grid> grids = new List<Grid>();

            string jsonGridEasy = @"
            [
                [ 0, 0, 0, 1, 7, 0, 4, 0, 0 ],
                [ 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
                [ 0, 0, 7, 0, 0, 9, 1, 0, 0 ],
                [ 3, 0, 2, 5, 0, 7, 6, 0, 0 ],
                [ 4, 5, 0, 0, 0, 0, 0, 1, 0 ],
                [ 7, 0, 0, 0, 1, 2, 0, 4, 0 ],
                [ 0, 0, 1, 8, 0, 0, 9, 0, 4 ],
                [ 0, 7, 3, 9, 2, 4, 0, 6, 1 ],
                [ 0, 4, 0, 0, 6, 1, 0, 3, 2 ]
            ]";

            string jsonGridMedium = @"
            [
                [ 4, 0, 0, 0, 0, 0, 0, 0, 3 ],
                [ 0, 0, 5, 0, 4, 0, 0, 0, 0 ],
                [ 0, 0, 0, 3, 0, 8, 1, 0, 0 ],
                [ 0, 2, 0, 0, 0, 0, 0, 0, 0 ],
                [ 5, 4, 0, 0, 8, 0, 2, 0, 0 ],
                [ 0, 0, 7, 2, 0, 3, 0, 5, 0 ],
                [ 3, 0, 0, 0, 7, 2, 0, 0, 8 ],
                [ 0, 6, 2, 0, 0, 0, 3, 1, 0 ],
                [ 0, 0, 4, 5, 3, 1, 0, 7, 0 ]
            ]";

            string jsonGridHard = @"
            [
                [ 0, 0, 6, 8, 0, 0, 0, 0, 0 ],
                [ 1, 0, 0, 0, 0, 0, 0, 0, 0 ],
                [ 0, 8, 0, 0, 0, 6, 0, 0, 0 ],
                [ 0, 1, 0, 0, 0, 0, 7, 0, 0 ],
                [ 0, 5, 7, 0, 0, 0, 3, 0, 0 ],
                [ 0, 0, 0, 7, 0, 0, 0, 0, 1 ],
                [ 0, 0, 0, 0, 7, 0, 9, 0, 0 ],
                [ 8, 6, 0, 9, 0, 4, 5, 0, 0 ],
                [ 9, 7, 5, 6, 1, 2, 8, 4, 0 ]
            ]";

            grids.Add(new Grid(jsonGridEasy, "Default 1", "Easy", false));
            grids.Add(new Grid(jsonGridMedium, "Default 2", "Medium", false));
            grids.Add(new Grid(jsonGridHard, "Default 3", "Hard", false));

            return grids;
        }
    }
}

