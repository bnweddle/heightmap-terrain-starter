using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeightmapTerrainStarter
{
    /// <summary>
    /// An interface providing methods for determining the 
    /// height at a point in a height map
    /// </summary>
    public interface IHeightMap
    {
        /// <summary>
        /// Gets the height of the map at the specified position
        /// </summary>
        /// <param name="x">The x coordinate in the world</param>
        /// <param name="z">The z coordinate in the world</param>
        /// <returns>The height at the specified position</returns>
        float GetHeightAt(float x, float z);
    }

}
