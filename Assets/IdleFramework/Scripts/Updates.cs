namespace IdleFramework
{
    /**
     * Marks an entity which modified its state in response to ticks.
     */ 
    public interface Updates
    {
        /**
         * Advance the state of this entity by one tick.
         */
        void Update();
    }
}