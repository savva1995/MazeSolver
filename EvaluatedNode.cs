using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeSolver
{
    class EvaluatedNode
    {
        public EvaluatedNode(int[] intPosition,int[] intCameFrom,int intCost, int intTotalOptimisticScore)
        {
            this.Position = intPosition;
            this.CameFrom = intCameFrom;
            this.Cost = intCost;
            this.TotaloptimisticScore = intTotalOptimisticScore;
        }
        public EvaluatedNode() { }
        private int[] _intCameFrom;
        private  int[] _intPosition;
        private  int _intCost;
        private  int _intTotalOptimisticScore;
              
            
        public int[] Position
        {
            get
            {
                return _intPosition;
            }
            set
            {
                _intPosition = value;
            }
        }
        public int[] CameFrom
        {
            get
            {
                return _intCameFrom;
            }
            set
            {
                _intCameFrom = value;
            }
        }
        public int Cost
        {
            get
            {
                return _intCost;
            }
            set
            {
                _intCost = value;
            }
        }
        public int TotaloptimisticScore
        {
            get
            {
                return _intTotalOptimisticScore;
            }
            set
            {
                _intTotalOptimisticScore = value;
            }
        }
    }
}
