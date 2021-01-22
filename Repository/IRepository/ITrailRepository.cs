using System.Collections;
using System.Collections.Generic;
using NationalParkAPI.Models;

namespace NationalParkAPI.Repository.IRepository
{
    public interface ITrailRepository
    {
        ICollection<Trail> GetTrail();
        ICollection<Trail> GetTrailInNationalPark(int npId);
        Trail GetTrail(int trailId);
        bool TrailExist(string name);
        bool TrailExist(int id);
        bool CreateTrail(Trail trail);
        bool EditTrail(Trail trail);
        bool DeleteTrail(Trail trail);
        bool Save();
    }
}