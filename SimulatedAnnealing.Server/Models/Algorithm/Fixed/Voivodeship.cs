using SimulatedAnnealing.Server.Models.Algorithm.Fixed.Parliament;

namespace SimulatedAnnealing.Server.Models.Algorithm.Fixed;

public partial class Voivodeship
{
    public int VoivodeshipsId { get; set; }
    public string Name { get; set; } = null!;
    public virtual ICollection<District> Districts { get; set; } = new List<District>();
    public virtual ICollection<ParliamentDistrict> ParliamentDistricts { get; set; } = new List<ParliamentDistrict>();

    public Voivodeship DeepClone(Voivodeship blueprint, Voivodeship toClone)
    {
        return new Voivodeship
        {
            VoivodeshipsId = blueprint.VoivodeshipsId,
            Name = blueprint.Name,
            Districts = toClone.Districts.Select(d => new District
            {
                VoivodeshipsId = d.VoivodeshipsId,
                DistrictId = d.DistrictId,
                Name = d.Name,
                Counties = d.Counties.Select(c => new County
                {
                    CountyId = c.CountyId,
                    DistrictId = c.DistrictId,
                    Name = c.Name,
                    Inahabitants = c.Inahabitants,
                    NeighboringCounties = c.NeighboringCounties?.Select(nc => new County
                    {
                        CountyId = nc.CountyId, 
                        Name = nc.Name  
                    }).ToList() ?? new List<County>(), 

                    VotingResults = c.VotingResults?.Select(vr => new VotingResult
                    {
                        ResultsId = vr.ResultsId,
                        CountyId = vr.CountyId,
                        Committee = vr.Committee,
                        NumberVotes = vr.NumberVotes,
                        Year = vr.Year
                    }).ToList() ?? new List<VotingResult>() 

                }).ToList() 

            }).ToList() 

        };
    }
}
