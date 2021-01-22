using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NationalParkAPI.Data;
using NationalParkAPI.Models;
using NationalParkAPI.Repository.IRepository;

namespace NationalParkAPI.Repository
{
    public class TrailRepository : ITrailRepository
    {
        private readonly ApplicationDbContext _db;

        public TrailRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public ICollection<Trail> GetTrail()
        {
            return _db.Trails.Include(c=>c.NationalPark).OrderBy(a => a.Name).ToList();
        }

        public ICollection<Trail> GetTrailInNationalPark(int npId)
        {
            return _db.Trails.Include(c=>c.NationalPark).Where(c=> c.NationalParkId == npId).OrderBy(a => a.Name).ToList();
        }

        public Trail GetTrail(int trailId)
        {
            return _db.Trails.Include(c=>c.NationalPark).FirstOrDefault(a => a.Id == trailId);
        }

        public bool TrailExist(string name)
        {
            bool value = _db.Trails.Any(a => a.Name.ToLower().Trim() == name.ToLower().Trim());
            return value;
        }

        public bool TrailExist(int id)
        {
            bool value = _db.Trails.Any(a => a.Id == id);
            return value;
        }

        public bool CreateTrail(Trail trail)
        {
            _db.Trails.Add(trail);
            return Save();
        }

        public bool EditTrail(Trail trail)
        {
            _db.Trails.Update(trail);
            return Save();
        }

        public bool DeleteTrail(Trail trail)
        {
            _db.Trails.Remove(trail);
            return Save();
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }
    }
}