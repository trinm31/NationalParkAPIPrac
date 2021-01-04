using System.Collections;
using System.Collections.Generic;
using NationalParkAPI.Models;

namespace NationalParkAPI.Repository.IRepository
{
    public interface INationalParkRepository
    {
        ICollection<NationalPark> GetNationalPark();
        NationalPark GetNationalPark(int nationalParkId);
        bool NationalParkExist(string name);
        bool NationalParkExist(int id);
        bool CreateNationalPark(NationalPark nationalPark);
        bool EditNationalPark(NationalPark nationalPark);
        bool DeleteNationalPark(NationalPark nationalPark);
        bool Save();
    }
}