using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SimulatedAnnealing.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedAnnealing.Services.Geography
{
    public class Radar
    {
        private SimulatedAnnealingContext _context;

        public Radar(SimulatedAnnealingContext context)
        {
            this._context = context;
        }

        public bool AreCountiesNeighbouring(int countyXId, int countyYId)
        {
            using (var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (var command = new SqlCommand("CheckIfCountiesAreNeighbors", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@PowiatId1", countyXId));
                    command.Parameters.Add(new SqlParameter("@PowiatId2", countyYId));

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetInt32(0) == 1;
                        }
                    }
                }
            }
            return false;
        }
        public bool AreDistrictsNeighbouring(int districtXId, int districtYId)
        {
            var sql = "EXEC CheckIfDistrictsAreNeighbors @DistrictId1, @DistrictId2";
            var result = _context.Database
                .ExecuteSqlRaw(sql,
                    new SqlParameter("@DistrictId1", districtXId),
                    new SqlParameter("@DistrictId2", districtYId)
                );   
            return result == 1;
        }
        public bool IsDistrictBoundaryUnbroken(int districtId)
        {
            // Fetch all counties in the district
            var counties = _context.Powiaties
                .Where(p => p.OkregId == districtId)
                .ToList();

            if (counties.Count == 0)
                return false; // No counties, hence boundary cannot be unbroken

            // Initialize a list to track adjacency
            var adjacencyMatrix = new bool[counties.Count, counties.Count];

            // Populate the adjacency matrix
            for (int i = 0; i < counties.Count; i++)
            {
                for (int j = 0; j < counties.Count; j++)
                {
                    if (i != j)
                    {
                        adjacencyMatrix[i, j] = AreCountiesNeighbouring(counties[i].PowiatId, counties[j].PowiatId);
                    }
                }
            }

            // Use a graph traversal method to check if all nodes (counties) are connected
            return IsGraphConnected(adjacencyMatrix);
        }

        // Helper method to check if the graph is connected
        private bool IsGraphConnected(bool[,] adjacencyMatrix)
        {
            int numberOfNodes = adjacencyMatrix.GetLength(0);
            bool[] visited = new bool[numberOfNodes];

            // Perform DFS or BFS to mark all reachable nodes
            void DFS(int node)
            {
                visited[node] = true;
                for (int i = 0; i < numberOfNodes; i++)
                {
                    if (adjacencyMatrix[node, i] && !visited[i])
                    {
                        DFS(i);
                    }
                }
            }

            // Start DFS from the first node
            DFS(0);

            // Check if all nodes were visited
            return visited.All(v => v);
        }

    }
}
