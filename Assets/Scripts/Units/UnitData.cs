using System;
using Abilities;

namespace Units
{
    public class UnitData
    {
        #region Constructors
        
        protected UnitData()
        {
            
        }

        public UnitData(UnitData other)
        {
            throw new NotImplementedException();
        }
        
        #endregion
        
        
        public int GetStacks(Tenet tenet)
        {
            throw new NotImplementedException();
        }

        public void Damage(int amount)
        {
            throw new NotImplementedException();
        }

        public void Defend(int amount)
        {
            throw new NotImplementedException();
        }
        
        public void Expend(Tenet tenet, int amount)
        {
            throw new NotImplementedException();
        }
    }
}
