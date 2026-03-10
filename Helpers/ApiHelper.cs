using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SudokuBruteForce.Models;

namespace SudokuBruteForce.Helpers
{
    public class ApiHelper
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string SudokuApiBaseUrl = "https://sudoku-api.vercel.app/api/dosuku";

        public static async Task<Grid?> GetRandomGrid(string difficulty = "easy")
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{SudokuApiBaseUrl}?query=newboard(boardsize:9, diff:{difficulty})");
                
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to fetch grid from API. Status: {response.StatusCode}");
                    return null;
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return ParseApiResponse(jsonResponse, difficulty);
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
                JToken? newboard = root["newboard"];
                
                if (newboard != null)
                {
                    JToken? grids = newboard["grids"];
                    if (grids != null && grids.Type == JTokenType.Array && grids.HasValues)
                    {
                        JToken? grid = grids[0];
                        if (grid != null)
                        {
                            JToken? valueArray = grid["value"];
                            if (valueArray != null && valueArray.Type == JTokenType.Array)
                            {
                                List<List<int>> gridData = new List<List<int>>();
                                
                                foreach (JToken row in valueArray)
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
                                
                                string gridJson = Newtonsoft.Json.JsonConvert.SerializeObject(gridData);
                                string name = grid["difficulty"]?.ToString() ?? "Unknown";
                                
                                return new Grid(gridJson, name, CapitalizeFirst(difficulty));
                            }
                        }
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing API response: {ex.Message}");
                return null;
            }
        }

        private static string CapitalizeFirst(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            
            return char.ToUpper(str[0]) + str.Substring(1).ToLower();
        }

        public static Grid GetDefaultGrid()
        {
            string jsonGrid = @"
            [
                [ 5, 6, 1, 0, 4, 0, 7, 0, 0 ],
                [ 0, 9, 0, 0, 2, 0, 1, 0, 8 ],
                [ 0, 0, 0, 0, 0, 0, 0, 6, 0 ],
                [ 8, 0, 0, 2, 0, 6, 0, 0, 0 ],
                [ 6, 0, 0, 1, 0, 9, 2, 0, 4 ],
                [ 9, 0, 2, 7, 0, 4, 5, 0, 6 ],
                [ 1, 2, 0, 0, 0, 0, 8, 0, 7 ],
                [ 0, 5, 6, 8, 7, 2, 0, 0, 9 ],
                [ 4, 0, 0, 5, 0, 1, 6, 2, 0 ]
            ]";

            return new Grid(jsonGrid, "Default", "Easy");
        }
    }
}

