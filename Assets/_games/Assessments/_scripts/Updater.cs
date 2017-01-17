using Kore.Utils;
using System.Collections.Generic;

namespace EA4S.Assessment
{
    /// <summary>
    /// Used to update Non-MonoBehaviours
    /// </summary>
    public interface IUpdater
    {
        void UpdateDelta( float delta);

        void AddTimedUpdate( ITimedUpdate timedUpdate);

        void Clear();
    }

    /// <summary>
    /// Implement IUpdater
    /// </summary>
    public class Updater : SceneScopedSingletonI< Updater, IUpdater>, IUpdater
    {
        List< ITimedUpdate> updates = null;

        public void AddTimedUpdate( ITimedUpdate timedUpdate)
        {
            if (updates == null)
                updates = new List<ITimedUpdate>();

            updates.Add( timedUpdate);
        }

        public void UpdateDelta( float delta)
        {
            if(updates!=null)
                foreach (var u in updates)
                    u.Update(delta);
        }

        public void Clear()
        {
            updates = null;
        }
    }
}
