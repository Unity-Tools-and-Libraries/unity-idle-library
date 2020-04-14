using System.Collections.Generic;

namespace IdleFramework
{
    public  interface Updates
    {
        void Update(IdleEngine engine, float deltaTime);
    }
}