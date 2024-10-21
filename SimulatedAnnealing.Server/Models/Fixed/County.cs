using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimulatedAnnealing.Server.Models.Fixed;

public partial class County
{
    public int CountyId { get; set; }
    public string Name { get; set; } = null!;
    public int? DistrictId { get; set; }
    public int Inahabitants { get; set; }
    public virtual District? District { get; set; }

    [NotMapped]
    public virtual List<County> NeighboringCounties { get; set; } = new List<County>();
    public virtual ICollection<Neighbor> Neighbors { get; set; } = new List<Neighbor>();
    public virtual ICollection<VotingResult> VotingResults { get; set; } = new List<VotingResult>();

    private bool AreCountiesNeighboring(County county, int neighborId)
    {
        return county.NeighboringCounties.Any(c => c.CountyId == neighborId);
    }

    private bool IsCountyNeighbouringWithDistrict(County county, District district)
    {
        foreach (var currCounty in district.Counties )
        {
            if (AreCountiesNeighboring(currCounty, county.CountyId))
            {
                return true;
            }
        }
        return false;
    }
}
